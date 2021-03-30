using System;
using System.Diagnostics;

namespace ToxicRagers.Helpers
{
    public class PVTRC
    {
        const int PT_INDEX = 2;
        const int BLK_X_2BPP = 8;
        const int BLK_X_4BPP = 4;
        const int BLK_Y_SIZE = 4;

        public class Rep
        {
            public int[,] Reps { get; set; } = new int[2, 4];
        }

        public class AMTCBlock
        {
            public uint[] PackedData { get; set; } = new uint[2];

            public void SetData(byte[] data, uint index)
            {
                PackedData[0] = BitConverter.ToUInt32(data, (int)index);
                PackedData[1] = BitConverter.ToUInt32(data, (int)index + 4);
            }

            public override bool Equals(object obj)
            {
                if ((obj == null) || !GetType().Equals(obj.GetType()))
                {
                    return false;
                }
                else
                {
                    AMTCBlock b = (AMTCBlock)obj;
                    return (PackedData[0] == b.PackedData[0]) && (PackedData[1] == b.PackedData[1]);
                }
            }

            public override int GetHashCode()
            {
                return base.GetHashCode();
            }
        }

        public static void Decompress(byte[] rawData, bool do2BitMode, int xDim, int yDim, bool assumeImageTiles, byte[] resultData)
        {
            int x, y;
            int i, j;

            int blkX, blkY;
            int blkXp1, blkYp1;
            int xBlockSize;
            int blkXDim, blkYDim;

            int startX, startY;

            int[,] ModulationVals = new int[8, 16];
            int[,] ModulationModes = new int[8, 16];
            int uPosition;
            int Mod = 0;
            bool DoPT = false;

            int[] ASig = new int[4], BSig = new int[4];
            int[] Result = new int[4];


            AMTCBlock[,] pBlocks = new AMTCBlock[2, 2];
            AMTCBlock[,] pPrevious = new AMTCBlock[2, 2];

            pBlocks[0, 0] = new AMTCBlock();
            pBlocks[0, 1] = new AMTCBlock();
            pBlocks[1, 0] = new AMTCBlock();
            pBlocks[1, 1] = new AMTCBlock();
            pPrevious[0, 0] = new AMTCBlock();
            pPrevious[0, 1] = new AMTCBlock();
            pPrevious[1, 0] = new AMTCBlock();
            pPrevious[1, 1] = new AMTCBlock();

            Rep[,] Colours5554 = new Rep[2, 2];

            Colours5554[0, 0] = new Rep();
            Colours5554[0, 1] = new Rep();
            Colours5554[1, 0] = new Rep();
            Colours5554[1, 1] = new Rep();

            xBlockSize = do2BitMode ? BLK_X_2BPP : BLK_X_4BPP;

            blkXDim = Math.Max(2, xDim / xBlockSize);
            blkYDim = Math.Max(2, yDim / BLK_Y_SIZE);

            for (y = 0; y < yDim; y++)
            {
                for (x = 0; x < xDim; x++)
                {
                    // map this pixel to the top left neighbourhood of blocks
                    blkX = x - xBlockSize / 2;
                    blkY = y - BLK_Y_SIZE / 2;

                    blkX = limitCoord(blkX, xDim, assumeImageTiles);
                    blkY = limitCoord(blkY, yDim, assumeImageTiles);

                    blkX /= xBlockSize;
                    blkY /= BLK_Y_SIZE;

                    // compute the positions of the other 3 blocks
                    blkXp1 = limitCoord(blkX + 1, blkXDim, assumeImageTiles);
                    blkYp1 = limitCoord(blkY + 1, blkYDim, assumeImageTiles);

                    pBlocks[0, 0].SetData(rawData, 8 * twiddleUV((uint)blkYDim, (uint)blkXDim, (uint)blkY, (uint)blkX));
                    pBlocks[0, 1].SetData(rawData, 8 * twiddleUV((uint)blkYDim, (uint)blkXDim, (uint)blkY, (uint)blkXp1));
                    pBlocks[1, 0].SetData(rawData, 8 * twiddleUV((uint)blkYDim, (uint)blkXDim, (uint)blkYp1, (uint)blkX));
                    pBlocks[1, 1].SetData(rawData, 8 * twiddleUV((uint)blkYDim, (uint)blkXDim, (uint)blkYp1, (uint)blkXp1));

                    // extract the colours and the modulation information IF the previous values have changed
                    if (!pPrevious[0, 0].Equals(pBlocks[0, 0]) |
                        !pPrevious[0, 1].Equals(pBlocks[0, 1]) |
                        !pPrevious[1, 0].Equals(pBlocks[1, 0]) |
                        !pPrevious[1, 1].Equals(pBlocks[1, 1]))
                    {
                        startY = 0;

                        for (i = 0; i < 2; i++)
                        {
                            startX = 0;

                            for (j = 0; j < 2; j++)
                            {
                                unpack5554Colour(pBlocks[i, j], Colours5554[i, j].Reps);

                                unpackModulations(pBlocks[i, j],
                                                  do2BitMode,
                                                  ModulationVals,
                                                  ModulationModes,
                                                  startX, startY);

                                startX += xBlockSize;
                            }

                            startY += BLK_Y_SIZE;
                        }

                        // make a copy of the new pointers
                        pPrevious[0, 0].PackedData[0] = pBlocks[0, 0].PackedData[0];
                        pPrevious[0, 0].PackedData[1] = pBlocks[0, 0].PackedData[1];
                        pPrevious[0, 1].PackedData[0] = pBlocks[0, 1].PackedData[0];
                        pPrevious[0, 1].PackedData[1] = pBlocks[0, 1].PackedData[1];
                        pPrevious[1, 0].PackedData[0] = pBlocks[1, 0].PackedData[0];
                        pPrevious[1, 0].PackedData[1] = pBlocks[1, 0].PackedData[1];
                        pPrevious[1, 1].PackedData[0] = pBlocks[1, 1].PackedData[0];
                        pPrevious[1, 1].PackedData[1] = pBlocks[1, 1].PackedData[1];
                    }

                    // decompress the pixel.  First compute the interpolated A and B signals
                    interpolateColours(Colours5554[0, 0].Reps,
                                       Colours5554[0, 1].Reps,
                                       Colours5554[1, 0].Reps,
                                       Colours5554[1, 1].Reps,
                                       0,
                                       do2BitMode, x, y,
                                       ASig);

                    interpolateColours(Colours5554[0, 0].Reps,
                                       Colours5554[0, 1].Reps,
                                       Colours5554[1, 0].Reps,
                                       Colours5554[1, 1].Reps,
                                       1,
                                       do2BitMode, x, y,
                                       BSig);

                    getModulationValue(x, y, do2BitMode, ModulationVals, ModulationModes, ref Mod, ref DoPT);

                    // compute the modulated colour
                    for (i = 0; i < 4; i++)
                    {
                        Result[i] = ASig[i] * 8 + Mod * (BSig[i] - ASig[i]);
                        Result[i] >>= 3;
                    }

                    if (DoPT) { Result[3] = 0; }

                    // Store the result in the output image
                    uPosition = (x + y * xDim) << 2;
                    resultData[uPosition + 0] = (byte)Result[0];
                    resultData[uPosition + 1] = (byte)Result[1];
                    resultData[uPosition + 2] = (byte)Result[2];
                    resultData[uPosition + 3] = (byte)Result[3];
                }
            }
        }

