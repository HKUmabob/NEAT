using Microsoft.VisualBasic;
using NEAT.SnakeGame;
using Raylib_cs;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Text;

namespace NEAT.SnakeGame {
    public class Snake {
        private int length;
        private List<Vector2> body;
        private int headX;
        private int headY;
        private int xDir;
        private int yDir;
        private int speed;
        private Food foodSourse;


        public Snake(Food foodSourse) {
            this.length = 1;
            this.body = new List<Vector2>();
            this.body.Add(new Vector2(Constants.WINDOWWIDTH / 2, Constants.WINDOWHEIGHT / 2));
            this.foodSourse = foodSourse;
            this.speed = Constants.SCALE;
            this.xDir = 0;
            this.yDir = -this.speed;

        }

        public bool canEat() {
            if (this.foodSourse.getPosition().Equals(this.body[this.length - 1])) {
                return true;
            }

            return false;
        }

        public void eat() {

            this.body.Add(
                new Vector2(
                    this.body[this.length - 1].X,
                    this.body[this.length - 1].Y
                    )
                );

            this.length++;
            this.foodSourse.changePosition(this.body);
        }


        public List<Vector2> getBody() {
            return this.body;
        }


        public void update() {

            this.headX = (int)this.body[this.length - 1].X;
            this.headY = (int)this.body[this.length - 1].Y;

            this.headX += this.xDir;
            this.headY += this.yDir;

            this.body.RemoveAt(0);
            this.body.Add(new Vector2(this.headX, this.headY));
        }

        public bool isDead() {
            if (
                this.headX >= Constants.WINDOWWIDTH ||
                this.headY >= Constants.WINDOWHEIGHT ||
                this.headX < 0 || this.headY < 0
                ) {
                return true;
            }

            if (this.body[..^1].Contains(this.body[this.length - 1])) {
                return true;
            }

            return false;
        }

        public int getScore() {
            return this.length;
        }

        public void setDir(RelativeDirection direction) {
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

    }
}
