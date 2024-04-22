using UnityEngine;

namespace UI.HighScore
{
    public class HighScorePanelBehaviour : MonoBehaviour
    {
        public void RestartGame()
        {
            GameManager.Instance.RestartGame();
        }
    }
}