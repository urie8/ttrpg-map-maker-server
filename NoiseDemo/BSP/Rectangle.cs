public struct Rectangle
{
    public float X, Y;
    public float Width, Height;

    public Rectangle(float x, float y, float width, float height)
    {
        X = x;
        Y = y;
        Width = width;
        Height = height;
    }

    public bool IsLargeEnough(float minWidth, float minHeight)
    {
        return Width >= minWidth && Height >= minHeight;
    }
}
