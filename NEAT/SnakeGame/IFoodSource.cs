using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace NEAT.SnakeGame {
    internal interface IFoodSource {

        Vector2 getPosition();

        void changePosition(List<Vector2>? snake);

    }
}
