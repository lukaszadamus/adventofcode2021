var input = File.ReadAllLines("input.txt")
    .Select(line => line);

Console.WriteLine($"Result A:{FindA(input, GammaCriteria) * FindA(input, EpsilonCriteria)}");
Console.WriteLine($"Result B:{FindB(input, OxygenGeneratorCriteria) * FindB(input, CO2ScrubberCriteria)}");

int FindA(IEnumerable<string> input, Func<int, char> criteria)
{   
    var s = input.Aggregate(Enumerable.Repeat(0, input.First().Length), (acc, n) => acc.Zip(n, (a, b) => a + (b == '1' ? 1 : -1)))
        .Select(criteria)
        .Aggregate(string.Empty, (s, i) => s + i);

    return Convert.ToInt32(s, 2);
}

int FindB(IEnumerable<string> input, Func<int, char> criteria)
{
    var index = 0;

    while (input.Count() > 1)
    {
        var sum = input.Select(x => x[index] == '1' ? 1 : -1).Sum();
        input = input.Where(x => x[index] == criteria(sum)).ToArray();
        index++;
    }

    return Convert.ToInt32(input.First().Aggregate(string.Empty, (s, i) => s + i), 2);
}

char GammaCriteria(int x) => x >= 0 ? '1' : '0';
char EpsilonCriteria(int x) => x <= 0 ? '1' : '0';
char OxygenGeneratorCriteria(int x) => x >= 0 ? '1' : '0';
char CO2ScrubberCriteria(int x) => x < 0 ? '1' : '0';
