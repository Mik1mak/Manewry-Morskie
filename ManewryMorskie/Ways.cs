namespace ManewryMorskie
{
    public enum Ways
    {
        None = 0,
        Left = 1,
        Top = 2,
        TopLeft = Top | Left,
        Right = 4,
        TopRight = Top | Right,
        Bottom = 8,
        BottomLeft = Bottom | Left,
        BottomRight = Bottom | Right,
    }
}
