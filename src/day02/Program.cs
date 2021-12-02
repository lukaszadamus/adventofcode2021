var input = File.ReadAllLines("input.txt").Select(x =>
{
    var line = x.Split(' ');
    Enum.TryParse(line[0], out Operation operation);
    return new Instruction(operation, int.Parse(line[1]));
}).ToArray();

var routeA = new Tracker(Tracker.PositioningA).Track(input);

Console.WriteLine($"Result A: {routeA.Last().Horizontal * routeA.Last().Depth}");

var routeB = new Tracker(Tracker.PositioningB).Track(input);

Console.WriteLine($"Result B: {routeB.Last().Horizontal * routeB.Last().Depth}");

public class Tracker
{
    private static Position _initialPosition = new Position(0, 0, 0);
    private static ICollection<Position> _track = new List<Position>() { _initialPosition };
    private readonly Func<Position, Instruction, Position> _instructionProcessor;
    private readonly Func<Position, IEnumerable<Position>> _resultSelector = result => _track;

    public Tracker(Func<Instruction, Position, Position> positioning)
    {
        _instructionProcessor = (acc, x) =>
        {
            var p = positioning(x, acc);
            _track.Add(p);
            return p;
        };
    }

    public Position[] Track(IEnumerable<Instruction> instructions)
        => instructions.Aggregate(_initialPosition, _instructionProcessor, _resultSelector).ToArray();

<<<<<<< Updated upstream
public record Instruction(Operation Operation, int Value);
public record Position(int Horizontal, int Depth, int Aim);
public enum Operation
{
    forward,
    down,
    up,
=======
    public static Func<Instruction, Position, Position> PositioningA = (i, p) =>
    {
        var horizontal = p.Horizontal;
        var depth = p.Depth;
        var aim = p.Aim;

        switch (i.Operation)
        {
            case Operation.down:
                depth += i.Value;
                break;
            case Operation.up:
                depth -= i.Value;
                break;
            case Operation.forward:
                horizontal += i.Value;                
                break;
        }

        return new Position(horizontal, depth, aim);
    };

    public static Func<Instruction, Position, Position> PositioningB = (i, p) =>
    {
        var horizontal = p.Horizontal;
        var depth = p.Depth;
        var aim = p.Aim;

        switch (i.Operation)
        {
            case Operation.down:
                aim += i.Value;
                break;
            case Operation.up:
                aim -= i.Value;
                break;
            case Operation.forward:
                horizontal += i.Value;
                depth += aim * i.Value;
                break;
        }

        return new Position(horizontal, depth, aim);
    };
>>>>>>> Stashed changes
}

public record Instruction(Operation Operation, int Value);
public record Position(int Horizontal, int Depth, int Aim);
public enum Operation { forward, down, up }