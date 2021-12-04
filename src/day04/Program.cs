var input = new Input("input.txt");

var bingo = Bingo(input);
var boardA = bingo.First();
var boardB = bingo.Last();

Console.WriteLine($"Result A: {boardA.Result}");
Console.WriteLine($"Result B: {boardB.Result}");

List<Log> Bingo(Input input)
{
    var resultsLog = new List<Log>();

    foreach (var drawnNumber in input.Numbers)
    {        
        foreach ((List<Number> board, int boardIndex) in input.Boards.Select((value,i) => (value, i)))
        {
            if(resultsLog.Any(x => x.BoardIndex == boardIndex))
            {
                continue;
            }
            foreach(var row in board.Chunk(5))
            {
                foreach ((Number boardNumber, int i) in row.Select((value, i) => (value, i)))
                {
                    if(boardNumber.Value == drawnNumber)
                    {
                        boardNumber.Selected = true;
                        
                        if (row.All(x => x.Selected) || board.Select((number, index) => (number, index)).Where(x => x.index % 5 == i).All(x => x.number.Selected))
                        {
                            resultsLog.Add(new Log(boardIndex, Result(board, drawnNumber)));
                            break;
                        }
                    }
                }
            }
        }        
    }

    return resultsLog;
}

int Result(List<Number> winningBoard, int drawnNumber) 
    => winningBoard.Where(x => !x.Selected).Select(x => x.Value).Sum() * drawnNumber;

record Log(int BoardIndex, int Result);

class Input
{
    public List<int> Numbers { get; set; } = new List<int>();
    public List<List<Number>> Boards { get; set; } = new List<List<Number>>();

    public Input(string path)
    {
        var lines = File.ReadAllLines(path);

        var tmpBoard = new List<Number>();

        foreach ((string line, int i) in lines.Select((value, i) => (value, i)))
        {
            if (i == 0)
            {
                Numbers = line.Split(',').Select(x => int.Parse(x)).ToList();
                continue;
            }

            if (string.IsNullOrWhiteSpace(line))
            {
                tmpBoard = new List<Number>();
                Boards.Add(tmpBoard);
                continue;
            }

            tmpBoard.AddRange(line.Split(' ', StringSplitOptions.RemoveEmptyEntries).Select(x => new Number(int.Parse(x.Trim()))));
        }   
    }
}

class Number
{
    public int Value { get; set; }
    public bool Selected { get; set; }

    public Number(int value)
    {
        Value = value;        
    }
}
