using System;
using System.Runtime.CompilerServices;

#pragma warning disable CA1062 // Validate arguments of public methods

namespace Prowl.Runtime
{
    /// <summary>
    /// Provides some general help math-related functions and values.
    /// </summary>
    public static class MathUtilities
    {
        /// <summary>The value of PI divided by 2.</summary>
        public const double PiOver2 = 1.5707963267948966192313216916398;

        /// <summary>The value of PI divided by 4.</summary>
        public const double PiOver4 = 0.78539816339744830961566084581988;

        /// <summary>The value of PI multiplied by 3 and divided by 2.</summary>
        public const double ThreePiOver2 = 4.7123889803846898576939650749193;

        /// <summary>The value of PI multiplied by 2.</summary>
        public const double TwoPI = 6.283185307179586476925286766559;

        public const double DegToRadFactor = Math.PI / 180;
        public const double RadToDefFactor = 180 / Math.PI;

        public static double UnwindDegrees(double angle)
        {
            while (angle > 180.0)
                angle -= 360.0;
            while (angle < -180.0)
                angle += 360.0;
            return angle;
        }

        public static double ArcTanAngle(double X, double Y)
        {
            if (X == 0)
            {
                if (Y == 1) return PiOver2;
                else return -PiOver2;
            }
            else if (X > 0) return (double)Math.Atan(Y / X);
            else if (X < 0)
            {
                if (Y > 0) return (double)Math.Atan(Y / X) + (double)Math.PI;
                else return (double)Math.Atan(Y / X) - (double)Math.PI;
            }
            else return 0;
        }

        public static Vector2 ToDouble(this System.Numerics.Vector2 v) => new Vector2(v.X, v.Y);
        public static Vector3 ToDouble(this System.Numerics.Vector3 v) => new Vector3(v.X, v.Y, v.Z);
        public static Vector4 ToDouble(this System.Numerics.Vector4 v) => new Vector4(v.X, v.Y, v.Z, v.W);
        public static Quaternion ToDouble(this System.Numerics.Quaternion v) => new Quaternion(v.X, v.Y, v.Z, v.W);

        public static Matrix4x4 ToDouble(this System.Numerics.Matrix4x4 m)
        {
            Matrix4x4 result;
            result.M11 = (double)m.M11;
            result.M12 = (double)m.M12;
            result.M13 = (double)m.M13;
            result.M14 = (double)m.M14;

            result.M21 = (double)m.M21;
            result.M22 = (double)m.M22;
            result.M23 = (double)m.M23;
            result.M24 = (double)m.M24;

            result.M31 = (double)m.M31;
            result.M32 = (double)m.M32;
            result.M33 = (double)m.M33;
            result.M34 = (double)m.M34;

            result.M41 = (double)m.M41;
            result.M42 = (double)m.M42;
            result.M43 = (double)m.M43;
            result.M44 = (double)m.M44;
            return result;
        }

        //returns Euler angles that point from one point to another
        public static Vector3 AngleTo(Vector3 from, Vector3 location)
        {
            Vector3 angle = new Vector3();
            Vector3 v3 = Vector3.Normalize(location - from);
            angle.x = (double)Math.Asin(v3.y);
            angle.y = ArcTanAngle(-v3.z, -v3.x);
            return angle;
        }

        /// <summary>
        /// Interpolates linearly between two values.
        /// </summary>
        /// <param name="min">The initial value in the interpolation.</param>
        /// <param name="max">The final value in the interpolation.</param>
        /// <param name="amount">The amount of interpolation, measured between 0 and 1.</param>
        public static double Lerp(double min, double max, double amount)
        {
            return min + (max - min) * amount;
        }

        /// <summary>
        /// Interpolates linearly between two values.
        /// </summary>
        /// <param name="min">The initial value in the interpolation.</param>
        /// <param name="max">The final value in the interpolation.</param>
        /// <param name="amount">The amount of interpolation, measured between 0 and 1.</param>
        /// <remarks>
        /// In comparison to <see cref="Lerp(double, double, double)"/>, this function is more
        /// precise when working with big values.
        /// </remarks>
        public static double LerpPrecise(double min, double max, double amount)
        {
            return (1 - amount) * min + max * amount;
        }

