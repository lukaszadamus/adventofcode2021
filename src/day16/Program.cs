var input = InputParser.Load("input.txt");



var foo = 1;

internal static class InputParser
{
    private static readonly Queue<char> _bits = new Queue<char>();

    public static Queue<char> Load(string path)
    {
        var line = File.ReadAllText(path);

        line.Aggregate(_bits, (acc, c) =>
        {
            foreach (var bit in Map(c))
                _bits.Enqueue(bit);

            return acc;
        });

        return _bits;
    }

    private static char[] Map(char input)
        => input switch
        {
            '0' => new char[] { '0','0','0','0' },
            '1' => new char[] { '0','0','0','1' },
            '2' => new char[] { '0','0','1','0' },
            '3' => new char[] { '0','0','1','1' },
            '4' => new char[] { '0','1','0','0' },
            '5' => new char[] { '0','1','0','1' },
            '6' => new char[] { '0','1','1','0' },
            '7' => new char[] { '0','1','1','1' },
            '8' => new char[] { '1','0','0','0' },
            '9' => new char[] { '1','0','0','1' },
            'A' => new char[] { '1','0','1','0' },
            'B' => new char[] { '1','0','1','1' },
            'C' => new char[] { '1','1','0','0' },
            'D' => new char[] { '1','1','0','1' },
            'E' => new char[] { '1','1','1','0' },
            'F' => new char[] { '1','1','1','1' },
            _ => new char[0],
        };
}