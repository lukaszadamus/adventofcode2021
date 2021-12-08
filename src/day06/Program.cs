var input = File.ReadAllText("input.txt")
    .Split(',').Select(c => long.Parse(c))
    .Aggregate(new long[9], (acc, f) => { acc[f]++; return acc; });

Console.WriteLine($"Result A:{Result(input, 80)}");
Console.WriteLine($"Result B:{Result(input, 256)}");

static long Result(long[] input, int days)
{    
    for(var day = 0; day < days; day++)
    {
        var dDayFor = input[0];
        Array.Copy(input, 1, input, 0, input.Length - 1);
        input[6] += dDayFor;
        input[8] = dDayFor;        
    }
    return input.Sum();
}