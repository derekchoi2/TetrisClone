using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Tetris
{
    public class Boundary
    {
        GraphicsDevice graphics;

        Rectangle box;
        int boundaryWidth = 5;
        BoundarySide side; //to know which way to repel collisions

        Texture2D texture;

        public Boundary(BoundarySide side, GraphicsDevice graphics )
        {
            this.graphics = graphics;
            this.side = side;

            switch (side)
            {
                case BoundarySide.Left:
                    box = new Rectangle((Game1.gameWidth / 2) - (Tetrimino.tileSize * 5) - boundaryWidth, (Game1.gameHeight - Tetrimino.tileSize * 25), boundaryWidth, Tetrimino.tileSize * 21);
                    break;
                case BoundarySide.Right:
                    box = new Rectangle((Game1.gameWidth / 2) + (Tetrimino.tileSize * 5), (Game1.gameHeight - Tetrimino.tileSize * 25), boundaryWidth, Tetrimino.tileSize * 21);
                    break;
                case BoundarySide.Bottom:
                    box = new Rectangle((Game1.gameWidth / 2) - (Tetrimino.tileSize * 5) - boundaryWidth, (Game1.gameHeight - Tetrimino.tileSize * 4), Tetrimino.tileSize * 10 + boundaryWidth * 2, boundaryWidth);
                    break;
                case BoundarySide.Top:
                    box = new Rectangle((Game1.gameWidth / 2) - (Tetrimino.tileSize * 5) - boundaryWidth, (Game1.gameHeight - Tetrimino.tileSize * 24 - 2), Tetrimino.tileSize * 10 + boundaryWidth * 2, 2);
                    break;
            }

            texture = new Texture2D(graphics, box.Width, box.Height);
            Color[] color = new Color[box.Width * box.Height];
            for (int i = 0; i < color.Length; i++) color[i] = new Color(0, 0, 0);
            texture.SetData(color);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(texture, box, Color.White);
        }

        //for rotations that go out of bounds
        public void CheckCollision(Tetrimino t)
        {
            if (side != BoundarySide.Top)
            {
                for (int i = 0; i < t.rectangles.Count; i++)
                {
                    while (box.Intersects(t.rectangles[i]))
                    {
                        //move back according to boundary side collided with
                        switch (side)
                        {
                            case BoundarySide.Left:
                                t.Move(MoveDirection.Right);
                                break;
                            case BoundarySide.Right:
                                t.Move(MoveDirection.Left);
                                break;
                            case BoundarySide.Bottom:
                                t.Move(MoveDirection.Up);
                                break;
                        }
                    }
                }
            }
        }

        public bool CheckPotentialCollision(CollisionCheckType type, Tetrimino tetrimino, MoveDirection direction)
        {
            List<Rectangle> check;
            if (type == CollisionCheckType.Tetrimino) check = tetrimino.rectangles;
            else check = tetrimino.shadowRectangles;
            switch (direction)
            {
                case MoveDirection.Left:
                    if (side == BoundarySide.Left)
                    {
                        foreach (Rectangle rectangle in check)
                        {
                            Rectangle r = rectangle; //copy
                            r.X -= Tetrimino.tileSize;
                            if (r.X <= box.X) return true;
                        }
                    }
                    break;
                case MoveDirection.Right:
                    if (side == BoundarySide.Right)
                    {
                        foreach (Rectangle rectangle in check)
                        {
                            Rectangle r = rectangle; //copy
                            r.X += Tetrimino.tileSize;
                            if (r.X >= box.X) return true;
                        }
                    }
                    break;
                case MoveDirection.Down:
                    if (side == BoundarySide.Bottom)
                    {
                        foreach (Rectangle rectangle in check)
                        {
                            Rectangle r = rectangle; //copy
                            r.Y += Tetrimino.tileSize;
                            if (r.Y >= box.Y)
                                return true;
                        }
                    }
                    break;
            }
            return false;
        }

    }
}
