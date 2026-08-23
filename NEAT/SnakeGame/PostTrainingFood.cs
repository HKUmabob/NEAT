using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace NEAT.SnakeGame {
    public class PostTrainingFood : IFoodSource {
        private Vector2 position;

        public PostTrainingFood() {
            int centerX = Constants.WINDOWWIDTH / (2 * Constants.SCALE);
            int centerY = Constants.WINDOWHEIGHT / (2 * Constants.SCALE);
            int column = 0;
            int row = 0;
            while (
                Math.Abs(column - centerX) < 10 &&
                Math.Abs(row - centerY) < 10) {
                column = Random.Shared.Next(0, Constants.NUMCOLS);
                row = Random.Shared.Next(0, Constants.NUMROWS);
            }

            this.position = new Vector2(
                    column * Constants.SCALE,
                    row * Constants.SCALE
                    );
        }

        public Vector2 getPosition() {
            return this.position;
        }

        public void changePosition(List<Vector2>? snake) {

            List<Vector2> freeSpace = new List<Vector2>();
            HashSet<Vector2> snakeSet = new HashSet<Vector2>(snake);
            foreach (Vector2 cell in Constants.grid) {
                if (!snake.Contains(cell)) {
                    freeSpace.Add(cell);
                }
            }

            int randIndex = Random.Shared.Next(freeSpace.Count);
            this.position = freeSpace[randIndex];

        }
    }
}
