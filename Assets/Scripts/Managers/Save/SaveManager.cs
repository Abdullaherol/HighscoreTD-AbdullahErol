using System;
using Firebase.Auth;
using Managers.Grid;
using Managers.Save;
using Newtonsoft.Json;
using UI;
using UnityEngine;

public class SaveManager : MonoBehaviour
{
    public static SaveManager Instance;

    public SaveData SaveData;

    private FirebaseHelper _firebaseHelper;

    private FirebaseUser _firebaseUser;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }

        SaveData = null;
    }

    private void Start()
    {
        _firebaseHelper = new FirebaseHelper();
        _firebaseHelper.SignInAnonymously(UserSignIn);
    }

    private void UserSignIn(FirebaseUser user)
    {
        _firebaseUser = user;
        _firebaseHelper.GetUserSaveData(user.UserId, OnDataReceived, OnFailure);
    }

    private void OnDataReceived(string obj)
    {
        SaveData = JsonConvert.DeserializeObject<SaveData>(obj);
        Debug.Log("Save received from FireBase");
    }

    private void OnFailure()
    {
        UnityEngine.Debug.Log("Getting save data failure");
    }

    public void SaveGame()
    {
        var json = GetSaveDataAsJson();

        _firebaseHelper.SaveUserData(_firebaseUser.UserId, json, OnSaveSuccess, OnSaveFailure);
    }

    private void OnSaveFailure(string obj)
    {
        Debug.Log("Save failure");
    }

    private void OnSaveSuccess()
    {
        
    }

    private string GetSaveDataAsJson()
    {
        var saveData = new SaveData()
        {
            Towers = GridManager.Instance.GetTowersSaveData(),
            TowerPrices = GameUIManager.Instance.GetTowerUIController().GetTowerPricesSaveData(),
            Money = DataManager.Instance.GetMoneyBehaviour().GetMoney(),
            Score = DataManager.Instance.GetScoreBehaviour().GetScore(),
            EnemySpawnInterval = EnemyManager.Instance.GetCurrentEnemySpawnInterval(),
            HighScore = DataManager.Instance.GetScoreBehaviour().GetHighScore(),
            Difficulty = EnemyManager.Instance.GetDifficultyTime()
        };

        var json = JsonConvert.SerializeObject(saveData);

        return json;
    }
}