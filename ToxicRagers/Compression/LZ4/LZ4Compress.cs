/*
   LZ4 - Fast LZ compression algorithm
   Copyright (C) 2011-2012, Yann Collet.
   BSD 2-Clause License (http://www.opensource.org/licenses/bsd-license.php)
   Redistribution and use in source and binary forms, with or without
   modification, are permitted provided that the following conditions are
   met:
	   * Redistributions of source code must retain the above copyright
   notice, this list of conditions and the following disclaimer.
	   * Redistributions in binary form must reproduce the above
   copyright notice, this list of conditions and the following disclaimer
   in the documentation and/or other materials provided with the
   distribution.
   THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS
   "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT
   LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR
   A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT
   OWNER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL,
   SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT
   LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE,
   DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY
   THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
   (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE
   OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
   You can contact the author at :
   - LZ4 homepage : http://fastcompression.blogspot.com/p/lz4.html
   - LZ4 source repository : http://code.google.com/p/lz4/
*/

using System;
using System.IO;

namespace ToxicRagers.Compression.LZ4
{
    public class LZ4Compress : Stream
    {
        private Stream stream;

        private const int MINMATCH = 4;
        private const int COPYLENGTH = 8;
        private const int MFLIMIT = COPYLENGTH + MINMATCH;

        private const int LZ4_minLength = MFLIMIT + 1;
        private const int LZ4_skipTrigger = 6;

        private const int MAXD_LOG = 16;
        private const int MAX_DISTANCE = ((1 << MAXD_LOG) - 1);

        private const int MEMORY_USAGE = 14;

        private const int HASH_LOG = MEMORY_USAGE - 2;
        private const int HASH_TABLESIZE = 1 << HASH_LOG;
        private const int HASH_ADJUST = (MINMATCH * 8) - HASH_LOG;

        private const int ML_BITS = 4;
        private const int ML_MASK = ((1 << ML_BITS) - 1);
        private const int RUN_BITS = (8 - ML_BITS);
        private const int RUN_MASK = ((1 << RUN_BITS) - 1);

        private const int BLOCK_COPY_LIMIT = 16;

        private const int LASTLITERALS = 5;

        private const int STEPSIZE_32 = 4;

        private static readonly int[] DEBRUIJN_TABLE_32 = new[] {
            0, 0, 3, 0, 3, 1, 3, 0, 3, 2, 2, 1, 3, 2, 0, 1,
            3, 3, 1, 2, 2, 2, 2, 0, 3, 1, 2, 0, 1, 0, 1, 1
        };

        public LZ4Compress(MemoryStream ms)
        {
            stream = ms;
        }

        public override bool CanRead => throw new NotImplementedException();
        public override bool CanSeek => throw new NotImplementedException();
        public override bool CanWrite => throw new NotImplementedException();
        public override long Length => throw new NotImplementedException();

        public override long Position
        {
            get => throw new NotImplementedException();
            set => throw new NotImplementedException();
        }

