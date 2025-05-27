using System.Drawing;

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

    // New properties to make comparisons easier
    public float Left => X;
    public float Right => X + Width;
    public float Top => Y;
    public float Bottom => Y + Height;

    public bool IsLargeEnough(float minWidth, float minHeight)
    {
        return Width >= minWidth && Height >= minHeight;
    }

    public override string ToString()
    {
        return $"Rectangle(X: {X}, Y: {Y}, Width: {Width}, Height: {Height})";
    }

    public PointF GetCenter()
    {
        return new PointF(X + Width / 2f, Y + Height / 2f);
    }
}
