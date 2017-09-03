using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Tetris
{
    public class Inputs
    {
        KeyboardState prevKeyboardState;
        KeyboardState currKeyboardState;

        TimeSpan keyRepeatStartTime;

        Game1 game;

        public Inputs(Game1 game)
        {
            this.game = game;
        }

        public void CheckUserInput(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                game.Exit();

            if (prevKeyboardState == null)
            {
                prevKeyboardState = Keyboard.GetState();
                currKeyboardState = prevKeyboardState;
            }
            else
            {
                prevKeyboardState = currKeyboardState;
                currKeyboardState = Keyboard.GetState();
            }
            if (!Game1.gameOver)
            {
                //Left
                if (currKeyboardState.IsKeyDown(Keys.Left) && !prevKeyboardState.IsKeyDown(Keys.Left))
                {
                    game.MoveLeft();
                    keyRepeatStartTime = gameTime.TotalGameTime;
                }
                else if (currKeyboardState.IsKeyDown(Keys.Left) && prevKeyboardState.IsKeyDown(Keys.Left))
                {
                    if (keyRepeatStartTime.CompareTo(gameTime.TotalGameTime - TimeSpan.FromSeconds(0.2)) < 0)
                        game.MoveLeft();
                }

                //Right
                if (currKeyboardState.IsKeyDown(Keys.Right) && !prevKeyboardState.IsKeyDown(Keys.Right))
                {
                    game.MoveRight();
                    keyRepeatStartTime = gameTime.TotalGameTime;
                }
                else if (currKeyboardState.IsKeyDown(Keys.Right) && prevKeyboardState.IsKeyDown(Keys.Right))
                {
                    if (keyRepeatStartTime.CompareTo(gameTime.TotalGameTime - TimeSpan.FromSeconds(0.2)) < 0)
                        game.MoveRight();
                }

                //Down
                if (currKeyboardState.IsKeyDown(Keys.Down) && !prevKeyboardState.IsKeyDown(Keys.Down))
                {
                    game.MoveDown();
                    keyRepeatStartTime = gameTime.TotalGameTime;
                }
                else if (currKeyboardState.IsKeyDown(Keys.Down) && prevKeyboardState.IsKeyDown(Keys.Down))
                {
                    if (keyRepeatStartTime.CompareTo(gameTime.TotalGameTime - gameTime.ElapsedGameTime) < 0)
                    {
                        keyRepeatStartTime = gameTime.TotalGameTime;
                        game.MoveDown();
                    }
                }

                //Space
                if (currKeyboardState.IsKeyDown(Keys.Space) && !prevKeyboardState.IsKeyDown(Keys.Space))
                    game.Drop();

                //Up
                if (currKeyboardState.IsKeyDown(Keys.Up) && !prevKeyboardState.IsKeyDown(Keys.Up))
                    game.Rotate();

                //Hold
                if (currKeyboardState.IsKeyDown(Keys.C) && !prevKeyboardState.IsKeyDown(Keys.C))
                    game.Hold();

            }

            //R
            if (currKeyboardState.IsKeyDown(Keys.R) && !prevKeyboardState.IsKeyDown(Keys.R) && Game1.gameOver)
                game.Reset();
        }

    }
}
