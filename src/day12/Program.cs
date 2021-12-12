var input = File.ReadAllLines("input.txt")
    .Select(line => (From: new Cave(line.Split('-').First()), To: new Cave(line.Split('-').Last())))
    .Aggregate(new Dictionary<Cave, List<Cave>>(), (acc, caves) =>
    {
        if (!acc.ContainsKey(caves.From))
            acc.Add(caves.From, new List<Cave>());

        if (!caves.From.IsEnd && !caves.To.IsStart)
            acc[caves.From].Add(caves.To);

        if (!acc.ContainsKey(caves.To))
            acc.Add(caves.To, new List<Cave>());

        if (!caves.To.IsEnd && !caves.From.IsStart)
            acc[caves.To].Add(caves.From);

        return acc;
    });

Console.WriteLine($"Result A: {Travers(input, SmallCaveLimit(input)).Count}");
Console.WriteLine($"Result B: {SmallCaveLimitCombinations(input).Aggregate(new List<string>(), (acc, l) => { acc.AddRange(Travers(input, l)); return acc; }, acc => acc.Distinct()).Count()}");

static List<string> Travers(Dictionary<Cave, List<Cave>> input, Dictionary<Cave, int> limits)
{
    var allPaths = new List<string>();
    var start = input.Where(x => x.Key.IsStart).First().Key;
    var paths = new Queue<List<Cave>>();

    paths.Enqueue(new List<Cave>() { start });

    while (paths.Count > 0)
    {
        var currentPath = paths.Dequeue();

        if (currentPath.Last().IsEnd)
        {
            allPaths.Add($"{string.Join(",", currentPath.Select(x => x.Id))}");
            continue;
        }

        var nextCaves = input[currentPath.Last()].Where(x => !x.IsStart && (!limits.ContainsKey(x) || (limits.ContainsKey(x) && currentPath.Count(c => c == x) < limits[x])));

        foreach (var cave in nextCaves)
        {
            var pathToEnqueue = new List<Cave>(currentPath);
            pathToEnqueue.Add(cave);
            paths.Enqueue(pathToEnqueue);
        }
    }
    return allPaths;
}

static Dictionary<Cave, int> SmallCaveLimit(Dictionary<Cave, List<Cave>> input)
    => input.Keys.Where(x => x.IsSmall && !x.IsStart && !x.IsEnd).ToDictionary(x => x, x => 1);

static List<Dictionary<Cave, int>> SmallCaveLimitCombinations(Dictionary<Cave, List<Cave>> input)
{
    var combinations = new List<Dictionary<Cave, int>>();
    var limits = SmallCaveLimit(input);
    foreach (var l in limits)
    {
        var tmp = new Dictionary<Cave, int>(limits);
        tmp[l.Key]++;
        combinations.Add(tmp);
    }
    return combinations;
}

record Cave
{
    public string Id { get; init; }
    public bool IsSmall { get; init; }
    public bool IsStart { get; init; }
    public bool IsEnd { get; init; }
    public Cave(string id)
    {
        Id = id;
        IsSmall = char.IsLower(id.First());
        IsStart = id == "start";
        IsEnd = id == "end";
    }
}