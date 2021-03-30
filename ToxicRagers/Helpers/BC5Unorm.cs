using System;
using System.Collections.Generic;

namespace ToxicRagers.Helpers
{
    public class BC5Unorm
    {
        public static byte[] Compress(byte[] buffer, ushort width, ushort height, int byteCount)
        {
            List<byte> compressed = new List<byte>();

            for (int pixY = 0, i = 0; pixY < height; pixY += 4)
            {
                for (int pixX = 0; pixX < width; pixX += 4)
                {
                    int startIndex = pixY * width * 4 + pixX * 4;
                    byte[] redPixels = new byte[16];
                    byte[] greenPixels = new byte[16];

                    for (int y = 0; y < 4; y++)
                    {
                        for (int x = 0; x < 4; x++)
                        {
                            redPixels[4 * y + x] = buffer[startIndex + x * 4 + y * width * 4];
                            greenPixels[4 * y + x] = buffer[startIndex + 1 + x * 4 + y * width * 4];
                        }
                    }

                    byte minRedColour = 255;
                    byte maxRedColour = 0;
                    byte minGreenColour = 255;
                    byte maxGreenColour = 0;

                    for (int j = 0; j < 16; j++)
                    {
                        minRedColour = Math.Min(minRedColour, redPixels[j]);
                        minGreenColour = Math.Min(minGreenColour, greenPixels[j]);
                        maxRedColour = Math.Max(maxRedColour, redPixels[j]);
                        maxGreenColour = Math.Max(maxGreenColour, greenPixels[j]);
                    }

                    redPixels = flipBlockRows(redPixels);
                    greenPixels = flipBlockRows(greenPixels);

                    byte[] redIndices = convertIndicesToBytes(calculateIndices(redPixels, minRedColour, maxRedColour));
                    byte[] greenIndices = convertIndicesToBytes(calculateIndices(greenPixels, minGreenColour, maxGreenColour));

                    compressed.Add(maxRedColour);
                    compressed.Add(minRedColour);
                    compressed.Add(redIndices[5]);
                    compressed.Add(redIndices[4]);
                    compressed.Add(redIndices[3]);
                    compressed.Add(redIndices[2]);
                    compressed.Add(redIndices[1]);
                    compressed.Add(redIndices[0]);

                    compressed.Add(maxGreenColour);
                    compressed.Add(minGreenColour);
                    compressed.Add(greenIndices[5]);
                    compressed.Add(greenIndices[4]);
                    compressed.Add(greenIndices[3]);
                    compressed.Add(greenIndices[2]);
                    compressed.Add(greenIndices[1]);
                    compressed.Add(greenIndices[0]);

                    i += 4 * 4;
                }

                i += width * 4 * 3;
            }

            return compressed.ToArray();
        }

