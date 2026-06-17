namespace DataTypes.Task2;

public static class DiamondPrinter
{
    public static void GenerateDiamondFromSymbolX(int N)
    {
        var spaces = new string[N, N];

        for (int i = 0; i < N; i++)
            for (int j = 0; j < N; j++)
                spaces[i, j] = " ";

        var center = (N - 1) / 2;

        for (int r = 0; r < N; r++)
        {
            int offset = center - Math.Abs(center - r);

            spaces[r, center + offset] = spaces[r, center - offset] = "x";
        }

        spaces[center, center] = " ";

        for (int i = 0; i < N; i++)
        {
            for (int j = 0; j < N; j++)
            {
                Console.Write(spaces[i, j]);
            }
            Console.WriteLine();
        }
    }
}