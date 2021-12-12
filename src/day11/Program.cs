var input = File.ReadAllLines("input.txt")
    .Select((x, i) => (Index: i, Line: x))
    .Aggregate(new Dictionary<Location, int>(), (acc, x) =>
    {
        for (var i = 0; i < x.Line.Length; i++)
            acc.Add(new Location(x.Index, i), (int)char.GetNumericValue(x.Line[i]));
        return acc;
    });

var result = Simulate(input);

Console.WriteLine($"Result A: {FindA(result)}");
Console.WriteLine($"Result B: {FindB(result)}");

static int FindA(Dictionary<int, int> input)
    => input.Take(100).Select(x => x.Value).Sum();

static int FindB(Dictionary<int, int> input)
    => input.Last().Key;

static Dictionary<int, int> Simulate(Dictionary<Location, int> input)
{
    var result = new Dictionary<int, int>();
    var step = 1;
    while (true)
    {
        result.Add(step, 0);

        var flashedCurrentStep = new HashSet<Location>();

        foreach (var location in input.Keys)
            input[location]++;

        foreach (var octopus in input.Where(x => x.Value > 9))
        {
            var toCheck = new Queue<Location>();
            toCheck.Enqueue(octopus.Key);

            while (toCheck.Count > 0)
            {
                var currentLoction = toCheck.Dequeue();

                if (flashedCurrentStep.Contains(currentLoction))
                    continue;

                if (input[currentLoction] > 9)
                {
                    flashedCurrentStep.Add(currentLoction);
                    result[step]++;
                    input[currentLoction] = 0;

                    foreach (var adj in Adjacent(input, currentLoction))
                    {
                        if (!flashedCurrentStep.Contains(adj))
                        {
                            input[adj]++;                            
                            toCheck.Enqueue(adj);
                        }
                    }
                }
            }
        }

        if (flashedCurrentStep.Count == input.Count)
            return result;

        step++;
    }
}

static IEnumerable<Location> Adjacent(Dictionary<Location, int> input, Location location)
{
    int Rows = 10;
    const int Columns = 10;

    if (location.Row - 1 >= 0 && location.Column - 1 >= 0)
        yield return new Location(location.Row - 1, location.Column - 1);
    if (location.Row - 1 >= 0)
        yield return new Location(location.Row - 1, location.Column);
    if (location.Row - 1 >= 0 && location.Column + 1 < Columns)
        yield return new Location(location.Row - 1, location.Column + 1);

    if (location.Column - 1 >= 0)
        yield return new Location(location.Row, location.Column - 1);
    if (location.Column + 1 < Columns)
        yield return new Location(location.Row, location.Column + 1);

    if (location.Row + 1 < Rows && location.Column - 1 >= 0)
        yield return new Location(location.Row + 1, location.Column - 1);
    if (location.Row + 1 < Rows)
        yield return new Location(location.Row + 1, location.Column);
    if (location.Row + 1 < Rows && location.Column + 1 < Columns)
        yield return new Location(location.Row + 1, location.Column + 1);
}

record Location(int Row, int Column);