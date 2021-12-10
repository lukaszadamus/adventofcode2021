var input = File.ReadAllLines("input.txt");

var ErrorScore = new Dictionary<char, int>()
{
    {')', 3},
    {']', 57},
    {'}', 1197},
    {'>', 25137},
};

var CompleteScore = new Dictionary<char, int>()
{
    {')', 1},
    {']', 2},
    {'}', 3},
    {'>', 4},
};

bool IsMatch(char left, char right) => (left, right) switch
{
    ('(', ')') => true,
    ('[', ']') => true,
    ('{', '}') => true,
    ('<', '>') => true,
    _ => false,
};

char Map(char c) => (c) switch
{
    '(' => ')',
    '[' => ']',
    '{' => '}',
    '<' => '>',
    ')' => '(',
    ']' => '[',
    '}' => '{',
    '>' => '<',
    _ => c,
};

var lines = Lines(input);

Console.WriteLine($"Result A: {lines.Select(x => x.SyntaxErrorScore).Sum()}");
Console.WriteLine($"Result B: {lines.Where(x => x.CompleteScore > 0).Select(x => x.CompleteScore).OrderBy(x => x).Skip(lines.Where(x => x.CompleteScore > 0).Count() / 2).First()}");

IEnumerable<Line> Lines(IEnumerable<string> input)
    => input.Select((x, i) => (LineNumber: i, Characters: x.ToArray()))
        .Aggregate(new List<Line>(), (acc, x) =>
        {
            var stack = new Stack<char>();
            for (var c = 0; c < x.Characters.Length; c++)
            {
                if (!ErrorScore.Keys.Contains(x.Characters[c]))
                {
                    stack.Push(x.Characters[c]);
                }
                else
                {
                    if (IsMatch(stack.Peek(), x.Characters[c]))
                    {
                        stack.Pop();
                    }
                    else
                    {
                        acc.Add(new Line(x.LineNumber, ErrorScore[x.Characters[c]], 0));
                        return acc;
                    }
                }
            }

            long completeScore = 0;
            while (stack.Count > 0)
            {
                completeScore *= 5L;
                completeScore += CompleteScore[Map(stack.Pop())];
            }

            acc.Add(new Line(x.LineNumber, 0, completeScore));

            return acc;
        });

record Line(int LineNumber, long SyntaxErrorScore, long CompleteScore);