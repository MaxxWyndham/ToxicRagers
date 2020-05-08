using System;
using System.Collections.Generic;

namespace ToxicRagers.Helpers
{
    public class BoundingBox
    {
        public Vector3 Min { get; set; } = Vector3.Zero;
        public Vector3 Max { get; set; } = Vector3.Zero;

        public BoundingBox() { }
        public BoundingBox(Vector3 min, Vector3 max)
        {
            Min = min;
            Max = max;
        }

        public static BoundingBox FromPoints(List<Vector3> points)
        {
            BoundingBox box = new BoundingBox
            {
                Min = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue),
                Max = new Vector3(float.MinValue, float.MinValue, float.MinValue)
            };

            foreach (Vector3 point in points)
            {
                if (box.Min.X > point.X) { box.Min.X = point.X; }
                if (box.Min.Y > point.Y) { box.Min.Y = point.Y; }
                if (box.Min.Z > point.Z) { box.Min.Z = point.Z; }
                if (box.Max.X < point.X) { box.Max.X = point.X; }
                if (box.Max.Y < point.Y) { box.Max.Y = point.Y; }
                if (box.Max.Z < point.Z) { box.Max.Z = point.Z; }
            }

            return box;
        }

        public bool IntersectsBox(BoundingBox box)
        {
            return
                (Min.X < box.Max.X) && (Max.X > box.Min.X) &&
                (Min.Y < box.Max.Y) && (Max.Y > box.Min.Y) &&
                (Min.Z < box.Max.Z) && (Max.Z > box.Min.Z);
        }

