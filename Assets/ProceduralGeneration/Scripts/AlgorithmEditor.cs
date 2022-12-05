using System;
using Unity.Collections;
using UnityEditor;
using UnityEngine;
#if UNITY_EDITOR

public class AlgorithmEditor : EditorWindow
{
    private static AlgorithmEditor editorWindow;
    private static Texture2D proceduralImage;
    
    private static Rect mainImageRect;
    private static Rect layerImageRect;
    private static int layersRectHeight => 300;//currentlyEditing.GetAlgorithmLayers().Count * 145;

    private static GenerationAlgorithm currentlyEditing;
    private static AlgorithmLayer currentLayer => currentlyEditing.GetAlgorithmLayers()[0];

    GUILayoutOption[] layersRect = new GUILayoutOption[] {GUILayout.Width(240f), GUILayout.Height(layersRectHeight) };
    [MenuItem("Tools / Noise Factory")]
   
   public static void ShowWindow()
   {
       editorWindow = GetWindow<AlgorithmEditor>("Noise Factory");
       editorWindow.maxSize = new Vector2(600, 600);
       editorWindow.minSize = new Vector2(600, 600);
       proceduralImage = new Texture2D(300, 300);
       
       var xPosition = editorWindow.maxSize.x - proceduralImage.width - 20;
       mainImageRect = new Rect(xPosition, 20, proceduralImage.width, proceduralImage.height);
       layerImageRect = new Rect(0,0,80,80);
       currentlyEditing = CreateInstance<GenerationAlgorithm>();
   }
   
   private void OnGUI()
   {
       Repaint();

       ////////[SEED [____]]///////////
       GUILayout.BeginHorizontal(EditorStyles.helpBox); 
       currentlyEditing.seed = EditorGUILayout.TextField("Seed", currentlyEditing.seed, GUIStyle.none);
       currentlyEditing.SetIntSeed();
       GUILayout.EndHorizontal();
       ///////////////////////////////////
       
       GUILayout.BeginHorizontal();//1
       GUILayout.Space(95);
       GUILayout.EndHorizontal();//1
       GUILayout.Space(20);
       GUILayout.BeginHorizontal();//3

       DetailsGUI();
       ImageArea();
       
       GUILayout.EndHorizontal();//3
   }

   private void ImageArea()
   {
       GUI.Box(mainImageRect,proceduralImage);
    
       GUILayout.BeginVertical(); //4       
       GUILayout.Space(300);

       ImageGUI();
       
       GUILayout.EndVertical();
   }

   private void ImageGUI()
   {
       var genType = (GenericNoiseGenerationAlgorithm)EditorGUILayout.EnumPopup("Generation Algorithm", currentLayer.GenerationAlgorithm);
       var scale = EditorGUILayout.FloatField("Scale",currentLayer.Scale);
       var xOffset = EditorGUILayout.FloatField("Coordinate X",currentLayer.XOffset);
       var yOffset = EditorGUILayout.FloatField("Coordinate Y",currentLayer.YOffset);
       var amplitude = Math.Clamp(EditorGUILayout.FloatField("Amplitude",currentLayer.Amplitude),0,1);

       NoiseVariationsGUI();

       currentLayer.SetAlgorithmVariables(genType,scale,xOffset,yOffset,amplitude);
   }

   private void NoiseVariationsGUI()
   {
       switch (currentLayer.GenerationAlgorithm)
       {
           case GenericNoiseGenerationAlgorithm.Worley:
               Worley.Distance = (Worley.VORONOI_DISTANCE)EditorGUILayout.EnumPopup("Distance Setting",Worley.Distance);
               Worley.Combination = (Worley.VORONOI_COMBINATION)EditorGUILayout.EnumPopup("Combination Setting",Worley.Combination);
               break;
       }
   }
   private void DetailsGUI()
   {
       GUILayout.Space(20);
       GUILayout.BeginVertical(EditorStyles.helpBox,layersRect); //4
       GUILayout.BeginHorizontal(); //5
       GUILayout.Space(40);

       ////////[ALGORITHM LAYERS]///////////
       GUILayout.BeginHorizontal(EditorStyles.helpBox); //6
       GUILayout.Space(15);
       GUILayout.Label("Algorithm Layers");
       GUILayout.Space(15);
       GUILayout.EndHorizontal();//6
       ////////////////////////////////////

       var algorithmLayers = currentlyEditing.GetAlgorithmLayers();
       var layerCount = algorithmLayers.Count;
       for (int x = 0; x < layerCount; x++)
       {
           var layer = algorithmLayers[x];
       
           GUILayout.BeginVertical();//7
           GUILayout.Space(100);
           
           layerImageRect.position = new Vector2(23, (x * 30) + 60);
           GUI.Box(layerImageRect,proceduralImage);
           
           var layerStrength = Mathf.RoundToInt(layer.layerStrength * 100);
           layerStrength = EditorGUILayout.IntSlider(layerStrength, 0, 100);
           layer.SetLayerStrength(layerStrength / 100f);

           GUILayout.EndVertical();//7
       }

       GUILayout.EndHorizontal(); //5
       GUILayout.Space(100);
       GUILayout.EndVertical(); //4
   }
   void RenderImage()
   {
       var imageSize = proceduralImage.width;
       var noiseArray = GenerateGenericNoise(imageSize);
        var pixelsInPerlin = new Color[(int)Math.Pow(imageSize,2)];
        for (int y = 0; y < imageSize; y++)
        {
            for (int x = 0; x < imageSize; x++)
            {
                float sample = noiseArray[x, y];
                pixelsInPerlin[x+(y*imageSize)] = new Color(sample,sample,sample);
            }
        }
        proceduralImage.SetPixels(pixelsInPerlin);
        proceduralImage.Apply();
    }

   float[,] GenerateGenericNoise(int imageSize)
   {
       var noiseArray = new float[imageSize,imageSize];
       switch (currentLayer.GenerationAlgorithm)
       {
           case GenericNoiseGenerationAlgorithm.Perlin:
               noiseArray = Perlin.Generate(imageSize, currentLayer.Scale, currentLayer.XOffset, currentLayer.YOffset,
                   currentLayer.Amplitude,currentlyEditing.intSeed);
               break;
           case GenericNoiseGenerationAlgorithm.Worley:
               noiseArray = Worley.Generate(imageSize, currentLayer.Scale, currentLayer.XOffset, currentLayer.YOffset,
                   currentLayer.Amplitude,currentlyEditing.intSeed);
               break;
       }

       return noiseArray;
   }
    void Update()
    {
        RenderImage();
    }
}
#endif