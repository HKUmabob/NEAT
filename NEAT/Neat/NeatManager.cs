using NEAT.NN;

namespace NEAT.Neat {
    internal class NeatManager {
        public readonly int inputs;
        public readonly int outputs;
        private readonly int populationSize;
        private int generation;
        private Genome? champion;
        private Genome[] population;
        private INeatEnvironment[] environments;
        private readonly Func<INeatEnvironment> environmentFacotry;

        public NeatManager(int inputs, int outputs, Func<INeatEnvironment> environment, int populationSize = 100) {
            this.inputs = inputs;
            this.outputs = outputs;
            this.populationSize = populationSize;
            this.environmentFacotry = environment;
            this.generation = 1;
            this.population = new Genome[this.populationSize];
            this.environments = new INeatEnvironment[this.populationSize];

            this.Innitialize();

        }

        private void Innitialize() {
            List<NodeGene> nodeGenes = new List<NodeGene>();
            List<ConnectionGene> connectionGenes = new List<ConnectionGene>();

            // Number of inputs = inputs+1 for bias node
            for (int i = 0; i < inputs + 1; i++) {
                nodeGenes.Add(new NodeGene(i, NodeType.Input, 0));
            }


            for (int i = 0; i < outputs; i++) {
                nodeGenes.Add(new NodeGene(inputs + 1 + i, NodeType.Output, 1));
            }


            for (int i = 0; i < inputs + 1; i++) {
                for (int j = 0; j < outputs; j++) {

                    connectionGenes.Add(new ConnectionGene(
                        i,
                        inputs + j + 1,
                        (float)Random.Shared.NextDouble() * 2 - 1,
                        true,
                        InnovationTracker.Instance.GetInnovationNumber(i, j)
                        )
                    );

                }
            }

            this.population[0] = new Genome(nodeGenes, connectionGenes, this.inputs, this.outputs);
            this.environments[0] = environmentFacotry();
            Genome firstGenome = population[0];
            for (int i = 1; i < this.populationSize; i++) {
                this.population[i] = firstGenome.Clone();
                this.environments[i] = environmentFacotry();

            }

        }


        public void EvaluatePopulation() {

            Parallel.For(0, this.populationSize, i => {
                NeuralNetwork network = this.population[i].neuralNetwork;
                INeatEnvironment env = this.environments[i];

                env.Reset();
                bool isFinished = false;

                while (!isFinished) {
                    float[] inputs = env.GetInputs();
                    float[] outputs = network.feedForward(inputs);
                    isFinished = env.Step(outputs);

                }
                this.population[i].fitness = env.GetFitness();

            });

            for (int i = 0; i < this.populationSize; i++) {
                this.population[i].Mutate();
            }

            this.champion = this.population.OrderBy(g => g.fitness).Last();
            Console.WriteLine($"Genration: {this.generation} \nChampion fitness: {this.champion.fitness}");
            this.generation++;

        }


    }
}
