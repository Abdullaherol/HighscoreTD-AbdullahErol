using System;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;
using Random = UnityEngine.Random;

public class EnemyManager : MonoBehaviour
{
    public static EnemyManager Instance;
    
    [SerializeField] private Vector3 _enemyPoolPosition;
    [SerializeField] private Vector3 _enemyDestination;
    [SerializeField] private float _startSpawnInterval;
    [SerializeField] private float _durationDecreaseRate;
    [SerializeField] private Vector2 _difficultyRange;
    [SerializeField] private float _difficultyMaxDuration;
    
    private float _currentEnemySpawnInterval;

    private DateTime _startTime;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    private void Start()
    {
        _startTime = DateTime.Now;
    }

    public float GetDecreaseRate()
    {
        return _durationDecreaseRate;
    }

    public float GetStartSpawnInterval()
    {
        return _startSpawnInterval;
    }
    
    public Vector3 GetEnemyPoolPosition()
    {
        return _enemyPoolPosition;
    }

    public Vector3 GetEnemyDestination()
    {
        return _enemyDestination;
    }

    public float GetCurrentEnemySpawnInterval()
    {
        return _currentEnemySpawnInterval;
    }

    public void SetCurrentEnemySpawnInterval(float interval)
    {
        _currentEnemySpawnInterval = interval;
        SaveManager.Instance.SaveGame();
    }

    public float GetDifficulty()
    {
        var seconds = GetDifficultyTime();

        var percentage = (float)seconds / _difficultyMaxDuration;

        return Mathf.Lerp(_difficultyRange.x, _difficultyRange.y, Mathf.Clamp(percentage,0,1));
    }

    public float GetDifficultyTime()
    {
        var seconds = (DateTime.Now - _startTime).TotalSeconds;

        return (float)seconds;
    }

    public void SetDifficultyTime(float time)
    {
        _startTime = DateTime.Now.AddSeconds(-time);
    }
}