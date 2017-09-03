using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace Tetris
{
    public class Tetrimino
    {
        public TetriminoType type;
        public Texture2D texture;
        List<Vector2> offsets;
        public List<Rectangle> rectangles; //for collisions only
        public Vector2 startPos = Game1.gameTopLeft + new Vector2(tileSize * 4, 0);
        public Vector2 topLeftPosition;

        public List<Rectangle> shadowRectangles;
        public Vector2 shadowTopLeftPosition;

        public bool active;

        Color color;
        public static int tileSize = 20;

        public Tetrimino(Texture2D texture, TetriminoType type)
        {
            offsets = new List<Vector2>();
            rectangles = new List<Rectangle>();
            shadowRectangles = new List<Rectangle>();
            this.texture = texture;
            this.type = type;

            topLeftPosition = startPos;
            shadowTopLeftPosition = startPos;

            active = true;

            switch (type)
            {
                case TetriminoType.I:
                    offsets.Add(new Vector2(-1, 0));
                    offsets.Add(new Vector2(0, 0));
                    offsets.Add(new Vector2(1, 0));
                    offsets.Add(new Vector2(2, 0));
                    color = Color.Cyan;
                    break;
                case TetriminoType.J:
                    offsets.Add(new Vector2(-1, 0));
                    offsets.Add(new Vector2(0, 0));
                    offsets.Add(new Vector2(1, 0));
                    offsets.Add(new Vector2(1, 1));
                    color = Color.DeepSkyBlue;
                    break;
                case TetriminoType.L:
                    offsets.Add(new Vector2(-1, 1));
                    offsets.Add(new Vector2(-1, 0));
                    offsets.Add(new Vector2(0, 0));
                    offsets.Add(new Vector2(1, 0));
                    color = Color.Orange;
                    break;
                case TetriminoType.O:
                    offsets.Add(new Vector2(0, 0));
                    offsets.Add(new Vector2(1, 0));
                    offsets.Add(new Vector2(0, -1));
                    offsets.Add(new Vector2(1, -1));
                    color = Color.Yellow;
                    break;
                case TetriminoType.S:
                    offsets.Add(new Vector2(-1, 1));
                    offsets.Add(new Vector2(0, 1));
                    offsets.Add(new Vector2(0, 0));
                    offsets.Add(new Vector2(1, 0));
                    color = Color.LawnGreen;
                    break;
                case TetriminoType.T:
                    offsets.Add(new Vector2(-1, 0));
                    offsets.Add(new Vector2(0, 0));
                    offsets.Add(new Vector2(1, 0));
                    offsets.Add(new Vector2(0, 1));
                    color = Color.MediumPurple;
                    break;
                case TetriminoType.Z:
                    offsets.Add(new Vector2(-1, 0));
                    offsets.Add(new Vector2(0, 0));
                    offsets.Add(new Vector2(0, 1));
                    offsets.Add(new Vector2(1, 1));
                    color = Color.Red;
                    break;
            }
            UpdateRectangles();
        }

        public void RotateClockwise()
        {
            if (type != TetriminoType.O)
            {
                Matrix rotation = Matrix.CreateRotationZ(MathHelper.ToRadians(90));
                for (int i = 0; i < offsets.Count; i++)
                {
                    Vector2 newOffset = Vector2.Transform(offsets[i], rotation);
                    //round to keep things lined up
                    newOffset.X = (int)Math.Round(newOffset.X);
                    newOffset.Y = (int)Math.Round(newOffset.Y);

                    offsets[i] = newOffset;
                }
                shadowTopLeftPosition = topLeftPosition;
                UpdateRectangles();
            }
        }

        public void UpdateRectangles()
        {
            //update rectangles
            rectangles.Clear();
            foreach (Vector2 offset in offsets)
                rectangles.Add(new Rectangle((int)(offset.X * tileSize) + (int)topLeftPosition.X, (int)(offset.Y * tileSize) + (int)topLeftPosition.Y, tileSize, tileSize));

            //shadow rectangles
            shadowRectangles.Clear();
            foreach (Vector2 offset in offsets)
                shadowRectangles.Add(new Rectangle((int)(offset.X * tileSize) + (int)shadowTopLeftPosition.X, (int)(offset.Y * tileSize) + (int)shadowTopLeftPosition.Y, tileSize, tileSize));
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            //tetrimino
            foreach (Rectangle rectangle in rectangles)
                spriteBatch.Draw(texture, rectangle, color);

            //shadow
            //don't draw shadow if not active
            if (active)
                foreach (Rectangle rectangle in shadowRectangles)
                    spriteBatch.Draw(texture, rectangle, new Color(color, 50));
        }

        public void Move(MoveDirection dir)
        {
            switch (dir)
            {
                case MoveDirection.Left:
                    topLeftPosition.X -= tileSize;
                    break;
                case MoveDirection.Right:
                    topLeftPosition.X += tileSize;
                    break;
                case MoveDirection.Down:
                    topLeftPosition.Y += tileSize;
                    break;
                case MoveDirection.Up:
                    topLeftPosition.Y -= tileSize;
                    break;
            }
            shadowTopLeftPosition = topLeftPosition;
            UpdateRectangles();
        }

        public void RemoveSquare(Vector2 position)
        {
            Vector2 offset = position - topLeftPosition;
            offset /= tileSize;
            offsets.Remove(offset);
            UpdateRectangles();
        }

        public void ApplyGravity(Vector2 position)
        {
            Vector2 offset = position - topLeftPosition;
            offset /= tileSize;
            int index = offsets.IndexOf(offset);
            Vector2 oCopy = offsets[index];
            oCopy.Y += 1;
            offsets[index] = oCopy;

            UpdateRectangles();
        }
    }
}
