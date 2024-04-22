using System;
using System.Collections.Generic;
using Managers.Boost;
using Managers.Grid;
using Money;
using UI;
using Unity.Scenes;
using UnityEngine;

namespace Managers.Save
{
    public class DataManager : MonoSingleton<DataManager>
    {
        [SerializeField] private Animator _loadSavePanel;

        [SerializeField] private MoneyBehaviour _moneyBehaviour;
        [SerializeField] private ScoreBehaviour _scoreBehaviour;

        private void Start()
        {
            ShowLoadSavePanel();
        }

        public void GetSaveData()
        {
            var saveData = SaveManager.Instance.SaveData;

            _moneyBehaviour.SetMoney(saveData.Money);
            _scoreBehaviour.SetScore(saveData.Score);
            _scoreBehaviour.SetHighScore(saveData.HighScore);
            
            GridManager.Instance.SetTowersDataFromSaveData(saveData.Towers);
            GameUIManager.Instance.GetTowerUIController().SetTowerPricesFromSaveData(saveData.TowerPrices);
            
            EnemyManager.Instance.SetDifficultyTime(saveData.Difficulty);

            _loadSavePanel.SetBool("Show", false);
            
            GameManager.Instance.IsPlaying = true;
            GameManager.Instance.FirstTowerPlaced = true;
        }

        public void ContinueWithoutSaveData()
        {
            SaveManager.Instance.SaveData = null;
            _loadSavePanel.SetBool("Show", false);
            
            GameManager.Instance.IsPlaying = true;
        }

        private void ShowLoadSavePanel()
        {
            if (SaveManager.Instance == null ||
                SaveManager.Instance.SaveData == null) return;

            GameManager.Instance.IsPlaying = false;

            _loadSavePanel.SetBool("Show", true);
        }

        public MoneyBehaviour GetMoneyBehaviour()
        {
            return _moneyBehaviour;
        }

        public ScoreBehaviour GetScoreBehaviour()
        {
            return _scoreBehaviour;
        }

        public void AddMoneyAndScore()
        {
            _moneyBehaviour.AddMoney();
            _scoreBehaviour.AddScore();

            BoostManager.Instance.AddBoost();
            SaveManager.Instance.SaveGame();
        }
    }
}