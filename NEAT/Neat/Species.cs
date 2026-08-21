namespace NEAT.Neat {
    internal class Species {
        public int age;
        public int generationsSinceImprovement;
        public float averageFitness;
        public float bestFitness;
        public List<Genome> population;
        public Genome representative;
        public Genome champion;



        public Species(Genome representative) {
            this.representative = representative.Clone();
            this.champion = representative.Clone();
            this.age = 0;
            this.generationsSinceImprovement = 0;
            this.averageFitness = 0;
            this.bestFitness = 0;
            this.population = new List<Genome>() { representative };
        }


        public void UpdateFitnessAndStagnation() {
            if (this.population.Count == 0) return;

            float sum = 0.0f;
            float maxfitness = -99999999999999999999.0f;
            Genome currentTopPerformer = this.population[0];
            for (int i = 0; i < this.population.Count; i++) {
                Genome genome = this.population[i];
                genome.adjustedFitness = genome.fitness / this.population.Count;
                sum += genome.adjustedFitness;

                if (genome.fitness > maxfitness) {
                    maxfitness = genome.fitness;
                    currentTopPerformer = genome;
                }
            }

            if (maxfitness > this.bestFitness) {
                this.bestFitness = maxfitness;
                this.champion = currentTopPerformer;
                this.generationsSinceImprovement = 0;

            } else {
                this.generationsSinceImprovement++;
            }

            this.representative = this.population[Random.Shared.Next(this.population.Count)].Clone();
            this.age++;

            this.averageFitness = sum;
        }
        

        public List<Genome> GetOffsprings(int offSpringCount) {
            List<Genome> offsprings = new List<Genome>();
            if (offSpringCount == 0) {
                return offsprings;
            }

            this.population.Sort();
            int cutoffIndex = (int)Math.Ceiling(this.population.Count * 0.2);
            int startIndex = 0;

            if (this.population.Count >5) {
                startIndex = 1;
                offsprings.Add(this.population[0].Clone());
            }

            for (int i = startIndex; i < offSpringCount; i++) {
                Genome child;
                if (Random.Shared.NextDouble() < 0.75) {
                    child = Genome.Crossover(
                        this.population[Random.Shared.Next(cutoffIndex)],
                        this.population[Random.Shared.Next(cutoffIndex)]
                        );
                
                } else {
                    child = this.population[Random.Shared.Next(cutoffIndex)].Clone();
                }

                child.Mutate();
                offsprings.Add(child);
            }

            return offsprings;
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