        private static int limitCoord(int val, int size, bool tiles)
        {
            if (tiles)
            {
                return wrapCoord(val, size);
            }
            else
            {
                return Math.Min(size - 1, Math.Max(val, 0));
            }
        }

        private static int wrapCoord(int val, int size)
        {
            return (val & (size - 1));
        }

        private static bool numberIsPower2(uint imp)
        {
            if (imp == 0) { return false; }

            uint minus1 = imp - 1;
            return ((imp | minus1) == (imp ^ minus1));
        }

        private static uint twiddleUV(uint ySize, uint xSize, uint yPos, uint xPos)
        {
            uint twiddled;

            uint minDimension;
            uint maxValue;

            uint srcBitPos;
            uint dstBitPos;

            int shiftCount;

            Debug.Assert(yPos < ySize);
            Debug.Assert(xPos < xSize);

            Debug.Assert(numberIsPower2(ySize));
            Debug.Assert(numberIsPower2(xSize));

            if (ySize < xSize)
            {
                minDimension = ySize;
                maxValue = xPos;
            }
            else
            {
                minDimension = xSize;
                maxValue = yPos;
            }

            // step through all the bits in the "minimum" dimension
            srcBitPos = 1;
            dstBitPos = 1;
            twiddled = 0;
            shiftCount = 0;

            while (srcBitPos < minDimension)
            {
                if ((yPos & srcBitPos) > 0)
                {
                    twiddled |= dstBitPos;
                }

                if ((xPos & srcBitPos) > 0)
                {
                    twiddled |= dstBitPos << 1;
                }

                srcBitPos <<= 1;
                dstBitPos <<= 2;
                shiftCount++;
            }

            // prepend any unused bits
            maxValue >>= shiftCount;
            twiddled |= maxValue << (2 * shiftCount);

            return twiddled;
        }

