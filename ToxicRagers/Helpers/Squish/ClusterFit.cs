using System;

namespace ToxicRagers.Helpers
{
    public class ClusterFit : ColourFit
    {
        int m_iterationCount;
        Vector3 m_principle;
        byte[] m_order = new byte[16 * 8];
        Vector4[] m_points_weights = new Vector4[16];
        Vector4 m_xsum_wsum;
        Vector4 m_metric;
        Vector4 m_besterror;

        public ClusterFit(ColourSet colours, SquishFlags flags, float? metric)
            : base(colours, flags)
        {
            // set the iteration count
            m_iterationCount = ((m_flags & SquishFlags.kColourIterativeClusterFit) != 0 ? 8 : 1);

            // initialise the metric (old perceptual = 0.2126f, 0.7152f, 0.0722f)
            if (metric != null)
            {
                //m_metric = Vec4( metric[0], metric[1], metric[2], 1.0f );
            }
            else
            {
                m_metric = new Vector4(1.0f);
            }

            // initialise the best error
            m_besterror = new Vector4(float.MaxValue);

            // cache some values
            int count = m_colours.Count;
            Vector3[] values = m_colours.Points;

            // get the covariance matrix
            Sym3x3 covariance = Sym3x3.ComputeWeightedCovariance(count, values, m_colours.Weights);

            // compute the principle component
            m_principle = Sym3x3.ComputePrincipleComponent(covariance);
        }

        public bool ConstructOrdering(Vector3 axis, int iteration)
        {
            // cache some values
            int count = m_colours.Count;
            Vector3[] values = m_colours.Points;

            // build the list of dot products
            float[] dps = new float[16];

            for (int i = 0; i < count; ++i)
            {
                dps[i] = Vector3.Dot(values[i], axis);
                m_order[(16 * iteration) + i] = (byte)i;
            }

            // stable sort using them
            for (int i = 0; i < count; ++i)
            {
                for (int j = i; j > 0 && dps[j] < dps[j - 1]; --j)
                {
                    float tf = dps[j];
                    dps[j] = dps[j - 1];
                    dps[j - 1] = tf;

                    byte tb = m_order[(16 * iteration) + j];
                    m_order[(16 * iteration) + j] = m_order[(16 * iteration) + j - 1];
                    m_order[(16 * iteration) + j - 1] = tb;
                }
            }

            // check this ordering is unique
            for (int it = 0; it < iteration; ++it)
            {
                bool same = true;
                for (int i = 0; i < count; ++i)
                {
                    if (m_order[(16 * iteration) + i] != m_order[(16 * it) + i])
                    {
                        same = false;
                        break;
                    }
                }
                if (same)
                    return false;
            }

            // copy the ordering and weight all the points
            Vector3[] unweighted = m_colours.Points;
            float[] weights = m_colours.Weights;
            m_xsum_wsum = new Vector4(0.0f);
            for (int i = 0; i < count; ++i)
            {
                int j = m_order[(16 * iteration) + i];
                Vector4 p = new Vector4(unweighted[j].X, unweighted[j].Y, unweighted[j].Z, 1.0f);
                Vector4 w = new Vector4(weights[j]);
                Vector4 x = p * w;
                m_points_weights[i] = x;
                m_xsum_wsum += x;
            }
            return true;
        }

