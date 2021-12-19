var receiver = new Receiver(InputParser.Load("input.txt"));
var transmission = receiver.ReadTransmission();

Console.WriteLine($"Result A: {transmission.GetAllHeaders().Sum(x => x.Version)}");
Console.WriteLine($"Result B: {transmission.Evaluate()}");

internal class Transmission
{
    private readonly List<Packet> _packets;

    public Transmission(List<Packet> packets)
    {
        _packets = packets;
    }

    public List<Header> GetAllHeaders() 
        => _packets.Aggregate(new List<Header>(), (acc, p) => GetHeaders(p, acc));

    public decimal Evaluate()
        => _packets.Select(x => EvaluatePacket(x)).Sum();

    private List<Header> GetHeaders(Packet root, List<Header> headers)
    {
        headers.Add(root.Header);

        if(root is Operator op)
        {
            foreach (var packet in op.SubPackets)
            {
                if (packet.Header.Type == PacketType.Literal)
                {
                    headers.Add(packet.Header);
                }
                else
                {
                    GetHeaders(packet, headers);
                }
            }
        }

        return headers;
    }

    private decimal EvaluatePacket(Packet packet) => packet.Header.Type switch
    {
        PacketType.Literal => ((Literal)packet).Value,
        PacketType.Sum => ((Operator)packet).SubPackets.Select(x => EvaluatePacket(x)).Sum(),
        PacketType.Product => ((Operator)packet).SubPackets.Aggregate(1M, (acc, p) => acc *= EvaluatePacket(p)),
        PacketType.Minimum => ((Operator)packet).SubPackets.Select(x => EvaluatePacket(x)).Min(),
        PacketType.Maximum => ((Operator)packet).SubPackets.Select(x => EvaluatePacket(x)).Max(),
        PacketType.GreaterThan => EvaluatePacket(((Operator)packet).SubPackets.First()) > EvaluatePacket(((Operator)packet).SubPackets.Last()) ? 1 : 0,
        PacketType.LessThan => EvaluatePacket(((Operator)packet).SubPackets.First()) < EvaluatePacket(((Operator)packet).SubPackets.Last()) ? 1 : 0,
        PacketType.EqualTo => EvaluatePacket(((Operator)packet).SubPackets.First()) == EvaluatePacket(((Operator)packet).SubPackets.Last()) ? 1 : 0,
        _ => 0,
    };
}

internal class Receiver
{
    private readonly List<char> _transmision = new List<char>();
    private int Pointer = 0;

    public Receiver(List<char> transmision)
    {
        _transmision = transmision;
    }

    public Transmission ReadTransmission()
    {
        var packets = new List<Packet>();
        while (true)
        {
            var readedBits = 0;

            if (_transmision.Count - Pointer < 11)
                break;

            var header = ReadHeader(ref readedBits);
            if (header.Type == PacketType.Literal)
            {
                packets.Add(ReadLiteralValue(header, ref readedBits));
            }
            else
            {
                packets.Add(ReadOperator(header, ref readedBits));
            }
        }

        return new Transmission(packets);
    }

    private Header ReadHeader(ref int readedBits)
    {
        var version = ReadAsDecimal(3, ref readedBits);
        var type = ReadAsDecimal(3, ref readedBits);
        return new Header((int)version, (PacketType)type);
    }

    private Literal ReadLiteralValue(Header header, ref int readedBits)
    {
        var readNext = true;
        var bits = new List<char>();

        while (readNext)
        {
            readNext = ReadAsDecimal(1, ref readedBits) == 1;
            bits.AddRange(Read(4, ref readedBits));
        }

        return new Literal(header, ToDecimal(bits));
    }

    private Operator ReadOperator(Header header, ref int readedBits)
    {
        var lengthTypeId = ReadAsDecimal(1, ref readedBits);
        var lengthBits = lengthTypeId == 0 ? 15 : 11;
        var bitsToRead = ReadAsDecimal(lengthBits, ref readedBits);
        var actualReadedBits = 0;
        var packets = new List<Packet>();

        if(lengthBits == 15)
        {
            while (actualReadedBits < bitsToRead)
            {
                var subHeader = ReadHeader(ref actualReadedBits);
                if (subHeader.Type == PacketType.Literal)
                {
                    packets.Add(ReadLiteralValue(subHeader, ref actualReadedBits));
                }
                else
                {
                    packets.Add(ReadOperator(subHeader, ref actualReadedBits));
                }
            }
        }
        else
        {
            for(var i = 0; i < bitsToRead; i++)
            {
                var subHeader = ReadHeader(ref actualReadedBits);
                if (subHeader.Type == PacketType.Literal)
                {
                    packets.Add(ReadLiteralValue(subHeader, ref actualReadedBits));
                }
                else
                {
                    packets.Add(ReadOperator(subHeader, ref actualReadedBits));
                }
            }        
        }
        
        readedBits += actualReadedBits;
        return new Operator(header, packets);
    }

    private List<char> Read(int numberOfBits, ref int readedBits)
    {
        var value = _transmision.GetRange(Pointer, numberOfBits);
        Pointer += numberOfBits;
        readedBits += numberOfBits;
        return value;
    }

    private decimal ReadAsDecimal(int numberOfBits, ref int readedBits)
    {
        var value = ToDecimal(_transmision.GetRange(Pointer, numberOfBits));
        Pointer += numberOfBits;
        readedBits += numberOfBits;
        return value;
    }

    private decimal ToDecimal(List<char> bits)
        => Convert.ToDecimal(Convert.ToInt64(new string(bits.ToArray()), 2));
}

record Header(int Version, PacketType Type);
abstract record Packet(Header Header);
record Literal(Header Header, decimal Value) : Packet(Header);
record Operator(Header Header, List<Packet> SubPackets) : Packet(Header);

internal enum PacketType
{
    Sum = 0,
    Product = 1,
    Minimum = 2,
    Maximum = 3,
    Literal = 4,
    GreaterThan = 5,
    LessThan = 6,
    EqualTo = 7,
}

internal static class InputParser
{
    public static List<char> Load(string path)
    {
        var bits = new List<char>();
        var line = File.ReadAllText(path);

        foreach (var c in line)
            bits.AddRange(Map(c));

        return bits;
    }

    private static char[] Map(char input)
        => input switch
        {
            '0' => new char[] { '0', '0', '0', '0' },
            '1' => new char[] { '0', '0', '0', '1' },
            '2' => new char[] { '0', '0', '1', '0' },
            '3' => new char[] { '0', '0', '1', '1' },
            '4' => new char[] { '0', '1', '0', '0' },
            '5' => new char[] { '0', '1', '0', '1' },
            '6' => new char[] { '0', '1', '1', '0' },
            '7' => new char[] { '0', '1', '1', '1' },
            '8' => new char[] { '1', '0', '0', '0' },
            '9' => new char[] { '1', '0', '0', '1' },
            'A' => new char[] { '1', '0', '1', '0' },
            'B' => new char[] { '1', '0', '1', '1' },
            'C' => new char[] { '1', '1', '0', '0' },
            'D' => new char[] { '1', '1', '0', '1' },
            'E' => new char[] { '1', '1', '1', '0' },
            'F' => new char[] { '1', '1', '1', '1' },
            _ => new char[0],
        };
}