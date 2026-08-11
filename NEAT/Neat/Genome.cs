using NEAT.NN;
using System;
using System.Collections.Generic;
using System.Text;

namespace NEAT.Neat {
    internal class Genome {
        private List<NodeGene> nodeGenes;
        private List<ConnectionGene> connectionGenes;

        public Genome() {
            this.nodeGenes = new List<NodeGene>();
            this.connectionGenes = new List<ConnectionGene>();
        }

        private Genome(List<NodeGene> nodeGenes, List<ConnectionGene> connectionGenes) {
            this.connectionGenes = connectionGenes;
            this.nodeGenes = nodeGenes;
        }

        public Genome Clone() {
            return new Genome(this.nodeGenes, this.connectionGenes);
        }
    }
}
