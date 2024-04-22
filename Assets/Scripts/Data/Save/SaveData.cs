using System.Collections.Generic;

[System.Serializable]
public class SaveData
{
    public List<int> Towers = new List<int>();
    public List<int> TowerPrices = new List<int>();
    public int Money;
    public int Score;
    public int HighScore;
    public float Difficulty;
    public float EnemySpawnInterval;
}