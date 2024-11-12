using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class LevelData
{
    // level number
    public int level;

    // tile size
    public int mapSize;

    // tile cycle
    public int pattern;

    // goal color
    public string targetColor;

    // level pattern
    public string[] levelMap;

    // steps
    public int steps;

    // solution
    public string solution;
}

[Serializable]
public class GameLevels
{
    public List<LevelData> levels = new List<LevelData>();
}