        public bool IntersectsFace(List<Vector3> points)
        {
            if (IntersectsBox(FromPoints(points)))
            {
                Plane plane = Plane.FromPoints(points[0], points[1], points[2]);
            }

            return false;
            //bool inside = false;

            //for (int n = 0; n < 3; n++)
            //{
            //    Vector3 d = points[n];

            //    inside |= ((Min.X < d.X && d.X < Max.X) &&
            //               (Min.Y < d.Y && d.Y < Max.Y) &&
            //               (Min.Z < d.Z && d.Z < Max.Z));
            //}

            //return inside;

            //// Classify each face vertex w.r.t. the planes of the box:
            //// 0 = inside, 1 = outside

            //int c0 = (points[0].X < Min.X ? 1 : 0) << 0 | (points[0].X > Max.X ? 1 : 0) << 3;
            //int c1 = (points[1].X < Min.X ? 1 : 0) << 0 | (points[1].X > Max.X ? 1 : 0) << 3;
            //int c2 = (points[2].X < Min.X ? 1 : 0) << 0 | (points[2].X > Max.X ? 1 : 0) << 3;

            //if ((c0 & c1 & c2) > 0)
            //{
            //    return false; // All vertices are outside one side of the box.
            //}

            //c0 |= (points[0].Z < Min.Z ? 1 : 0) << 2 | (points[0].Z > Max.Z ? 1 : 0) << 5;
            //c1 |= (points[1].Z < Min.Z ? 1 : 0) << 2 | (points[1].Z > Max.Z ? 1 : 0) << 5;
            //c2 |= (points[2].Z < Min.Z ? 1 : 0) << 2 | (points[2].Z > Max.Z ? 1 : 0) << 5;

            //if ((c0 & c1 & c2) > 0)
            //{
            //    return false; // All vertices are outside one side of the box.
            //}

            //c0 |= (points[0].Y < Min.Y ? 1 : 0) << 1 | (points[0].Y > Max.Y ? 1 : 0) << 4;
            //c1 |= (points[1].Y < Min.Y ? 1 : 0) << 1 | (points[1].Y > Max.Y ? 1 : 0) << 4;
            //c2 |= (points[2].Y < Min.Y ? 1 : 0) << 1 | (points[2].Y > Max.Y ? 1 : 0) << 4;

            //if ((c0 & c1 & c2) > 0)
            //{
            //    return false; // All vertices are outside one side of the box.
            //}

            //// See if any of the face edges intersect the box.
            //if (
            //    ((c0 & c1) == 0 && IntersectsEdge(points[0], points[1])) ||
            //    ((c1 & c2) == 0 && IntersectsEdge(points[1], points[2])) ||
            //    ((c2 & c0) == 0 && IntersectsEdge(points[2], points[0])))
            //{
            //    return true;
            //}

            //// See if any of the box edges intersect the face.
            //int c = c0 | c1 | c2;
            //Plane plane = Plane.FromPoints(points[0], points[1], points[2]);

            //// get min and max of box wrt plane
            //// if line between min and max does not intersect face
            //// then face is outside box
            //// else face intersects box
            //float dist_min, dist_max;
            //float e;
            //float factor;
            //int i;
            //Vector3 point_max = Vector3.Zero, point_min = Vector3.Zero;
            //int axis_0, axis_1;
            //float abs_plane_0, abs_plane_1;
            //float vx, vy, v1x, v1y, v2x, v2y;
            //float u, v, det;

            //dist_min = plane.SignedDistToPoint(Min);
            //dist_max = dist_min;

            //for (i = 0; i < 3; i++)
            //{
            //    e = plane.Normal[i] * (Max[i] - Min[i]);

            //    point_max[i] = Min[i];
            //    point_min[i] = Min[i];
            //    if (plane.Normal[i] > 0)
            //    {
            //        dist_max += e;
            //        point_max[i] = Max[i];
            //    }
            //    else
            //    {
            //        dist_min += e;
            //        point_min[i] = Max[i];
            //    }
            //}

            //if (dist_min * -dist_max < 0.0f) { return false; }

            //factor = dist_max / (dist_max - dist_min);

            //// Work out dominant axis of normal
            //axis_0 = 1;
            //axis_1 = 2;
            //abs_plane_0 = Math.Abs(plane.Normal[0]);
            //abs_plane_1 = Math.Abs(plane.Normal[1]);

            //if (abs_plane_1 > abs_plane_0)
            //{
            //    axis_0 = 0;
            //    abs_plane_0 = abs_plane_1;
            //}

            //if (Math.Abs(plane.Normal[2]) > abs_plane_0)
            //{
            //    // axis_m = 2;
            //    axis_1 = axis_0 ^ 1;
            //}

            //vx = point_max[axis_0] + factor * (point_min[axis_0] - point_max[axis_0]);
            //vx = vx - points[0][axis_0];
            //vy = point_max[axis_1] + factor * (point_min[axis_1] - point_max[axis_1]);
            //vy = vy - points[0][axis_1];

            //v1x = points[1][axis_0] - points[0][axis_0];
            //v1y = points[1][axis_1] - points[0][axis_1];
            //v2x = points[2][axis_0] - points[0][axis_0];
            //v2y = points[2][axis_1] - points[0][axis_1];

            //det = v2x * v1y - v2y * v1x;

            //if (det < 0)
            //{
            //    vx = -vx;
            //    vy = -vy;
            //}

            //u = (vy * v2x - vx * v2y);
            //if (u < 0) { return false; }
            //v = (vx * v1y - vy * v1x);
            //if (v < 0) { return false; }

            //if (det < 0) { det = -det; }

            //if (u + v <= det) { return true; }

            //// If we get here, the face does not intersect the box.
            //return false;
        }

        public bool IntersectsEdge(Vector3 p0, Vector3 p1)
        {
            // we know both points are outside box.
            // and that there is no plane that both points are outside
            float min_den, min_num;
            float max_den, max_num;
            float den, num, num1, num2;
            int i;

            den = p1.X - p0.X;

            min_num = Max.X - p0.X;
            max_num = Min.X - p0.X;

            if (den < 0)
            {
                num = min_num;
                min_num = -max_num;
                max_num = -num;
                den = -den;
            }

            min_den = den;
            max_den = den;

            for (i = 1; i != 3; i++)
            {
                den = p1[i] - p0[i];
                num1 = Max[i] - p0[i];
                num2 = Min[i] - p0[i];

                if (den < 0)
                {
                    den = -den;
                    num = num1;
                    num1 = -num2;
                    num2 = -num;
                }

                if (num1 * min_den < min_num * den)
                {
                    min_num = num1;
                    min_den = den;
                }

                if (num2 * max_den > max_num * den)
                {
                    max_num = num2;
                    max_den = den;
                }
            }

            if (min_den * max_num > max_den * min_num)
            {
                return false;
            }

            return true;
        }
    }
}
