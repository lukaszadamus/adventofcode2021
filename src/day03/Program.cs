
var input = File.ReadAllLines("input.txt")
    .Select(line => line.Select(bit => bit == '1').ToArray());

var numberLength = input.First().Length;

Func<Func<int, int>, int> FindRate = criteria => Convert.ToInt32(NumberAsString(Counts(input), criteria), 2);
Func<Func<int, bool>, int> FindRating = criteria => Find(input, criteria);

Console.WriteLine($"Result A:{FindRate(GammaCriteria) * FindRate(EpsilonCriteria)}");
Console.WriteLine($"Result B:{FindRating(OxygenGeneratorCriteria) * FindRating(CO2ScrubberCriteria)}");

int Find(IEnumerable<bool[]> input, Func<int, bool> criteria)
{
    var index = 0;

    while (input.Count() > 1)
    {
        var bar = input.Select(x => x[index] ? 1 : -1).Sum();
        input = input.Where(x => x[index] == criteria(bar)).ToArray();
        index++;
    }

    return Convert.ToInt32(input.First().Aggregate(string.Empty, (s, i) => s + (i ? "1" : "0")), 2);
}

IEnumerable<int> Counts(IEnumerable<bool[]> input)
    => input.Aggregate(Enumerable.Repeat(0, numberLength), (acc, n) => acc.Zip(n, (a, b) => a + (b ? 1 : -1)));

string NumberAsString(IEnumerable<int> counts, Func<int, int> criteria)
     => counts.Select(criteria).Aggregate(string.Empty, (s, i) => s + i.ToString());

int GammaCriteria(int x) => x >= 0 ? 1 : 0;
int EpsilonCriteria(int x) => x <= 0 ? 1 : 0;
bool OxygenGeneratorCriteria(int x) => x >= 0;
bool CO2ScrubberCriteria(int x) => x < 0;
