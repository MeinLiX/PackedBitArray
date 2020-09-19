using System;

namespace PackedBitArray.lib
{
    class PackedBitArray
    {
        private long[] Battary { get; set; } //Buffer
        private int Capacity { get; set; } //Buffer capacity
        private byte ValueBits { get; set; } //Value bits size
        private long Mask { get; set; } //Max Value

        public PackedBitArray(byte valueBits, int capacity)
        {
            if (valueBits < 4 || valueBits > 14) // 1< BITS >64
                throw new Exception("Bits per value incorrect");

            if (capacity < 1)
                throw new Exception("Capacity can't be negative");
            Capacity = capacity;
            ValueBits = valueBits;

            Battary = new long[(int)Math.Ceiling(valueBits * capacity / 64.0)];

            Mask = ((long)1 << ValueBits) - (long)1;
        }

        public int this[int i]
        {
            get { return this.Get(i); }
            set { this.Set(i, value); }
        }

        private int Get(int index)
        {
            if (index < 0 || index >= Capacity)
                throw new Exception($"Index out of range(Max {Capacity})");

            index *= ValueBits; // taken index in bites

            int ArrIndex = index >> 6; // 64|128..._valueBits * capacity
            int StartPointer = index & 63; // Get first bit position 

            long value = Battary[ArrIndex] >> StartPointer;

            int EndPointer = StartPointer + ValueBits; // Get last bit position 
            if (EndPointer > 64)
                value |= Battary[++ArrIndex] << 64 - StartPointer;

            return (int)(value & Mask);
        }

        private void Set(int index, long value)
        {
            if (index < 0 || index >= Capacity)
                throw new Exception($"Index out of range(Max {Capacity})");

            if (value < 0 || value > Mask)
                throw new Exception($"Value out of range(Max {Mask})");

            index *= ValueBits; // taken index in bites

            int ArrIndex = index >> 6; // 64|128..._valueBits * capacity
            int StartPointer = index & 63; // Get first bit position 

            Battary[ArrIndex] = Battary[ArrIndex] & ~(Mask << StartPointer) | (Mask & value) << StartPointer;

            int EndPointer = StartPointer + ValueBits; // Get last bit position 
            if (EndPointer > 64)
                Battary[++ArrIndex] = Battary[ArrIndex] & ~(((long)1 << EndPointer - 64) - (long)1) | value >> 64 - StartPointer;
        }
    }
}
