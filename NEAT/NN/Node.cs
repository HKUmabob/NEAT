using System;
using System.Collections.Generic;
using System.Text;

namespace NEAT.NN {

    public enum NodeType {
        Input,
        Hidden,
        Output
    }

    internal class Node {
        public int id;
        public float value;
        public int layer;
        public NodeType nodeType;


        public Node(int id, NodeType nodeType, int layer = 0) { 
            this.id = id;
            this.nodeType = nodeType;
            this.layer = layer;
            this.value = 0;
        }


        public Node clone() {
            return new Node(this.id, this.nodeType, this.layer) 
            {
                value = this.value
            };
        }
    }


}
