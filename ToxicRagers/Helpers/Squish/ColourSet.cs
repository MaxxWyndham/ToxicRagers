using System;

namespace ToxicRagers.Helpers
{
    public class ColourSet
    {
        int m_count = 0;
        Vector3[] m_points = new Vector3[16];
        float[] m_weights = new float[16];
        int[] m_remap = new int[16];
        bool m_transparent = false;

        public int Count { get { return m_count; } }
        public bool IsTransparent { get { return m_transparent; } }
        public Vector3[] Points { get { return m_points; } }
        public float[] Weights { get { return m_weights; } }

        public ColourSet(byte[] rgba, int mask, SquishFlags flags)
        {
            // check the compression mode for dxt1
            bool isDxt1 = ((flags & SquishFlags.kDxt1) != 0);
            bool weightByAlpha = ((flags & SquishFlags.kWeightColourByAlpha) != 0);

            // create the minimal set
            for (int i = 0; i < 16; ++i)
            {
                // check this pixel is enabled
                int bit = 1 << i;
                if ((mask & bit) == 0)
                {
                    m_remap[i] = -1;
                    continue;
                }

                // check for transparent pixels when using dxt1
                if (isDxt1 && rgba[4 * i + 3] < 128)
                {
                    m_remap[i] = -1;
                    m_transparent = true;
                    continue;
                }

                // loop over previous points for a match
                for (int j = 0; ; ++j)
                {
                    // allocate a new point
                    if (j == i)
                    {
                        // normalise coordinates to [0,1]
                        float x = (float)rgba[4 * i] / 255.0f;
                        float y = (float)rgba[4 * i + 1] / 255.0f;
                        float z = (float)rgba[4 * i + 2] / 255.0f;

                        // ensure there is always non-zero weight even for zero alpha
                        float w = (float)(rgba[4 * i + 3] + 1) / 256.0f;

                        // add the point
                        m_points[m_count] = new Vector3(x, y, z);
                        m_weights[m_count] = (weightByAlpha ? w : 1.0f);
                        m_remap[i] = m_count;

                        // advance
                        ++m_count;
                        break;
                    }

                    // check for a match
                    int oldbit = 1 << j;
                    bool match = ((mask & oldbit) != 0)
                            && (rgba[4 * i] == rgba[4 * j])
                            && (rgba[4 * i + 1] == rgba[4 * j + 1])
                            && (rgba[4 * i + 2] == rgba[4 * j + 2])
                            && (rgba[4 * j + 3] >= 128 || !isDxt1);
                    if (match)
                    {
                        // get the index of the match
                        int index = m_remap[j];

                        // ensure there is always non-zero weight even for zero alpha
                        float w = (float)(rgba[4 * i + 3] + 1) / 256.0f;

                        // map to this point and increase the weight
                        m_weights[index] += (weightByAlpha ? w : 1.0f);
                        m_remap[i] = index;
                        break;
                    }
                }
            }

            // square root the weights
            for (int i = 0; i < m_count; ++i)
            {
                m_weights[i] = (float)Math.Sqrt(m_weights[i]);
            }
        }

        public void RemapIndices(byte[] source, byte[] target)
        {
            for (int i = 0; i < 16; ++i)
            {
                int j = m_remap[i];
                if (j == -1)
                {
                    target[i] = 3;
                }
                else
                {
                    target[i] = source[j];
                }
            }
        }
    }
}
