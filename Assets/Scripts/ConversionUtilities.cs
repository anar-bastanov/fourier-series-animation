using System.Numerics;
using Vector2 = UnityEngine.Vector2;

namespace Local
{
    public static class ConversionUtilities
    {
        public static Vector2 ToVector(this Complex value) =>
            new((float)value.Real, (float)value.Imaginary);

        public static Complex ToComplex(this Vector2 value) =>
            new(value.x, value.y);
    }
}
