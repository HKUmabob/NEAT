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
        private IFoodSource foodSourse;
        private float fitness;
        private int stepsSinceEaten;
        private float lastDistanceToFood;
        private bool isTraining;


        public Snake(bool isTraining = true) {
            this.length = 1;
            this.body = new List<Vector2>();
            this.body.Add(new Vector2(Constants.WINDOWWIDTH / 2, Constants.WINDOWHEIGHT / 2));
            this.foodSourse = this.GetFoodSource(123); //this will be used when the snake is run outside of neatmanager
            this.speed = Constants.SCALE;
            this.xDir = 0;
            this.yDir = -this.speed;
            this.fitness = 0;
            this.stepsSinceEaten = 0;
            this.isTraining = isTraining;
            this.headX = (int)this.body[0].X;
            this.headY = (int)this.body[0].Y;
            float xDelta = Math.Abs((this.foodSourse.getPosition().X - this.headX) / 25f);
            float yDelta = Math.Abs((this.foodSourse.getPosition().Y - this.headY) / 25f);
            this.lastDistanceToFood = xDelta  + yDelta;
        }

        public Vector2 GetFoodPosiotion() {
            return this.foodSourse.getPosition();
        }

        private IFoodSource GetFoodSource(int seed) {
            if (this.isTraining) {
                return new TrainingFood(seed);
            }

            return new PostTrainingFood();
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
            float xDelta = Math.Abs((this.foodSourse.getPosition().X - this.headX) / 25f);
            float yDelta = Math.Abs((this.foodSourse.getPosition().Y - this.headY) / 25f);
            this.lastDistanceToFood = xDelta + yDelta;
        }


        public List<Vector2> GetBody() {
            return this.body;
        }


        public bool isDead() {
            int maxSteps = 100 + this.length * 5;
            if (
                this.headX >= Constants.WINDOWWIDTH ||
                this.headY >= Constants.WINDOWHEIGHT ||
                this.headX < 0 || this.headY < 0
                ) {
                this.fitness -= 60;
                return true;

            } else if (this.stepsSinceEaten > maxSteps) {
                this.fitness -= 70;
                return true;

            }else if (this.body[..^1].Contains(this.body[this.length - 1])) {
                this.fitness -= 50;
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


        private void CloserToFoodReward(float xDelta, float yDelta) {
            float currentFoodDistance = xDelta + yDelta;
            float stepDelta = 0.0f;
            float scale = 1.5f;
            stepDelta = this.lastDistanceToFood - currentFoodDistance;

            this.fitness += stepDelta * scale;
            this.lastDistanceToFood = currentFoodDistance;
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



        private float GetDistanceToWall(RelativeDirection direction) {
            int dx = 0;
            int dy = 0;
            if (direction == RelativeDirection.Straight) {
                dx = this.xDir / Constants.SCALE;
                dy = this.yDir / Constants.SCALE;
            } else if (direction == RelativeDirection.Left) {
                dx = this.yDir / Constants.SCALE;
                dy = -this.xDir / Constants.SCALE;
            } else if ( direction == RelativeDirection.Right) {
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

                currentX += dx;
                currentY += dy;
                distance++;
            }

            return 1.0f / distance;
        }


        public float[] GetInputs() {
            float hDistanceToFood = (this.foodSourse.getPosition().X - this.headX) / (float)Constants.SCALE;
            float vDistanceToFood = (this.foodSourse.getPosition().Y - this.headY) / (float)Constants.SCALE;

            float snakeForwardX = this.xDir / Constants.SCALE;
            float snakeForwardY = this.yDir / Constants.SCALE;
            float snakeRightX = -snakeForwardY;
            float snakeRightY = snakeForwardX;

            float relForwardFood = hDistanceToFood * snakeForwardX + vDistanceToFood * snakeForwardY;
            float relRightFood = hDistanceToFood * snakeRightX + vDistanceToFood * snakeRightY;

            float forwardWallDistance = this.GetDistanceToWall(RelativeDirection.Straight);
            float rightWallDistance = this.GetDistanceToWall(RelativeDirection.Right);
            float leftWallDistance = this.GetDistanceToWall(RelativeDirection.Left);

            float forwardBodyDistance = GetDistanceToBody(RelativeDirection.Straight);
            float leftBodyDistance = GetDistanceToBody(RelativeDirection.Left);
            float rightBodyDistance = GetDistanceToBody(RelativeDirection.Right);


            float maxDimension = Math.Max(Constants.NUMCOLS, Constants.NUMROWS);
            return new float[] {
                relForwardFood / maxDimension,
                relRightFood / maxDimension,

                forwardWallDistance,
                leftWallDistance,
                rightWallDistance,

                forwardBodyDistance,
                leftBodyDistance,
                rightBodyDistance

            };
        }


        public void Reset(int seed) {
            this.length = 1;
            this.body = new List<Vector2>();
            this.body.Add(new Vector2(Constants.WINDOWWIDTH / 2, Constants.WINDOWHEIGHT / 2));
            this.foodSourse = this.GetFoodSource(seed);
            this.speed = Constants.SCALE;
            this.xDir = 0;
            this.yDir = -this.speed;
            this.fitness = 0;
            this.stepsSinceEaten = 0;
            this.headX = (int)this.body[0].X;
            this.headY = (int)this.body[0].Y;
            float xDelta = Math.Abs((this.foodSourse.getPosition().X - this.headX) / 25f);
            float yDelta = Math.Abs((this.foodSourse.getPosition().Y - this.headY) / 25f);
            this.lastDistanceToFood = xDelta + yDelta ;

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

            float hDistanceToFood = Math.Abs((this.foodSourse.getPosition().X - this.headX) / 25f);
            float vDistanceToFood = Math.Abs((this.foodSourse.getPosition().Y - this.headY) / 25f);
            this.CloserToFoodReward(hDistanceToFood, vDistanceToFood);

            if (this.CanEat()) {
                this.fitness += 100;
                this.Eat();
                this.fitness++;
                return false;

            }

            return this.isDead();
        }


        public float GetFitness() {
            return this.fitness;
        }
    }
}