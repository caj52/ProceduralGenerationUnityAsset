using System;
using System.Collections.Generic;
using UnityEngine;

public class GenerationAlgorithm : ScriptableObject
{
    public Vector2Int mapSize = new Vector2Int();
    private List<AlgorithmLayer> _algorithmLayers;

    public GenerationAlgorithm()
    {
        _algorithmLayers = new List<AlgorithmLayer>();
        var defaultLayer = new AlgorithmLayer(GenericNoiseGenerationAlgorithm.Perlin, 1, 0, 0, 1);
        AddAlgorithmLayer(defaultLayer);
    }
    void AddAlgorithmLayer(AlgorithmLayer newLayer)
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
    public float Scale {get; private set;}
    public float XOffset {get; private set;}
    public float YOffset {get; private set;}
    public float Amplitude {get; private set;}
    
    public float layerStrength {get; private set;}

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
    Random,
    Simplex,
    Perlin,
    Worley,
}
