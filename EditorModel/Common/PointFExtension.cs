using System;
using System.Drawing;

namespace EditorModel.Common
{
    public static class PointFExtension
    {
        public const float PI = (float)Math.PI;
        public const float PI2 = 2 * PI;
        public const float TO_DEGREES = 180 / (float)Math.PI;
        public const float TO_RADIANS = (float)Math.PI / 180;

        public static float Length(this PointF vector)
        {
            return (float) Math.Sqrt(vector.X*vector.X + vector.Y*vector.Y);
        }

      
        public static float LengthSqr(this PointF vector)
        {
            return vector.X * vector.X + vector.Y * vector.Y;
        }

      
        public static float DotScalar(this PointF vector1, PointF vector2)
        {
            return vector1.X * vector2.X + vector1.Y * vector2.Y;
        }

      
        public static PointF Add(this PointF vector1, PointF vector2)
        {
            return new PointF(vector1.X + vector2.X, vector1.Y + vector2.Y);
        }

   
        public static PointF Sub(this PointF vector1, PointF vector2)
        {
            return new PointF(vector1.X - vector2.X, vector1.Y - vector2.Y);
        }

    
        public static float Angle(this PointF vector1, PointF vector2)
        {
            var a = vector1.Angle() - vector2.Angle();
            a += (a > PI) ? -2 * PI : (a < -PI) ? 2 * PI : 0;

            return a;
        }

    
        public static float Angle(this PointF c)
        {
            return (float)Math.Atan2(c.Y, c.X);
        }


    }
}
