using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

namespace OsuPatcher.Runtime.Helpers
{
    internal class ILReader
    {
        private readonly byte[] _ilInstructions;
        private int _pos;

        private static readonly Dictionary<(byte prefix, byte code), OpCode> Opcodes;

        static ILReader()
        {
            Opcodes = new Dictionary<(byte prefix, byte code), OpCode>();

            foreach (var field in typeof(OpCodes).GetFields(BindingFlags.Public | BindingFlags.Static))
            {
                var opcode = (OpCode)field.GetValue(null);
                if (opcode.OpCodeType == OpCodeType.Nternal) continue;

                var prefix = (byte)(opcode.Size == 1 ? 0 : 0xFE);
                var code = (byte)(opcode.Value & 0xFF);
                Opcodes[(prefix, code)] = opcode;
            }
        }

        public ILReader(byte[] ilInstructions) => _ilInstructions = ilInstructions ?? throw new ArgumentNullException(nameof(ilInstructions));

        public IEnumerable<OpCode> GetOpCodes()
        {
            while (_pos < _ilInstructions.Length)
            {
                var currentByte = _ilInstructions[_pos++];
                var prefix = (byte)(currentByte == 0xFE ? 0xFE : 0);
                var code = prefix == 0xFE ? _ilInstructions[_pos++] : currentByte;

                var op = Opcodes[(prefix, code)];

                switch (op.OperandType)
                {
                    case OperandType.InlineSwitch:
                        _pos += 1 + (_ilInstructions[_pos] | 
                            (_ilInstructions[_pos + 1] << 8) | 
                            (_ilInstructions[_pos + 2] << 16) | 
                            (_ilInstructions[_pos + 3] << 24)) * 4;
                        break;
                    case OperandType.InlineI8:
                    case OperandType.InlineR:
                        _pos += 8; 
                        break;
                    case OperandType.InlineBrTarget:
                    case OperandType.InlineField:
                    case OperandType.InlineI:
                    case OperandType.InlineMethod:
                    case OperandType.InlineString:
                    case OperandType.InlineTok:
                    case OperandType.InlineType:
                    case OperandType.InlineSig:
                    case OperandType.ShortInlineR: 
                        _pos += 4; 
                        break;
                    case OperandType.InlineVar: 
                        _pos += 2; 
                        break;
                    case OperandType.ShortInlineBrTarget:
                    case OperandType.ShortInlineI:
                    case OperandType.ShortInlineVar: 
                        _pos++; 
                        break;
                    case OperandType.InlineNone:
                        break;
                    default: 
                        throw new NotSupportedException($"Unsupported operand type: {op.OperandType}");
                }

                yield return op;
            }
        }
    }
}