        public override void Compress3(ref byte[] block, int offset)
        {
            // declare variables
            int count = m_colours.Count;
            Vector4 two = new Vector4(2.0f);
            Vector4 one = new Vector4(1.0f);
            Vector4 half_half2 = new Vector4(0.5f, 0.5f, 0.5f, 0.25f);
            Vector4 zero = new Vector4(0.0f);
            Vector4 half = new Vector4(0.5f);
            Vector4 grid = new Vector4(31.0f, 63.0f, 31.0f, 0.0f);
            Vector4 gridrcp = new Vector4(1.0f / 31.0f, 1.0f / 63.0f, 1.0f / 31.0f, 0.0f);

            // prepare an ordering using the principle axis
            ConstructOrdering(m_principle, 0);

            // check all possible clusters and iterate on the total order
            Vector4 beststart = new Vector4(0.0f);
            Vector4 bestend = new Vector4(0.0f);
            Vector4 besterror = m_besterror;
            byte[] bestindices = new byte[16];
            int bestiteration = 0;
            int besti = 0, bestj = 0;

            // loop over iterations (we avoid the case that all points in first or last cluster)
            for (int iterationIndex = 0; ; )
            {
                // first cluster [0,i) is at the start
                Vector4 part0 = new Vector4(0.0f);
                for (int i = 0; i < count; ++i)
                {
                    // second cluster [i,j) is half along
                    Vector4 part1 = (i == 0) ? m_points_weights[0] : new Vector4(0.0f);
                    int jmin = (i == 0) ? 1 : i;
                    for (int j = jmin; ; )
                    {
                        // last cluster [j,count) is at the end
                        Vector4 part2 = m_xsum_wsum - part1 - part0;

                        // compute least squares terms directly
                        Vector4 alphax_sum = Vector4.MultiplyAdd(part1, half_half2, part0);
                        Vector4 alpha2_sum = alphax_sum.SplatW();

                        Vector4 betax_sum = Vector4.MultiplyAdd(part1, half_half2, part2);
                        Vector4 beta2_sum = betax_sum.SplatW();

                        Vector4 alphabeta_sum = (part1 * half_half2).SplatW();

                        // compute the least-squares optimal points
                        Vector4 factor = Vector4.Reciprocal(Vector4.NegativeMultiplySubtract(alphabeta_sum, alphabeta_sum, alpha2_sum * beta2_sum));
                        Vector4 a = Vector4.NegativeMultiplySubtract(betax_sum, alphabeta_sum, alphax_sum * beta2_sum) * factor;
                        Vector4 b = Vector4.NegativeMultiplySubtract(alphax_sum, alphabeta_sum, betax_sum * alpha2_sum) * factor;

                        // clamp to the grid
                        a = Vector4.Min(one, Vector4.Max(zero, a));
                        b = Vector4.Min(one, Vector4.Max(zero, b));
                        a = Vector4.Truncate(Vector4.MultiplyAdd(grid, a, half)) * gridrcp;
                        b = Vector4.Truncate(Vector4.MultiplyAdd(grid, b, half)) * gridrcp;

                        // compute the error (we skip the constant xxsum)
                        Vector4 e1 = Vector4.MultiplyAdd(a * a, alpha2_sum, b * b * beta2_sum);
                        Vector4 e2 = Vector4.NegativeMultiplySubtract(a, alphax_sum, a * b * alphabeta_sum);
                        Vector4 e3 = Vector4.NegativeMultiplySubtract(b, betax_sum, e2);
                        Vector4 e4 = Vector4.MultiplyAdd(two, e3, e1);

                        // apply the metric to the error term
                        Vector4 e5 = e4 * m_metric;
                        Vector4 error = e5.SplatX() + e5.SplatY() + e5.SplatZ();

                        // keep the solution if it wins
                        if (Vector4.CompareAnyLessThan(error, besterror))
                        {
                            beststart = a;
                            bestend = b;
                            besti = i;
                            bestj = j;
                            besterror = error;
                            bestiteration = iterationIndex;
                        }

                        // advance
                        if (j == count)
                            break;
                        part1 += m_points_weights[j];
                        ++j;
                    }

                    // advance
                    part0 += m_points_weights[i];
                }

                // stop if we didn't improve in this iteration
                if (bestiteration != iterationIndex)
                    break;

                // advance if possible
                ++iterationIndex;
                if (iterationIndex == m_iterationCount)
                    break;

                // stop if a new iteration is an ordering that has already been tried
                Vector3 axis = (bestend - beststart).ToVector3();
                if (!ConstructOrdering(axis, iterationIndex))
                    break;
            }

            // save the block if necessary
            if (Vector4.CompareAnyLessThan(besterror, m_besterror))
            {
                byte[] unordered = new byte[16];
                for (int m = 0; m < besti; ++m)
                    unordered[m_order[(16 * bestiteration) + m]] = 0;
                for (int m = besti; m < bestj; ++m)
                    unordered[m_order[(16 * bestiteration) + m]] = 2;
                for (int m = bestj; m < count; ++m)
                    unordered[m_order[(16 * bestiteration) + m]] = 1;

                m_colours.RemapIndices(unordered, bestindices);

                // save the block
                ColourBlock.WriteColourBlock3(beststart.ToVector3(), bestend.ToVector3(), bestindices, ref block, offset);

                // save the error
                m_besterror = besterror;
            }
        }

