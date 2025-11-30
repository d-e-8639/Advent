using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;


public class BitList {
    private List<bool> _bits = new List<bool>();

    public void Add(bool b) {
        _bits.Add(b);
    }

    public void Add(uint data, int count) {
        for (int i=0; i < count; i++) {
            Add(((data >> i) & 0x1) == 1 ? true : false);
        }
    }

    private readonly byte[] setBits = new byte[]{0x1, 0x2, 0x4, 0x8, 0x10, 0x20, 0x40, 0x80};
    public List<byte> ToBytes() {
        List<byte> data = new List<byte>();
        for (int i=0; i < _bits.Count; i+=8) {
            byte b=0;
            int lim = _bits.Count - i >= 8 ? 8 : _bits.Count - i;
            for (int j=0; j < lim; j++) {
                if (_bits[i + j]) {
                    b |= setBits[j];
                }
                
            }
            data.Add(b);
        }
        return data;
    }

    public override string ToString() {
        // return string.Join(System.Environment.NewLine,
        //      _bits.Select(b => b ? '1' : '0')
        //     .Chunk(32)
        //     .Select(chunk32 => string.Join('-', chunk32.Chunk(8).Select(c => new string(c))))
        //     );

        StringBuilder sb = new StringBuilder();
        int i=0;
        foreach (bool b in _bits) {
            if (i % 32 == 0 && i > 0) {
                sb.AppendLine();
            }
            else if (i % 8 == 0 && i > 0) {
                sb.Append('-');
            }
            sb.Append(b ? '1' : '0');
            i++;
        }
        return sb.ToString();
    }

}


/*
public class BitList {
    // List<byte> bits = new List<byte>();
    private int count = 0;
    private int byteIndex => count >> 5;
    List<uint> _bits = new List<uint>();


    private static readonly uint[] singleBits = new uint[32];
    static BitList (){
        for (int i=0; i < 32; i++) {
            singleBits[i] = ((uint)1) << (31 - i);
        }
    }

    public void Add(bool b) {
        if ((count & 0x1F) == 0) {
            _bits.Add(0);
        }
        if (b) {
            _bits[byteIndex] |= singleBits[count & 0x1F];
        }
        count ++;
    }

    public void Add(uint data, int count) {
        int neededSize = this.count + count;

        if ((neededSize >> 5) + ((neededSize & 0x1F) > 0 ? 1 : 0) > _bits.Count) {
            _bits.Add(0);
        }

        int bitsAvailableForFirstPart = 32 - (this.count & 0x1F);
        int bitsInSecondPart = count - bitsAvailableForFirstPart;

        // We may need to split the bits we're adding up to fit them into the remaining spots in the bit array.
        if (bitsInSecondPart <= 0) {
            // All inserted bits will fit into available space in the uint at the end of the list.
            _bits[byteIndex] |= data << ((32 - count) - (this.count & 0x1F));
        }
        else {
            // left part
            _bits[byteIndex] |= data >> (count - bitsAvailableForFirstPart);

            // right part
            _bits[byteIndex + 1] |= data << (32 - bitsInSecondPart);
        }

        this.count += count;
    }

    public List<byte> ToBytes() {
        List<byte> data = new List<byte>();
        int chunkIndex = 24;
        for (int i=0 ; i < this.count; i+=8) {
            data.Add((byte) ((_bits[i >> 5] >> chunkIndex) & 0xFF));
            chunkIndex -=8;
            chunkIndex = chunkIndex < 0 ? 24 : chunkIndex;
        }
        return data;
    }

    public override string ToString()
    {
        StringBuilder sb = new StringBuilder();

        return string.Join(
            System.Environment.NewLine,
            _bits.Select(data => Convert.ToString((long)data, 2).PadLeft(32, '0'))
                 .Select(part => string.Join(' ', part.Chunk(8).Select(chunk => new string(chunk.ToArray()))))
        );
    }

    public static string ToString(uint x) {
        List<string> chunks = new List<string>();
        for (int i=0; i < 32; i +=8) {
            chunks.Add(Convert.ToString(x >> i & 0xFF));
        }
        return string.Join(" ", chunks);
    }
}
*/