        public static byte[] Decompress(byte[] blocks, uint width, uint height)
        {
            byte[] redBuffer = new byte[width * height];
            byte[] greenBuffer = new byte[width * height];

            for (int row = 0, col = 0, j = 0; j < blocks.Length; j += 16, col += 4)
            {
                int k = j + 8;

                byte[] redColours = calcColours(blocks[j], blocks[j + 1]);
                byte[] greenColours = calcColours(blocks[k], blocks[k + 1]);

                uint[] redIndices = getIndices(blocks[j + 7], blocks[j + 6], blocks[j + 5], blocks[j + 4], blocks[j + 3], blocks[j + 2]);
                uint[] greenIndices = getIndices(blocks[k + 7], blocks[k + 6], blocks[k + 5], blocks[k + 4], blocks[k + 3], blocks[k + 2]);

                if (col >= width)
                {
                    col -= (int)width;
                    row += 4;
                }

                redBuffer[(row + 2) * width + col] = redColours[redIndices[0]];
                redBuffer[(row + 2) * width + col + 1] = redColours[redIndices[1]];
                redBuffer[(row + 2) * width + col + 2] = redColours[redIndices[2]];
                redBuffer[(row + 2) * width + col + 3] = redColours[redIndices[3]];

                redBuffer[(row + 3) * width + col] = redColours[redIndices[4]];
                redBuffer[(row + 3) * width + col + 1] = redColours[redIndices[5]];
                redBuffer[(row + 3) * width + col + 2] = redColours[redIndices[6]];
                redBuffer[(row + 3) * width + col + 3] = redColours[redIndices[7]];

                redBuffer[(row + 0) * width + col] = redColours[redIndices[8]];
                redBuffer[(row + 0) * width + col + 1] = redColours[redIndices[9]];
                redBuffer[(row + 0) * width + col + 2] = redColours[redIndices[10]];
                redBuffer[(row + 0) * width + col + 3] = redColours[redIndices[11]];

                redBuffer[(row + 1) * width + col] = redColours[redIndices[12]];
                redBuffer[(row + 1) * width + col + 1] = redColours[redIndices[13]];
                redBuffer[(row + 1) * width + col + 2] = redColours[redIndices[14]];
                redBuffer[(row + 1) * width + col + 3] = redColours[redIndices[15]];

                greenBuffer[(row + 2) * width + col] = greenColours[greenIndices[0]];
                greenBuffer[(row + 2) * width + col + 1] = greenColours[greenIndices[1]];
                greenBuffer[(row + 2) * width + col + 2] = greenColours[greenIndices[2]];
                greenBuffer[(row + 2) * width + col + 3] = greenColours[greenIndices[3]];

                greenBuffer[(row + 3) * width + col] = greenColours[greenIndices[4]];
                greenBuffer[(row + 3) * width + col + 1] = greenColours[greenIndices[5]];
                greenBuffer[(row + 3) * width + col + 2] = greenColours[greenIndices[6]];
                greenBuffer[(row + 3) * width + col + 3] = greenColours[greenIndices[7]];

                greenBuffer[(row + 0) * width + col] = greenColours[greenIndices[8]];
                greenBuffer[(row + 0) * width + col + 1] = greenColours[greenIndices[9]];
                greenBuffer[(row + 0) * width + col + 2] = greenColours[greenIndices[10]];
                greenBuffer[(row + 0) * width + col + 3] = greenColours[greenIndices[11]];

                greenBuffer[(row + 1) * width + col] = greenColours[greenIndices[12]];
                greenBuffer[(row + 1) * width + col + 1] = greenColours[greenIndices[13]];
                greenBuffer[(row + 1) * width + col + 2] = greenColours[greenIndices[14]];
                greenBuffer[(row + 1) * width + col + 3] = greenColours[greenIndices[15]];
            }

            byte[] buffer = new byte[width * height * 4];

            for (uint i = 0, j = 0; i < width * height * 4; i += 4, j++)
            {
                buffer[i + 2] = redBuffer[j];
                buffer[i + 1] = greenBuffer[j];
                buffer[i + 0] = 255;
                buffer[i + 3] = 255;
            }

            return buffer;
        }

        private static byte[] flipBlockRows(byte[] pixels)
        {
            byte[] output = new byte[16];

            output[0] = pixels[8];
            output[1] = pixels[9];
            output[2] = pixels[10];
            output[3] = pixels[11];

            output[4] = pixels[12];
            output[5] = pixels[13];
            output[6] = pixels[14];
            output[7] = pixels[15];

            output[8] = pixels[0];
            output[9] = pixels[1];
            output[10] = pixels[2];
            output[11] = pixels[3];

            output[12] = pixels[4];
            output[13] = pixels[5];
            output[14] = pixels[6];
            output[15] = pixels[7];

            return output;
        }

        private static int[] calculateIndices(byte[] pixels, byte minColour, byte maxColour)
        {
            byte[] colours = calcColours(maxColour, minColour);
            int[] indices = new int[16];

            for (int i = 0; i < 16; i++)
            {
                int index = 0;
                int closestDiff = 255;

                for (int colour = 0; colour < 8; colour++)
                {
                    int diff = 0;

                    if (pixels[i] == colours[colour])
                    {
                        index = colour;
                        break;
                    }

                    if (pixels[i] < colours[colour])
                    {
                        diff = colours[colour] - pixels[i];
                    }
                    else if (pixels[i] > colours[colour])
                    {
                        diff = pixels[i] - colours[colour];
                    }

                    if (diff < closestDiff)
                    {
                        closestDiff = diff;
                        index = colour;
                    }
                }

                indices[i] = index;
            }

            return indices;
        }

