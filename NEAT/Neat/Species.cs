using System;
using System.Collections.Generic;
using System.Text;

namespace NEAT.Neat {
    internal class Species {
        public int age;
        public int generationsSinceImprovement;
        public float averageFitness;
        public List<Genome> population;
        public Genome representative;


        public Species(Genome representative) {
            this.representative = representative;
            this.age = 0;
            this.generationsSinceImprovement = 0;
            this.averageFitness = 0;
            this.population = new List<Genome>() { this.representative };
        }


        public void CalculateAvergaeFiness() {
            float sum = 0;
            for (int i = 0; i < population.Count; i++) {
                sum += this.population[i].adjustedFitness;
            }
            this.averageFitness = sum / this.population.Count;
        }


        public float GetCompatibilityDistance(Genome candidate, float c1 = 1.0f, float c2 = 1.0f, float c3 = 0.4f) {
            ConnectionGene[] repGenes = this.representative.connectionGenes.OrderBy(c => c.innovationNumber).ToArray();
            ConnectionGene[] candidateGenes = candidate.connectionGenes.OrderBy(c => c.innovationNumber).ToArray();

            if (repGenes.Length == 0 && candidateGenes.Length == 0) return 0.0f;

            int representativeMax = repGenes[^1].innovationNumber;
            int candidateMax = candidateGenes[^1].innovationNumber;

            int matchingCount = 0;
            int disjointCount = 0;
            int excessCount = 0;
            float weightDifference = 0.0f;

            int repIndex = 0;
            int candIndex = 0;

            while (repIndex < repGenes.Length && candIndex < candidateGenes.Length) {
                ConnectionGene repGene = repGenes[repIndex];
                ConnectionGene candGene = candidateGenes[candIndex];

                if (repGene.innovationNumber == candGene.innovationNumber) {
                    matchingCount++;
                    repIndex++;
                    candIndex++;
                    weightDifference += MathF.Abs(repGene.weight - candGene.weight);

                } else if (repGene.innovationNumber < candGene.innovationNumber) {
                    if (repGene.innovationNumber < candidateMax) {
                        disjointCount++;
                    } else {
                        excessCount ++;
                    }
                    repIndex++;

                }else {
                    if (candGene.innovationNumber < representativeMax) {
                        disjointCount++;
                    } else {
                        excessCount++;
                    }
                    candIndex++;

                }
            }

            excessCount += repGenes.Length - repIndex;
            excessCount += candidateGenes.Length - candIndex;

            // Max-gene count
            float N = MathF.Max(candidateGenes.Length, repGenes.Length);
            if (N < 20) {
                N = 1.0f;
            }

            float averageWeightDiff = matchingCount > 0 ? (weightDifference / matchingCount) : 0.0f;

            return ((c1 * excessCount + c2 * disjointCount) / N) + c3 * averageWeightDiff;
        }
    }
}
