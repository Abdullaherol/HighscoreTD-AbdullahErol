using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using Managers.Tower;
using UI;
using UnityEngine;

namespace Managers.Grid
{
    public class GridManager : MonoSingleton<GridManager>, ITowerButtonClickListener
    {
        public bool OnTowerPlace;
        public bool TowerPlaced;
        public Vector3 PlaceTowerPosition;
        public TowerData SelectedTower;

        [SerializeField] [CanBeNull] private List<GridNode> _gridNodes;
        [SerializeField] private LayerMask _gridLayerMask;
        [SerializeField] private LayerMask _groundLayerMask;
        [SerializeField] private Vector3 _towerOffset;

        private void Start()
        {
            var towerUIController = GameUIManager.Instance.GetTowerUIController();
            towerUIController.TowerSubscribe(this);
        }

        public void OnTowerButtonClicked(TowerData tower)
        {
            OnTowerPlace = true;
            SelectedTower = tower;

            TowerPlaced = false;

            ShowPlaceAbleNodes();
        }

        private void ShowPlaceAbleNodes()
        {
            foreach (var gridNode in _gridNodes)
            {
                gridNode.ShowPlaceAble();
            }
        }

        private void HidePlaceAbleNodes()
        {
            foreach (var gridNode in _gridNodes)
            {
                gridNode.HidePlaceAble();
            }
        }

        private void Update()
        {
            MoveTower();
            PlaceTower();
        }

        private void MoveTower()
        {
            if (!OnTowerPlace || SelectedTower == null) return;

            if (Input.GetMouseButton(0))
            {
                if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out RaycastHit hit,
                        Mathf.Infinity,_groundLayerMask))
                {
                    PlaceTowerPosition = hit.point + _towerOffset;
                }

                if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition),
                        out RaycastHit gridHit,
                        Mathf.Infinity,
                        _gridLayerMask))
                {
                    if (gridHit.transform.TryGetComponent(out GridNode node))
                    {
                        PlaceTowerPosition = node.transform.position + _towerOffset;
                    }
                }
            }
        }

        private void PlaceTower()
        {
            if (!OnTowerPlace || SelectedTower == null) return;

            if (Input.GetMouseButtonUp(0))
            {
                if (!Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition),
                        out RaycastHit hit,
                        Mathf.Infinity,
                        _gridLayerMask)) return;

                if (!hit.transform.TryGetComponent(out GridNode gridNode)) return;

                if (gridNode.Tower != null) return;

                PlaceTowerPosition = gridNode.transform.position + _towerOffset;

                gridNode.SetTower(SelectedTower);

                GameManager.Instance.TowerPlaced();

                OnTowerPlace = false;
                SelectedTower = null;

                HidePlaceAbleNodes();
                SaveManager.Instance.SaveGame();
            }
        }

        public Vector3 GetGridNodePositionByIndex(int index)
        {
            return _gridNodes[index].transform.position + _towerOffset;
        }

        public List<int> GetTowersSaveData()
        {
            var list = new List<int>();

            foreach (var gridNode in _gridNodes)
            {
                if (gridNode.Tower == null)
                {
                    list.Add(-1);
                }
                else
                {
                    list.Add((int)gridNode.Tower.Type);
                }
            }

            return list;
        }

        public void SetTowersDataFromSaveData(List<int> data)
        {
            for (var i = 0; i < _gridNodes.Count; i++)
            {
                var saveType = data[i];
                
                if(saveType == -1) continue;
                
                var node = _gridNodes[i];
                node.Tower = TowerManager.Instance.GetTowerDataByType(saveType);
            }
        }
    }
}