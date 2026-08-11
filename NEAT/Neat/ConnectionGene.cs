using NEAT.NN;
using System;
using System.Collections.Generic;
using System.Text;

namespace NEAT.Neat {
    internal class ConnectionGene {
        public int inNodeId;
        public int outNodeId;
        public float weight;
        public bool enabled;
        public int innovationNumber;


        public ConnectionGene(int inNodeId, int outNodeId, float weight, bool enabled, int innovationNumber) {
            this.inNodeId = inNodeId;
            this.outNodeId = outNodeId;
            this.weight = weight;
            this.enabled = enabled;
            this.innovationNumber = innovationNumber;
        }


        public ConnectionGene clone() {
            return new ConnectionGene(this.inNodeId, this.outNodeId, this.weight, this.enabled, this.innovationNumber);
        }
    }
}