        public override void Compress4(ref byte[] block, int offset)
        {
            // declare variables
            int count = m_colours.Count;
            Vector4 two = new Vector4(2.0f);
            Vector4 one = new Vector4(1.0f);
            Vector4 onethird_onethird2 = new Vector4(1.0f / 3.0f, 1.0f / 3.0f, 1.0f / 3.0f, 1.0f / 9.0f);
            Vector4 twothirds_twothirds2 = new Vector4(2.0f / 3.0f, 2.0f / 3.0f, 2.0f / 3.0f, 4.0f / 9.0f);
            Vector4 twonineths = new Vector4(2.0f / 9.0f);
            Vector4 zero = new Vector4(0.0f);
            Vector4 half = new Vector4(0.5f);
            Vector4 grid = new Vector4(31.0f, 63.0f, 31.0f, 0.0f);
            Vector4 gridrcp = new Vector4(1.0f / 31.0f, 1.0f / 63.0f, 1.0f / 31.0f, 0.0f);

            // prepare an ordering using the principle axis
            ConstructOrdering(m_principle, 0);

            // check all possible clusters and iterate on the total order
            Vector4 beststart = new Vector4(0.0f);
            Vector4 bestend = new Vector4(0.0f);
            Vector4 besterror = m_besterror;
            byte[] bestindices = new byte[16];
            int bestiteration = 0;
            int besti = 0, bestj = 0, bestk = 0;

            // loop over iterations (we avoid the case that all points in first or last cluster)
            for (int iterationIndex = 0; ; )
            {
                // first cluster [0,i) is at the start
                Vector4 part0 = new Vector4(0.0f);
                for (int i = 0; i < count; ++i)
                {
                    // second cluster [i,j) is one third along
                    Vector4 part1 = new Vector4(0.0f);
                    for (int j = i; ; )
                    {
                        // third cluster [j,k) is two thirds along
                        Vector4 part2 = (j == 0) ? m_points_weights[0] : new Vector4(0.0f);
                        int kmin = (j == 0) ? 1 : j;
                        for (int k = kmin; ; )
                        {
                            // last cluster [k,count) is at the end
                            Vector4 part3 = m_xsum_wsum - part2 - part1 - part0;

                            // compute least squares terms directly
                            Vector4 alphax_sum = Vector4.MultiplyAdd(part2, onethird_onethird2, Vector4.MultiplyAdd(part1, twothirds_twothirds2, part0));
                            Vector4 alpha2_sum = alphax_sum.SplatW();

                            Vector4 betax_sum = Vector4.MultiplyAdd(part1, onethird_onethird2, Vector4.MultiplyAdd(part2, twothirds_twothirds2, part3));
                            Vector4 beta2_sum = betax_sum.SplatW();

                            Vector4 alphabeta_sum = twonineths * (part1 + part2).SplatW();

                            // compute the least-squares optimal points
                            Vector4 factor = Vector4.Reciprocal(Vector4.NegativeMultiplySubtract(alphabeta_sum, alphabeta_sum, alpha2_sum * beta2_sum));
                            Vector4 a = Vector4.NegativeMultiplySubtract(betax_sum, alphabeta_sum, alphax_sum * beta2_sum) * factor;
                            Vector4 b = Vector4.NegativeMultiplySubtract(alphax_sum, alphabeta_sum, betax_sum * alpha2_sum) * factor;

                            // clamp to the grid
                            a = Vector4.Min(one, Vector4.Max(zero, a));
                            b = Vector4.Min(one, Vector4.Max(zero, b));
                            a = Vector4.Truncate(Vector4.MultiplyAdd(grid, a, half)) * gridrcp;
                            b = Vector4.Truncate(Vector4.MultiplyAdd(grid, b, half)) * gridrcp;

                            // compute the error (we skip the constant xxsum)
                            Vector4 e1 = Vector4.MultiplyAdd(a * a, alpha2_sum, b * b * beta2_sum);
                            Vector4 e2 = Vector4.NegativeMultiplySubtract(a, alphax_sum, a * b * alphabeta_sum);
                            Vector4 e3 = Vector4.NegativeMultiplySubtract(b, betax_sum, e2);
                            Vector4 e4 = Vector4.MultiplyAdd(two, e3, e1);

                            // apply the metric to the error term
                            Vector4 e5 = e4 * m_metric;
                            Vector4 error = e5.SplatX() + e5.SplatY() + e5.SplatZ();

                            // keep the solution if it wins
                            if (Vector4.CompareAnyLessThan(error, besterror))
                            {
                                beststart = a;
                                bestend = b;
                                besterror = error;
                                besti = i;
                                bestj = j;
                                bestk = k;
                                bestiteration = iterationIndex;
                            }

                            // advance
                            if (k == count)
                                break;
                            part2 += m_points_weights[k];
                            ++k;
                        }

                        // advance
                        if (j == count)
                            break;
                        part1 += m_points_weights[j];
                        ++j;
                    }

                    // advance
                    part0 += m_points_weights[i];
                }

                // stop if we didn't improve in this iteration
                if (bestiteration != iterationIndex)
                    break;

                // advance if possible
                ++iterationIndex;
                if (iterationIndex == m_iterationCount)
                    break;

                // stop if a new iteration is an ordering that has already been tried
                Vector3 axis = (bestend - beststart).ToVector3();
                if (!ConstructOrdering(axis, iterationIndex))
                    break;
            }

            // save the block if necessary
            if (Vector4.CompareAnyLessThan(besterror, m_besterror))
            {
                // remap the indices
                byte[] unordered = new byte[16];
                for (int m = 0; m < besti; ++m)
                    unordered[m_order[(16 * bestiteration) + m]] = 0;
                for (int m = besti; m < bestj; ++m)
                    unordered[m_order[(16 * bestiteration) + m]] = 2;
                for (int m = bestj; m < bestk; ++m)
                    unordered[m_order[(16 * bestiteration) + m]] = 3;
                for (int m = bestk; m < count; ++m)
                    unordered[m_order[(16 * bestiteration) + m]] = 1;

                m_colours.RemapIndices(unordered, bestindices);

                // save the block
                ColourBlock.WriteColourBlock4(beststart.ToVector3(), bestend.ToVector3(), bestindices, ref block, offset);

                // save the error
                m_besterror = besterror;
            }
        }
    }
}
