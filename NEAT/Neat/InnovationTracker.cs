using System;
using System.Collections.Generic;
using System.Text;

namespace NEAT.Neat {
    public sealed class InnovationTracker {

        // Singleton
        private static readonly InnovationTracker instance = new InnovationTracker();
        public static InnovationTracker Instance { get { return instance; } }

        // { ( inputID, outputID ): InnovationNumber
        public readonly Dictionary<(int, int), int> innovationRegister;
        public int lastInnovationNumber;

        public InnovationTracker() {
            this.lastInnovationNumber = 0;
            this.innovationRegister = new Dictionary<(int, int), int>();
        }

        public int GetInnovationNumber(int inNodeId, int outNodeId) {

            (int, int) key = (inNodeId, outNodeId);
            if (this.innovationRegister.TryGetValue( key,  out int existingInnovationNumber) ) {
                return existingInnovationNumber;
            }

            this.lastInnovationNumber++;
            this.innovationRegister[key] = this.lastInnovationNumber;
            return this.lastInnovationNumber;
        }
    }
}
