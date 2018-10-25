using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;

public class Bitbuffer {
    private ulong _bits;

    private int _currentBitCount;

    private MemoryStream buffer = new MemoryStream(1000000);

    private static float epsilon = 1E-4f;

    public void WriteBool(bool value) {
        WriteBit(value);
    }

    private void WriteBit(bool value) {
        ulong longValue = value ? 1UL : 0UL;    
        _bits |= longValue << _currentBitCount;
        _currentBitCount++;
        WriteIfNecessary();
    }

    private void WriteBits(long value, int bitCount) {
        ulong uvalue = (ulong) value;
        _bits |= uvalue << _currentBitCount;
        _currentBitCount+=bitCount;
        WriteIfNecessary();
    }

    public void WriteInt(int value, int min, int max) {
        if(min>=max) throw new ArgumentException("min should be lower than max");
        if(value > max || value < min) throw new ArgumentException("Value must be between min and max");
        int bits = Mathf.CeilToInt(Mathf.Log(max-min+1, 2));
        WriteBits(value - min, bits);
    }

    public void WriteFloat(float value, float min, float max, float step) {
        if(min>=max) throw new ArgumentException("min should be lower than max");
        if(value > max || value < min) throw new ArgumentException("Value must be between min and max");
        var divisor = ((max - min) / step);
        if (Mathf.Abs(divisor - Mathf.RoundToInt(divisor)) > epsilon ) {
            throw new ArgumentException("Range must be divisible by step ");
        }
        
        divisor = (value-min) / step;
        if (Mathf.Abs(divisor - Mathf.RoundToInt(divisor)) > epsilon ) {
            throw new ArgumentException("Value must be divisible by step " + Mathf.Abs(divisor - Mathf.RoundToInt(divisor))); 
        }

        int bits = Mathf.CeilToInt(Mathf.Log((max-min + 1)/step, 2));
        int countedValue = Mathf.RoundToInt((value - min) / step);
        WriteBits(countedValue, bits);
    }

    public void Flush() {
        if(_currentBitCount == 0) return;
        if (buffer.Position + 4 > buffer.Capacity) {
            throw new InvalidOperationException("write buffer overflow");
        }
        
        int bytes = Mathf.CeilToInt(_currentBitCount / 8f);
        for (int i = 0; i < bytes; i++) {
            byte word = (byte) _bits;
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

    public bool ReadBit() {
        if (_currentBitCount < 1) {
            LoadInt();
        }
        _currentBitCount--;
        bool answer = (_bits & 1) == 1;
        _bits >>= 1;
        return answer;
    }
    
    private ulong ReadBits(int bitCount) {
        if (_currentBitCount < bitCount) {
            LoadInt();
        }
        _currentBitCount -= bitCount;
        ulong word =  _bits & (ulong)~(-1<<bitCount);
        _bits >>= bitCount;
        return word;
    }

    public int ReadInt(int min, int max) {
        if(min>=max) throw new ArgumentException("min should be lower than max");
        int intSize = Mathf.CeilToInt(Mathf.Log(max-min+1, 2));
        uint answer = (uint)ReadBits(intSize);
        if (answer + min > max) {
            throw new ArgumentException("value was greater than max");
        }
        return (int) ( answer+ min);
    }

    public float ReadFloat(float min, float max, float step) {
        if(min>=max) throw new ArgumentException("min should be lower than max");
        var divisor = ((max - min) / step);
        if (Mathf.Abs(divisor - Mathf.RoundToInt(divisor)) > epsilon) {
            throw new ArgumentException("Range must be divisible by step ");
        }
        int floatSize = Mathf.CeilToInt(Mathf.Log((max-min)/step, 2));
        return min + ReadBits(floatSize)*step;
    }
    
    private void LoadInt() {
        byte a = (byte) buffer.ReadByte();
        byte b = (byte) buffer.ReadByte();
        byte c = (byte) buffer.ReadByte();
        byte d = (byte) buffer.ReadByte();
        ulong word = 0;
        word = word | a;
        word = word | (uint)(b << 8);
        word = word | (uint)(c << 16);
        word = word | (uint)(d << 24);
        word <<= _currentBitCount;
        _bits |= word;
        _currentBitCount += 32;
    }

    private string Reverse(string s) {
        char[] arr = s.ToCharArray();
        Array.Reverse(arr);
        return new string(arr);
    }
    
    private string PrettyPrint() {
        return Reverse(Regex.Replace(Reverse(Convert.ToString((long)_bits, 2).PadLeft(64, '0')), ".{6}", "$0-"));
    }

    private void WriteIfNecessary() {
        if (_currentBitCount < 32) return;
        if (buffer.Position + 4 > buffer.Capacity) {
            throw new InvalidOperationException("write buffer overflow");
        }

        ulong word = ((uint) _bits);
        byte a = (byte) (word);
        byte b = (byte) (word >> 8);
        byte c = (byte) (word >> 16);
        byte d = (byte) (word >> 24);
        buffer.WriteByte(a);
        buffer.WriteByte(b);
        buffer.WriteByte(c);
        buffer.WriteByte(d);
        _bits >>= 32;
        _currentBitCount -= 32;
    }

    public byte[] GenerateBytes() {
        Flush();
        long size = buffer.Length;
        byte[] dest = new byte[size];
        Array.Copy(buffer.GetBuffer(), dest, size);
        return dest;
    }

    public void ToWrite() {
        _currentBitCount = 0;
        _bits = 0;
        buffer.Position = 0;
    }
}