        private static void unpack5554Colour(AMTCBlock pBlock, int[,] abColours)
        {
            uint[] rawBits = new uint[2];
            int i;

            // extract A and B
            rawBits[0] = pBlock.PackedData[1] & 0xfffe;       // 15 bits (shifted up by one)
            rawBits[1] = pBlock.PackedData[1] >> 16;          // 16 bits

            // step through both colours
            for (i = 0; i < 2; i++)
            {
                // if completely opaque
                if ((rawBits[i] & (1 << 15)) > 0)
                {
                    // extract R and G (both 5 bit)
                    abColours[i, 0] = (int)((rawBits[i] >> 10) & 0x1f);
                    abColours[i, 1] = (int)((rawBits[i] >> 5) & 0x1f);

                    // the precision of Blue depends on A or B.  If A then we need to
                    // replicate the top bit to get 5 bits in total

                    abColours[i, 2] = (int)(rawBits[i] & 0x1f);

                    if (i == 0) { abColours[0, 2] |= abColours[0, 2] >> 4; }

                    // set 4bit alpha fully on...
                    abColours[i, 3] = 0xf;
                }
                else
                {
                    // else if colour has variable translucency

                    // extract r and g (both 4 bit)
                    // (leave a space on the end for the replication of bits)
                    abColours[i, 0] = (int)((rawBits[i] >> (8 - 1)) & 0x1e);
                    abColours[i, 1] = (int)((rawBits[i] >> (4 - 1)) & 0x1e);

                    // replicate bits to truly expand to 5 bits
                    abColours[i, 0] |= abColours[i, 0] >> 4;
                    abColours[i, 1] |= abColours[i, 1] >> 4;

                    // grab the 3(+padding) or 4 bits of blue and add an extra padding bit
                    abColours[i, 2] = (int)((rawBits[i] & 0xf) << 1);

                    // expand from 3 to 5 bits if this is from colour A, or 4 to 5 bits if from
                    // colour B
                    abColours[0, 2] |= abColours[0, 2] >> (i == 0 ? 3 : 4);

                    // Set the alpha bits to be 3 + a zero on the end
                    abColours[i, 3] = (int)(rawBits[i] >> 11) & 0xe;
                }
            }
        }

        private static void unpackModulations(AMTCBlock pBlock, bool do2bitMode, int[,] modulationVals, int[,] modulationModes, int startX, int startY)
        {
            int blockModMode;
            uint modulationBits;
            int x, y;

            blockModMode = (int)(pBlock.PackedData[1] & 1);
            modulationBits = pBlock.PackedData[0];

            // if it's in an interpolated mode
            if (do2bitMode && blockModMode > 0)
            {
                // run through all the pixels in the block.  note we can now treat all the
                // "stored" values as if they have 2bits (even when they didn't!)

                for (y = 0; y < BLK_Y_SIZE; y++)
                {
                    for (x = 0; x < BLK_X_2BPP; x++)
                    {
                        modulationModes[y + startY, x + startX] = blockModMode;

                        // if this is a stored value...
                        if (((x ^ y) & 1) == 0)
                        {
                            modulationVals[y + startY, x + startX] = (int)(modulationBits & 3);
                            modulationBits >>= 2;
                        }
                    }
                }
            }
            else if (do2bitMode)    // else if direct encoded 2bit mode - i.e. 1 mode bit per pixel
            {
                for (y = 0; y < BLK_Y_SIZE; y++)
                {
                    for (x = 0; x < BLK_X_2BPP; x++)
                    {
                        modulationModes[y + startY, x + startX] = blockModMode;

                        // double the bits so 0=> 00 and 1=>11
                        if ((modulationBits & 1) > 0)
                        {
                            modulationVals[y + startY, x + startX] = 0x3;
                        }
                        else
                        {
                            modulationVals[y + startY, x + startX] = 0x0;
                        }

                        modulationBits >>= 1;
                    }
                }
            }
            else                    // else its the 4bpp mode so each value has 2 bits
            {
                for (y = 0; y < BLK_Y_SIZE; y++)
                {
                    for (x = 0; x < BLK_X_4BPP; x++)
                    {
                        modulationModes[y + startY, x + startX] = blockModMode;

                        modulationVals[y + startY, x + startX] = (int)(modulationBits & 3);
                        modulationBits >>= 2;
                    }
                }
            }

            if (modulationBits != 0) { throw new OverflowException(); }
        }

