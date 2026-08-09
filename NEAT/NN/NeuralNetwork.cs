namespace NEAT.NN {

    internal class NeuralNetwork {

        public List<Connection> connections { get; private set; }
        public List<Node> nodes { get; private set; }


        // use for testing
        public NeuralNetwork(int inputs, int outputs) {
            this.nodes = new List<Node>();
            this.connections = new List<Connection>();

            // Number of inputs = inputs+1 for bias node
            for (int i = 0; i < inputs + 1; i++) {
                nodes.Add(new Node(i, NodeType.Input, 0));
            }

            this.nodes[0].value = 1f;

            for (int i = 0; i < outputs; i++) {
                nodes.Add(new Node(inputs + 1 + i, NodeType.Output, 1));
            }


            int innovationNumber = 1;
            for (int i = 0; i < inputs + 1; i++) {
                for (int j = 0; j < outputs; j++) {
                    connections.Add(new Connection(i, inputs + j + 1, (float)Random.Shared.NextDouble() * 2 - 1, true, innovationNumber));
                    innovationNumber++;

                }
            }

        }

        public float[] feedForward(float[] inputs) {

            List<Node> inputNodes = this.nodes.Where(n => n.nodeType == NodeType.Input).OrderBy(n => n.id).ToList();
            List<Node> otherNodes = this.nodes.Where(n => n.nodeType != NodeType.Input).OrderBy(n => n.layer).ToList();

            for (int i = 1; i < inputNodes.Count; i++) {
                inputNodes[i].value = inputs[i - 1];
            }

            for (int i = 0; i < otherNodes.Count; i++) {
                Node currentNode = otherNodes[i];
                float sum = 0;
                List<Connection> incomingConnections = this.connections.Where(c => c.outNodeId == currentNode.id && c.enabled).ToList();
                for (int j = 0; j < incomingConnections.Count; j++) {
                    Node sourceNode = this.nodes.First( n => n.id == incomingConnections[j].inNodeId);
                    sum += incomingConnections[j].weight * sourceNode.value;
                }

                currentNode.value = sigmoid(sum);
            }

            List<Node> outNodes = this.nodes.Where(n => n.nodeType == NodeType.Output).ToList();
            float[] outputs = new float[outNodes.Count];
            for (int i = 0; i < outNodes.Count; i++) {
                outputs[i] = outNodes[i].value;
            }
            return outputs;
        }


        private float sigmoid(float x) {
            return 1.0f / (1.0f + MathF.Exp(-x));
        }


        private NeuralNetwork(List<Node> nodes, List<Connection> connections) {
            this.connections = connections;
            this.nodes = nodes;
        }


        public NeuralNetwork clone() {

            return new NeuralNetwork(
                this.nodes.Select(n => n.clone()).ToList(),
                this.connections.Select(c => c.clone()).ToList()
                );

        }

    }
}
