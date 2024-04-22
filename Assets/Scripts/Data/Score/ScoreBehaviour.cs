using UnityEngine;

public class ScoreBehaviour : MonoBehaviour
{
    [SerializeField] private int _eachEnemyScore;
    [SerializeField] private TMPro.TextMeshProUGUI _scoreTxt;

    [Space, SerializeField] private Animator _highScorePanel;
    [SerializeField] private TMPro.TextMeshProUGUI _highScoreTxt;
    [SerializeField] private TMPro.TextMeshProUGUI _highScorePanelCurrentScoreTxt;

    private int _score;
    private int _highScore;

    public void AddScore()
    {
        _score += _eachEnemyScore;
        
        if (_highScore < _score)
        {
            _highScore = _score;
        }
        UpdateScoreTxt();
        SaveManager.Instance.SaveGame();
    }

    public void SetScore(int score)
    {
        if (_highScore < score)
        {
            _highScore = score;
        }

        _score = score;
        UpdateScoreTxt();
    }

    public void SetHighScore(int highScore)
    {
        _highScore = highScore;
    }

    public int GetHighScore()
    {
        return _highScore;
    }

    public int GetScore()
    {
        return _score;
    }

    public void ShowHighScore()
    {
        _highScorePanel.SetBool("Show", true);
        _highScoreTxt.text = "HighScore: " + _highScore;
        _highScorePanelCurrentScoreTxt.text = "Score: " + _score;
    }

    private void UpdateScoreTxt()
    {
        _scoreTxt.text = _score.ToString();
    }
}