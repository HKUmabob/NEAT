using NEAT.NN;
using System;
using System.Collections.Generic;
using System.Text;

namespace NEAT.Neat {
    internal class NeatHandler {
        private int inputs;
        private int outputs;

        public NeatHandler(int inputs, int outputs) {
            this.inputs = inputs;
            this.outputs = outputs;

        }

        private void Innitialize() {
            List<NodeGene> nodeGenes = new List<NodeGene>();
            List<ConnectionGene> connectionGenes = new List<ConnectionGene>();
            // Number of inputs = inputs+1 for bias node
            for (int i = 0; i < inputs + 1; i++) {
                nodeGenes.Add(new NodeGene(i, NodeType.Input, 0));
            }

            nodeGenes[0].value = 1f;

            for (int i = 0; i < outputs; i++) {
                nodeGenes.Add(new NodeGene(inputs + 1 + i, NodeType.Output, 1));
            }

            int innovationNumber
            for (int i = 0; i < inputs + 1; i++) {
                for (int j = 0; j < outputs; j++) {
                    connectionGenes.Add(new ConnectionGene(i, inputs + j + 1, (float)Random.Shared.NextDouble() * 2 - 1, true));

                }
            }
        }
    }
}
