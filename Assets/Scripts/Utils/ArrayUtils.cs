using System;

namespace Utils {
    public static class ArrayUtils {
        
        public static byte[] AddByteToArray(byte[] bArray, byte newByte) {
            var newArray = new byte[bArray.Length + 1];
            bArray.CopyTo(newArray, 0);
            newArray[bArray.Length] = newByte;
            return newArray;
        }
        
        public static byte[] AddBytesToArray(byte[] bArray, byte[] newBytes) {
            var newArray = new byte[bArray.Length + newBytes.Length];
            bArray.CopyTo(newArray, 0);
            newBytes.CopyTo(newArray, bArray.Length);
            return newArray;
        }

        public static byte[] RemoveBytes(byte[] bArray, int amount) {
            var newArray = new byte[bArray.Length - amount];
            Buffer.BlockCopy(bArray,0,newArray,0,bArray.Length - amount);
            return newArray;
        }
    }
}