        private static byte[] convertIndicesToBytes(int[] indices)
        {
            byte[] output = new byte[6];

            bool[] index7bits = getBitsForIndex(indices[7]);
            output[0] = (byte)(
                (index7bits[2] ? (1 << 7) : 0) +
                (index7bits[1] ? (1 << 6) : 0) +
                (index7bits[0] ? (1 << 5) : 0));

            bool[] index6bits = getBitsForIndex(indices[6]);
            output[0] += (byte)(
                (index6bits[2] ? (1 << 4) : 0) +
                (index6bits[1] ? (1 << 3) : 0) +
                (index6bits[0] ? (1 << 2) : 0));

            bool[] index5bits = getBitsForIndex(indices[5]);
            output[0] += (byte)(
                (index5bits[2] ? (1 << 1) : 0) +
                (index5bits[1] ? (1) : 0));

            bool[] index4bits = getBitsForIndex(indices[4]);
            output[1] = (byte)(
                (index5bits[0] ? (1 << 7) : 0) +
                (index4bits[2] ? (1 << 6) : 0) +
                (index4bits[1] ? (1 << 5) : 0) +
                (index4bits[0] ? (1 << 4) : 0));

            bool[] index3bits = getBitsForIndex(indices[3]);
            output[1] += (byte)(
                (index3bits[2] ? (1 << 3) : 0) +
                (index3bits[1] ? (1 << 2) : 0) +
                (index3bits[0] ? (1 << 1) : 0));

            bool[] index2bits = getBitsForIndex(indices[2]);
            output[1] += (byte)(index2bits[2] ? 1 : 0);

            bool[] index1bits = getBitsForIndex(indices[1]);
            bool[] index0bits = getBitsForIndex(indices[0]);
            output[2] += (byte)(
                (index2bits[1] ? (1 << 7) : 0) +
                (index2bits[0] ? (1 << 6) : 0) +
                (index1bits[2] ? (1 << 5) : 0) +
                (index1bits[1] ? (1 << 4) : 0) +
                (index1bits[0] ? (1 << 3) : 0) +
                (index0bits[2] ? (1 << 2) : 0) +
                (index0bits[1] ? (1 << 1) : 0) +
                (index0bits[0] ? 1 : 0));

            bool[] index15bits = getBitsForIndex(indices[15]);
            output[3] = (byte)(
                (index15bits[2] ? (1 << 7) : 0) +
                (index15bits[1] ? (1 << 6) : 0) +
                (index15bits[0] ? (1 << 5) : 0));

            bool[] index14bits = getBitsForIndex(indices[14]);
            output[3] += (byte)(
                (index14bits[2] ? (1 << 4) : 0) +
                (index14bits[1] ? (1 << 3) : 0) +
                (index14bits[0] ? (1 << 2) : 0));

            bool[] index13bits = getBitsForIndex(indices[13]);
            output[3] += (byte)(
                (index13bits[2] ? (1 << 1) : 0) +
                (index13bits[1] ? 1 : 0));

            bool[] index12bits = getBitsForIndex(indices[12]);
            output[4] = (byte)(
                (index13bits[0] ? (1 << 7) : 0) +
                (index12bits[2] ? (1 << 6) : 0) +
                (index12bits[1] ? (1 << 5) : 0) +
                (index12bits[0] ? (1 << 4) : 0));

            bool[] index11bits = getBitsForIndex(indices[11]);
            output[4] += (byte)(
                (index11bits[2] ? (1 << 3) : 0) +
                (index11bits[1] ? (1 << 2) : 0) +
                (index11bits[0] ? (1 << 1) : 0));

            bool[] index10bits = getBitsForIndex(indices[10]);
            output[4] += (byte)(index10bits[2] ? 1 : 0);

            bool[] index9bits = getBitsForIndex(indices[9]);
            bool[] index8bits = getBitsForIndex(indices[8]);
            output[5] += (byte)(
                (index10bits[1] ? (1 << 7) : 0) +
                (index10bits[0] ? (1 << 6) : 0) +
                (index9bits[2] ? (1 << 5) : 0) +
                (index9bits[1] ? (1 << 4) : 0) +
                (index9bits[0] ? (1 << 3) : 0) +
                (index8bits[2] ? (1 << 2) : 0) +
                (index8bits[1] ? (1 << 1) : 0) +
                (index8bits[0] ? (1) : 0));

            return output;
        }

        private static bool[] getBitsForIndex(int index)
        {
            bool[] output = new bool[3];

            switch (index)
            {
                case 0:
                    output[0] = false;
                    output[1] = false;
                    output[2] = false;
                    break;

                case 1:
                    output[0] = true;
                    output[1] = false;
                    output[2] = false;
                    break;

                case 2:
                    output[0] = false;
                    output[1] = true;
                    output[2] = false;
                    break;

                case 3:
                    output[0] = true;
                    output[1] = true;
                    output[2] = false;
                    break;

                case 4:
                    output[0] = false;
                    output[1] = false;
                    output[2] = true;
                    break;

                case 5:
                    output[0] = true;
                    output[1] = false;
                    output[2] = true;
                    break;

                case 6:
                    output[0] = false;
                    output[1] = true;
                    output[2] = true;
                    break;

                case 7:
                    output[0] = true;
                    output[1] = true;
                    output[2] = true;
                    break;
            }

            return output;
        }

