var input = File.ReadAllLines("input.txt").Select(x => int.Parse(x)).ToArray();

var resultA = 0;
input.Aggregate((a, b) =>
{
    resultA += a < b ? 1 : 0;
    return b;
});

Console.WriteLine($"Result A: {resultA}");

var sums = new List<int>();
for (var i = 0; i < input.Length - 2; i++)
{
    sums.Add(input.Skip(i).Take(3).Sum());
}

var resultB = 0;
sums.Aggregate((a, b) =>
{
    resultB += a < b ? 1 : 0;
    return b;
});

Console.WriteLine($"Result B: {resultB}");