        private static void interpolateColours(int[,] colourP, int[,] colourQ, int[,] colourR, int[,] colourS, int index, bool do2bitMode, int x, int y, int[] result)
        {
            int u, v, uscale;
            int k;
            int tmp1, tmp2;

            int[] P = new int[4], Q = new int[4], R = new int[4], S = new int[4];

            // Copy the colours
            for (k = 0; k < 4; k++)
            {
                P[k] = colourP[index, k];
                Q[k] = colourQ[index, k];
                R[k] = colourR[index, k];
                S[k] = colourS[index, k];
            }

            // put the x and y values into the right range
            v = (y & 0x3) | ((~y & 0x2) << 1);

            if (do2bitMode)
            {
                u = (x & 0x7) | ((~x & 0x4) << 1);
            }
            else
            {
                u = (x & 0x3) | ((~x & 0x2) << 1);
            }

            // get the u and v scale amounts
            v -= BLK_Y_SIZE / 2;

            if (do2bitMode)
            {
                u -= BLK_X_2BPP / 2;
                uscale = 8;
            }
            else
            {
                u -= BLK_X_4BPP / 2;
                uscale = 4;
            }

            for (k = 0; k < 4; k++)
            {
                tmp1 = P[k] * uscale + u * (Q[k] - P[k]);
                tmp2 = R[k] * uscale + u * (S[k] - R[k]);

                tmp1 = tmp1 * 4 + v * (tmp2 - tmp1);

                result[k] = tmp1;
            }

            // lop off the appropriate number of bits to get us to 8 bit precision
            if (do2bitMode)
            {
                // do RGB
                for (k = 0; k < 3; k++)
                {
                    result[k] >>= 2;
                }

                result[3] >>= 1;
            }
            else
            {
                // do RGB (A is ok)
                for (k = 0; k < 3; k++)
                {
                    result[k] >>= 1;
                }
            }

            // sanity check
            for (k = 0; k < 4; k++) { if (result[k] >= 256) { throw new OverflowException(); } }

            // convert from 5554 to 8888
            // do RGB 5.3 => 8

            for (k = 0; k < 3; k++)
            {
                result[k] += result[k] >> 5;
            }

            result[3] += result[3] >> 4;

            // 2nd sanity check
            for (k = 0; k < 4; k++) { if (result[k] >= 256) { throw new OverflowException(); } }
        }

        private static void getModulationValue(int x, int y, bool do2bitMode, int[,] modulationVals, int[,] modulationModes, ref int mod, ref bool doPT)
        {
            int[] repVals0 = new int[4] { 0, 3, 5, 8 };
            int[] repVals1 = new int[4] { 0, 4, 4, 8 };

            int modVal;

            // map x and y into the local 2x2 block
            y = (y & 0x3) | ((~y & 0x2) << 1);

            if (do2bitMode)
            {
                x = (x & 0x7) | ((~x & 0x4) << 1);
            }
            else
            {
                x = (x & 0x3) | ((~x & 0x2) << 1);
            }

            // assume no PT for now
            doPT = false;

            // extract the modulation value.  If a simple encoding
            if (modulationModes[y, x] == 0)
            {
                modVal = repVals0[modulationVals[y, x]];
            }
            else if (do2bitMode)
            {
                // if this is a stored value
                if (((x ^ y) & 1) == 0)
                {
                    modVal = repVals0[modulationVals[y, x]];
                }
                else if (modulationModes[y, x] == 1)    // else average from the neighbours if H&V interpolation...
                {
                    modVal = (repVals0[modulationVals[y - 1, x]] +
                              repVals0[modulationVals[y + 1, x]] +
                              repVals0[modulationVals[y, x - 1]] +
                              repVals0[modulationVals[y, x + 2]] + 2) / 4;
                }
                else if (modulationModes[y, x] == 2)    // else if H-only
                {
                    modVal = (repVals0[modulationVals[y, x - 1]] + repVals0[modulationVals[y, x + 1]] + 1) / 2;
                }
                else                                    // else it's V-only
                {
                    modVal = (repVals0[modulationVals[y - 1, x]] + repVals0[modulationVals[y + 1, x]] + 1) / 2;
                }
            }
            else        // else it's 4bpp and PT encoding
            {
                modVal = repVals1[modulationVals[y, x]];
                doPT = modulationVals[y, x] == PT_INDEX;
            }

            mod = modVal;
        }
    }
}
