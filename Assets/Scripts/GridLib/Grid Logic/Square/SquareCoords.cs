using GridLib.Pathing;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

namespace GridLib.Square
{
    public enum SquareFacing
    {
        n = 0,
        e = 1,
        s = 2,
        w = 3,

        bad = -1,
    }

    public static class SquareFacingMethods
    {
        public static SquareFacing random { get { return SquareFacing.n.Rotate(Random.Range(0, 3)); } }

        #region Rotation

        public static SquareFacing Apply(this SquareFacing facing, SquareRotation rot)
        {
            if (facing == SquareFacing.bad) return SquareFacing.bad;
            else
            {
                return (SquareFacing)NumUtil.WrapPos((int)facing - rot.CWcount, 6);
            }
        }

        public static SquareFacing Rotate(this SquareFacing facing, int CWcount)
        {
            return facing.Apply(new SquareRotation(CWcount));
        }

        #endregion

        #region Compass turns

        public static IEnumerable<SquareFacing> CompassTurnCW(this SquareFacing facing, SquareFacing final)
        {
            SquareFacing current = facing;

            while (current != final)
            {
                yield return current;
                current = current.Apply(SquareRotation.CW);
            }

            yield return final;
        }

        public static IEnumerable<SquareFacing> CompassTurnCW(this SquareFacing facing, SquareRotation rot)
        {
            return facing.CompassTurnCW(facing.Apply(rot));
        }

        public static IEnumerable<SquareFacing> CompassTurnCW(this SquareFacing facing)
        {
            return facing.CompassTurnCW(SquareRotation.CCW);
        }

        public static IEnumerable<SquareFacing> CompassTurnCCW(this SquareFacing facing, SquareFacing final)
        {
            SquareFacing current = facing;

            while (current != final)
            {
                yield return current;
                current = current.Apply(SquareRotation.CCW);
            }

            yield return final;
        }

        public static IEnumerable<SquareFacing> CompassTurnCCW(this SquareFacing facing, SquareRotation rot)
        {
            return facing.CompassTurnCCW(facing.Apply(rot));
        }

        public static IEnumerable<SquareFacing> CompassTurnCCW(this SquareFacing facing)
        {
            return facing.CompassTurnCCW(SquareRotation.CCW);
        }

        #endregion

        #region Spatial relationships

        public static SquareCoords ToOffset(this SquareFacing facing)
        {
            switch(facing)
            {
                case SquareFacing.n: return SquareCoords.N;
                case SquareFacing.e: return SquareCoords.E;
                case SquareFacing.s: return SquareCoords.S;
                case SquareFacing.w: return SquareCoords.W;
                default: return SquareCoords.O;
            }
        }

        #endregion
    }

    public class Extremes
    {
        public int minX = 0;
        public int maxX = 0;
        public int minY = 0;
        public int maxY = 0;
    }

    public static class IEnumerableExt
    {
        public static IEnumerable<SquareCoords> Frontier(this IEnumerable<SquareCoords> region)
        {
            if (region.Any())
            {
                HashSet<SquareCoords> done = new HashSet<SquareCoords>(region);

                foreach (SquareCoords target in region)
                {
                    foreach (SquareCoords neighbor in target.neighbors)
                    {
                        if (!done.Contains(neighbor))
                        {
                            yield return neighbor;
                            done.Add(neighbor);
                        }
                    }
                }
            }
            else
            {
                yield return SquareCoords.O;
            }
        }

        public static Extremes FindExtremes(this IEnumerable<SquareCoords> region)
        {
            Extremes result = new Extremes();

            foreach (SquareCoords loc in region)
            {
                if (loc.x < result.minX) result.minX = loc.x;
                if (loc.x > result.maxX) result.maxX = loc.x;
                if (loc.y < result.minY) result.minY = loc.y;
                if (loc.y > result.maxY) result.maxY = loc.y;
            }

            return result;
        }
    }
    
    [Serializable]
    public class SquareCoords : ICoords<SquareCoords>
    {
        #region Internal state

        public int x, y;

        public SquareCoords(int x, int y)
        {
            this.x = x;
            this.y = y;
        }

        public override string ToString()
        {
            return "(" + x + ", " + y + ")";
        }

        #endregion

        #region Operators

        public static SquareCoords operator +(SquareCoords left, SquareCoords right)
        {
            return new SquareCoords(
                left.x + right.x,
                left.y + right.y);
        }

        public static SquareCoords operator -(SquareCoords left, SquareCoords right)
        {
            return new SquareCoords(
                left.x - right.x,
                left.y - right.y);
        }

        public static SquareCoords operator *(SquareCoords left, int right)
        {
            return new SquareCoords(
                left.x * right,
                left.y * right);
        }

        public static SquareCoords operator +(SquareCoords left, SquareFacing right)
        {
            return left + right.ToOffset();
        }

        public bool Equals(SquareCoords other)
        {
            if (other == null) return false;
            else return (x == other.x) && (y == other.y);
        }

        public override bool Equals(object obj)
        {
            if (obj == null || !(obj is SquareCoords)) return false;
            else
            {
                return Equals((SquareCoords)obj);
            }
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 17;
                hash *= (23 + x.GetHashCode());
                hash *= (23 + y.GetHashCode());
                return hash;
            }
        }

