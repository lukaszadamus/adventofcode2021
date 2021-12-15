using System.Text;
using System.Text.RegularExpressions;

var input = InputParser.Parse("input.txt");

Console.WriteLine($"Result A: {Find(input.Polymer, input.Rules, 10)}");
Console.WriteLine($"Result B: {Find(input.Polymer, input.Rules, 40)}");

static long Find(string polymerTemplate, Dictionary<string, string[]> rules, int iterations)
{
    var counts = new Dictionary<string, long>();

    var pairs = SplitToPairs(polymerTemplate);

    foreach (var pair in pairs)
    {
        if(counts.ContainsKey(pair))
        {
            counts[pair]++;
        }
        else
        {
            counts.Add(pair, 1);
        }        
    }

    for (int i = 0; i < iterations; i++)
    {
        var newCounts = new Dictionary<string, long>();
        foreach (var pair in counts)
        {
            foreach (var newPair in rules[pair.Key])
            {
                if (newCounts.ContainsKey(newPair))
                {
                    newCounts[newPair] += pair.Value;
                }
                else
                {
                    newCounts.Add(newPair, pair.Value);
                }
            }
        }
        counts = newCounts;
    }

    var result = CountCharacters(counts, polymerTemplate.Last());

    return result.Last().Count - result.First().Count;
}

static List<(char Character, long Count)> CountCharacters(Dictionary<string, long> counts, char last)
{
    var result = new Dictionary<char, long>();
    foreach (var pair in counts)
    {
        var character = pair.Key[0];
        var count = pair.Value;

        if (result.ContainsKey(character))
        {
            result[character] += count;
        }
        else
        {
            result.Add(character, count);
        }
    }

    result[last]++;

    return result.Select(x => (Character: x.Key, Count: x.Value)).OrderBy(x => x.Count).ToList();
}

static string[] SplitToPairs(string input)
{
    var pairs = new string[input.Length - 1];
    var offset = 0;
    while (offset < input.Length - 1)
    {
        pairs[offset] = input.Substring(offset, 2);
        offset++;
    }
    return pairs;
}

internal static class InputParser
{
    private static readonly Regex PolymerRegEx = new Regex(@"^(?<polymer>[A-Z]+)$", RegexOptions.Compiled | RegexOptions.IgnoreCase);
    private static readonly Regex RuleRegEx = new Regex(@"^(?<left>[A-Z]{2})(\s{1}->\s{1})(?<right>[A-Z]{1})$", RegexOptions.Compiled | RegexOptions.IgnoreCase);

    public static (string Polymer, Dictionary<string, string[]> Rules) Parse(string path)
        => File.ReadAllLines(path).Aggregate((Polymer: string.Empty, Rules: new Dictionary<string, string[]>()), (acc, l) =>
        {
            if (PolymerRegEx.IsMatch(l))
            {
                var m = PolymerRegEx.Match(l);
                acc.Polymer = m.Groups["polymer"].Value;
            }
            else if (RuleRegEx.IsMatch(l))
            {
                var m = RuleRegEx.Match(l);
                var left = m.Groups["left"].Value;
                var right = new string[] { left[0] + m.Groups["right"].Value, m.Groups["right"].Value + left[1] };
                acc.Rules.Add(left, right);
            }

            return acc;
        });
}