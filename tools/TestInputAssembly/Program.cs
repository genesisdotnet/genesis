using System;
using Genesis;

namespace TestInputAssembly
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
        }
    }

    [GenesisObject]
    public sealed class AttributeTestObjectOne
    {
        public string TestString { get; set; } = string.Empty;
        public byte TestByte { get; set; }

        public short TestShort { get; set; }

        public int TestInt { get; set; }

        public long TestLong { get; set; }

        public DateTime TestDateTime { get; set; }
    }

    [GenesisObject]
    public sealed class AttributeTestObjectTwo
    {
        public string TestString { get; set; } = string.Empty;
        public byte TestByte { get; set; }

        public short TestShort { get; set; }

        public int TestInt { get; set; }

        public long TestLong { get; set; }

        public DateTime TestDateTime { get; set; }
    }

    [GenesisObject]
    public sealed class AttributeTestObjectThree
    {
        public string TestString { get; set; } = string.Empty;
        public byte TestByte { get; set; }

        public short TestShort { get; set; }

        public int TestInt { get; set; }

        public long TestLong { get; set; }

        public DateTime TestDateTime { get; set; }
    }
}
