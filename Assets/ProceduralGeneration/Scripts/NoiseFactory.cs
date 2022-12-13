using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class NoiseFactory
{
   public static float[,] GenerateLayeredNoise(int imageSize,GenerationAlgorithm noiseAlgorithm)
   {
      var noiseMap = new float[imageSize,imageSize];
      var totalLayerStrengths = 0f;
      foreach (var layer in noiseAlgorithm._algorithmLayers)
      {
         var layerNoise = GenerateGenericNoise(imageSize, noiseAlgorithm, layer);
         for (var i = 0; i < imageSize; i++)
         for (var e = 0; e < imageSize; e++)
            noiseMap[i, e] += layerNoise[i, e] * layer.layerStrength;
         totalLayerStrengths += layer.layerStrength;
      }
      for (var i = 0; i < imageSize; i++)
      for (var e = 0; e < imageSize; e++)
         noiseMap[i, e] /= totalLayerStrengths;

      return noiseMap;
   }
   public static float[,] GenerateGenericNoise(int imageSize, GenerationAlgorithm algorithm,AlgorithmLayer layer)
   {
      var noiseArray = new float[imageSize,imageSize];
      switch (layer.GenerationAlgorithm)
      {
         case GenericNoiseGenerationAlgorithm.Perlin:
            noiseArray = Perlin.Generate(imageSize, layer.Scale, layer.XOffset, layer.YOffset,
               layer.Amplitude,algorithm.intSeed);
            break;
         case GenericNoiseGenerationAlgorithm.Worley:
            noiseArray = Worley.Generate(imageSize, layer.Scale, layer.XOffset, layer.YOffset,
               layer.Amplitude,algorithm.intSeed);
            break;
      }

      return noiseArray;
   }
}
