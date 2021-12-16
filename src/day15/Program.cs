var graph = InputParser.LoadCave("input.txt");

Console.WriteLine($"Result A: {FindA(graph)}");


static int FindA(Dictionary<Vertex, List<Vertex>> graph)
{
    var source = graph.First().Key;
    var target = graph.Last().Key;

    var result = Dijkstra(graph, source, target);

    return result[target];
}

static Dictionary<Vertex, int> Dijkstra(Dictionary<Vertex, List<Vertex>> graph, Vertex source, Vertex? target)
{    
    var distance = new Dictionary<Vertex, int>();
    distance[source] = 0;

    var pq = new Q();        

    foreach (var vertex in graph.Keys)
    {
        if (vertex != source)
            distance[vertex] = int.MaxValue;            
        
        pq.Enqueue(vertex, distance[vertex]);
    }

    while (!pq.Empty())
    {
        var current = pq.Dequeue();

        if(target is not null && current == target)
            return distance;

        foreach (var vertex in graph[current])
        {
            var alt = distance[current] + vertex.Risk;
            if (alt < distance[vertex])
            {
                distance[vertex] = alt;
                pq.ChangePriority(vertex, alt);
            }
        }
    }

    return distance;
}

record Vertex(int X, int Y, int Risk);

internal static class InputParser
{
    public static Dictionary<Vertex, List<Vertex>> LoadCave(string path)
    {
        var lines = File.ReadAllLines(path);
        var r = lines.Count();
        var c = lines.First().Length;
        var risks = new byte[r, c];
        lines.Select((line, y) => (Y: y, Line: line)).Aggregate(risks, (acc, line) =>
        {
            for (var x = 0; x < line.Line.Length; x++)
                acc[line.Y, x] = (byte)char.GetNumericValue(line.Line[x]);
            return acc;
        });

        var graph = lines
            .Select((line, y) => (Y: y, Line: line))
            .Aggregate(new Dictionary<Vertex, List<Vertex>>(), (acc, vertex) =>
            {
                for (var x = 0; x < vertex.Line.Length; x++)
                    acc.Add(new Vertex(x, vertex.Y, (int)char.GetNumericValue(vertex.Line[x])), new List<Vertex>());
                return acc;
            });

        foreach (var vertex in graph.Keys)
        {
            graph[vertex] = FindNeighbors(vertex, risks).OrderBy(x => x.Risk).ToList();
        }

        return graph;
    }

    private static IEnumerable<Vertex> FindNeighbors(Vertex vertex, byte[,] risks)
    {
        if (vertex.X - 1 >= 0)
            yield return new Vertex(vertex.X - 1, vertex.Y, risks[vertex.Y, vertex.X - 1]);
        if (vertex.Y - 1 >= 0)
            yield return new Vertex(vertex.X, vertex.Y - 1, risks[vertex.Y - 1, vertex.X]);
        if (vertex.X + 1 < risks.GetLength(1))
            yield return new Vertex(vertex.X + 1, vertex.Y, risks[vertex.Y, vertex.X + 1]);
        if (vertex.Y + 1 < risks.GetLength(0))
            yield return new Vertex(vertex.X, vertex.Y + 1, risks[vertex.Y + 1, vertex.X]);
    }
}

internal class Q
{
    private readonly Dictionary<Vertex, int> _elements = new Dictionary<Vertex, int>();

    public void Enqueue(Vertex vertex, int priority)
        => _elements.Add(vertex, priority);
    

    public Vertex Dequeue()
    {        
        var element = _elements.OrderBy(x => x.Value).First().Key;
        _elements.Remove(element);
        return element;
    }

    public void ChangePriority(Vertex vertex, int priority)
        => _elements[vertex] = priority;

    public bool Empty()
        => _elements.Count == 0;
}