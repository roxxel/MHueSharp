using MagicHue.Enums;
using System.Linq;
using System.Net;
using System.Net.Sockets;

namespace MagicHue
{
    public class Light
    {
        private const int _port = 5577;

        private string _address;
        private string _name;
        private readonly bool _confirmReceive;
        private bool _allowFading;

        private Socket _socket;
        public Light(string address, string name = "NoName", bool confirmReceive = false, bool allowFading = true)
        {
            _address = address;
            _name = name;
            _confirmReceive = confirmReceive;
            _allowFading = allowFading;
            Status = new();
        }
        public string Address { get => _address; }
        public string Name { get => _name; }
        public Status Status { get; }

        public bool IsEnabled
        {
            get
            {
                return Status.IsEnabled == Packets.TurnOn2;
            }
            set
            {
                if (value)
                    TurnOn();
                else
                    TurnOff();
                Status.IsEnabled = (byte)(value ? Packets.TurnOn2 : Packets.TurnOff2);
            }
        }

        public (byte R, byte G, byte B) RGB
        {
            get
            {
                return (Status.R, Status.G, Status.B);
            }
            set
            {
                var (r, g, b) = value;
                if (r < 0)
                    r = Status.R;
                if (g < 0)
                    g = Status.G;
                if (b < 0)
                    b = Status.B;
                Status.R = r;
                Status.G = g;
                Status.B = b;
                UpdateColor();
            }
        }


        public void Connect()
        {
            _socket = new Socket(SocketType.Stream, ProtocolType.Tcp);
            _socket.Connect(IPEndPoint.Parse($"{_address}:{_port}"));
            Status.Parse(GetStatus());
        }

        private void UpdateColor()
        {
            var (r, g, b, w) = (Status.R, Status.G, Status.B, Status.W);
            var isWhite = Status.IsWhite;
            if (Status.BulbType == BulbType.RGBWWCW)
            {
                var data = new int[]
                {
                    (byte)Packets.SetColor,
                    r,
                    g,
                    b,
                    w,
                    Status.CW,
                    isWhite,
                    (byte)0x0f // <-- terminator
                };
                SendWithChecksum(data, Packets.ResponseLenSetColor);
            }
            else
            {
                var data = new int[]
                {
                    (byte)Packets.SetColor,
                    r,
                    g,
                    b,
                    w,
                    isWhite,
                    (byte)0x0f // <-- terminator
                };
                SendWithChecksum(data, Packets.ResponseLenSetColor);
            }
        }

        private byte[] GetStatus()
        {
            var data = SendWithChecksum(new[] { Packets.QueryStatus1, Packets.QueryStatus2, Packets.QueryStatus3 }, Packets.ResponseLenQueryStatus);
            return data;
        }

        private void TurnOff()
        {
            SendWithChecksum(new[] { Packets.TurnOff1, Packets.TurnOff2, Packets.TurnOff3 }, 4);
        }

        private void TurnOn()
        {
            SendWithChecksum(new[] { Packets.TurnOn1, Packets.TurnOn2, Packets.TurnOn3 }, 4);
        }

        private int Send(int[] data)
        {
            return _socket.Send(data.GetBytes());
        }

        private byte[] SendWithChecksum(int[] data, int responseLength, bool receive = true)
        {
            var dataWithChecksum = AttachChecksum(data);
            Send(dataWithChecksum);
            if (receive)
            {
                var resp = Receive(responseLength);
                return resp;
            }
            return Array.Empty<byte>();
        }
        private byte[] Receive(int length)
        {
            byte[] data = new byte[length];
            _socket.Receive(data, length, SocketFlags.None);
            return data;
        }

        private int CalculateChecksum(int[] data)
        {
            var hex = data.Sum(x => x).ToString("X2");
            var checksum = Convert.ToInt32(hex.Substring(hex.Length - 2), 16);
            return checksum;
        }
        private int[] AttachChecksum(int[] data)
        {
            var checksum = CalculateChecksum(data);
            var list = data.ToList();
            list.Add(checksum);
            return list.ToArray();
        }
    }

    public class Status
    {
        public BulbType BulbType { get; internal set; }
        public byte IsEnabled { get; internal set; }
        public int Mode { get; internal set; }
        public byte R { get; internal set; }
        public byte G { get; internal set; }
        public byte B { get; internal set; }
        public byte W { get; internal set; }
        public byte Version { get; internal set; }
        public byte CW { get; internal set; }
        public byte IsWhite { get; internal set; }
        public byte Slowness { get; internal set; }
        public byte Speed { get => (byte)((31 - (int)Slowness) / 30); }


        internal Status()
        {

        }

        internal void Parse(byte[] data)
        {
            if (data[0] != 0x81)
                return;

            BulbType = (BulbType)data[1];
            IsEnabled = data[2];
            Mode = data[3];
            Slowness = data[5];
            R = data[6];
            G = data[7];
            B = data[8];
            W = data[9];
            Version = data[10];
            CW = data[11];
            IsWhite = data[12];
        }
    }
}