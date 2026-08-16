using System;
using System.Collections.Generic;
using System.Text;

namespace NEAT.Neat {
    public sealed class InnovationTracker {

        // Singleton
        private static readonly InnovationTracker instance = new InnovationTracker();
        public static InnovationTracker Instance { get { return instance; } }

        // ( inputID, outputID ): InnovationNumber
        private readonly Dictionary<(int, int), int> innovationRegister;

        //( inputID, outputID ): correctId
        private readonly Dictionary<(int, int), int> nodeIdRegidter;
        public int lastInnovationNumber;
        public int lastNodeId;

        public InnovationTracker() {
            this.lastInnovationNumber = 0;
            this.innovationRegister = new Dictionary<(int, int), int>();
            this.nodeIdRegidter = new Dictionary<(int, int), int>();
        }

        public int GetInnovationNumber(int inNodeId, int outNodeId) {
            
            if (this.innovationRegister.TryGetValue((inNodeId, outNodeId),  out int existingInnovationNumber) ) {
                return existingInnovationNumber;
            }

            this.lastInnovationNumber++;
            this.innovationRegister[(inNodeId, outNodeId)] = this.lastInnovationNumber;
            return this.lastInnovationNumber;
        }


        public int GetNodeIdForSplit(int sourceNodeId, int endNodeId) {

            if (this.nodeIdRegidter.TryGetValue((sourceNodeId, endNodeId), out int existingNodeId)) {
                return existingNodeId;
            }

            this.lastNodeId++;
            this.nodeIdRegidter[(sourceNodeId, endNodeId)] = this.lastNodeId;

            return lastNodeId;
        }

        /// <summary>
        /// NOTE: Only use ONE TIME in the NeatManager Constructor
        /// <code>
        /// public NeatManager(...){
        ///     ...     
        ///     InnovationTracker.Instance.SetLastNodeId(inputs + outputs + 1);
        ///     ...
        /// }
        /// </code>
        /// </summary>
        /// <param name="lastNodeid">lastNodeId = inouts + outputs + 1</param>
        public void SetLastNodeId(int lastNodeId) {
            this.lastNodeId = lastNodeId;
        }
    }
}