        private static byte[] calcColours(byte minColour, byte maxColour)
        {
            byte[] output = new byte[8];

            output[0] = minColour;
            output[1] = maxColour;

            float colour0 = minColour;
            float colour1 = maxColour;

            if (minColour > maxColour)
            {
                output[2] = (byte)((6 * colour0 + 1 * colour1) / 7.0f);
                output[3] = (byte)((5 * colour0 + 2 * colour1) / 7.0f);
                output[4] = (byte)((4 * colour0 + 3 * colour1) / 7.0f);
                output[5] = (byte)((3 * colour0 + 4 * colour1) / 7.0f);
                output[6] = (byte)((2 * colour0 + 5 * colour1) / 7.0f);
                output[7] = (byte)((1 * colour0 + 6 * colour1) / 7.0f);
            }
            else
            {
                output[2] = (byte)((4 * colour0 + 1 * colour1) / 5.0f);
                output[3] = (byte)((3 * colour0 + 2 * colour1) / 5.0f);
                output[4] = (byte)((2 * colour0 + 3 * colour1) / 5.0f);
                output[5] = (byte)((1 * colour0 + 4 * colour1) / 5.0f);
                output[6] = 0;
                output[7] = 255;
            }

            return output;
        }

        private static uint[] getIndices(byte b1, byte b2, byte b3, byte b4, byte b5, byte b6)
        {
            uint[] indices = new uint[16];

            indices[0] = makeIndexFrom3Bits(getBit(b3, 3), getBit(b3, 2), getBit(b3, 1));
            indices[1] = makeIndexFrom3Bits(getBit(b3, 6), getBit(b3, 5), getBit(b3, 4));
            indices[2] = makeIndexFrom3Bits(getBit(b2, 1), getBit(b3, 8), getBit(b3, 7));
            indices[3] = makeIndexFrom3Bits(getBit(b2, 4), getBit(b2, 3), getBit(b2, 2));
            indices[4] = makeIndexFrom3Bits(getBit(b2, 7), getBit(b2, 6), getBit(b2, 5));
            indices[5] = makeIndexFrom3Bits(getBit(b1, 2), getBit(b1, 1), getBit(b2, 8));
            indices[6] = makeIndexFrom3Bits(getBit(b1, 5), getBit(b1, 4), getBit(b1, 3));
            indices[7] = makeIndexFrom3Bits(getBit(b1, 8), getBit(b1, 7), getBit(b1, 6));

            indices[8] = makeIndexFrom3Bits(getBit(b6, 3), getBit(b6, 2), getBit(b6, 1));
            indices[9] = makeIndexFrom3Bits(getBit(b6, 6), getBit(b6, 5), getBit(b6, 4));
            indices[10] = makeIndexFrom3Bits(getBit(b5, 1), getBit(b6, 8), getBit(b6, 7));
            indices[11] = makeIndexFrom3Bits(getBit(b5, 4), getBit(b5, 3), getBit(b5, 2));
            indices[12] = makeIndexFrom3Bits(getBit(b5, 7), getBit(b5, 6), getBit(b5, 5));
            indices[13] = makeIndexFrom3Bits(getBit(b4, 2), getBit(b4, 1), getBit(b5, 8));
            indices[14] = makeIndexFrom3Bits(getBit(b4, 5), getBit(b4, 4), getBit(b4, 3));
            indices[15] = makeIndexFrom3Bits(getBit(b4, 8), getBit(b4, 7), getBit(b4, 6));

            return indices;
        }

        private static uint makeIndexFrom3Bits(bool mostSigBit, bool midBit, bool leastSigBit)
        {
            return makeIndexFrom3Bits(mostSigBit, midBit, leastSigBit, false);
        }

        private static uint makeIndexFrom3Bits(bool mostSigBit, bool midBit, bool leastSigBit, bool flip)
        {
            if (flip) { return (uint)((leastSigBit ? 1 << 2 : 0) + (midBit ? 1 << 1 : 0) + (mostSigBit ? 1 : 0)); }

            return (uint)((mostSigBit ? 1 << 2 : 0) + (midBit ? 1 << 1 : 0) + (leastSigBit ? 1 : 0));
        }

        private static bool getBit(byte b, int n)
        {
            return (b & (1 << n - 1)) != 0;
        }
    }
}
