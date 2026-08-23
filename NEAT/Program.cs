using NEAT.Neat;
using NEAT.NN;
using NEAT.SnakeGame;
using Raylib_cs;
using System.Numerics;

namespace NEAT {

    public static class Constants {
        public const int WINDOWHEIGHT = 1200;
        public const int WINDOWWIDTH = 1200;
        public const int SCALE = 25;
        public const int NUMROWS = WINDOWHEIGHT / SCALE;
        public const int NUMCOLS = WINDOWWIDTH / SCALE;
        public static readonly Vector2[] grid = new Vector2[NUMROWS * NUMCOLS];

        static Constants() {
            int index = 0;
            for (int i = 0; i < NUMCOLS; i++) {
                for (int j = 0; j < NUMROWS; j++) {
                    grid[index] = new Vector2(i * SCALE, j * SCALE);
                    index++;
                }
            }
        }
    }



    internal class Program {
        static void Main(string[] args) {
            // Testing only
            NeatManager neatManager = new NeatManager(9, 3, () => new Snake(isTraining: true), 150);
            neatManager.Train(5000, 5000);
            Console.WriteLine();

            NeuralNetwork champNetork = neatManager.GetChampionNetwork();

            Raylib.InitWindow(Constants.WINDOWWIDTH, Constants.WINDOWHEIGHT, "Snek Game");
            Raylib.SetTargetFPS(10);
            bool isGameOver = false;

            Snake snake = new Snake(false);
            List<Vector2> body = snake.GetBody();
            while (!Raylib.WindowShouldClose()) {

                if (isGameOver) {
                    if (Raylib.IsKeyPressed(KeyboardKey.R)) {
                        // Restart the game by resetting the objects and flag
                        snake.Reset(123);
                        body = snake.GetBody();
                        isGameOver = false;
                    }

                } else {
                    float[] inputs = snake.GetInputs();
                    float[] outputs = champNetork.feedForward(inputs);
                    isGameOver = snake.Step(outputs);

                }

                Raylib.BeginDrawing();
                Raylib.ClearBackground(Color.Black);

                if (isGameOver) {
                    Raylib.DrawText("GAME OVER", Constants.WINDOWWIDTH / 2 - 100, Constants.WINDOWHEIGHT / 2 - 40, 30, Color.Red);
                    Raylib.DrawText($"Score: {snake.GetScore()}", Constants.WINDOWWIDTH / 2 - 50, Constants.WINDOWHEIGHT / 2 + 10, 20, Color.White);
                    Raylib.DrawText("Press [R] to Restart", Constants.WINDOWWIDTH / 2 - 95, Constants.WINDOWHEIGHT / 2 + 50, 16, Color.DarkGray);

                } else {

                    Vector2 foodPosition = snake.GetFoodPosiotion();

                    //drawing food
                    Raylib.DrawRectangle(
                        (int)foodPosition.X,
                        (int)foodPosition.Y,
                        (int)(Constants.SCALE),
                        (int)(Constants.SCALE),
                        Color.Red);


                    //drawing snake
                    Raylib.DrawRectangle(
                            (int)body[body.Count() - 1].X,
                            (int)body[body.Count() - 1].Y,
                            Constants.SCALE,
                            Constants.SCALE,
                            Color.Yellow);

                    for (int i = 0; i < body.Count() - 1; i++) {
                        Raylib.DrawRectangle(
                            (int)body[i].X,
                            (int)body[i].Y,
                            Constants.SCALE,
                            Constants.SCALE,
                            Color.Green);
                    }
                }
                Raylib.EndDrawing();

            }

            Raylib.CloseWindow();
        }
    }
}