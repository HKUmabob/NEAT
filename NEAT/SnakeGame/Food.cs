using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace NEAT.SnakeGame {
    public class Food {
        private Vector2 position;
        private Random rand = new Random();

        public Food() {

            this.changePosition();
        }

        public Vector2 getPosition() {
            return this.position;
        }

        public void changePosition(List<Vector2>? snake = null) {

            if (snake is null) {

                this.position = new Vector2(
                    rand.Next(5, Constants.NUMCOLS) * Constants.SCALE,
                    rand.Next(5, Constants.NUMROWS) * Constants.SCALE
                    );

                return;
            }

            List<Vector2> freeSpace = new List<Vector2>();
            HashSet<Vector2> snakeSet = new HashSet<Vector2>(snake);
            foreach (Vector2 cell in Constants.grid) {
                if (!snake.Contains(cell)) {
                    freeSpace.Add(cell);
                }
            }

            int randIndex = rand.Next(freeSpace.Count);
            this.position = freeSpace[randIndex];

        }
    }
}