        public static bool operator ==(SquareCoords left, SquareCoords right)
        {
            if ((object)left == null) return (object)right == null;
            else return left.Equals(right);
        }

        public static bool operator !=(SquareCoords left, SquareCoords right)
        {
            return !(left == right);
        }

        #endregion

        #region Static definitions

        public static readonly SquareCoords O  = new SquareCoords( 0,  0);

        public static readonly SquareCoords N = new SquareCoords( 0,  1);
        public static readonly SquareCoords E = new SquareCoords( 1,  0);
        public static readonly SquareCoords S = new SquareCoords( 0, -1);
        public static readonly SquareCoords W = new SquareCoords(-1,  0);

        #endregion

        #region Measurements

        public bool Adjacent(SquareCoords other)
        {
            return DistanceTo(other) == 1;
        }

        public int DistanceTo(ICoords<SquareCoords> other)
        {
            return DistanceTo((SquareCoords)other);
        }

        public int DistanceTo(SquareCoords other)
        {
            SquareCoords d = other - this;

            return Math.Abs(d.x) + Math.Abs(d.y);
        }
        
        public static int Distance(SquareCoords left, SquareCoords right)
        {
            return left.DistanceTo(right);
        }

        public SquareFacing FacingTo(SquareCoords other)
        {
            if (DistanceTo(other) == 0) return SquareFacing.bad;
            else return (LerpTo(other, 1.0f / DistanceTo(other)) - this).ToFacing();
        }

        public static SquareCoords Round(float x, float y)
        {
            int rx = (int)Math.Round(x);
            int ry = (int)Math.Round(y);

            return new SquareCoords(rx, ry);
        }

        public static SquareCoords Round(Vector3 orig)
        {
            return Round(orig.x, orig.y);
        }

        public static SquareCoords Round(Vector2 orig)
        {
            return Round(orig.x, orig.y);
        }

        public SquareFacing ToFacing()
        {
            if (this == N) return SquareFacing.n;
            if (this == E) return SquareFacing.e;
            if (this == S) return SquareFacing.s;
            if (this == W) return SquareFacing.w;

            Debug.Log("Tried to convert " + this + " to a facing!");
            return SquareFacing.bad;
        }

        #endregion

        #region One-to-one relationships

        public SquareCoords Rotate(SquareCoords center, SquareRotation rot)
        {
            SquareCoords d = this - center;

            switch (rot.CWcount)
            {
                default:
                case 0: return center + d; //             x,  y
                case 1: return center + new SquareCoords( y, -x);
                case 2: return center + new SquareCoords(-x, -y);
                case 3: return center + new SquareCoords(-y,  x);
            }
        }

        public SquareCoords Rotate(SquareRotation rot)
        {
            return Rotate(SquareCoords.O, rot);
        }

        public SquareCoords LerpTo(SquareCoords other, float t)
        {
            const float eX =  1E-6F;
            const float eY =  2E-6F;

            return Round(
                NumUtil.Lerp(x + eX, other.x + eX, t),
                NumUtil.Lerp(y + eY, other.y + eY, t));
        }

        public static SquareCoords Lerp(SquareCoords left, SquareCoords right, float t)
        {
            return left.LerpTo(right, t);
        }
        
        #endregion

        #region One-to-many relationships

        public IEnumerable<SquareCoords> neighbors { get { return Ring(1); } }

        public IEnumerable<SquareCoords> Ring(uint radius)
        {
            if (radius == 0)
            {
                yield return this;
            }
            else
            {
                for(int x = -(int)radius; x <= radius; x++)
                {
                    int y = (int)radius - Math.Abs(x);
                    if (y == 0) yield return this + new SquareCoords(x, y);
                    else
                    {
                        yield return this + new SquareCoords(x, y);
                        yield return this + new SquareCoords(x, -y);
                    }
                }
            }
        }

        public IEnumerable<SquareCoords> CompoundRing(uint minRadius, uint maxRadius)
        {
            for (uint radius = minRadius; radius <= maxRadius; radius++)
            {
                foreach (SquareCoords loc in Ring(radius))
                    yield return loc;
            }
        }

        public IEnumerable<SquareCoords> LineTo(SquareCoords other)
        {
            int dist = DistanceTo(other);

            if (dist == 0)
                yield return other;
            else
            {
                SquareCoords d = other - this;
                SquareCoords n = new SquareCoords(Math.Abs(d.x), Math.Abs(d.y));
                SquareCoords dx = new SquareCoords((d.x > 0) ? 1 : -1, 0);
                SquareCoords dy = new SquareCoords(0, (d.y > 0) ? 1 : -1);

                SquareCoords cursor = new SquareCoords(x, y);
                for(int ix = 0, iy = 0; ix < n.x || iy < n.y;)
                {
                    yield return cursor;

                    float xScore = (0.5f + ix) / n.x;
                    float yScore = (0.5f + iy) / n.y;

                    if (xScore <= yScore)
                    {
                        cursor += dx;
                        ix++;
                    }
                    else
                    {
                        cursor += dy;
                        iy++;
                    }
                }

                yield return other;
            }
        }

        public static IEnumerable<SquareCoords> Line(SquareCoords left, SquareCoords right)
        {
            return left.LineTo(right);
        }

        #endregion
    }
}
