using System;
using System.Collections.Generic;
using UnityEngine;

namespace Money
{
    public class MoneyBehaviour : MonoBehaviour
    {
        [SerializeField] private TMPro.TextMeshProUGUI _moneyText;
        [SerializeField] private int _startMoney;
        [SerializeField] private int _eachEnemyMoney;
        
        private int _money;
        private List<IMoneyChangeListener> _listeners;

        private void Awake()
        {
            _listeners = new List<IMoneyChangeListener>();
        }

        private void Start()
        {
            _money = _startMoney;
            Notify();
        }

        public void Subscribe(IMoneyChangeListener listener)
        {
            if (!_listeners.Contains(listener))
            {
                _listeners.Add(listener);
            }
        }

        public void UnSubscribe(IMoneyChangeListener listener)
        {
            if (_listeners.Contains(listener))
            {
                _listeners.Remove(listener);
            }
        }

        private void Notify()
        {
            foreach (var listener in _listeners)
            {
                listener.OnMoneyChanged(_money);
            }

            UpdateMoneyText();
        }

        public void AddMoney(int amount)
        {
            _money += amount;
            Notify();
        }

        public void AddMoney()
        {
            _money += _eachEnemyMoney;
            Notify();
        }

        public void RemoveMoney(int amount)
        {
            _money -= amount;
            Notify();
        }

        public void SetMoney(int money)
        {
            _money = money;
            Notify();
        }

        public int GetMoney()
        {
            return _money;
        }

        public bool HasMoney(int price)
        {
            return _money >= price;
        }

        private void UpdateMoneyText()
        {
            _moneyText.text = _money + "G";
        }
    }
}