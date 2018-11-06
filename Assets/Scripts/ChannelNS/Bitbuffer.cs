using System;
using System.IO;
using System.Text.RegularExpressions;
using UnityEngine;

namespace ChannelNS {
    public class Bitbuffer {
        private static readonly float epsilon = 1E-3f;
        private ulong _bits;

        private int _currentBitCount;

        private MemoryStream buffer = new MemoryStream(100);

        public void WriteBool(bool value) {
            WriteBit(value);
        }

        public void WriteByte(byte b) {
            WriteBits(b, 8);
        }

        private void WriteBit(bool value) {
            var longValue = value ? 1UL : 0UL;
            _bits |= longValue << _currentBitCount;
            _currentBitCount++;
            WriteIfNecessary();
        }

        private void WriteBits(long value, int bitCount) {
            var uvalue = (ulong) value;
            _bits |= uvalue << _currentBitCount;
            _currentBitCount += bitCount;
            WriteIfNecessary();
        }

        public void WriteInt(int value, int min, int max) {
            if (min >= max) throw new ArgumentException("min should be lower than max");
            if (value > max || value < min) throw new ArgumentException("Value must be between min and max");
            var bits = Mathf.CeilToInt(Mathf.Log(max - min + 1, 2));
            WriteBits(value - min, bits);
        }

        public void WriteFloatRounded(float value, float min, float max, float step) {
            var newValue = Mathf.RoundToInt((value - min) / step) * step + min;
            WriteFloat(newValue, min, max, step);
        }

        public void WriteFloat(float value, float min, float max, float step) {
            if (min >= max) throw new ArgumentException("min should be lower than max");
            if (value > max || value < min) throw new ArgumentException("Value must be between min and max");
            var divisor = (max - min) / step;
            if (Mathf.Abs(divisor - Mathf.RoundToInt(divisor)) > epsilon)
                throw new ArgumentException("Range (" + min + "-" + max + ") must be divisible by step " + step);

            divisor = (value - min) / step;
            if (Mathf.Abs(divisor - Mathf.RoundToInt(divisor)) > epsilon)
                throw new ArgumentException("Value must be divisible by step " +
                                            Mathf.Abs(divisor - Mathf.RoundToInt(divisor)));

            var bits = Mathf.CeilToInt(Mathf.Log((max - min + 1) / step, 2));
            var countedValue = Mathf.RoundToInt((value - min) / step);
            WriteBits(countedValue, bits);
        }

        public void Flush() {
            if (_currentBitCount == 0) return;
            if (buffer.Position + 4 > buffer.Capacity) throw new InvalidOperationException("write buffer overflow");

            var bytes = Mathf.CeilToInt(_currentBitCount / 8f);
            for (var i = 0; i < bytes; i++) {
                var word = (byte) _bits;
                _bits >>= 8;
                buffer.WriteByte(word);
            }

            _currentBitCount = 0;
            _bits = 0;
        }

        public void ToRead() {
            Flush();
            buffer.Position = 0;
        }
        
        public byte Readbyte() {
            return (byte)ReadBits(8);
        }

        public bool ReadBit() {
            if (_currentBitCount < 1) LoadInt();
            _currentBitCount--;
            var answer = (_bits & 1) == 1;
            _bits >>= 1;
            return answer;
        }

        private ulong ReadBits(int bitCount) {
            if (_currentBitCount < bitCount) LoadInt();
            _currentBitCount -= bitCount;
            var word = _bits & (ulong) ~(-1 << bitCount);
            _bits >>= bitCount;
            return word;
        }

        public int ReadInt(int min, int max) {
            if (min >= max) throw new ArgumentException("min should be lower than max");
            var intSize = Mathf.CeilToInt(Mathf.Log(max - min + 1, 2));
            var answer = (uint) ReadBits(intSize);
            if (answer + min > max) throw new ArgumentException("value was greater than max");
            return (int) (answer + min);
        }

        public float ReadFloat(float min, float max, float step) {
            if (min >= max) throw new ArgumentException("min should be lower than max");
            var divisor = (max - min) / step;
            if (Mathf.Abs(divisor - Mathf.RoundToInt(divisor)) > epsilon)
                throw new ArgumentException("Range (" + min + "-" + max + ") must be divisible by step " + step);
            var floatSize = Mathf.CeilToInt(Mathf.Log((max - min) / step, 2));
            return min + ReadBits(floatSize) * step;
        }

        private void LoadInt() {
            var a = (byte) buffer.ReadByte();
            var b = (byte) buffer.ReadByte();
            var c = (byte) buffer.ReadByte();
            var d = (byte) buffer.ReadByte();
            ulong word = 0;
            word = word | a;
            word = word | (uint) (b << 8);
            word = word | (uint) (c << 16);
            word = word | (uint) (d << 24);
            word <<= _currentBitCount;
            _bits |= word;
            _currentBitCount += 32;
        }

        private string Reverse(string s) {
            var arr = s.ToCharArray();
            Array.Reverse(arr);
            return new string(arr);
        }

        private string PrettyPrint() {
            return Reverse(Regex.Replace(Reverse(Convert.ToString((long) _bits, 2).PadLeft(64, '0')), ".{6}", "$0-"));
        }

        private void WriteIfNecessary() {
            if (_currentBitCount < 32) return;
            if (buffer.Position + 4 > buffer.Capacity) throw new InvalidOperationException("write buffer overflow");

            ulong word = (uint) _bits;
            var a = (byte) word;
            var b = (byte) (word >> 8);
            var c = (byte) (word >> 16);
            var d = (byte) (word >> 24);
            buffer.WriteByte(a);
            buffer.WriteByte(b);
            buffer.WriteByte(c);
            buffer.WriteByte(d);
            _bits >>= 32;
            _currentBitCount -= 32;
        }

        public byte[] GenerateBytes() {
            Flush();
            var size = buffer.Length;
            var dest = new byte[size];
            Array.Copy(buffer.GetBuffer(), dest, size);
            return dest;
        }

        public void ToWrite() {
            _currentBitCount = 0;
            _bits = 0;
            buffer.Position = 0;
        }

        public void LoadBytes(byte[] bytes) {
            //TODO look for a better way for this
            buffer = new MemoryStream(100);
            buffer.Write(bytes, 0, bytes.Length);
        }
    }
}