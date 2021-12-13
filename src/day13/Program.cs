using System.Text.RegularExpressions;

var input = InputParser.Parse("input.txt");

Console.WriteLine($"Result A:{Fold(input.Points, input.Folds.First()).Count}");
Console.WriteLine($"Result B:");
Print(input.Folds.Aggregate(new List<HashSet<Point>>(), (acc, f) =>
{
    acc.Add(Fold(acc.Count > 0 ? acc.Last() : input.Points, f));
    return acc;
}).Last(), 2);

static HashSet<Point> Fold(HashSet<Point> input, Fold fold)
    => input.Aggregate(new HashSet<Point>(), (acc, p) =>
    {
        var point = fold.X > 0
        ? p.X < fold.X ? new Point(p.X, p.Y) : new Point(fold.X + (fold.X - p.X), p.Y)
        : p.Y < fold.Y ? new Point(p.X, p.Y) : new Point(p.X, fold.Y + (fold.Y - p.Y));

        if (!acc.Contains(point))
            acc.Add(point);

        return acc;
    });

static void Print(HashSet<Point> points, int rowsOffset = 0)
{    
    var maxR = points.Max(x => x.Y);
    var maxC = points.Max(x => x.X);
    for (var r = 0; r <= maxR; r++)
        for (var c = 0; c <= maxC; c++)
        {
            Console.SetCursorPosition(c, r + rowsOffset);
            if (points.Contains(new Point(c, r)))
            {
                Console.Write("#");
            }
            else
            {
                Console.Write(".");
            }
        }

    Console.WriteLine();
}

internal static class InputParser
{
    private static readonly Regex DotRegEx = new Regex(@"^(?<x>\d+)([,])(?<y>\d+)$", RegexOptions.Compiled | RegexOptions.IgnoreCase);
    private static readonly Regex FoldRegEx = new Regex(@"(?<axis>[x,y]{1})([=])(?<value>\d+)$", RegexOptions.Compiled | RegexOptions.IgnoreCase);

    public static (HashSet<Point> Points, List<Fold> Folds) Parse(string path)
        => File.ReadAllLines(path).Aggregate((Points: new HashSet<Point>(), Operations: new List<Fold>()), (acc, l) =>
        {
            if (DotRegEx.IsMatch(l))
            {
                var m = DotRegEx.Match(l);
                acc.Points.Add(new Point(int.Parse(m.Groups["x"].Value), int.Parse(m.Groups["y"].Value)));
            }
            else if (FoldRegEx.IsMatch(l))
            {
                var m = FoldRegEx.Match(l);
                if (m.Groups["axis"].Value == "x")
                {
                    acc.Operations.Add(new Fold(int.Parse(m.Groups["value"].Value), 0));
                }
                else
                {
                    acc.Operations.Add(new Fold(0, int.Parse(m.Groups["value"].Value)));
                }
            }

            return acc;
        });
}

record Point(int X, int Y);
record Fold(int X, int Y);