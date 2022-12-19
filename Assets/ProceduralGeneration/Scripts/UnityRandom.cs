using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnityRandom : MonoBehaviour
{
    public static float [,] Generate(float _size,float _scale, int seed)
    {
        float[,] map = new float[(int)_size,(int)_size];

        /*for (int x = 0; x < _size; x++)
        {
            for (int y = 0; y < _size; y++)
            {
                var xCoord = _xCoord + x * (_scale/10);
                var yCoord = _yCoord + y * (_scale/10);

                map[x,y] += Mathf.Clamp(Generate(xCoord, yCoord,0) * _amplitude,0,1);
            }
        }*/
        return map;
    }
}
