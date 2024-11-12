//using UnityEditor;
//using UnityEngine;
//using System.Collections.Generic;
//using System.IO;

//public class LevelEditorWindow : EditorWindow
//{
//    private int levelWidth = 10;
//    private int levelHeight = 10;
//    private GameObject selectedPrefab;
//    private List<GameObject> levelObjects = new List<GameObject>();

//    [MenuItem("Window/Level Editor")]
//    public static void ShowWindow()
//    {
//        GetWindow<LevelEditorWindow>("Level Editor");
//    }

//    private void OnGUI()
//    {
//        GUILayout.Label("Level Editor", EditorStyles.boldLabel);

//        levelWidth = EditorGUILayout.IntField("Level Width", levelWidth);
//        levelHeight = EditorGUILayout.IntField("Level Height", levelHeight);

//        selectedPrefab = (GameObject)EditorGUILayout.ObjectField("Select Object to Place", selectedPrefab, typeof(GameObject), false);

//        if (GUILayout.Button("Place Object"))
//            PlaceObject();

//        if (GUILayout.Button("Save Level"))
//            SaveLevel();

//        if (GUILayout.Button("Load Level"))
//            LoadLevel();
//    }

//    private void PlaceObject()
//    {
//        if (selectedPrefab != null)
//        {
//            GameObject obj = (GameObject)PrefabUtility.InstantiatePrefab(selectedPrefab);
//            obj.transform.position = Vector3.zero;
//            levelObjects.Add(obj);
//        }
//    }

//    private void SaveLevel()
//    {
//        LevelData levelData = new LevelData
//        {
//            levelWidth = levelWidth,
//            levelHeight = levelHeight
//        };

//        foreach (GameObject obj in levelObjects)
//        {
//            LevelData.ObjectData objectData = new LevelData.ObjectData
//            {
//                prefabName = obj.name.Replace("(Clone)", ""),
//                position = obj.transform.position,
//                rotation = obj.transform.rotation
//            };
//            levelData.objects.Add(objectData);
//        }

//        string json = JsonUtility.ToJson(levelData, true);
//        File.WriteAllText("Assets/Levels/level.json", json);
//        Debug.Log("Level saved.");
//    }

//    private void LoadLevel()
//    {
//        if (!File.Exists("Assets/Levels/level.json"))
//        {
//            Debug.LogError("No level file found.");
//            return;
//        }

//        string json = File.ReadAllText("Assets/Levels/level.json");
//        LevelData levelData = JsonUtility.FromJson<LevelData>(json);

//        // Clear existing level objects
//        foreach (GameObject obj in levelObjects)
//            DestroyImmediate(obj);
//        levelObjects.Clear();

//        // Recreate objects from loaded data
//        foreach (LevelData.ObjectData objectData in levelData.objects)
//        {
//            GameObject prefab = Resources.Load<GameObject>(objectData.prefabName);
//            if (prefab != null)
//            {
//                GameObject obj = (GameObject)PrefabUtility.InstantiatePrefab(prefab);
//                obj.transform.position = objectData.position;
//                obj.transform.rotation = objectData.rotation;
//                levelObjects.Add(obj);
//            }
//            else
//            {
//                Debug.LogError("Prefab not found for " + objectData.prefabName);
//            }
//        }

//        Debug.Log("Level loaded.");
//    }
//}