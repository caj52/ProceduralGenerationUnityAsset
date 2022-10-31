using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProceduralGeneration
{
    public float[] Generate(GenerationAlgorithm generationSettings, float xCoordinate = 0, float yCoordinate = 0)
    {
        var arraySize = generationSettings.mapSize.x * generationSettings.mapSize.y;
        var generatedMap = new float[arraySize];
        return generatedMap;
    }
}
