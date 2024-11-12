using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelLoader : MonoBehaviour
{
    private string levelsFileName = "Levels/pattern";
    private int levelToLoad = 1;

    //public GameObject[] tilePrefabs; //Blue, Red, Yellow, Green, Purple, Orange
    public Sprite[] tileSprites;

    public Image levelTarget;

    public GameObject tileMap3;
    public GameObject tileMap4;
    public GameObject tileMap5;

    public GameObject cycle3;
    public GameObject cycle4;
    public GameObject cycle5;

    private GameLevels allLevels;

    public LevelData levelData;

    private void Start()
    {
        Time.timeScale = 1f;
        SetLevelFileAndLevelToLoad();
        LoadAllLevels();
        levelData = GetLevelData(levelToLoad);

        SetCycleAndTileMapOn();

        if (levelData != null)
        {
            DisplayLevelData(levelData);
        }
        else
        {
            Debug.LogError("Level not found.");
        }
    }

    private void LoadAllLevels()
    {
        TextAsset jsonFile = Resources.Load<TextAsset>(levelsFileName);
        if (jsonFile == null)
        {
            Debug.LogError("Could not find file: " + levelsFileName);
            return;
        }

        allLevels = JsonUtility.FromJson<GameLevels>(jsonFile.text);
    }

    private LevelData GetLevelData(int levelNumber)
    {
        if (allLevels == null || allLevels.levels.Count == 0)
        {
            Debug.LogError("No levels loaded.");
            return null;
        }

        foreach (LevelData level in allLevels.levels)
        {
            if (level.level == levelNumber)
                return level;
        }

        Debug.LogError("Level " + levelNumber + " not found.");
        return null;
    }

    private void DisplayLevelData(LevelData levelData)
    {
        levelTarget.sprite = GetSprite(levelData.targetColor[0]);

        string objectName = "TileMapSize" + levelData.mapSize.ToString();

        for (int i = 0; i < levelData.mapSize; i++)
        {
            for (int j = 0; j < levelData.mapSize; j++)
            {
                string tileName = i.ToString() + "_" + j.ToString();
                GameObject target = GameObject.Find(objectName + "/" + tileName);

                if (target != null)
                {
                    target.GetComponent<SpriteRenderer>().sprite = GetSprite(levelData.levelMap[i][j]);
                }
                else
                {
                    Debug.LogWarning("Tile not found: " + tileName);
                }
            }
        }
    }

    public Sprite GetSprite(char spriteCode)
    {
        switch (spriteCode)
        {
            case '0':
                return tileSprites[0];
            case '1':
                return tileSprites[1];
            case '2':
                return tileSprites[2];
            case '3':
                return tileSprites[3];
            case '4':
                return tileSprites[4];
            case '5':
                return tileSprites[5];
            default:
                Debug.LogWarning("Unrecognized tile character at " + spriteCode);
                return tileSprites[0];
        }
    }

    private void SetCycleAndTileMapOn()
    {
        if (levelData.pattern == 3)
        {
            cycle3.SetActive(true);
        }
        else if (levelData.pattern == 4)
        {
            cycle4.SetActive(true);
        }
        else if (levelData.pattern == 5)
        {
            cycle5.SetActive(true);
        }

        if (levelData.mapSize == 3)
        {
            tileMap3.SetActive(true);
        }
        else if (levelData.mapSize == 4)
        {
            tileMap4.SetActive(true);
        }
        else if (levelData.mapSize == 5)
        {
            tileMap5.SetActive(true);
        }
    }

    private void SetLevelFileAndLevelToLoad()
    {
        int modePref = PlayerPrefs.GetInt("Mode");
        levelsFileName += modePref.ToString();

        if (modePref == 3)
        {
            levelToLoad = UserData.mode3Stage;
        }
        else if (modePref == 4)
        {
            levelToLoad = UserData.mode4Stage;
        }
        else if (modePref == 5)
        {
            levelToLoad = UserData.mode5Stage;
        }

        if (levelToLoad > 600)
        {
            levelToLoad = levelToLoad % 300 + 301;
        }
    }
}