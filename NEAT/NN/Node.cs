using System.Runtime.InteropServices.Swift;

namespace NEAT.NN {

    public enum NodeType {
        Input,
        Hidden,
        Output
    }

    internal class Node {
        public int id;
        public float value;
        public float layer;
        public NodeType nodeType;
        public List<Connection> incomingConnections;


        public Node(int id, NodeType nodeType, float layer) { 
            this.id = id;
            this.nodeType = nodeType;
            this.layer = layer;
            this.value = 0;
            this.incomingConnections = new List<Connection>();
        }


    }


}