        /// <summary>
        /// Interpolates between two values using a cubic equation.
        /// </summary>
        /// <param name="min">The initial value in the interpolation.</param>
        /// <param name="max">The final value in the interpolation.</param>
        /// <param name="amount">The amount of interpolation, measured between 0 and 1.</param>
        public static double SmoothStep(double min, double max, double amount)
        {
            // Lerp using the polynomial: 3xx - 2xxx
            return Lerp(min, max, (3 - 2 * amount) * amount * amount);
        }

        /// <summary>
        /// Interpolates between two values using a cubic equation.
        /// </summary>
        /// <param name="min">The initial value in the interpolation.</param>
        /// <param name="max">The final value in the interpolation.</param>
        /// <param name="amount">The amount of interpolation, measured between 0 and 1.</param>
        /// <remarks>
        /// In comparison to <see cref="SmoothStep(double, double, double)"/>, this function is more
        /// precise when working with big values.
        /// </remarks>
        public static double SmoothStepPrecise(double min, double max, double amount)
        {
            return LerpPrecise(min, max, (3 - 2 * amount) * amount * amount);
        }

        /// <summary>
        /// Interpolates between two values using a 5th-degree equation.
        /// </summary>
        /// <param name="min">The initial value in the interpolation.</param>
        /// <param name="max">The final value in the interpolation.</param>
        /// <param name="amount">The amount of interpolation, measured between 0 and 1.</param>
        public static double SmootherStep(double min, double max, double amount)
        {
            // Lerp using the polynomial: 6(x^5) - 15(x^4) + 10(x^3)
            return Lerp(min, max, (6 * amount * amount - 15 * amount + 10) * amount * amount * amount);
        }

        /// <summary>
        /// Interpolates between two values using a 5th-degree equation.
        /// </summary>
        /// <param name="min">The initial value in the interpolation.</param>
        /// <param name="max">The final value in the interpolation.</param>
        /// <param name="amount">The amount of interpolation, measured between 0 and 1.</param>
        /// <remarks>
        /// In comparison to <see cref="SmootherStep(double, double, double)"/>, this function is more
        /// precise when working with big values.
        /// </remarks>
        public static double SmootherStepPrecise(double min, double max, double amount)
        {
            // Lerp using the polynomial: 6(x^5) - 15(x^4) + 10(x^3)
            return LerpPrecise(min, max, (6 * amount * amount - 15 * amount + 10) * amount * amount * amount);
        }

        /// <summary>
        /// Calculates the size to use for an array that needs resizing, where the new size
        /// will be a power of two times the previous capacity.
        /// </summary>
        /// <param name="currentCapacity">The current length of the array.</param>
        /// <param name="requiredCapacity">The minimum required length for the array.</param>
        /// <remarks>
        /// This is calculated with the following equation:<para/>
        /// <code>
        /// newCapacity = currentCapacity * pow(2, ceiling(log2(requiredCapacity/currentCapacity)));
        /// </code>
        /// </remarks>
        public static int GetNextCapacity(int currentCapacity, int requiredCapacity)
        {
            // Finds the smallest number that is greater than requiredCapacity and satisfies this equation:
            // " newCapacity = oldCapacity * 2^X " where X is an integer

            const double log2 = 0.69314718055994530941723212145818;
            int power = (int)Math.Ceiling(Math.Log(requiredCapacity / (double)currentCapacity) / log2);
            return currentCapacity * IntegerPow(2, power);
        }

        /// <summary>
        /// Calculates an integer value, raised to an integer exponent. Only works with positive values.
        /// </summary>
        public static int IntegerPow(int value, int exponent)
        {
            int r = 1;
            while (exponent > 0)
            {
                if ((exponent & 1) == 1)
                    r *= value;
                exponent >>= 1;
                value *= value;
            }
            return r;
        }

