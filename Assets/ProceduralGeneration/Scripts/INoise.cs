using System;
using System.Collections;
using UnityEngine;

namespace ProceduralNoiseProject
{
    /// <summary>
    /// Interface for generating noise.
    /// </summary>
    public interface INoise 
    {

        /// <summary>
        /// The frequency of the fractal.
        /// </summary>
        float Frequency { get; set; }

        /// <summary>
        /// Sample the noise in 2 dimensions.
        /// </summary>
        float Sample2D(float x, float y);
        
        /// <summary>
        /// Update the seed.
        /// </summary>
        void UpdateSeed(int seed);

    }

}