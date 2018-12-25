using UnityEngine;
using System;

namespace GridLib.Matrix
{
    public class Matrix2
    {
        float a, b, c, d;

        public Vector2 c0
        {
            get { return new Vector2(a, c); }
            set
            {
                a = value.x;
                c = value.y;
            }
        }
        public Vector2 c1
        {
            get{ return new Vector2(b, d); }
            set
            {
                b = value.x;
                d = value.y;
            }
        }

        public Vector2 this[int index]
        {
            get
            {
                switch(index)
                {
                    case 0: return c0;
                    case 1: return c1;
                    default: throw new ArgumentOutOfRangeException("Matrix2x2 indexed with " + index);
                }
            }
        }

        public float this[int x, int y]
        {
            get { return this[x][y]; }
        }

        public override string ToString()
        {
            return "(" + c0 + ", " + c1 + ")";
        }

        public Matrix2()
        {
            a = b = c = d = 0;
        }

        public Matrix2(Vector2 c0, Vector2 c1)
        {
            a = c0.x;
            b = c1.x;
            c = c0.y;
            d = c1.y;
        }

        public Matrix2(float a, float b, float c, float d)
        {
            this.a = a;
            this.b = b;
            this.c = c;
            this.d = d;
        }

        public float determinant { get { return (a * d) - (b * c); } }

        public Matrix2 inverse
        {
            get
            {
                Vector2 c0 = new Vector2(d, -c) / determinant;
                Vector2 c1 = new Vector2(-b, a) / determinant;
                return new Matrix2(c0, c1);
            }
        }

        public static Vector2 operator *(Matrix2 m, Vector2 v)
        {
            float x = (m.a * v.x) + (m.b * v.y);
            float y = (m.c * v.x) + (m.d * v.y);
            return new Vector2(x, y);
        }
    }

    public class Matrix3
    {
        float a, b, c, d, e, f, g, h, k;

        public Vector3 c0 {
            get { return new Vector3(a, d, g); }
            set
            {
                a = value.x;
                d = value.y;
                g = value.z;
            }
        }
        public Vector3 c1 {
            get { return new Vector3(b, e, h); }
            set
            {
                b = value.x;
                e = value.y;
                h = value.z;
            }
        }
        public Vector3 c2 {
            get { return new Vector3(c, f, k); }
            set
            {
                c = value.x;
                f = value.y;
                k = value.z;
            }
        }

        public Vector3 this[int index]
        {
            get
            {
                switch (index)
                {
                    case 0: return c0;
                    case 1: return c1;
                    case 2: return c2;
                    default: throw new ArgumentOutOfRangeException("Matrix3x3 indexed with " + index);
                }
            }
        }

        public float this[int x, int y]
        {
            get { return this[x][y]; }
        }

        public override string ToString()
        {
            return "(" + c0 + ", " + c1 + ", " + c2 + ")";
        }

        public Matrix3()
        {
            a = b = c = d = e = f = g = h = k = 0;
        }

        public Matrix3(Vector3 c0, Vector3 c1, Vector3 c2)
        {
            a = c0.x;
            b = c1.x;
            c = c2.x;
            d = c0.y;
            e = c1.y;
            f = c2.y;
            g = c0.z;
            h = c1.z;
            k = c2.z;
        }

        public Matrix3(Vector3 c0, Vector3 c1) : this(c0, c1, new Vector3(0, 0, 1))
        {

        }

        public Matrix3(
            float a, float b, float c, float d, float e, float f, float g, float h, float k)
        {
            this.a = a;
            this.b = b;
            this.c = c;
            this.d = d;
            this.e = e;
            this.f = f;
            this.g = g;
            this.h = h;
            this.k = k;
        }

        public float determinant
        {
            get
            {
                //float result = 0.0f;

                float result1 = a * new Matrix2(e, f, h, k).determinant;
                float result2 = b * new Matrix2(d, f, g, k).determinant;
                float result3 = c * new Matrix2(d, e, g, h).determinant;

                //return result;
                return result1 - result2 + result3;
            }
        }

        public Matrix3 transpose
        {
            get
            {
                Vector3 c0 = new Vector3(a, b, c);
                Vector3 c1 = new Vector3(d, e, f);
                Vector3 c2 = new Vector3(g, h, k);

                return new Matrix3(c0, c1, c2);
            }
        }

        public Matrix2 minorA { get { return new Matrix2(e, f, h, k); } }
        public Matrix2 minorB { get { return new Matrix2(d, f, g, k); } }
        public Matrix2 minorC { get { return new Matrix2(d, e, g, h); } }

        public Matrix2 minorD { get { return new Matrix2(b, c, h, k); } }
        public Matrix2 minorE { get { return new Matrix2(a, c, g, k); } }
        public Matrix2 minorF { get { return new Matrix2(a, b, g, h); } }

        public Matrix2 minorG { get { return new Matrix2(b, c, e, f); } }
        public Matrix2 minorH { get { return new Matrix2(a, c, d, f); } }
        public Matrix2 minorK { get { return new Matrix2(a, b, d, e); } }

        public Matrix3 adjoint
        {
            get
            {
                Matrix3 mt = transpose;

                float aa =  mt.minorA.determinant;
                float ab = -mt.minorB.determinant;
                float ac =  mt.minorC.determinant;

                float ad = -mt.minorD.determinant;
                float ae =  mt.minorE.determinant;
                float af = -mt.minorF.determinant;

                float ag =  mt.minorG.determinant;
                float ah = -mt.minorH.determinant;
                float ak =  mt.minorK.determinant;

                return new Matrix3(aa, ab, ac, ad, ae, af, ag, ah, ak);
            }
        }

        public Matrix3 inverse
        {
            get
            {
                return adjoint / determinant;
            }
        }

        public static Vector3 operator *(Matrix3 m, Vector3 v)
        {
            float x = (m.a * v.x) + (m.b * v.y) + (m.c * v.z);
            float y = (m.d * v.x) + (m.e * v.y) + (m.f * v.z);
            float z = (m.g * v.x) + (m.h * v.y) + (m.k * v.z);
            return new Vector3(x, y, z);
        }

        public static Matrix3 operator *(Matrix3 m, float f)
        {
            return new Matrix3(m.c0 * f, m.c1 * f, m.c2 * f);
        }

        public static Matrix3 operator /(Matrix3 m, float f)
        {
            return new Matrix3(m.c0 / f, m.c1 / f, m.c2 / f);
        }
    }
}
