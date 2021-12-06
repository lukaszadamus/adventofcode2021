var input = File.ReadAllText("input.txt")
    .Split(',').Select(c => long.Parse(c))
    .Aggregate(Enumerable.Repeat(0L, 9).ToArray(), (acc, f) => { acc[f]++; return acc; });

Console.WriteLine($"Result A:{Result(input, 80)}");
Console.WriteLine($"Result B:{Result(input, 256)}");

static long Result(long[] input, int days)
{
    var day = 0;
    while (day < days)
    {
        var dDayFor = input[0];
        Array.Copy(input, 1, input, 0, input.Length - 1);
        input[6] += dDayFor;
        input[8] = dDayFor;
        day++;
    }
    return input.Sum();
}