var lines = File.ReadAllLines("input.txt");
var r = lines.Count();
var c = lines.First().Length;
var input = new byte[r, c];
lines.Select((x, i) => (Index: i, Line: x)).Aggregate(input, (acc, x) =>
 {
     for (var i = 0; i < x.Line.Length; i++)
         acc[x.Index, i] = (byte)char.GetNumericValue(x.Line[i]);
     return acc;
 });

Console.WriteLine($"Result A: {FindA(input)}");
Console.WriteLine($"Result B: {FindB(input)}");

static int FindA(byte[,] input)
    => Lows(input).Select(x => x.height + 1).Sum();

static int FindB(byte[,] input)
    => Basins(input).Values.OrderByDescending(x => x.Count).Take(3).Aggregate(1, (acc, x) => acc *= x.Count);

static Dictionary<Location, List<Location>> Basins(byte[,] input)
{
    var basins = new Dictionary<Location, List<Location>>();

    foreach (var low in Lows(input))
    {
        basins.Add(low, new List<Location>());
        var toCheck = new Queue<Location>();

        toCheck.Enqueue(low);

        while (toCheck.Count > 0)
        {
            var curr = toCheck.Dequeue();

            if (basins[low].Contains(curr))
                continue;

            basins[low].Add(curr);

            foreach (var adj in Adjacent(input, curr.r, curr.c))
            {
                if (adj.height != 9)
                {
                    toCheck.Enqueue(adj);
                }
            }
        }
    }
    return basins;
}

static Location[] Lows(byte[,] input)
{
    var lows = new List<Location>();
    for (int r = 0; r < input.GetLength(0); r++)
        for (int c = 0; c < input.GetLength(1); c++)
        {
            var adj = Adjacent(input, r, c);
            if (adj.All(x => x.height > input[r, c]))
            {
                lows.Add(new Location(r, c, input[r, c]));
            }
        }
    return lows.ToArray();
}

static IEnumerable<Location> Adjacent(byte[,] input, int r, int c)
{
    if (c - 1 >= 0)
        yield return new Location(r, c - 1, input[r, c - 1]);
    if (r - 1 >= 0)
        yield return new Location(r - 1, c, input[r - 1, c]);
    if (c + 1 < input.GetLength(1))
        yield return new Location(r, c + 1, input[r, c + 1]);
    if (r + 1 < input.GetLength(0))
        yield return new Location(r + 1, c, input[r + 1, c]);
}

record Location(int r, int c, byte height);