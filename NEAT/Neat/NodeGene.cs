using NEAT.NN;
using System;
using System.Collections.Generic;
using System.Text;

namespace NEAT.Neat {
    internal class NodeGene {
        public int id;
        public float value;
        public int layer;
        public NodeType nodeType;


        public NodeGene(int id, NodeType nodeType, int layer) {
            this.id = id;
            this.nodeType = nodeType;
            this.layer = layer;
        }


        public NodeGene clone() {
            return new NodeGene(this.id, this.nodeType, this.layer);
        }
    }
}