        public static int Compress(int[] hash, byte[] source, byte[] dest, int size, int maxOutputSize)
        {
            // maxOutputSize = 0
            // outputLimited = notLimited
            // tableType = LZ4_64bits() ? byU32 : byPtr
            // dict = noDictq
            // dictIssue = noDictIssue

            int ip = 0;
            int basep = ip;
            int lowLimit = ip;
            int anchor = ip;

            int op = 0;

            int mflimit = source.Length - MFLIMIT;
            int olimit = op + maxOutputSize;

            int src_end = ip + size;
            int src_LASTLITERALS = src_end - LASTLITERALS;
            int src_LASTLITERALS_1 = src_LASTLITERALS - 1;
            int src_LASTLITERALS_STEPSIZE_1 = src_LASTLITERALS - (STEPSIZE_32 - 1);

            uint forwardH;
            uint h;

            if (size > LZ4_minLength)
            {
                hash[(((lookInt32(source, ip)) * 2654435761u) >> HASH_ADJUST)] = (ip - basep);
                ip++;
                forwardH = (((lookInt32(source, ip)) * 2654435761u) >> HASH_ADJUST);

                while (true)
                {
                    int match;
                    int token;
                    int forwardIp = ip;
                    int step = 1;
                    int searchMatchNb = (1 << LZ4_skipTrigger) + 3;

                    // Find a match
                    do
                    {
                        h = forwardH;
                        ip = forwardIp;
                        forwardIp += step;
                        step = searchMatchNb++ >> LZ4_skipTrigger;

                        if (forwardIp > mflimit) { goto _last_literals; }

                        match = basep + hash[h];
                        forwardH = (((lookInt32(source, forwardIp)) * 2654435761u) >> HASH_ADJUST);
                        hash[h] = (ip - basep);
                    } while ((match < ip - MAX_DISTANCE) || (!compareInt32(source, match, ip)));

                    // Catch up
                    while ((ip > anchor) && (match > lowLimit) && (source[ip - 1] == source[match - 1]))
                    {
                        ip--;
                        match--;
                    }

                    // Encode Literal length
                    int litLength = (ip - anchor);
                    token = op++;

                    if (op + litLength + (litLength >> 8) > olimit)
                    {
                        return 0; // Check output limit
                    }

                    if (litLength >= RUN_MASK)
                    {
                        int len = litLength - RUN_MASK;
                        dest[token] = (RUN_MASK << ML_BITS);
                        if (len > 254)
                        {
                            do
                            {
                                dest[op++] = 255;
                                len -= 255;
                            } while (len > 254);
                            dest[op++] = (byte)len;

                            blockCopy(source, anchor, dest, op, litLength);
                            op += litLength;

                            goto _next_match;
                        }
                        else
                        {
                            dest[op++] = (byte)len;
                        }
                    }
                    else
                    {
                        dest[token] = (byte)(litLength << ML_BITS);
                    }

                    // Copy Literals
                    if (litLength > 0)
                    {
                        wildCopy(source, anchor, dest, op, op + litLength);
                        op += litLength;
                    }

                _next_match:
                    // Encode Offset
                    writeInt16(dest, op, (ushort)(ip - match));
                    op += 2;

                    // Start Counting
                    ip += MINMATCH;
                    match += MINMATCH; // MinMatch already verified
                    anchor = ip;

                    while (ip < src_LASTLITERALS_STEPSIZE_1)
                    {
                        int diff = (int)xor4(source, match, ip);

                        if (diff == 0)
                        {
                            ip += STEPSIZE_32;
                            match += STEPSIZE_32;
                            continue;
                        }

                        ip += DEBRUIJN_TABLE_32[((uint)((diff) & -(diff)) * 0x077CB531u) >> 27];

                        goto _endCount;
                    }

                    if ((ip < src_LASTLITERALS_1) && (compareInt16(source, match, ip)))
                    {
                        ip += 2;
                        match += 2;
                    }

                    if ((ip < src_LASTLITERALS) && (source[match] == source[ip])) { ip++; }

                _endCount:
                    // Encode MatchLength
                    litLength = (ip - anchor);

                    if (op + (litLength >> 8) > olimit)
                    {
                        return 0; // Check output limit
                    }

                    if (litLength >= ML_MASK)
                    {
                        dest[token] += ML_MASK;
                        litLength -= ML_MASK;
                        for (; litLength > 509; litLength -= 510)
                        {
                            dest[op++] = 255;
                            dest[op++] = 255;
                        }
                        if (litLength > 254)
                        {
                            litLength -= 255;
                            dest[op++] = 255;
                        }
                        dest[op++] = (byte)litLength;
                    }
                    else
                    {
                        dest[token] += (byte)litLength;
                    }

                    // Test end of chunk
                    if (ip > mflimit)
                    {
                        anchor = ip;
                        break;
                    }

                    // Fill table
                    hash[(((lookInt32(source, ip - 2)) * 2654435761u) >> HASH_ADJUST)] = (ip - 2 - basep);

                    // Test next position

                    h = (((lookInt32(source, ip)) * 2654435761u) >> HASH_ADJUST);
                    match = basep + hash[h];
                    hash[h] = (ip - basep);

                    if ((match > ip - (MAX_DISTANCE + 1)) && (compareInt32(source, match, ip)))
                    {
                        token = op++;
                        dest[token] = 0;
                        goto _next_match;
                    }

                    // Prepare next loop
                    anchor = ip++;
                    forwardH = (((lookInt32(source, ip)) * 2654435761u) >> HASH_ADJUST);
                }
            }

        _last_literals:
            int lastRun = (src_end - anchor);

            if (op + lastRun + 1 + ((lastRun + 255 - RUN_MASK) / 255) > olimit)
            {
                return 0;
            }

            if (lastRun >= RUN_MASK)
            {
                dest[op++] = (RUN_MASK << ML_BITS);
                lastRun -= RUN_MASK;
                for (; lastRun > 254; lastRun -= 255) { dest[op++] = 255; }
                dest[op++] = (byte)lastRun;
            }
            else
            {
                dest[op++] = (byte)(lastRun << ML_BITS);
            }

            blockCopy(source, anchor, dest, op, src_end - anchor);
            op += src_end - anchor;

            return op;
        }

