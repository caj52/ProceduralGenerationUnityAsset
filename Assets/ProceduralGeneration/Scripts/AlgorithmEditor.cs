using System;
using System.Collections.Generic;
using System.Linq;
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
    private static int imageViewMode = 1;

    private static NoiseAlgorithm currentlyEditing;
    private static AlgorithmLayer currentLayer;

    private static float refreshTimer = .1f;
    private static float lastImageRefresh;

    private bool algorithmFoldout;
    private int selected = 0;

    GUILayoutOption[] layersRect = new GUILayoutOption[] {GUILayout.Width(240f), GUILayout.Height(layersRectHeight) };
    GUILayoutOption[] deleteLayer = new GUILayoutOption[] {GUILayout.Width(20f), GUILayout.Height(20) };

    [MenuItem("Tools / Noise Factory")]
   
   public static void ShowWindow()
   {
       editorWindow = GetWindow<AlgorithmEditor>("Noise Factory");
       editorWindow.maxSize = new Vector2(600, 600);
       editorWindow.minSize = new Vector2(600, 600);
       proceduralImage = new Texture2D(300, 300);
       
       var xPosition = editorWindow.maxSize.x - proceduralImage.width - 20;
       mainImageRect = new Rect(xPosition, 30, proceduralImage.width, proceduralImage.height);
       layerImageRect = new Rect(0,0,100,100);
       currentlyEditing = CreateInstance<NoiseAlgorithm>();
       currentLayer = currentlyEditing._algorithmLayers[0];
   }

   private void OnGUI()
   {
       Repaint();
       
       ////////[SEED [____]]///////////
       GUILayout.BeginHorizontal(EditorStyles.helpBox);
 
       var noiseAlgorithmsInProject = GetAllNoiseAlgorithmsInProject();
       var namesList = noiseAlgorithmsInProject.Select(noiseAlgorithm => noiseAlgorithm.name).ToList();
       namesList.Add("New Algorithm");
       selected = EditorGUILayout.Popup("", selected, namesList.ToArray(), new []{GUILayout.Width(20)});

       if (selected == noiseAlgorithmsInProject.Count)
       {
           var amountOfNewNames = namesList.Count(n => n.Contains("New Noise Algorithm"));
           AssetDatabase.CreateAsset(CreateInstance<NoiseAlgorithm>(), $"Assets/New Noise Algorithm {amountOfNewNames}.asset");
           GUILayout.EndHorizontal();
           return;
       }

       currentlyEditing = noiseAlgorithmsInProject[selected];
       currentlyEditing.name = EditorGUILayout.TextArea(currentlyEditing.name);
       currentlyEditing.seed = EditorGUILayout.TextField("Seed", currentlyEditing.seed, GUIStyle.none);
       currentlyEditing.SetIntSeed();

       if (GUILayout.Button($"Switch View"))
           imageViewMode = (imageViewMode == 0) ? 1 : 0;
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

       if (Time.time - refreshTimer > lastImageRefresh)
       {
           var noise = (imageViewMode == 0 || currentLayer == null)
               ? NoiseFactory.GenerateLayeredNoise((int)mainImageRect.width, currentlyEditing)
               : NoiseFactory.GenerateGenericNoise((int)mainImageRect.width, currentlyEditing, currentLayer);
           proceduralImage.SetPixels(GetPixelsFromNoiseArray((int)mainImageRect.width, noise));
           proceduralImage.Apply();

           lastImageRefresh = Time.time;
       }

       GUI.Box(mainImageRect,proceduralImage);
       GUILayout.Space(310);
       ImageGUI();
       GUILayout.EndVertical();
   }
   
   private void ImageGUI()
   {
       var genType = GenericNoiseGenerationAlgorithm.Perlin;
       var scale = 1f;
       var xOffset = 0f;
       var yOffset = 0f;
       var amplitude = 1f;
       
       if (currentLayer == null)
       { }
       else
       {
           genType =
               (GenericNoiseGenerationAlgorithm)EditorGUILayout.EnumPopup("Generation Algorithm", currentLayer.GenerationAlgorithm);
           scale = EditorGUILayout.FloatField("Scale", currentLayer.Scale);
           xOffset = EditorGUILayout.FloatField("Coordinate X", currentLayer.XOffset);
           yOffset = EditorGUILayout.FloatField("Coordinate Y", currentLayer.YOffset);
           amplitude = Math.Clamp(EditorGUILayout.FloatField("Amplitude", currentLayer.Amplitude), 0, 1);
       }

       if (currentLayer == null)
           return;
       
       NoiseVariationsGUI();
       currentLayer.SetAlgorithmVariables(genType, scale, xOffset, yOffset, amplitude);
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
   List<NoiseAlgorithm> GetAllNoiseAlgorithmsInProject()
   {
       return AssetDatabase.FindAssets($"t: {nameof(NoiseAlgorithm)}").ToList()
           .Select(AssetDatabase.GUIDToAssetPath)
           .Select(AssetDatabase.LoadAssetAtPath<NoiseAlgorithm>)
           .ToList();
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
       var layerCount = algorithmLayers.Count;
       for (int x = 0; x < layerCount; x++)
       {
           layerImageRect.position = new Vector2(15, (x * 102) + 60);

           var layer = algorithmLayers[x];
           var layerNoiseArray = NoiseFactory.GenerateGenericNoise((int)layerImageRect.width,currentlyEditing,layer);
           var pixels = GetPixelsFromNoiseArray((int)layerImageRect.width,layerNoiseArray);
           var layerImage = new Texture2D(100,100);
           var rectScreenPosition = layerImageRect.position + editorWindow.position.position;
           var mousePosition = GUIUtility.GUIToScreenPoint(Event.current.mousePosition);
           var mouseInArea = mousePosition.x > rectScreenPosition.x
                             && mousePosition.x < rectScreenPosition.x + layerImage.width
                             && mousePosition.y > rectScreenPosition.y
                             && mousePosition.y < rectScreenPosition.y + layerImage.height;
           
           layerImage.SetPixels(pixels);
           layerImage.Apply();
           GUI.Box(layerImageRect,layerImage);

           if (mouseInArea || currentLayer == layer)
           {
               GUILayout.BeginVertical(EditorStyles.helpBox);
               if (mouseInArea && Event.current.type == EventType.MouseUp)
                   currentLayer = (currentLayer == layer)? null : layer;
           }
           else
               GUILayout.BeginVertical();
           
           GUILayout.BeginHorizontal();
           GUILayout.Space(220);
           if (GUILayout.Button("X",deleteLayer))
           {
               algorithmLayers.RemoveAt(x);
               GUILayout.EndHorizontal();
               GUILayout.EndVertical();
               GUILayout.EndVertical();
               return;
           }
           GUILayout.EndHorizontal();
           
           GUILayout.Space(60);
           GUILayout.BeginHorizontal();
           GUILayout.Space(120);

           var layerStrength = Mathf.RoundToInt(layer.layerStrength * 100);
           GUILayoutOption[] layerSlider = new GUILayoutOption[] {GUILayout.Width(120f)};
           layerStrength = EditorGUILayout.IntSlider(layerStrength, 0, 100,layerSlider);
           layer.SetLayerStrength(layerStrength / 100f);

           GUILayout.EndHorizontal();
           GUILayout.EndVertical();
       }

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