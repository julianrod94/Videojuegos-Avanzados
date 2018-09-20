using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;

public class Bitbuffer {
    private ulong bits;

    private int currentBitCount;

    private MemoryStream buffer = new MemoryStream(10000000);

    public void WriteBool(bool value) {
        WriteBit(value);
    }

    private void WriteBit(bool value) {
        ulong longValue = value ? 1UL : 0UL;    
        bits |= longValue << currentBitCount;
        currentBitCount++;
        WriteIfNecessary();
    }

    private void WriteBits(long value, int bitCount) {
        ulong uvalue = (ulong) value;
        bits |= uvalue << currentBitCount;
        currentBitCount+=bitCount;
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
        int minFloor = Mathf.FloorToInt(min);
        float minDecimal = min - minFloor;
        if((minDecimal/step)%1 != 0) throw new ArgumentException("Min must be divisible by step");    
        int maxFloor = Mathf.FloorToInt(min);
        float maxDecimal = max - maxFloor;
        if((maxDecimal/step)%1 != 0) throw new ArgumentException("Max must be divisible by step"); 
        if(((value-min)/step)%1 != 0) throw new ArgumentException("Value must be divisible by step"); 
        
        int bits = Mathf.CeilToInt(Mathf.Log((max-min + 1)/step, 2));
        int countedValue = Mathf.FloorToInt((value - min) / step);
        WriteBits(countedValue, bits);
    }

    public void flush() {
        if(currentBitCount == 0) return;
        if (buffer.Position + 4 > buffer.Capacity) {
            throw new InvalidOperationException("write buffer overflow");
        }
        
        int bytes = Mathf.CeilToInt(currentBitCount / 8f);
        for (int i = 0; i < bytes; i++) {
            byte word = (byte) bits;
            bits >>= 8;
            buffer.WriteByte(word);
        }
        currentBitCount = 0;
    }

    public void toRead() {
        buffer.Position = 0;
    }

    public bool ReadBit() {
        if (currentBitCount < 1) {
            loadInt();
        }
        currentBitCount--;
        bool answer = (bits & 1) == 1;
        bits >>= 1;
        return answer;
    }
    
    private ulong ReadBits(int bitCount) {
        if (currentBitCount < bitCount) {
            loadInt();
        }
        currentBitCount -= bitCount;
        ulong word =  bits & (ulong)~(-1<<bitCount);
        bits >>= bitCount;
        return word;
    }

    public int ReadInt(int min, int max) {
        if(min>=max) throw new ArgumentException("min should be lower than max");
        int intSize = Mathf.CeilToInt(Mathf.Log(max-min+1, 2));
        return (int) ReadBits(intSize) + min;
    }

    public float ReadFloat(float min, float max, float step) {
        //TODO Revisar
        if(min>=max) throw new ArgumentException("min should be lower than max");
        int minFloor = Mathf.FloorToInt(min);
        float minDecimal = min - minFloor;
        if((minDecimal/step)%1 != 0) throw new ArgumentException("Min must be divisible by step");    
        int maxFloor = Mathf.FloorToInt(min);
        float maxDecimal = max - maxFloor;
        if((maxDecimal/step)%1 != 0) throw new ArgumentException("Max must be divisible by step"); 
        int floatSize = Mathf.CeilToInt(Mathf.Log((max-min)/step, 2));
        return ReadBits(floatSize);
    }
    
    private void loadInt() {
        byte d = (byte) buffer.ReadByte();
        byte c = (byte) buffer.ReadByte();
        byte b = (byte) buffer.ReadByte();
        byte a = (byte) buffer.ReadByte();
        long word = 0;
        word |= d << 24;
        word |= c << 16;
        word |= b << 8;
        word |= a;
        word <<= currentBitCount;
        bits |= (ulong)word;
        currentBitCount += 32;
    }

    private string Reverse(string s) {
        char[] arr = s.ToCharArray();
        Array.Reverse(arr);
        return new string(arr);
    }

    private string PrettyPrint() {
        return Reverse(Regex.Replace(Reverse(Convert.ToString((long)bits, 2).PadLeft(64, '0')), ".{6}", "$0-"));
    }

    private void WriteIfNecessary() {
        if (currentBitCount < 32) return;
        if (buffer.Position + 4 > buffer.Capacity) {
            throw new InvalidOperationException("write buffer overflow");
        }

        int word = (int) bits;
        byte a = (byte) (word);
        byte b = (byte) (word >> 8);
        byte c = (byte) (word >> 16);
        byte d = (byte) (word >> 24);
        buffer.WriteByte(d);
        buffer.WriteByte(c);
        buffer.WriteByte(b);
        buffer.WriteByte(a);
        bits = bits>> 32;
        currentBitCount -= 32;
    }
}