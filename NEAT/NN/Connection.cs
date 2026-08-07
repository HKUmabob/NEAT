namespace NEAT.NN {
    internal class Connection {
        public int inNodeId;
        public int outNodeId;
        public float weight;
        public bool enabled;
        public int innovationNumber;


        public Connection(int inNodeId, int outNodeId, float weight, bool enabled, int innovationNumber) {
            this.inNodeId = inNodeId;
            this.outNodeId = outNodeId;
            this.weight = weight;
            this.enabled = enabled;
            this.innovationNumber = innovationNumber;
        }


        public Connection clone() {
            return new Connection(this.inNodeId, this.outNodeId, this.weight, this.enabled, this.innovationNumber);
        }

    }
}
