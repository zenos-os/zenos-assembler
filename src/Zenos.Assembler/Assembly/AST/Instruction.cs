using System;
using System.Collections.Generic;
using System.Linq;

namespace Zenos.Assembly.AST
{
    public class Instruction : SectionEntry, IEquatable<Instruction>
    {
        public Instruction(string name, Operand operand) : this(64, name, new[] { operand })
        {
        }

        public Instruction(string name, Operand[] operands)
            : this(64, name, operands)
        {
        }

        public Instruction(int bits, string name, Operand[] operands)
        {
            Bits = bits;
            Name = name;
            Operands = operands;
        }

        public int Bits { get; }
        public string Name { get; }
        public Operand[] Operands { get; }

        public override bool Equals(object obj) => Equals(obj as Instruction);

        public bool Equals(Instruction other) => other != null &&
                                                 Bits == other.Bits &&
                                                 Name == other.Name &&
                                                 EqualityComparer<Operand[]>.Default.Equals(Operands, other.Operands);

        public override int GetHashCode() => HashCode.Combine(Bits, Name, Operands);

        public static bool operator ==(Instruction instruction1, Instruction instruction2)
        {
            return EqualityComparer<Instruction>.Default.Equals(instruction1, instruction2);
        }

        public static bool operator !=(Instruction instruction1, Instruction instruction2) => !(instruction1 == instruction2);

        public override string ToString() => $"{Name} {string.Join(", ", Operands.Select(x => x.ToString()))}";
    }
}