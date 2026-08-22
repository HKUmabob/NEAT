using NEAT.NN;
// TODO write the fallback scan for addConnection

namespace NEAT.Neat {
    internal class Genome: IComparable<Genome> {
        public List<NodeGene> nodeGenes { get; private set; }
        public List<ConnectionGene> connectionGenes { get; private set; }
        private readonly int inputs;
        private readonly int outputs;
        public float adjustedFitness;
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
            this.adjustedFitness = 0;
            this.nodeLookup = this.nodeGenes.ToDictionary(n => n.id);
            this.connectionLookup = this.connectionGenes.Select(c => (c.inNodeId, c.outNodeId)).ToHashSet();
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
                    this.connectionGenes[i].weight = (float)Random.Shared.NextDouble() * 16 - 8;
                    continue;
                }

                this.connectionGenes[i].weight += (float)(Random.Shared.NextDouble() * 1 - 0.5);
                this.connectionGenes[i].weight = Math.Clamp(this.connectionGenes[i].weight, -8.0f, 8.0f);
            }

        }


        private void AddNodeGene() {
            ConnectionGene[] enabledConnectionGenes = this.connectionGenes.Where(c => c.enabled).ToArray();
            ConnectionGene oldConnectionGene = enabledConnectionGenes[Random.Shared.Next(enabledConnectionGenes.Length)];

            int newId = InnovationTracker.Instance.GetNodeIdForSplit(oldConnectionGene.inNodeId, oldConnectionGene.outNodeId);
            if (this.nodeLookup.ContainsKey(newId)) {
                return;
            }

            NodeGene newNodeGene = new NodeGene(
                newId,
                NodeType.Hidden,
                (nodeLookup[oldConnectionGene.inNodeId].layer + nodeLookup[oldConnectionGene.outNodeId].layer) / 2.0f
                );

            oldConnectionGene.enabled = false;

            ConnectionGene newConnectionGene1 = new ConnectionGene(
                oldConnectionGene.inNodeId,
                newNodeGene.id,
                1.0f,
                true,
                InnovationTracker.Instance.GetInnovationNumber(oldConnectionGene.inNodeId, newNodeGene.id)
                );

            ConnectionGene newConnectionGene2 = new ConnectionGene(
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

            if (sourceNodes.Count == 0 || targetNodes.Count == 0) return;

            for (int i = 0; i < 20; i++) {
                NodeGene sourceNode = sourceNodes[Random.Shared.Next(sourceNodes.Count)];
                NodeGene targetNode = targetNodes[Random.Shared.Next(targetNodes.Count)];


                if (this.IsValidConnection(sourceNode, targetNode)) {
                    this.IntegrateConnectionGene(sourceNode.id, targetNode.id);
                    return;
                }
            }

            List<(int, int)> allAvailableConnections = new List<(int, int)>();
            for (int i = 0; i< sourceNodes.Count; i++) {
                for (int j = 0; j < targetNodes.Count; j++) {

                    NodeGene sourceNode = sourceNodes[i];
                    NodeGene targetNode = targetNodes[j];
                    if (this.IsValidConnection(sourceNode, targetNode)) {
                        allAvailableConnections.Add((sourceNode.id, targetNode.id));
                    }
                }
            }

            if (allAvailableConnections.Count > 0) {
                (int sourceId, int targetId) chosenNode = allAvailableConnections[Random.Shared.Next(allAvailableConnections.Count)];
                this.IntegrateConnectionGene(chosenNode.sourceId, chosenNode.targetId);
            }
        }


        private void IntegrateConnectionGene(int inNodeId, int outNodeId) {
            this.connectionGenes.Add(new ConnectionGene(
                    inNodeId,
                    outNodeId,
                    (float)Random.Shared.NextDouble() * 2 - 1,
                    true,
                    InnovationTracker.Instance.GetInnovationNumber(inNodeId, outNodeId)
                    )
                );

            this.connectionLookup.Add((inNodeId, outNodeId));
        }

        private bool IsValidConnection(NodeGene inNodeGene, NodeGene outNodeGene) {
            if (inNodeGene.layer >= outNodeGene.layer) {
                return false;
            }

            if (inNodeGene.layer == outNodeGene.layer) {
                return false;
            }

            if (this.connectionLookup.Contains((inNodeGene.id, outNodeGene.id))) {
                return false;
            }

            return true;
        }


        public static Genome Crossover(Genome parent1, Genome parent2) {
            List<ConnectionGene> childConnectioGenes = new List<ConnectionGene>();
            Genome fitParent;
            Genome unfitParent;
            if (parent1.fitness >= parent2.fitness) {
                fitParent = parent1;
                unfitParent = parent2;

            } else {
                fitParent = parent2;
                unfitParent = parent1;

            }

            bool isFitnessEqual = fitParent.fitness == unfitParent.fitness;


            Dictionary<int, ConnectionGene> fitParentInnovations = fitParent.connectionGenes.ToDictionary(c => c.innovationNumber);
            Dictionary<int, ConnectionGene> unfitParentInnovations = unfitParent.connectionGenes.ToDictionary(c => c.innovationNumber);

            foreach (var (inum, fitGene) in fitParentInnovations) {
                if (unfitParentInnovations.TryGetValue(inum, out ConnectionGene? unfitGene)) {

                    ConnectionGene chosenGene = (Random.Shared.NextDouble() >= 0.5) ? fitGene : unfitGene;
                    ConnectionGene childGene = chosenGene.Clone();
                    if (!fitGene.enabled || !unfitGene.enabled) {
                        if (Random.Shared.NextDouble() < 0.75) {
                            childGene.enabled = false;
                        } else {
                            childGene.enabled = true;
                        }
                    }


                    childConnectioGenes.Add(childGene);
                    continue;
                }

                childConnectioGenes.Add(fitGene.Clone());
            }

            if (isFitnessEqual) {
                foreach (var (inum, unfitGene) in unfitParentInnovations) {
                    if (!fitParentInnovations.ContainsKey(inum)) {
                        childConnectioGenes.Add(unfitGene.Clone());
                    }
                }
            }

            List<NodeGene> childNodeGenes = fitParent.nodeGenes.Select(n => n.Clone()).ToList();
            HashSet<int> childNodeLookup = childNodeGenes.Select(n => n.id).ToHashSet();

            for (int i = 0; i < childConnectioGenes.Count; i++) {
                ConnectionGene conn = childConnectioGenes[i];

                if (!childNodeLookup.Contains(conn.inNodeId)) {
                    childNodeGenes.Add(unfitParent.nodeLookup[conn.inNodeId].Clone());
                    childNodeLookup.Add(conn.inNodeId);
                }

                if (!childNodeLookup.Contains(conn.outNodeId)) {
                    childNodeGenes.Add(unfitParent.nodeLookup[conn.outNodeId].Clone());
                    childNodeLookup.Add(conn.outNodeId);
                }
            }


            return new Genome(childNodeGenes, childConnectioGenes, fitParent.inputs, fitParent.outputs);
        }

        public int CompareTo(Genome? otherGenome) {
            if (otherGenome == null) return 1;
            return otherGenome.fitness.CompareTo(this.fitness);
        }
    }
}