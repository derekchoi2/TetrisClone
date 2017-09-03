using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Tetris
{
	/// <summary>
	/// This is the main type for your game.
	/// </summary>
	public class Game1 : Game
	{
		public static int gameHeight = 640;
		public static int gameWidth = 800;

		public static Vector2 gameTopLeft = new Vector2((gameWidth / 2) - (Tetrimino.tileSize * 5), gameHeight - Tetrimino.tileSize * 24);

		RandomBag randomBag;

		Inputs inputs;
		GameTime gameTime;

		TimeSpan lastDrop = TimeSpan.FromSeconds(0);
		int level;
		int linesCleared;

		public static bool gameOver = false;

		public static TetriminoFactory factory;

		List<Tetrimino> tetriminos;
		int currentTetriminoIndex;
		Tetrimino nextTetrimino;
		Tetrimino holdTetrimino;

		GraphicsDeviceManager graphics;
		SpriteBatch spriteBatch;

		Texture2D tileTexture;

		List<Boundary> boundaries;

		SpriteFont font;
		Vector2 holdTextPos;
		Vector2 nextTextPos;
		Vector2 levelTextPos;
		Vector2 linesTextPos;

		Vector2 nextPos;
		Vector2 holdPos;

		bool held;

		public Game1()
		{
			Content.RootDirectory = "Content";

			IsMouseVisible = true;
			IsFixedTimeStep = false;

			graphics = new GraphicsDeviceManager(this);
			graphics.IsFullScreen = false;
			graphics.PreferredBackBufferHeight = gameHeight;
			graphics.PreferredBackBufferWidth = gameWidth;
		}

		protected override void Initialize()
		{
			LoadContent();
			//tetrimino factory
			factory = new TetriminoFactory(tileTexture);

			holdTextPos = new Vector2((gameWidth / 2) - Tetrimino.tileSize * 8 - font.MeasureString("HOLD").X, gameHeight / 4);
			nextTextPos = new Vector2((gameWidth / 2) + Tetrimino.tileSize * 8, gameHeight / 4);
			linesTextPos = new Vector2((gameWidth / 2) + Tetrimino.tileSize * 7, 6 * gameHeight / 8);
			levelTextPos = new Vector2((gameWidth / 2) + Tetrimino.tileSize * 7, 5 * gameHeight / 8);

			nextPos = nextTextPos + new Vector2(20, 50);
			holdPos = holdTextPos + new Vector2(20, 50);

			inputs = new Inputs(this);

			boundaries = new List<Boundary>();
			boundaries.Add(new Boundary(BoundarySide.Left, graphics.GraphicsDevice));
			boundaries.Add(new Boundary(BoundarySide.Right, graphics.GraphicsDevice));
			boundaries.Add(new Boundary(BoundarySide.Bottom, graphics.GraphicsDevice));
			boundaries.Add(new Boundary(BoundarySide.Top, graphics.GraphicsDevice));

			Reset(); //everything else set/initialised here
			base.Initialize();
		}

		protected override void LoadContent()
		{
			// Create a new SpriteBatch, which can be used to draw textures.
			spriteBatch = new SpriteBatch(GraphicsDevice);
			tileTexture = Content.Load<Texture2D>("sprites/square");
			font = Content.Load<SpriteFont>("fonts/uiFont");
		}

		protected override void UnloadContent()
		{

		}

		void NewTetrimino()
		{
			if (!gameOver)
			{
				held = false;
				linesCleared += CheckLines();

				nextTetrimino.UpdateRectangles();
				tetriminos.Add(new Tetrimino(tileTexture, nextTetrimino.type));
				currentTetriminoIndex++;

				nextTetrimino = new Tetrimino(tileTexture, (TetriminoType)randomBag.get());
				nextTetrimino.active = false;
				nextTetrimino.topLeftPosition = nextPos;
				nextTetrimino.UpdateRectangles();


				if (currentTetriminoIndex >= 0)
				{
					CalculateShadowPos();
					if (CheckCollisions(CollisionCheckType.Tetrimino, MoveDirection.Down))
						GameOver();
				}


			}
		}

		int CheckLines()
		{
			int linesFound = 0;
			bool lineFound = true;

			while (lineFound)
			{
				//check each row
				for (int row = 0; row < 20; row++)
				{
					bool[] line = new bool[10];
					for (int i = 0; i < line.Length; i++) line[i] = false;
					//check each tile of row
					for (int tile = 0; tile < 10; tile++)
					{
						foreach (Tetrimino t in tetriminos)
						{
							foreach (Rectangle r in t.rectangles)
							{
								if (r.Location.ToVector2() == gameTopLeft + new Vector2(Tetrimino.tileSize * tile, Tetrimino.tileSize * row))
									line[tile] = true;

								if (line[tile]) break; //check next tile
							}
							if (line[tile]) break; //check next tile
						}
					}
					//finished checking row, see if it is a complete line
					foreach (bool b in line)
					{
						if (b) lineFound = true;
						else
						{
							lineFound = false;
							break;
						}
					}
					if (lineFound)
					{
						linesFound++;
						RemoveLine(row);
						break;
					}


				}
			}

			if (linesFound == 4) linesFound *= 2; //if tetris, double line count
			return linesFound;
		}

		void RemoveLine(int row)
		{
			for (int tile = 0; tile < 10; tile++)
			{
				bool found = false;
				foreach (Tetrimino t in tetriminos)
				{
					if (found) break; //check next tile
					foreach (Rectangle r in t.rectangles)
					{
						if (r.Location.ToVector2() == gameTopLeft + new Vector2(Tetrimino.tileSize * tile, Tetrimino.tileSize * row))
						{
							found = true;
							t.RemoveSquare(r.Location.ToVector2());
						}
						if (found) break; //check next tile
					}
				}
			}

			//apply gravity
			foreach (Tetrimino t in tetriminos)
				//if all rectangle is above line then move down 1
				for (int r = 0; r < t.rectangles.Count; r++)
					if (t.rectangles[r].Location.ToVector2().Y < gameTopLeft.Y + row * Tetrimino.tileSize)
						t.ApplyGravity(t.rectangles[r].Location.ToVector2());
		}

		void CalculateShadowPos()
		{
			while (!CheckCollisions(CollisionCheckType.Shadow, MoveDirection.Down))
			{
				tetriminos[currentTetriminoIndex].shadowTopLeftPosition.Y += Tetrimino.tileSize;
				tetriminos[currentTetriminoIndex].UpdateRectangles();
			}
		}

		protected override void Update(GameTime gameTime)
		{
			if (currentTetriminoIndex == -1) NewTetrimino();

			inputs.CheckUserInput(gameTime);

			this.gameTime = gameTime;

			if (linesCleared >= level * 20) level++;

			foreach (Tetrimino t in tetriminos)
				foreach (Boundary b in boundaries)
					b.CheckCollision(t);

			//move down if time is right
			if (lastDrop.CompareTo(gameTime.TotalGameTime - TimeSpan.FromMilliseconds(1000 / level)) < 0)
			{
				MoveDown();
				lastDrop = gameTime.TotalGameTime;
			}

			base.Update(gameTime);
		}

		bool CheckBoundaries(CollisionCheckType type, MoveDirection dir)
		{
			foreach (Boundary b in boundaries)
				if (b.CheckPotentialCollision(type, tetriminos[currentTetriminoIndex], dir)) return true;
			return false;
		}

		bool CheckTetriminos(CollisionCheckType type, MoveDirection dir)
		{
			List<Rectangle> check;
			if (type == CollisionCheckType.Tetrimino) check = tetriminos[currentTetriminoIndex].rectangles;
			else check = tetriminos[currentTetriminoIndex].shadowRectangles;
			foreach (Tetrimino t in tetriminos)
				if (t != tetriminos[currentTetriminoIndex])
					foreach (Rectangle r1 in t.rectangles)
						foreach (Rectangle r2 in check)
						{
							Rectangle r2Copy = r2;
							switch (dir)
							{
								case MoveDirection.Left: r2Copy.X -= Tetrimino.tileSize; break;
								case MoveDirection.Right: r2Copy.X += Tetrimino.tileSize; break;
								case MoveDirection.Down: r2Copy.Y += Tetrimino.tileSize; break;
								case MoveDirection.Up: r2Copy.Y -= Tetrimino.tileSize; break;
							}
							if (r1.Intersects(r2Copy))
								return true;
						}
			return false;
		}

		bool CheckCollisions(CollisionCheckType type, MoveDirection dir)
		{
			return CheckBoundaries(type, dir) || CheckTetriminos(type, dir);
		}

		public void MoveLeft()
		{
			if (!CheckCollisions(CollisionCheckType.Tetrimino, MoveDirection.Left))
			{
				tetriminos[currentTetriminoIndex].Move(MoveDirection.Left);
				CalculateShadowPos();
			}
		}

		public void MoveRight()
		{
			if (!CheckCollisions(CollisionCheckType.Tetrimino, MoveDirection.Right))
			{
				tetriminos[currentTetriminoIndex].Move(MoveDirection.Right);
				CalculateShadowPos();
			}
		}

		public void MoveDown()
		{
			if (!CheckCollisions(CollisionCheckType.Tetrimino, MoveDirection.Down))
			{
				tetriminos[currentTetriminoIndex].Move(MoveDirection.Down);
				CalculateShadowPos();
			}
			else
			{
				tetriminos[currentTetriminoIndex].active = false;
				NewTetrimino();
			}
		}

		public void Drop()
		{
			while (!CheckCollisions(CollisionCheckType.Tetrimino, MoveDirection.Down))
				tetriminos[currentTetriminoIndex].Move(MoveDirection.Down);

			tetriminos[currentTetriminoIndex].active = false;
			NewTetrimino();
		}

		public void Rotate()
		{
			tetriminos[currentTetriminoIndex].RotateClockwise();
			CalculateShadowPos();
		}

		public void Hold()
		{
			if (!held)
			{
				Tetrimino temp;
				if (holdTetrimino != null)
					temp = new Tetrimino(tileTexture, holdTetrimino.type);
				else temp = null;

				holdTetrimino = new Tetrimino(tileTexture, tetriminos[currentTetriminoIndex].type);
				holdTetrimino.active = false;
				holdTetrimino.topLeftPosition = holdPos;
				holdTetrimino.UpdateRectangles();

				if (temp != null)
					tetriminos[currentTetriminoIndex] = new Tetrimino(tileTexture, temp.type);
				else
				{
					tetriminos[currentTetriminoIndex] = new Tetrimino(tileTexture, nextTetrimino.type);

					nextTetrimino = new Tetrimino(tileTexture, (TetriminoType)randomBag.get());
					nextTetrimino.active = false;
					nextTetrimino.topLeftPosition = nextPos;
					nextTetrimino.UpdateRectangles();
				}
				CalculateShadowPos();
			}
			held = true;
		}

		public static void GameOver()
		{
			gameOver = true;
		}

		public void Reset()
		{
			gameOver = false;
			tetriminos = new List<Tetrimino>();
			level = 1;
			linesCleared = 0;
			currentTetriminoIndex = -1;

			randomBag = new RandomBag(7);
			nextTetrimino = new Tetrimino(tileTexture, (TetriminoType)randomBag.get());
			nextTetrimino.topLeftPosition = nextTextPos + new Vector2(0, 100);
			nextTetrimino.UpdateRectangles();
			NewTetrimino();
		}

		void DrawText(SpriteBatch spriteBatch)
		{
			if (gameOver)
				spriteBatch.DrawString(font, "Game Over. Press R to restart", new Vector2(0, 0), Color.Black);

			spriteBatch.DrawString(font, "NEXT", nextTextPos, Color.Black);

			//score
			spriteBatch.DrawString(font, "CLEARED: " + linesCleared, linesTextPos, Color.Black);
			//level
			spriteBatch.DrawString(font, "LEVEL: " + level, levelTextPos, Color.Black);
			//hold
			spriteBatch.DrawString(font, "HOLD", holdTextPos, Color.Black);
		}

		protected override void Draw(GameTime gameTime)
		{
			GraphicsDevice.Clear(Color.CornflowerBlue);

			// TODO: Add your drawing code here
			spriteBatch.Begin();

			//board tetriminos
			foreach (Tetrimino t in tetriminos)
				t.Draw(spriteBatch);

			//next tetrimino
			nextTetrimino.Draw(spriteBatch);

			//hold tetrimino
			if (holdTetrimino != null)
				holdTetrimino.Draw(spriteBatch);

			foreach (Boundary b in boundaries)
				b.Draw(spriteBatch);

			DrawText(spriteBatch);

			spriteBatch.End();

			base.Draw(gameTime);
		}
	}
}
