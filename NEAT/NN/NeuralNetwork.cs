using NEAT.Neat;

namespace NEAT.NN {
    // TODO make NeuralNetwork(Genome) constructor
    // TODO cleanup FeedForward()
    // TODO make connection in/out nodes reference types AND give node 2 lists containing in/out connections

    internal class NeuralNetwork {

        private Connection[] connections;
        private Node[] nodes;
        private Node[] inputNodes;
        private Node[] outputNodes;
        private Node[] processingNodes;


        public NeuralNetwork(Genome genome, int inputCount, int outputCount) {

            this.nodes = genome.nodeGenes.Select(
                n => new Node(
                    n.id,
                    n.nodeType,
                    n.layer
                    )
                )
                .OrderBy(n => n.layer)
                .ThenBy(n => n.id)
                .ToArray();

            Dictionary<int, Node> nodeLookup = this.nodes.ToDictionary(n => n.id);
            this.nodes[0].value = 1;                                                    // Bias node

            this.connections = genome.connectionGenes.Where(c => c.enabled)
                .Select(
                c => new Connection(
                    nodeLookup[c.inNodeId],
                    nodeLookup[c.outNodeId],
                    c.weight
                    )
                )
                .ToArray();


            int totalInputs = inputCount + 1;
            this.inputNodes = this.nodes[0..totalInputs];                                    // includes Bias node

            this.processingNodes = this.nodes[totalInputs..];

            this.outputNodes = this.nodes[^outputCount..];

                                                                                        // Setting the incoming connections
            for (int i = 0; i< this.connections.Length; i++) {
                Connection c = this.connections[i];
                c.outNode.incomingConnections.Add(c);
            }
        }

        public float[] feedForward(float[] inputValues) {
            
            for (int i = 0; i < inputValues.Length; i++) {
                this.inputNodes[i + 1].value = inputValues[i];
            }

            for (int i = 0; i < this.processingNodes.Length; i++) {
                Node currentNode = this.processingNodes[i];
                float sum = 0;
                for (int j = 0; j < currentNode.incomingConnections.Count; j++) {
                    Node sourceNode = currentNode.incomingConnections[j].inNode;
                    sum += currentNode.incomingConnections[j].weight * sourceNode.value;
                }

                currentNode.value = Activate(sum);
            }

            float[] outputs = new float[this.outputNodes.Length];
            for (int i = 0; i < this.outputNodes.Length; i++) {
                outputs[i] = this.outputNodes[i].value;
            }
            return outputs;
        }


        private float Activate(float x) {
            return 1.0f / (1.0f + MathF.Exp(-x));
        }


    }
}
