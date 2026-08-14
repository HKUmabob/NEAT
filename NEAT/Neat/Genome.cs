using NEAT.NN;
// TODO write the fallback scan for addConnection

namespace NEAT.Neat {
    internal class Genome {
        public List<NodeGene> nodeGenes { get; private set; }
        public List<ConnectionGene> connectionGenes { get; private set; }
        private readonly int inputs;
        private readonly int outputs;
        public float fitness;
        public readonly NeuralNetwork neuralNetwork;

        // See if these two are used in more than 1 function and move them to their respective functions
        private Dictionary<int, NodeGene> nodeLookup;
        private HashSet<(int, int)> connectionLookup;

        public Genome(List<NodeGene> nodeGenes, List<ConnectionGene> connectionGenes, int inputs, int outputs) {
            this.inputs = inputs;
            this.outputs = outputs;
            this.connectionGenes = connectionGenes;
            this.nodeGenes = nodeGenes;
            this.nodeLookup = this.nodeGenes.ToDictionary(n => n.id);
            this.connectionLookup = new HashSet<(int, int)>();
            for (int i = 0; i < this.connectionGenes.Count; i++) {
                this.connectionLookup.Add((this.connectionGenes[i].inNodeId, this.connectionGenes[i].outNodeId));
            }


            this.neuralNetwork = new NeuralNetwork(this, inputs, outputs);

        }


        public Genome Clone() {
            List<NodeGene> clonedNodeGenes = this.nodeGenes.Select(n => n.Clone()).ToList();
            List<ConnectionGene> clonedConnectionGenes = this.connectionGenes.Select(c => c.Clone()).ToList();

            return new Genome(clonedNodeGenes, clonedConnectionGenes, this.inputs, this.outputs);
        }


        public void Mutate() {
            this.MutateWeights();
            
            if (Random.Shared.NextDouble() < 0.03) {
                this.AddNodeGene();
            }

            if (Random.Shared.NextDouble() < 0.02) {
                this.ToggleConnectionGene();
            }

            if (Random.Shared.NextDouble() < 0.05) {
                this.AddConnectionGene();
            }

        }


        private void MutateWeights() {
            for (int i = 0; i < this.connectionGenes.Count; i++) {
                if (Random.Shared.NextDouble() > 0.8) {
                    continue;
                }

                if (Random.Shared.NextDouble() > 0.9) {
                    this.connectionGenes[i].weight = (float)Random.Shared.NextDouble() * 2 - 1;
                    continue;
                }

                this.connectionGenes[i].weight += (float)((Random.Shared.NextDouble() * 2 - 1) * 0.1);

            }

        }


        private void AddNodeGene() {
            ConnectionGene[] enabledConnectionGenes = this.connectionGenes.Where(c => c.enabled).ToArray();
            ConnectionGene oldConnectionGene = enabledConnectionGenes[Random.Shared.Next(enabledConnectionGenes.Length)];
            NodeGene newNodeGene = new NodeGene(
                this.nodeLookup.Keys.Max() + 1,
                NodeType.Hidden,
                (nodeLookup[oldConnectionGene.inNodeId].layer + nodeLookup[oldConnectionGene.outNodeId].layer) / 2.0f
                );

            oldConnectionGene.enabled = false;

            ConnectionGene newConnectionGene1 =  new ConnectionGene(
                oldConnectionGene.inNodeId,
                newNodeGene.id,
                1.0f,
                true,
                InnovationTracker.Instance.GetInnovationNumber(oldConnectionGene.inNodeId, newNodeGene.id)
                );

            ConnectionGene newConnectionGene2 =  new ConnectionGene(
                    newNodeGene.id,
                    oldConnectionGene.outNodeId,
                    oldConnectionGene.weight,
                    true,
                    InnovationTracker.Instance.GetInnovationNumber(newNodeGene.id, oldConnectionGene.outNodeId)
                    );

            this.nodeGenes.Add(newNodeGene);
            this.nodeLookup.Add(newNodeGene.id, newNodeGene);

            this.connectionGenes.Add(newConnectionGene1);  
            this.connectionGenes.Add(newConnectionGene2);
            this.connectionLookup.Add((newConnectionGene1.inNodeId, newConnectionGene1.outNodeId));
            this.connectionLookup.Add((newConnectionGene2.inNodeId, newConnectionGene2.outNodeId));
        }

        
        private void ToggleConnectionGene() {
            ConnectionGene randomConnectionGene = this.connectionGenes[Random.Shared.Next(this.connectionGenes.Count)];
            randomConnectionGene.enabled = !randomConnectionGene.enabled;

        }


        private void AddConnectionGene() {
            
            List<NodeGene> sourceNodes = new List<NodeGene>();
            List<NodeGene> targetNodes = new List<NodeGene>();

            NodeGene nodeGene;
            for (int i = 0; i < this.nodeGenes.Count; i++) {
                nodeGene = this.nodeGenes[i];

                if (nodeGene.nodeType == NodeType.Input) {
                    sourceNodes.Add(nodeGene);
                    continue;

                } else if (nodeGene.nodeType == NodeType.Output) {
                    targetNodes.Add(nodeGene);
                    continue;
                }

                sourceNodes.Add(nodeGene);
                targetNodes.Add(nodeGene);

            }


            for (int i = 0; i < 20; i++) {
                NodeGene sourceNode = sourceNodes[Random.Shared.Next(sourceNodes.Count)];
                NodeGene targetNode = targetNodes[Random.Shared.Next(targetNodes.Count)];

                if (IsValidConnection(sourceNode, targetNode)) {
                    this.connectionGenes.Add(
                        new ConnectionGene(
                            sourceNode.id,
                            targetNode.id,
                            (float)Random.Shared.NextDouble() * 2 - 1,
                            true,
                            InnovationTracker.Instance.GetInnovationNumber(sourceNode.id, targetNode.id)
                            )
                        );

                    this.connectionLookup.Add((sourceNode.id, targetNode.id));
                    break;
                }
            }
        }


        private bool IsValidConnection(NodeGene inNodeGene, NodeGene outNodeGene) {
            if (inNodeGene.layer >= outNodeGene.layer) {
                return false;
            }

            if (this.connectionLookup.Contains((inNodeGene.id, outNodeGene.id))) {
                return false;
            }

            return true;
        }

    }
}
