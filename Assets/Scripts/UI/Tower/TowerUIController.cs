using System;
using System.Collections.Generic;
using UI.Tower;
using Unity.VisualScripting;
using UnityEngine;

public class TowerUIController : MonoBehaviour
{
    [SerializeField] private List<TowerBuyButton> _towerBuyButtons;
    
    private List<ITowerButtonClickListener> _towerListeners;

    private void Awake()
    {
        _towerListeners = new List<ITowerButtonClickListener>();
    }

    public void TowerSubscribe(ITowerButtonClickListener listener)
    {
        if (!_towerListeners.Contains(listener))
        {
            _towerListeners.Add(listener);
        }
    }

    public void TowerUnSubscribe(ITowerButtonClickListener listener)
    {
        if (_towerListeners.Contains(listener))
        {
            _towerListeners.Remove(listener);
        }
    }

    public void TowerNotify(TowerData tower)
    {
        foreach (var towerListener in _towerListeners)
        {
            towerListener.OnTowerButtonClicked(tower);
        }
    }

    public List<int> GetTowerPricesSaveData()
    {
        var list = new List<int>();

        foreach (var button in _towerBuyButtons)
        {
            list.Add(button.GetPrice());
        }

        return list;
    }

    public void SetTowerPricesFromSaveData(List<int> data)
    {
        for (int i = 0; i < _towerBuyButtons.Count; i++)
        {
            var button = _towerBuyButtons[i];
            
            button.SetPrice(data[i]);
        }
    }
}