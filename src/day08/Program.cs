var input = File.ReadAllLines("input.txt")
    .Aggregate(new List<Entry>(), (acc, line) =>
    {
        var signalPatterns = line.Split('|').First().Split(' ', StringSplitOptions.RemoveEmptyEntries).Select(x => string.Concat(x.OrderBy(c => c))).ToArray();
        var outputDigits = line.Split('|').Last().Split(' ', StringSplitOptions.RemoveEmptyEntries).Select(x => string.Concat(x.OrderBy(c => c))).ToArray();
        acc.Add(new Entry(signalPatterns, outputDigits));
        return acc;
    }).ToArray();

Console.WriteLine($"Result A: {FindA(input)}");
Console.WriteLine($"Result B: {FindB(input)}");

static int FindA(Entry[] input)
    => input.SelectMany(x => x.OutputDigits).Count(x => x.Length is 2 or 3 or 4 or 7);

static int FindB(Entry[] input)
    => input.Select(x => FindOutput(x)).Sum();

static Dictionary<string, int> FindDigits(Entry entry)
{
    var digits = new string[10];
    digits[1] = entry.SignalPatterns.Where(x => x.Length == 2).First();
    digits[4] = entry.SignalPatterns.Where(x => x.Length == 4).First();
    digits[7] = entry.SignalPatterns.Where(x => x.Length == 3).First();
    digits[8] = entry.SignalPatterns.Where(x => x.Length == 7).First();

    digits[9] = entry.SignalPatterns.Where(x => x.Length == 6 && In(digits[4], x)).First();
    digits[0] = entry.SignalPatterns.Where(x => x.Length == 6 && x != digits[9] && In(digits[1], x) && In(digits[7], x)).First();
    digits[6] = entry.SignalPatterns.Where(x => x.Length == 6 && !In(digits[1], x)).First();

    digits[3] = entry.SignalPatterns.Where(x => x.Length == 5 && In(digits[1], x) && In(digits[7], x)).First();
    digits[2] = entry.SignalPatterns.Where(x => x.Length == 5 && x != digits[3] && DiffCount(digits[4], x) is 2).First();
    digits[5] = entry.SignalPatterns.Where(x => x.Length == 5 && x != digits[3] && DiffCount(digits[4], x) is 3).First();

    return digits.Select((x,i) => (Key: x, Value: i)).ToDictionary(x => x.Key, x => x.Value);
}

static bool In(string digitPattern, string pattern)
    => string.Join("", digitPattern.Intersect(pattern)) == digitPattern;

static int DiffCount(string digitPattern, string pattern)
    => digitPattern.Intersect(pattern).Count();

static int FindOutput(Entry entry)
{
    var digits = FindDigits(entry);
    return Convert.ToInt32(entry.OutputDigits.Aggregate(string.Empty, (outputValue, outputDigit) => outputValue += digits[outputDigit]), 10);
}

record Entry (string[] SignalPatterns, string[] OutputDigits);