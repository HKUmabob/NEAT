namespace NEAT.NN {
    internal class Connection {
        public int inNodeId;
        public int outNodeId;
        public float weight;


        public Connection(int inNodeId, int outNodeId, float weight, bool enabled) {
            this.inNodeId = inNodeId;
            this.outNodeId = outNodeId;
            this.weight = weight;
        }


        

    }
}
