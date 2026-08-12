using NEAT.Neat;
using System.Net;
using System.Numerics;

namespace NEAT.SnakeGame {

    public enum RelativeDirection {
        Straight,
        Left,
        Right,
    }


    public class Snake : INeatEnvironment{
        private int length;
        private List<Vector2> body;
        private int headX;
        private int headY;
        private int xDir;
        private int yDir;
        private int speed;
        private Food foodSourse;
        private float fitness;
        private int stepsSinceEaten;


        public Snake() {
            this.length = 1;
            this.body = new List<Vector2>();
            this.body.Add(new Vector2(Constants.WINDOWWIDTH / 2, Constants.WINDOWHEIGHT / 2));
            this.foodSourse = new Food();
            this.speed = Constants.SCALE;
            this.xDir = 0;
            this.yDir = -this.speed;
            this.fitness = 0;
            this.stepsSinceEaten = 0;
        }



        public bool CanEat() {
            if (this.foodSourse.getPosition().Equals(this.body[this.length - 1])) {
                this.stepsSinceEaten = 0;
                return true;
            }

            this.stepsSinceEaten++;
            return false;
        }

        public void Eat() {

            this.body.Add(
                new Vector2(
                    this.body[this.length - 1].X,
                    this.body[this.length - 1].Y
                    )
                );

            this.length++;
            this.foodSourse.changePosition(this.body);
        }


        public List<Vector2> GetBody() {
            return this.body;
        }


        public bool isDead() {
            if (
                this.headX >= Constants.WINDOWWIDTH ||
                this.headY >= Constants.WINDOWHEIGHT ||
                this.headX < 0 || this.headY < 0
                ) {
                this.fitness -= 150;
                return true;

            } else if (this.stepsSinceEaten > 100) {
                this.fitness -= 50;
                return true;

            }else if (this.body[..^1].Contains(this.body[this.length - 1])) {
                this.fitness -= 100;
                return true;
            }

            this.fitness++;
            return false;
        }


        public int GetScore() {
            return this.length - 1;
        }

        private void SetDir(RelativeDirection direction) {
            switch (direction) {
                case RelativeDirection.Straight:
                    break;

                case RelativeDirection.Left: {
                        int oldX = this.xDir;
                        this.xDir = this.yDir;
                        this.yDir = -oldX;
                        break;
                    }

                case RelativeDirection.Right: {
                        int oldX = this.xDir;
                        this.xDir = -this.yDir;
                        this.yDir = oldX;
                        break;
                    }
            }

        }


        private float GetDistanceToBody(RelativeDirection direction) {

            int dx = 0;
            int dy = 0;

            if (direction == RelativeDirection.Straight) {
                dx = this.xDir / Constants.SCALE;
                dy = this.yDir / Constants.SCALE;
            } else if (direction == RelativeDirection.Left) {
                dx = this.yDir / Constants.SCALE;
                dy = -this.xDir / Constants.SCALE;
            } else if (direction == RelativeDirection.Right) {
                dx = -this.yDir / Constants.SCALE;
                dy = this.xDir / Constants.SCALE;
            }

            int currentX = (this.headX / Constants.SCALE) + dx;
            int currentY = (this.headY / Constants.SCALE) + dy;
            int distance = 1;
            
            while ( 
                currentX >= 0 && currentX < Constants.NUMCOLS &&
                currentY >= 0 && currentY < Constants.NUMROWS
                ) {

                if (this.body[..^1].Contains( new Vector2(currentX * Constants.SCALE, currentY * Constants.SCALE))) {
                    return 1.0f / distance;
                }

                currentX += dx;
                currentY += dy;
                distance++;

            }

            return 0;
        }


        public float[] GetInputs() {
            float hDistanceToFood = (this.foodSourse.getPosition().X - this.headX) / 25f;
            float vDistanceToFood = (this.foodSourse.getPosition().Y - this.headY) / 25f;

            float rightWallDistance = Constants.NUMCOLS - 1.0f - this.headX / 25f;
            float leftWallDistance = this.headX / 25f;
            float downWallDistance = Constants.NUMROWS - 1.0f - this.headY / 25f;
            float upWallDistance = this.headY / 25f;

            float forwardBodyDistance = GetDistanceToBody(RelativeDirection.Straight);
            float leftBodyDistance = GetDistanceToBody(RelativeDirection.Left);
            float rightBodyDistance = GetDistanceToBody(RelativeDirection.Right);



            return new float[] {
                hDistanceToFood / Constants.NUMCOLS,
                vDistanceToFood / Constants.NUMROWS,
                1.0f / (rightWallDistance + 1.0f), // +1 to avoid deviding by 0
                1.0f / (leftWallDistance + 1.0f),
                1.0f / (upWallDistance + 1.0f),
                1.0f / (downWallDistance + 1.0f),
                forwardBodyDistance,
                leftBodyDistance,
                rightBodyDistance

            };
        }


        public void Reset() {
            this.length = 1;
            this.body = new List<Vector2>();
            this.body.Add(new Vector2(Constants.WINDOWWIDTH / 2, Constants.WINDOWHEIGHT / 2));
            this.foodSourse = new Food();
            this.speed = Constants.SCALE;
            this.xDir = 0;
            this.yDir = -this.speed;
            this.fitness = 0;
            this.stepsSinceEaten = 0;

        }


        public bool Step(float[] networkOutputs) {
            float[] outputs = networkOutputs;
            float maxval = outputs.Max();
            int index = outputs.ToList().IndexOf(maxval);
            this.SetDir((RelativeDirection)index);

            this.headX = (int)this.body[this.length - 1].X;
            this.headY = (int)this.body[this.length - 1].Y;

            this.headX += this.xDir;
            this.headY += this.yDir;

            this.body.RemoveAt(0);
            this.body.Add(new Vector2(this.headX, this.headY));

            if (this.CanEat()) {
                this.Eat();
            }

            return this.isDead();
        }


        public float GetFitness() {
            this.fitness += this.GetScore() * 100;

            return MathF.Max(this.fitness, 0.1f);
        }

    }
}