        public static int CalculateChunkSize(int length)
        {
            int chunkSize = 65535;

            while (length % chunkSize > 0 && length % chunkSize < 255)
            {
                chunkSize--;
            }

            return chunkSize;
        }

        private static uint lookInt32(byte[] buffer, int offset)
        {
            return (uint)(buffer[offset + 0] | buffer[offset + 1] << 8 | buffer[offset + 2] << 16 | buffer[offset + 3] << 24);
        }

        private static void writeInt16(byte[] buffer, int offset, ushort value)
        {
            buffer[offset + 0] = (byte)value;
            buffer[offset + 1] = (byte)(value >> 8);
        }

        private static bool compareInt16(byte[] buffer, int a, int b)
        {
            if (buffer[a + 0] != buffer[b + 0]) { return false; }
            if (buffer[a + 1] != buffer[b + 1]) { return false; }
            return true;
        }

        private static bool compareInt32(byte[] buffer, int a, int b)
        {
            if (a < 0 || a > buffer.Length || b < 0 || b > buffer.Length)
            {
                Console.WriteLine();
            }

            if (buffer[a + 0] != buffer[b + 0]) { return false; }
            if (buffer[a + 1] != buffer[b + 1]) { return false; }
            if (buffer[a + 2] != buffer[b + 2]) { return false; }
            if (buffer[a + 3] != buffer[b + 3]) { return false; }
            return true;
        }

        private static void blockCopy(byte[] source, int sOffset, byte[] dest, int dOffset, int length)
        {
            if (length >= BLOCK_COPY_LIMIT)
            {
                Buffer.BlockCopy(source, sOffset, dest, dOffset, length);
            }
            else
            {
                while (length >= 4)
                {
                    dest[dOffset + 0] = source[sOffset + 0];
                    dest[dOffset + 1] = source[sOffset + 1];
                    dest[dOffset + 2] = source[sOffset + 2];
                    dest[dOffset + 3] = source[sOffset + 3];
                    length -= 4;
                    sOffset += 4;
                    dOffset += 4;
                }

                while (length > 0)
                {
                    dest[dOffset++] = source[sOffset++];
                    length--;
                }
            }
        }

        private static int wildCopy(byte[] source, int sOffset, byte[] dest, int dOffset, int destEnd)
        {
            int length = destEnd - dOffset;

            if (length <= 8)
            {
                dest[dOffset + 0] = source[sOffset + 0];
                dest[dOffset + 1] = source[sOffset + 1];
                dest[dOffset + 2] = source[sOffset + 2];
                dest[dOffset + 3] = source[sOffset + 3];
                dest[dOffset + 4] = source[sOffset + 4];
                dest[dOffset + 5] = source[sOffset + 5];
                dest[dOffset + 6] = source[sOffset + 6];
                dest[dOffset + 7] = source[sOffset + 7];

                return 8;
            }
            else
            {
                length = (length + 7) & ~7;
                blockCopy(source, sOffset, dest, dOffset, length);
                return length;
            }
        }

        private static uint xor4(byte[] buffer, int offset1, int offset2)
        {
            return lookInt32(buffer, offset1) ^ lookInt32(buffer, offset2);
        }

        public override void Flush()
        {
            return;
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            throw new NotImplementedException();
        }

        public override void SetLength(long value)
        {
            throw new NotImplementedException();
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            throw new NotImplementedException();
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            int[] hashTable = new int[HASH_TABLESIZE];
            byte[] output = new byte[buffer.Length + (buffer.Length / 255) + 16];
            //int i = 0;

            //while (i < buffer.Length)
            //{
            //byte[] chunk = new byte[Math.Min(buffer.Length - i, output.Length)];

            //Array.Copy(buffer, i, chunk, 0, chunk.Length);
            //Array.Clear(hashTable, 0, hashTable.Length);

            int size = Compress(hashTable, buffer, output, buffer.Length, output.Length);

            stream.Write(output, 0, size);

            //i += chunk.Length;
            //}
        }
    }
}