using System;

namespace ToxicRagers.Helpers
{
    public class ColourFit
    {
        protected ColourSet m_colours;
        protected SquishFlags m_flags;

        public ColourFit(ColourSet colours, SquishFlags flags)
        {
            m_colours = colours;
            m_flags = flags;
        }

        public void Compress(ref byte[] block, int offset)
        {
            bool isDxt1 = ((m_flags & SquishFlags.kDxt1) != 0);

            if (isDxt1)
            {
                Compress3(ref block, offset);
                if (!m_colours.IsTransparent) {
                    Compress4(ref block, offset);
                }
            }
            else
            {
                Compress4(ref block, offset);
            }
        }

        public virtual void Compress3(ref byte[] block, int offset) { }
        public virtual void Compress4(ref byte[] block, int offset) { }
    }
}
