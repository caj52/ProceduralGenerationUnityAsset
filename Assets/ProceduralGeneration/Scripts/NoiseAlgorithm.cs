using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class NoiseAlgorithm : ScriptableObject
{
    public Vector2Int mapSize = new Vector2Int();
    public List<AlgorithmLayer> _algorithmLayers;
    public string seed = "1874293764";
    public int intSeed;

    public void SetIntSeed()
    {
        try
        {
            intSeed = int.Parse(seed);
        }
        catch
        {
            var asciiCharacters= Encoding.ASCII.GetBytes(seed);
            var returnInt = asciiCharacters.Sum(Convert.ToInt32);
            intSeed = returnInt;
        }
    }
    public NoiseAlgorithm()
    {
        _algorithmLayers = new List<AlgorithmLayer>();
        var defaultLayer = new AlgorithmLayer(GenericNoiseGenerationAlgorithm.Perlin, 1, 0, 0, 1);
        AddAlgorithmLayer(defaultLayer);
    }
    public void AddAlgorithmLayer(AlgorithmLayer newLayer)
    {
        _algorithmLayers.Add(newLayer);
    }
    public List<AlgorithmLayer> GetAlgorithmLayers()
    {
        return _algorithmLayers;
    }
}

public class AlgorithmLayer
{
    public GenericNoiseGenerationAlgorithm GenerationAlgorithm { get; private set;}
    public float Scale { get; private set; } = 1;
    public float XOffset {get; private set;}
    public float YOffset {get; private set;}
    public float Amplitude {get; private set;} = 1;
    
    public float layerStrength {get; private set;} = 1;
    public AlgorithmLayer()
    {
    }
    public AlgorithmLayer(GenericNoiseGenerationAlgorithm _genAlgorithm,float _scale, float _xOffset,float _yOffset,float _amplitude)
    {
        GenerationAlgorithm = _genAlgorithm;
        Scale = _scale;
        XOffset = _xOffset;
        YOffset = _yOffset;
        Amplitude = _amplitude;

        layerStrength = 1;
    }

    public void SetAlgorithmVariables(GenericNoiseGenerationAlgorithm _genAlgorithm,float _scale, float _xOffset,float _yOffset,float _amplitude)
    {
        GenerationAlgorithm = _genAlgorithm;
        Scale = _scale;
        XOffset = _xOffset;
        YOffset = _yOffset;
        Amplitude = _amplitude;
    }

    public void SetLayerStrength(float newStrength)
    {
        layerStrength = newStrength;
    }
    
    
}

public enum GenericNoiseGenerationAlgorithm
{
    Perlin,
    Random,
    Simplex,
    Worley,
    Jarnagin
}
