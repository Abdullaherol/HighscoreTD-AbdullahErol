using System;
using System.Collections.Generic;
using Managers.Grid;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

namespace Managers.Tower
{
    public class TowerManager : MonoSingleton<TowerManager>
    {
        [SerializeField] private Vector3 _bulletPoolPosition;
        [SerializeField] private Vector3 _baseTowerPosition;
        
        [SerializeField] private List<TowerData> _towers;

        public TowerData GetTowerDataByType(int type)
        {
            return _towers[type];
        }

        public Vector3 GetBulletPoolPosition()
        {
            return _bulletPoolPosition;
        }

        public Vector3 GetBaseTowerPosition()
        {
            return _baseTowerPosition;
        }
    }
}