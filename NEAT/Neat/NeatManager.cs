using NEAT.NN;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace NEAT.Neat {
    internal class NeatManager {
        public readonly int inputs;
        public readonly int outputs;
        private readonly int populationSize;
        private List<Genome> population;

        public NeatManager(int populationSize, int inputs, int outputs) {
            this.inputs = inputs;
            this.outputs = outputs;
            this.populationSize = populationSize;
            this.population = new List<Genome>();
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

            population.Add(new Genome(nodeGenes, connectionGenes, this.inputs, this.outputs));
            Genome firstGenome = population[0];
            for (int i = 0; i < this.populationSize; i++) {
                population.Add( firstGenome.Clone());
            }

        }


    }
}
