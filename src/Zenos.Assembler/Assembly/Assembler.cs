using System.Collections.Generic;
using System.Diagnostics;
using Zenos.Assembly.AST;

namespace Zenos.Assembly
{
    public class Assembler
    {
        private readonly List<byte> _data = new List<byte>();
        private readonly SymbolTable _symbolTable;
        private int _numReservations = 0;

        public Assembler()
        {
            _symbolTable = new SymbolTable();
        }

        public byte[] GetBytes() => _data.ToArray();

        public string Assemble(AstNode node)
        {
            switch (node)
            {
                case AssemblyListing listing:
                    foreach (var directive in listing.Directives)
                    {
                        var err = Assemble(directive);
                        if (err != null)
                            return err;
                    }

                    return null;

                case ExternDeclaration externDeclaration:
                    _symbolTable.Import(externDeclaration.Name);
                    return null;

                case GlobalDeclaration globalDeclaration:
                    _symbolTable.Export(globalDeclaration.Name);
                    return null;

                case SectionDeclaration sectionDeclaration:
                    foreach (var entry in sectionDeclaration.SectionEntries)
                    {
                        var error = Assemble(entry);
                        if (error != null)
                            return error;
                    }

                    return null;

                case Instruction instruction:
                    return InstructionEmitter.Emit(instruction, _data);

                case Data data:
                case SectionLabel label:
                    return null;
                default:
                    return $"unsupported AstNode: {node.GetType()}";
            }
        }


        private void EmitByte(byte emit)
        {
            _data.Add(emit);
        }

        private void EmitShort(short emit)
        {
            EmitByte((byte)(emit & 0xFF));
            EmitByte((byte)((emit >> 8) & 0xFF));
        }

        private void EmitInt(int emit)
        {
            EmitByte((byte)(emit & 0xFF));
            EmitByte((byte)((emit >> 8) & 0xFF));
            EmitByte((byte)((emit >> 16) & 0xFF));
            EmitByte((byte)((emit >> 24) & 0xFF));
        }

        private void EmitLong(long emit)
        {
            EmitByte((byte)(emit & 0xFF));
            EmitByte((byte)((emit >> 8) & 0xFF));
            EmitByte((byte)((emit >> 16) & 0xFF));
            EmitByte((byte)((emit >> 24) & 0xFF));
            EmitByte((byte)((emit >> 32) & 0xFF));
            EmitByte((byte)((emit >> 40) & 0xFF));
            EmitByte((byte)((emit >> 48) & 0xFF));
            EmitByte((byte)((emit >> 56) & 0xFF));
        }


        private void EmitBytes(byte[] bytes)
        {
            _data.AddRange(bytes);
        }

        private void EmitZeros(int count)
        {
            while (count-->0)
            {
                _data.Add(0);
            }
        }

        private Reservation GetReservationTicket(int size)
        {
            _numReservations++;
            Reservation ticket = (Reservation)_data.Count;
            EmitZeros(size);
            return ticket;
        }
        private int ReturnReservationTicket(Reservation reservation)
        {
            Debug.Assert(_numReservations > 0);
            _numReservations--;

            return (int)reservation;
        }

        private Reservation ReserveByte()
        {
            return GetReservationTicket(1);
        }

        private void EmitByte(Reservation reservation, byte emit)
        {
            int offset = ReturnReservationTicket(reservation);
            _data[offset] = emit;
        }

        private Reservation ReserveShort()
        {
            return GetReservationTicket(2);
        }

        private void EmitShort(Reservation reservation, short emit)
        {
            int offset = ReturnReservationTicket(reservation);
            _data[offset] = (byte)(emit & 0xFF);
            _data[offset + 1] = (byte)((emit >> 8) & 0xFF);
        }

        private Reservation ReserveInt()
        {
            return GetReservationTicket(4);
        }

        private void EmitInt(Reservation reservation, int emit)
        {
            int offset = ReturnReservationTicket(reservation);
            _data[offset] = (byte)(emit & 0xFF);
            _data[offset + 1] = (byte)((emit >> 8) & 0xFF);
            _data[offset + 2] = (byte)((emit >> 16) & 0xFF);
            _data[offset + 3] = (byte)((emit >> 24) & 0xFF);
        }

        enum Reservation
        {
        }
    }
}