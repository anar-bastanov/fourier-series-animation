using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using UnityEngine;

namespace Local
{
    public static class FourierSeries
    {
        public static Complex[] LoadSamples(TextAsset file)
        {
            if (file == null)
                return new Complex[1];

            var lines = file.text.Split('\n', StringSplitOptions.RemoveEmptyEntries);

            return lines.Select(line =>
            {
                // Throw on purpose
                var index = line.IndexOf(',');
                double x = double.Parse(line.AsSpan(0, index));
                double y = double.Parse(line.AsSpan(index + 1));
                return new Complex(x, y);
            }).ToArray();
        }

        public static IEnumerable<Complex> ComputeCoefficients(Complex[] samples, int precision)
        {
            const float period = 2 * MathF.PI;

            for (int i = 1; i <= precision; ++i)
            {
                var coefficient = new Complex();
                int n = i % 2 == 0 ? i / 2 : -i / 2;

                for (int t = 0; t < samples.Length; ++t)
                {
                    double cos = Math.Cos(period * n * t / samples.Length);
                    double sin = Math.Sin(period * n * t / samples.Length);
                    var ft = samples[t] * new Complex(cos, sin);
                    coefficient += ft;
                }

                yield return coefficient / samples.Length;
            }
        }
    }
}