        /// <summary>
        /// Returns a random direction, as a unit vector.
        /// </summary>
        /// <param name="random">The <see cref="Random"/> to use for randomizing.</param>
        public static Vector2 RandomDirection2(this Random random)
        {
            double angle = (double)(random.NextDouble() * 6.283185307179586476925286766559);
            return new Vector2(Math.Cos(angle), Math.Sin(angle));
        }

        /// <summary>
        /// Returns a random direction, as a vector of a specified length.
        /// </summary>
        /// <param name="random">The <see cref="Random"/> to use for randomizing.</param>
        /// <param name="length">The desired length of the direction vector.</param>
        public static Vector2 RandomDirection2(this Random random, double length)
        {
            return random.RandomDirection2() * length;
        }

        /// <summary>
        /// Returns a random direction, as a unit vector.
        /// </summary>
        /// <param name="random">The <see cref="Random"/> to use for randomizing.</param>
        public static Vector3 RandomDirection3(this Random random)
        {
            double a = (double)(random.NextDouble() * 6.283185307179586476925286766559);
            double b = (double)(random.NextDouble() * Math.PI);
            double sinB = System.Math.Sin(b);
            return new Vector3(sinB * System.Math.Cos(a), sinB * System.Math.Sin(a), System.Math.Cos(b));
        }

        /// <summary>
        /// Returns a random direction, as a vector of a specified length.
        /// </summary>
        /// <param name="random">The <see cref="Random"/> to use for randomizing.</param>
        /// <param name="length">The desired length of the direction vector.</param>
        public static Vector3 RandomDirection3(this Random random, double length)
        {
            return random.RandomDirection3() * length;
        }

        /// <summary>
        /// Returns a random doubleing-point number in the range [0.0, 1.0).
        /// </summary>
        /// <param name="random">The <see cref="Random"/> to use for randomizing.</param>
        public static double NextFloat(this Random random)
        {
            return (double)random.NextDouble();
        }

        /// <summary>
        /// Returns a random doubleing-point number in the range [0.0, max) (or (max, 0.0] if negative).
        /// </summary>
        /// <param name="random">The <see cref="Random"/> to use for randomizing.</param>
        /// <param name="max">The exclusive maximum value of the random number to be generated.</param>
        public static double NextFloat(this Random random, double max)
        {
            return (double)random.NextDouble() * max;
        }

        /// <summary>
        /// Returns a random doubleing-point number in the range [min, max) (or (max, min] if min>max)
        /// </summary>
        /// <param name="random">The <see cref="Random"/> to use for randomizing.</param>
        /// <param name="min">The inclusive minimum value of the random number to be generated.</param>
        /// <param name="max">The exclusive maximum value of the random number to be generated.</param>
        public static double NextFloat(this Random random, double min, double max)
        {
            return (double)random.NextDouble() * (max - min) + min;
        }

        /// <summary>
        /// Returns a random doubleing-point number in the range [0.0, max) (or (max, 0.0] if negative).
        /// </summary>
        /// <param name="random">The <see cref="Random"/> to use for randomizing.</param>
        /// <param name="max">The exclusive maximum value of the random number to be generated.</param>
        public static double NextDouble(this Random random, double max)
        {
            return random.NextDouble() * max;
        }

        /// <summary>
        /// Returns a random doubleing-point number in the range [min, max) (or (max, min] if min>max)
        /// </summary>
        /// <param name="random">The <see cref="Random"/> to use for randomizing.</param>
        /// <param name="min">The inclusive minimum value of the random number to be generated.</param>
        /// <param name="max">The exclusive maximum value of the random number to be generated.</param>
        public static double NextDouble(this Random random, double min, double max)
        {
            return random.NextDouble() * (max - min) + min;
        }

        /// <summary>
        /// Returns a random boolean value.
        /// </summary>
        /// <param name="random">The <see cref="Random"/> to use for randomizing.</param>
        public static bool NextBool(this Random random)
        {
            return (random.Next() & 1) == 0;
        }

