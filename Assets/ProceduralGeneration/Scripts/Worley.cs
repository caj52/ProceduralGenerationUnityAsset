using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;

namespace ProceduralNoiseProject
{
    public class Worley
    {
        public enum VORONOI_DISTANCE
        {
            EUCLIDIAN,
            MANHATTAN,
            CHEBYSHEV
        }

        public enum VORONOI_COMBINATION
        {
            D0,D1_D0,D2_D0
        }
        private static readonly float[] OFFSET_F = new float[] { -0.5f, 0.5f, 1.5f };

        private const float K = 1.0f / 7.0f;

        private const float Ko = 3.0f / 7.0f;

        private static float Jitter;

        public static VORONOI_DISTANCE Distance;

        public static VORONOI_COMBINATION Combination;

        private static PermutationTable Perm { get; set; }
        
        /// <summary>
        /// Update the seed.
        /// </summary>
        public static void UpdateSeed(int seed)
        {
            Perm.Build(seed);
        }
        /// <summary>
        /// Sample the noise in 2 dimensions.
        /// </summary>
        static float Generate(float x, float y, float amplitude)
        {
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

            return Combine(F0, F1, F2) * amplitude;
        }
        public static float[,] Generate(int _size,float _scale,float _xCoord,float _yCoord,float _amplitude, int seed)
        {
            Jitter = 1;
            Perm = new PermutationTable(_size, _size,seed);
            var map = new float[_size,_size];

            for (var x = 0; x < _size; x++)
            for (var y = 0; y < _size; y++)
            {
                var xCoord = _xCoord + x * (_scale/10);
                var yCoord = _yCoord + y * (_scale/10);
                map[x,y] += Generate(xCoord, yCoord,_amplitude);
            }
		
            return map;
        }
        private static float Mod(float x, float y)
        {
            return x - y * Mathf.Floor(x / y);
        }
        private static float Frac(float v)
        {
            return v - Mathf.Floor(v);
        }
        private static float Distance2(float p1x, float p1y, float p2x, float p2y)
        {
            switch (Distance)
            {
                case VORONOI_DISTANCE.EUCLIDIAN:
                    return (p1x - p2x) * (p1x - p2x) + (p1y - p2y) * (p1y - p2y);

                case VORONOI_DISTANCE.MANHATTAN:
                    return Math.Abs(p1x - p2x) + Math.Abs(p1y - p2y);

                case VORONOI_DISTANCE.CHEBYSHEV:
                    return Math.Max(Math.Abs(p1x - p2x), Math.Abs(p1y - p2y));
            }

            return 0;
        }
        private static float Combine(float f0, float f1, float f2)
        {
            
            switch (Combination)
            {
                case VORONOI_COMBINATION.D0:
                    return f0;

                case VORONOI_COMBINATION.D1_D0:
                    return f1 - f0;

                case VORONOI_COMBINATION.D2_D0:
                    return f2 - f0;
            }

            return 0;
        }
    }
}