using NEAT.NN;

namespace NEAT.Neat {
    internal class Genome {
        public List<NodeGene> nodeGenes { get; private set; }
        public List<ConnectionGene> connectionGenes { get; private set; }
        private readonly int inputs;
        private readonly int outputs;
        public readonly NeuralNetwork neuralNetwork;


        public Genome(List<NodeGene> nodeGenes, List<ConnectionGene> connectionGenes, int inputs, int outputs) {
            this.inputs = inputs;
            this.outputs = outputs;
            this.connectionGenes = connectionGenes;
            this.nodeGenes = nodeGenes;
            this.neuralNetwork = new NeuralNetwork(this, inputs, outputs);

        }


        public Genome Clone() {
            List<NodeGene> clonedNodeGenes = this.nodeGenes.Select(n => n.Clone()).ToList();
            List<ConnectionGene> clonedConnectionGenes = this.connectionGenes.Select(c => c.Clone()).ToList();

            return new Genome(clonedNodeGenes, clonedConnectionGenes, this.inputs, this.outputs);
        }
    }
}
