using NEAT.NN;

namespace NEAT.Neat {
    internal class NeatManager {
        public readonly int inputs;
        public readonly int outputs;
        private readonly int populationSize;
        private readonly float compatibilityThreshold;
        private int generation;
        private Genome? champion;
        private List<Genome> population;
        private List<Species> speciesList;
        private INeatEnvironment[] environments;
        private readonly Func<INeatEnvironment> environmentFacotry;

        public NeatManager(int inputs, int outputs, Func<INeatEnvironment> environment, int populationSize = 100, float compatibilityThreshold = 0.3f) {
            this.inputs = inputs;
            this.outputs = outputs;
            this.populationSize = populationSize;
            this.compatibilityThreshold = compatibilityThreshold;
            this.environmentFacotry = environment;
            this.generation = 1;
            this.population = new List<Genome>(this.populationSize);
            this.environments = new INeatEnvironment[this.populationSize];
            this.speciesList = new List<Species>();
            InnovationTracker.Instance.SetLastNodeId(inputs + outputs + 1);

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
                for (int j = inputs + 1; j < inputs + outputs + 1; j++) {

                    connectionGenes.Add(new ConnectionGene(
                        i,
                        j,
                        (float)Random.Shared.NextDouble() * 2 - 1,
                        true,
                        InnovationTracker.Instance.GetInnovationNumber(i, j)
                        )
                    );

                }
            }

            this.population.Add(new Genome(nodeGenes, connectionGenes, this.inputs, this.outputs));
            this.environments[0] = environmentFacotry();
            Genome firstGenome = population[0];
            for (int i = 1; i < this.populationSize; i++) {
                this.population.Add(firstGenome.Clone());
                this.environments[i] = environmentFacotry();
            }
        }


        private void AdvanceGeneration() {

            // Clearing species population to make space for new generation
            for (int i = 0; i < this.speciesList.Count; i++) {
                this.speciesList[i].population.Clear();
            }


            // Assigning genomes to species
            for (int i = 0; i < this.populationSize; i++) {
                Genome genome = this.population[i];
                bool speciesFound = false;

                for (int j = 0; j < this.speciesList.Count; j++) {
                    Species species = this.speciesList[j];
                    float distance = species.GetCompatibilityDistance(genome);
                    if (distance < this.compatibilityThreshold) {
                        species.population.Add(genome);
                        speciesFound = true;
                        break;
                    }
                }

                if (!speciesFound) {
                    this.speciesList.Add(new Species(genome));
                }
            }

            // Culling and updating species
            float totalAverageFitness = 0.0f;
            for (int i = this.speciesList.Count - 1; i >= 0; i--) {
                Species species = this.speciesList[i];
                species.UpdateFitnessAndStagnation();
                if (species.population.Count == 0 || species.generationsSinceImprovement > 15) {
                    this.speciesList.RemoveAt(i);
                }

                totalAverageFitness += species.averageFitness;
            }


            // Producing offsprings
            this.population.Clear();
            float[] exactquotas = new float[this.speciesList.Count];
            int allocated = 0;
            int[] offspringCounts = new int[this.speciesList.Count];
            for (int i = 0; i < this.speciesList.Count; i++) {
                Species species = this.speciesList[i];
                float quota = (species.averageFitness / totalAverageFitness) * this.populationSize;
                exactquotas[i] = quota;
                int floorQuota = (int)MathF.Floor(quota);
                offspringCounts[i] = floorQuota;
                allocated += floorQuota;
            }

            int leftovers = this.populationSize - allocated;
            if (leftovers > 0) {
                var sortedIndices = Enumerable.Range(0, this.speciesList.Count)
                    .OrderByDescending(i => exactquotas[i] - offspringCounts[i])
                    .ToArray();

                for (int i = 0; i < leftovers; i++) {
                    offspringCounts[sortedIndices[i % sortedIndices.Length]]++;
                }
            }

            for (int i = 0; i < this.speciesList.Count; i++) {
                this.population.AddRange(this.speciesList[i].GetOffsprings(offspringCounts[i]));
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
            
            this.population.Sort();
            this.champion = this.population[0];

            if (this.generation % 100 == 0) {
                Console.WriteLine($"Genration: {this.generation} \nChampion fitness: {this.champion.fitness}");
            }

        }

        public void Train(int maxGenerations, float fitnessThreshold) {
            for (int i = 0; i < maxGenerations; i++) {
                this.EvaluatePopulation();
                this.AdvanceGeneration();
                this.generation++;
                if (this.champion?.fitness >= fitnessThreshold) {
                    break;
                }
            }
            
            Console.WriteLine($"Genration: {this.generation} \nChampion fitness: {this.champion?.fitness}");
        }


        //public NeuralNetwork GetChampionNetwork() {
            //return this.champion.neuralNetwork.Clone();
        //}
    }
}