        /// <summary>
        /// Constructs a completely randomized <see cref="Color"/>.
        /// </summary>
        /// <param name="random">The <see cref="Random"/> to use for randomizing.</param>
        public static Color NextColor(this Random random)
        {
            // A single random value isn't enough, it's only 31 bits...
            // So we use one for RGB and an extra one for Alpha.
            unchecked
            {
                uint val = (uint)random.Next();
                return new Color32((byte)(val & 255), (byte)((val >> 8) & 255), (byte)((val >> 16) & 255), (byte)(random.Next() & 255));
            }
        }

        /// <summary>
        /// Constructs a randomized <see cref="Color"/> with an alpha value of 255.
        /// </summary>
        /// <param name="random">The <see cref="Random"/> to use for randomizing.</param>
        public static Color NextColorFullAlpha(this Random random)
        {
            unchecked
            {
                uint val = (uint)random.Next();
                Color color = new Color32((byte)(val & 255), (byte)((val >> 8) & 255), (byte)((val >> 16) & 255), (byte)255);
                return color;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int ComputeMipLevels(int width, int height)
        {
            return (int)System.Math.Log2(System.Math.Max(width, height));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Round(this double x)
        {
            return (int)System.Math.Floor(x);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Matrix4x4 RotationYawPitchRoll(double yaw, double pitch, double roll)
        {
            Quaternion quaternion = Quaternion.CreateFromYawPitchRoll(yaw, pitch, roll);
            return RotationQuaternion(quaternion);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Matrix4x4 CreateTransform(Vector3 pos, Vector3 rotation, Vector3 scale)
        {
            return Matrix4x4.CreateTranslation(pos) * Matrix4x4.CreateFromYawPitchRoll(rotation.x, rotation.y, rotation.z) * Matrix4x4.CreateScale(scale);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Matrix4x4 CreateTransform(Vector3 pos, Quaternion rotation, Vector3 scale)
        {
            return Matrix4x4.CreateTranslation(pos) * Matrix4x4.CreateFromQuaternion(rotation) * Matrix4x4.CreateScale(scale);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Matrix4x4 CreateTransform(Vector3 pos, Vector3 rotation, double scale)
        {
            return Matrix4x4.CreateTranslation(pos) * Matrix4x4.CreateFromYawPitchRoll(rotation.x, rotation.y, rotation.z) * Matrix4x4.CreateScale(scale);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Matrix4x4 CreateTransform(Vector3 pos, Quaternion rotation, double scale)
        {
            return Matrix4x4.CreateTranslation(pos) * Matrix4x4.CreateFromQuaternion(rotation) * Matrix4x4.CreateScale(scale);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Matrix4x4 CreateTransform(Vector3 pos, double scale)
        {
            return Matrix4x4.CreateTranslation(pos) * Matrix4x4.CreateScale(scale);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Matrix4x4 CreateTransform(Vector3 pos, Vector3 scale)
        {
            return Matrix4x4.CreateTranslation(pos) * Matrix4x4.CreateScale(scale);
        }

        /// <summary>Creates a new quaternion from the given yaw, pitch, and roll.</summary>
        /// <param name="yaw">The yaw angle, in radians, around the Y axis.</param>
        /// <returns>The resulting quaternion.</returns>
        public static Quaternion CreateFromYaw(double yaw)
        {
            //  Roll first, about axis the object is facing, then
            //  pitch upward, then yaw to face into the new heading
            double sy, cy;

            double halfYaw = yaw * 0.5;
            sy = System.Math.Sin(halfYaw);
            cy = System.Math.Cos(halfYaw);

            Quaternion result;

            result.x = 0;
            result.y = sy;
            result.z = 0;
            result.w = cy;

            return result;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 Transform(Vector3 value, double yaw)
        {
            double sy, cy;

            double halfYaw = yaw * 0.5;
            sy = System.Math.Sin(halfYaw);
            cy = System.Math.Cos(halfYaw);

            double y2 = sy + sy;

            double wy2 = cy * y2;
            double yy2 = sy * y2;

            return new Vector3(
                value.x * (1.0 - yy2) + value.z * wy2,
                value.y,
                value.x * wy2 + value.z * (1.0 - yy2)
            );
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 TransformOnlyYaw(this Vector3 value, Quaternion r)
        {
            double yaw = System.Math.Atan2(2.0f * (r.y * r.w + r.x * r.z), 1.0 - 2.0 * (r.x * r.x + r.y * r.y));

            double halfYaw = yaw * 0.5;
            double sy = System.Math.Sin(halfYaw);
            double cy = System.Math.Cos(halfYaw);

            double y2 = sy + sy;

            double wy2 = cy * y2;
            double yy2 = sy * y2;

            return new Vector3(value.x * (1.0 - yy2) + value.z * wy2, value.y, value.x * wy2 + value.z * (1.0 - yy2));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void GetYawPitchRoll(this Quaternion r, out double yaw, out double pitch, out double roll)
        {
            yaw = System.Math.Atan2(2.0 * (r.y * r.w + r.x * r.z), 1.0 - 2.0 * (r.x * r.x + r.y * r.y));
            pitch = System.Math.Asin(2.0 * (r.x * r.w - r.y * r.z));
            roll = System.Math.Atan2(2.0 * (r.x * r.y + r.z * r.w), 1.0 - 2.0 * (r.x * r.x + r.z * r.z));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void GetYaw(this Quaternion r, out double yaw)
        {
            yaw = System.Math.Atan2(2.0 * (r.y * r.w + r.x * r.z), 1.0 - 2.0 * (r.x * r.x + r.y * r.y));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 GetRotation(this Quaternion r)
        {
            double yaw = System.Math.Atan2(2.0 * (r.y * r.w + r.x * r.z), 1.0 - 2.0 * (r.x * r.x + r.y * r.y));
            double pitch = System.Math.Asin(2.0 * (r.x * r.w - r.y * r.z));
            double roll = System.Math.Atan2(2.0 * (r.x * r.y + r.z * r.w), 1.0 - 2.0 * (r.x * r.x + r.z * r.z));
            // If any nan or inf, set that value to 0
            if (double.IsNaN(yaw) || double.IsInfinity(yaw)) yaw = 0;
            if (double.IsNaN(pitch) || double.IsInfinity(pitch)) pitch = 0;
            if (double.IsNaN(roll) || double.IsInfinity(roll)) roll = 0;
            return new Vector3(yaw, pitch, roll);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector4 ApplyMatrix(this Vector4 self, Matrix4x4 matrix)
        {
            return new Vector4(
                matrix.M11 * self.x + matrix.M12 * self.y + matrix.M13 * self.z + matrix.M14 * self.w,
                matrix.M21 * self.x + matrix.M22 * self.y + matrix.M23 * self.z + matrix.M24 * self.w,
                matrix.M31 * self.x + matrix.M32 * self.y + matrix.M33 * self.z + matrix.M34 * self.w,
                matrix.M41 * self.x + matrix.M42 * self.y + matrix.M43 * self.z + matrix.M44 * self.w
            );
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 ApplyMatrix(this Vector3 self, Matrix4x4 matrix)
        {
            return new Vector3(
                matrix.M11 * self.x + matrix.M12 * self.y + matrix.M13 * self.z + matrix.M14,
                matrix.M21 * self.x + matrix.M22 * self.y + matrix.M23 * self.z + matrix.M24,
                matrix.M31 * self.x + matrix.M32 * self.y + matrix.M33 * self.z + matrix.M34
            );
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 ToDeg(this Vector3 v)
        {
            return new Vector3((double)(v.x * RadToDefFactor), (double)(v.y * RadToDefFactor), (double)(v.z * RadToDefFactor));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 ToRad(this Vector3 v)
        {
            return new Vector3((double)(v.x * DegToRadFactor), (double)(v.y * DegToRadFactor), (double)(v.z * DegToRadFactor));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double ToDeg(this double v)
        {
            return (double)(v * RadToDefFactor);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double ToRad(this double v)
        {
            return (double)(v * DegToRadFactor);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float ToDeg(this float v)
        {
            return (float)(v * RadToDefFactor);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float ToRad(this float v)
        {
            return (float)(v * DegToRadFactor);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Quaternion GetQuaternion(this Vector3 vector)
        {
            return Quaternion.CreateFromYawPitchRoll(vector.x, vector.y, vector.z);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Matrix4x4 RotationQuaternion(Quaternion rotation)
        {
            double xx = rotation.x * rotation.x;
            double yy = rotation.y * rotation.y;
            double zz = rotation.z * rotation.z;
            double xy = rotation.x * rotation.y;
            double zw = rotation.z * rotation.w;
            double zx = rotation.z * rotation.x;
            double yw = rotation.y * rotation.w;
            double yz = rotation.y * rotation.z;
            double xw = rotation.x * rotation.w;

            Matrix4x4 result = Matrix4x4.Identity;
            result.M11 = 1.0 - 2.0 * (yy + zz);
            result.M12 = 2.0 * (xy + zw);
            result.M13 = 2.0 * (zx - yw);
            result.M21 = 2.0 * (xy - zw);
            result.M22 = 1.0 - 2.0 * (zz + xx);
            result.M23 = 2.0 * (yz + xw);
            result.M31 = 2.0 * (zx + yw);
            result.M32 = 2.0 * (yz - xw);
            result.M33 = 1.0 - 2.0 * (yy + xx);
            return result;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 NormalizeEulerAngleDegrees(this Vector3 angle)
        {
            double normalizedX = angle.x % 360;
            double normalizedY = angle.y % 360;
            double normalizedZ = angle.z % 360;
            if (normalizedX < 0)
            {
                normalizedX += 360;
            }

            if (normalizedY < 0)
            {
                normalizedY += 360;
            }

            if (normalizedZ < 0)
            {
                normalizedZ += 360;
            }

            return new(normalizedX, normalizedY, normalizedZ);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2 NormalizeEulerAngleDegrees(this Vector2 angle)
        {
            double normalizedX = angle.x % 360;
            double normalizedY = angle.y % 360;
            if (normalizedX < 0)
            {
                normalizedX += 360;
            }

            if (normalizedY < 0)
            {
                normalizedY += 360;
            }

            return new(normalizedX, normalizedY);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint Pack(this Vector4 color)
        {
            return Pack((uint)(color.w * 255), (uint)(color.x * 255), (uint)(color.y * 255), (uint)(color.z * 255));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint Pack(uint a, uint r, uint g, uint b)
        {
            return (a << 24) + (r << 16) + (g << 8) + b;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void LockQuaternionAxis(Quaternion r, Quaternion q, Vector3 mask)
        {
            double x = System.Math.Atan2(2.0 * (r.y * r.w + r.x * r.z), 1.0 - 2.0 * (r.x * r.x + r.y * r.y)) * mask.x;
            double y = System.Math.Asin(2.0 * (r.x * r.w - r.y * r.z)) * mask.y;
            double z = System.Math.Atan2(2.0 * (r.x * r.y + r.z * r.w), 1.0 - 2.0 * (r.x * r.x + r.z * r.z)) * mask.z;

            double xHalf = x * 0.5;
            double yHalf = y * 0.5;
            double zHalf = z * 0.5;

            double cx = System.Math.Cos(xHalf);
            double cy = System.Math.Cos(yHalf);
            double cz = System.Math.Cos(zHalf);
            double sx = System.Math.Sin(xHalf);
            double sy = System.Math.Sin(yHalf);
            double sz = System.Math.Sin(zHalf);

            q.w = (cz * cx * cy) + (sz * sx * sy);
            q.x = (cz * sx * cy) - (sz * cx * sy);
            q.y = (cz * cx * sy) + (sz * sx * cy);
            q.z = (sz * cx * cy) - (cz * sx * sy);
        }
    }
}
