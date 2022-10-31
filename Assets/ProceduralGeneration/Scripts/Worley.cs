using System;
using System.Collections.Generic;
using UnityEngine;

namespace ProceduralNoiseProject
{
    public class WorleyNoise
    {
        private static readonly float[] OFFSET_F = new float[] { -0.5f, 0.5f, 1.5f };

        private const float K = 1.0f / 7.0f;

        private const float Ko = 3.0f / 7.0f;

        public float Jitter { get; set; }

        public int Distance { get; set; }

        public int Combination { get; set; }

        private int Perm { get; set; }

        public WorleyNoise(int seed, float frequency, float jitter, float amplitude = 1.0f)
        {
            Frequency = frequency;
            Amplitude = amplitude;
            Offset = Vector3.zero;
            Jitter = jitter;

            Perm = new PermutationTable(1024, 255, seed);
        }
        
        /// <summary>
        /// Sample the noise in 2 dimensions.
        /// </summary>
        public override float Sample2D(float x, float y)
        {

            x = (x + Offset.x) * Frequency;
            y = (y + Offset.y) * Frequency;

            int Pi0 = (int)Mathf.Floor(x);
            int Pi1 = (int)Mathf.Floor(y);

            float Pf0 = Frac(x);
            float Pf1 = Frac(y);

            Vector3 pX = new Vector3();
            pX[0] = Perm[Pi0 - 1];
            pX[1] = Perm[Pi0];
            pX[2] = Perm[Pi0 + 1];

            float d0, d1, d2;
            float F0 = float.PositiveInfinity;
            float F1 = float.PositiveInfinity;
            float F2 = float.PositiveInfinity;

            int px, py, pz;
            float oxx, oxy, oxz;
            float oyx, oyy, oyz;

            for (int i = 0; i < 3; i++)
            {
                px = Perm[(int)pX[i], Pi1 - 1];
                py = Perm[(int)pX[i], Pi1];
                pz = Perm[(int)pX[i], Pi1 + 1];

                oxx = Frac(px * K) - Ko;
                oxy = Frac(py * K) - Ko;
                oxz = Frac(pz * K) - Ko;

                oyx = Mod(Mathf.Floor(px * K), 7.0f) * K - Ko;
                oyy = Mod(Mathf.Floor(py * K), 7.0f) * K - Ko;
                oyz = Mod(Mathf.Floor(pz * K), 7.0f) * K - Ko;

                d0 = Distance2(Pf0, Pf1, OFFSET_F[i] + Jitter * oxx, -0.5f + Jitter * oyx);
                d1 = Distance2(Pf0, Pf1, OFFSET_F[i] + Jitter * oxy, 0.5f + Jitter * oyy);
                d2 = Distance2(Pf0, Pf1, OFFSET_F[i] + Jitter * oxz, 1.5f + Jitter * oyz);

                if (d0 < F0) { F2 = F1; F1 = F0; F0 = d0; }
                else if (d0 < F1) { F2 = F1; F1 = d0; }
                else if (d0 < F2) { F2 = d0; }

                if (d1 < F0) { F2 = F1; F1 = F0; F0 = d1; }
                else if (d1 < F1) { F2 = F1; F1 = d1; }
                else if (d1 < F2) { F2 = d1; }

                if (d2 < F0) { F2 = F1; F1 = F0; F0 = d2; }
                else if (d2 < F1) { F2 = F1; F1 = d2; }
                else if (d2 < F2) { F2 = d2; }

            }

            return Combine(F0, F1, F2) * Amplitude;
        }

        private float Mod(float x, float y)
        {
            return x - y * Mathf.Floor(x / y);
        }

        private float Frac(float v)
        {
            return v - Mathf.Floor(v);
        }

        private float Distance1(float p1x, float p2x)
        {
            switch (Distance)
            {
                case 0:
                    return (p1x - p2x) * (p1x - p2x);

                case 1:
                    return Math.Abs(p1x - p2x);

                case 2:
                    return Math.Abs(p1x - p2x);
            }

            return 0;
        }

        private float Distance2(float p1x, float p1y, float p2x, float p2y)
        {
            switch (Distance)
            {
                case 0:
                    return (p1x - p2x) * (p1x - p2x) + (p1y - p2y) * (p1y - p2y);

                case 1:
                    return Math.Abs(p1x - p2x) + Math.Abs(p1y - p2y);

                case 2:
                    return Math.Max(Math.Abs(p1x - p2x), Math.Abs(p1y - p2y));
            }

            return 0;
        }

        private float Distance3(float p1x, float p1y, float p1z, float p2x, float p2y, float p2z)
        {
            switch (Distance)
            {
                case 0:
                    return (p1x - p2x) * (p1x - p2x) + (p1y - p2y) * (p1y - p2y) + (p1z - p2z) * (p1z - p2z);

                case 1:
                    return Math.Abs(p1x - p2x) + Math.Abs(p1y - p2y) + Math.Abs(p1z - p2z);

                case 2:
                    return Math.Max(Math.Max(Math.Abs(p1x - p2x), Math.Abs(p1y - p2y)), Math.Abs(p1z - p2z));
            }

            return 0;
        }

        private float Combine(float f0, float f1, float f2)
        {
            switch (Combination)
            {
                case 0:
                    return f0;

                case 1:
                    return f1 - f0;

                case 2:
                    return f2 - f0;
            }

            return 0;
        }
    }
}