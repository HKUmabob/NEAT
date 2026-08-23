using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace NEAT.SnakeGame {
    internal class TrainingFood : IFoodSource {
        private Vector2 position;
        private Random rand;


        public TrainingFood(int seed) {
            this.rand = new Random(seed);
            int centerX = Constants.WINDOWWIDTH / (2 * Constants.SCALE);
            int centerY = Constants.WINDOWHEIGHT / (2 * Constants.SCALE);
            int column = 0;
            int row = 0;
            while (
                Math.Abs(column - centerX) < 12 && 
                Math.Abs(row - centerY) < 15) 
                {
                column = rand.Next(0, Constants.NUMCOLS);
                row = rand.Next(0, Constants.NUMROWS);
            }

            this.position = new Vector2(
                    column * Constants.SCALE,
                    row * Constants.SCALE
                    );
        }


        public void changePosition(List<Vector2>? snake) {
            this.position = new Vector2(
                   rand.Next(0, Constants.NUMCOLS) * Constants.SCALE,
                   rand.Next(0, Constants.NUMROWS) * Constants.SCALE
                   );
        }

        public Vector2 getPosition() {
            return this.position;
        }
    }
}
