using Managers.Save;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public bool IsPlaying;
    public bool FirstTowerPlaced;
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        
        DontDestroyOnLoad(transform.parent.gameObject);
    }

    public void StarGame()
    {
        IsPlaying = true;
        SceneManager.LoadScene("Game");
    }

    public void RestartGame()
    {
        FirstTowerPlaced = false;
        IsPlaying = true;
        SceneManager.LoadScene("MainMenu");
    }

    public void GameOver()
    {
        if(!IsPlaying) return;
        
        IsPlaying = false;
        
        DataManager.Instance.GetScoreBehaviour().ShowHighScore();
    }

    public void TowerPlaced()
    {
        FirstTowerPlaced = true;
    }
}