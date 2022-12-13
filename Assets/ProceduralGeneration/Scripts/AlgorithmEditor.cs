using System;
using System.Collections.Generic;
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

    private static int imageViewMode;

    private static GenerationAlgorithm currentlyEditing;
    private static AlgorithmLayer currentLayer;
    private static bool updateNoise = false;

    GUILayoutOption[] layersRect = new GUILayoutOption[] {GUILayout.Width(240f), GUILayout.Height(layersRectHeight) };
    [MenuItem("Tools / Noise Factory")]
   
   public static void ShowWindow()
   {
       editorWindow = GetWindow<AlgorithmEditor>("Noise Factory");
       editorWindow.maxSize = new Vector2(600, 600);
       editorWindow.minSize = new Vector2(600, 600);
       proceduralImage = new Texture2D(300, 300);
       
       var xPosition = editorWindow.maxSize.x - proceduralImage.width - 20;
       mainImageRect = new Rect(xPosition, 30, proceduralImage.width, proceduralImage.height);
       layerImageRect = new Rect(0,0,80,80);
       currentlyEditing = CreateInstance<GenerationAlgorithm>();
       currentLayer = currentlyEditing._algorithmLayers[0];
   }

   private void Update()
   {
       updateNoise = false;
   }
   private void OnGUI()
   {
       Repaint();

       ////////[SEED [____]]///////////
       GUILayout.BeginHorizontal(EditorStyles.helpBox);
       currentlyEditing.seed = EditorGUILayout.TextField("Seed", currentlyEditing.seed, GUIStyle.none);
       if (updateNoise)
           currentlyEditing.SetIntSeed();

       GUILayout.EndHorizontal();
       ///////////////////////////////////
       
       GUILayout.BeginHorizontal();//1
       GUILayout.EndHorizontal();//1
       GUILayout.BeginHorizontal();//3

       DetailsGUI();
       ImageArea();
       
       GUILayout.EndHorizontal();//3
   }
   
   private void ImageArea()
   {
       GUILayout.BeginVertical(); //4     
       if (updateNoise)
       {
           var noise = imageViewMode == 0
               ? NoiseFactory.GenerateLayeredNoise((int)mainImageRect.width, currentlyEditing)
               : NoiseFactory.GenerateGenericNoise((int)mainImageRect.width, currentlyEditing, currentLayer);

           proceduralImage.SetPixels(GetPixelsFromNoiseArray((int)mainImageRect.width, noise));
           proceduralImage.Apply();
       }

       GUI.Box(mainImageRect,proceduralImage);
       GUILayout.Space(310);
       ImageGUI();
       GUILayout.EndVertical();
   }
   
   private void ImageGUI()
   {
       var startingSettings = new [] {(int)currentLayer.GenerationAlgorithm,currentLayer.Scale,currentLayer.XOffset,currentLayer.YOffset,currentLayer.Amplitude};
       var genType = (GenericNoiseGenerationAlgorithm)EditorGUILayout.EnumPopup("Generation Algorithm", currentLayer.GenerationAlgorithm);
       var scale = EditorGUILayout.FloatField("Scale",currentLayer.Scale);
       var xOffset = EditorGUILayout.FloatField("Coordinate X",currentLayer.XOffset);
       var yOffset = EditorGUILayout.FloatField("Coordinate Y",currentLayer.YOffset);
       var amplitude = Math.Clamp(EditorGUILayout.FloatField("Amplitude",currentLayer.Amplitude),0,1);

       var postEditSettings = new [] {(int)currentLayer.GenerationAlgorithm,currentLayer.Scale,currentLayer.XOffset,currentLayer.YOffset,currentLayer.Amplitude};
       if (postEditSettings != startingSettings)
           updateNoise = true;
       
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
       GUILayout.BeginVertical(EditorStyles.helpBox,layersRect); //4
       ////////[ALGORITHM LAYERS]///////////
       GUILayout.BeginHorizontal(EditorStyles.helpBox); //6
       GUILayout.Space(15);
       GUILayout.Label("Algorithm Layers");
       GUILayout.Space(15);
       GUILayout.EndHorizontal();//6
       ////////////////////////////////////
       
       var algorithmLayers = currentlyEditing.GetAlgorithmLayers();
       var startingLayers = new List<AlgorithmLayer>(algorithmLayers);
       var layerCount = algorithmLayers.Count;
       for (int x = 0; x < layerCount; x++)
       {
           var layer = algorithmLayers[x];
           var layerNoiseArray = NoiseFactory.GenerateGenericNoise((int)layerImageRect.width,currentlyEditing,layer);
           var pixels = GetPixelsFromNoiseArray((int)layerImageRect.width,layerNoiseArray);
           layerImageRect.position = new Vector2(15, (x * 82) + 60);
           var layerImage = new Texture2D(80,80);
           layerImage.SetPixels(pixels);
           layerImage.Apply();
           GUI.Box(layerImageRect,layerImage);
           var rectScreenPosition = layerImageRect.position + editorWindow.position.position;
           var mousePosition = GUIUtility.GUIToScreenPoint(Event.current.mousePosition);
           var mouseInArea = mousePosition.x > rectScreenPosition.x
                             && mousePosition.x < rectScreenPosition.x + layerImage.width
                             && mousePosition.y > rectScreenPosition.y
                             && mousePosition.y < rectScreenPosition.y + layerImage.height;
           if (mouseInArea || currentLayer == layer)
           {
               GUILayout.BeginVertical(EditorStyles.helpBox);
               if (Input.GetMouseButton(0))
                   currentLayer = layer;
           }
           else
               GUILayout.BeginVertical();
           GUILayout.Space(60);
           
           GUILayout.BeginHorizontal();
           GUILayout.Space(100);

           var layerStrength = Mathf.RoundToInt(layer.layerStrength * 100);
           GUILayoutOption[] layerSlider = new GUILayoutOption[] {GUILayout.Width(120f)};
           layerStrength = EditorGUILayout.IntSlider(layerStrength, 0, 100,layerSlider);
           layer.SetLayerStrength(layerStrength / 100f);
           GUILayout.EndHorizontal();
           GUILayout.EndVertical();
       }

       if (startingLayers != algorithmLayers)
           updateNoise = true;

       GUILayout.Space(60);

       if (GUILayout.Button("Add Layer"))
           currentlyEditing.AddAlgorithmLayer(new AlgorithmLayer());

       GUILayout.EndVertical(); //4
   }

   Color[] GetPixelsFromNoiseArray(int imageSize,float[,] noiseArray)
   {
       var pixelsInArray = new Color[(int)Math.Pow(imageSize, 2)];
       for (var y = 0; y < imageSize; y++)
       for (var x = 0; x < imageSize; x++)
       {
           var sample = noiseArray[x, y];
           pixelsInArray[x+(y*imageSize)] = new Color(sample,sample,sample);
       }
       return pixelsInArray;
   }
}
#endif