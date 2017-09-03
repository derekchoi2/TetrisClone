namespace Tetris
{
    public enum TetriminoType
    {
        I = 0,
        J = 1,
        L = 2,
        O = 3,
        S = 4,
        Z = 5,
        T = 6
    }

    public enum MoveDirection
    {
        Up = 0,
        Left = 1,
        Right = 2,
        Down = 3
    }

    public enum BoundarySide
    {
        Left = 0,
        Right = 1,
        Bottom = 2,
        Top = 3
    }

    public enum CollisionCheckType
    {
        Tetrimino = 0,
        Shadow = 1
    }
}
