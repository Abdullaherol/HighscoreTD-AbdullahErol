using System;
using System.Collections.Generic;
using UnityEngine;

namespace UI
{
    public class GameUIManager : MonoSingleton<GameUIManager>
    {
        [SerializeField] private TowerUIController _towerUIController;

        public TowerUIController GetTowerUIController()
        {
            return _towerUIController;
        }
    }
}