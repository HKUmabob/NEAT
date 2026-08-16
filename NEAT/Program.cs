using NEAT.Neat;
using NEAT.SnakeGame;
using System.Numerics;

namespace NEAT {

    public static class Constants {
        public const int WINDOWHEIGHT = 900;
        public const int WINDOWWIDTH = 850;
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
            NeatManager neatManager = new NeatManager(9, 3, () => new Snake(), 300);
            for (int i = 0; i < 700; i++) {
                neatManager.EvaluatePopulation();
            }
            Console.WriteLine();

        }
    }
}

