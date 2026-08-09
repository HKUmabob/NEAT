using System;
using System.Collections.Generic;
using System.Text;

namespace NEAT.Neat {
    public interface INeatEnvironment {

        // Reset the simulation to its innitial state
        void Reset();

        // Takes the next step in the 
        // Returns true if the simulation has ended
        bool Step(float[] networkOutputs);

        // Return the inputs for the neural network
        float[] GetInputs();

        // Calculate and return the fitness
        float GetFitness();


    }
}
