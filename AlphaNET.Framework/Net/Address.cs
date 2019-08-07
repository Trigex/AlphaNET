using System.Diagnostics;
using System.IO;
using System.Text;

namespace AlphaNET.Framework.Net
{
    public class Address
    {
        public ushort Port { get; private set; }
        public string IpAddress { get; private set; }

        public Address(string ipAddress, ushort port)
        {
            IpAddress = ipAddress;
            Port = port;
        }

        public override string ToString()
        {
            return IpAddress + ":" + Port.ToString();
        }

        public byte[] ToBytes() // IpAddressLength(byte) IpAddress(UTF8 bytes) Port(uint16)
        {
            byte[] bytes;
            var stream = new MemoryStream();
            var writer = new BinaryWriter(stream);
            if (!(IpAddress.Length > 32))
            {
                var encodedString = Encoding.UTF8.GetBytes(IpAddress);
                writer.Write((byte)encodedString.Length); // IpAddressLength
                writer.Write(encodedString); // IpAddress
            }
            else
            {
                Debug.WriteLine("IP Address too large");
                return null;
            }
            writer.Write(Port); // Port
            bytes = stream.ToArray();
            writer.Close();
            return bytes;
        }

        public static Address FromBytes(byte[] bytes)
        {
            Address addr;

            var stream = new MemoryStream(bytes);
            var reader = new BinaryReader(stream);

            byte ipAddrLength = reader.ReadByte(); // IPAddressLength
            byte[] ipAddr = reader.ReadBytes(ipAddrLength); // IpAddress
            ushort port = reader.ReadUInt16(); // Port
            addr = new Address(Encoding.UTF8.GetString(ipAddr), port);
            reader.Close();
            return addr;
        }
    }
}
