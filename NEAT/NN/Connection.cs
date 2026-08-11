namespace NEAT.NN {
    internal class Connection {
        public Node inNode;
        public Node outNode;
        public float weight;


        public Connection(Node inNode, Node outNode, float weight) {
            this.inNode = inNode;
            this.outNode = outNode;
            this.weight = weight;
        }


    }
}
