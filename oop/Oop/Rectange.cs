namespace Oop;

public sealed class Rectangle
{
    private double _x;
    private double _y;
    private double _width;
    private double _height;

    public Rectangle(double x, double y, double width, double height)
    {
        X = x;
        Y = y;
        Width = width;
        Height = height;
    }

    public double X
    {
        get => _x;
        set
        {
            if (value < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(value), "Координата X не может быть отрицательной.");
            }
            _x = value;
        }
    }

    public double Y
    {
        get => _y;
        set
        {
            if (value < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(value), "Координата Y не может быть отрицательной.");
            }
            _y = value;
        }
    }

    public double Width
    {
        get => _width;
        set
        {
            if (value <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(value), "Ширина должна быть строго больше нуля.");
            }
            _width = value;
        }
    }

    public double Height
    {
        get => _height;
        set
        {
            if (value <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(value), "Высота должна быть строго больше нуля.");
            }
            _height = value;
        }
    }

    public double Area => _width * _height;
    public double Perimeter => 2 * (_width + _height);
}