using System;
using System.Collections.Generic;

namespace ToxicRagers.Helpers
{
    public class Palette : List<Colour>
    {
        public int GetNearestPixelIndex(Colour c)
        {
            float smallestDiff = float.MaxValue;
            int index = -1;

            for (int i = 0; i < Count; i++)
            {
                Colour p = this[i];

                //float hdiff = Math.Min(Math.Abs(p.H - c.H), Math.Abs(p.H - c.H + (p.H < c.H ? -360f : 360f))) * 1.2f;
                //float sdiff = (p.S - c.S) * 1.5f;
                //float ldiff = p.L - c.L;
                //if (p.L < c.L) { ldiff *= 2f; }

                //hdiff *= hdiff;
                //sdiff *= sdiff;
                //ldiff *= ldiff;

                //float currentDiff = hdiff + sdiff + ldiff;

                float currentDiff = (float)(Math.Pow((c.R - p.R) * 0.299f, 2) + Math.Pow((c.G - p.G) * 0.587f, 2) + Math.Pow((c.B - p.B) * 0.114f, 2));

                if (currentDiff < smallestDiff)
                {
                    smallestDiff = currentDiff;
                    index = i;
                }
            }

            return index;
        }
    }
}
