using Microsoft.Xna.Framework.Graphics;

namespace Tetris
{
    public class TetriminoFactory
    {
        Texture2D texture;

        public TetriminoFactory(Texture2D texture)
        {
            this.texture = texture;
        }

        public Tetrimino Create(TetriminoType type)
        {
            return new Tetrimino(texture, type);
        }
    }
}
