using System;
using Managers.Grid;
using Managers.Save;
using Money;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace UI.Tower
{
    public class TowerBuyButton : MonoBehaviour, IMoneyChangeListener,IPointerDownHandler
    {
        [SerializeField] private TowerData _tower;

        [Space, SerializeField] private TMPro.TextMeshProUGUI _priceTxt;
        [SerializeField] private Image _towerImage;
        
        [SerializeField] private Color _activeColor;
        [SerializeField] private Color _passiveColor;

        private int _currentPrice;

        private MoneyBehaviour _moneyBehaviour;

        private void Start()
        {
            _currentPrice = _tower.Price;

            _towerImage.sprite = _tower.Image;

            _moneyBehaviour = DataManager.Instance.GetMoneyBehaviour();
            _moneyBehaviour.Subscribe(this);
            
            UpdatePriceTxt();
        }

        public void OnMoneyChanged(int money)
        {
            UpdateTxt(money);
        }

        private void UpdateTxt(int money)
        {
            bool active = _currentPrice <= money;

            _priceTxt.color = (active) ? _activeColor : _passiveColor;
        }

        private void UpdatePrice()
        {
            _currentPrice += _tower.PriceIncreaseRate;
            
            SaveManager.Instance.SaveGame();

            UpdatePriceTxt();
        }

        private void UpdatePriceTxt()
        {
            _priceTxt.text = _currentPrice.ToString();
        }

        public void ClickButton()
        {
            if(GridManager.Instance.OnTowerPlace) return;
            
            int price = _currentPrice;

            if(!_moneyBehaviour.HasMoney(price)) return;
            
            UpdatePrice();
            _moneyBehaviour.RemoveMoney(price);
            
            GameUIManager.Instance.GetTowerUIController().TowerNotify(_tower);
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            ClickButton();
        }

        public int GetPrice()
        {
            return _currentPrice;
        }

        public void SetPrice(int price)
        {
            _currentPrice = price;
            UpdatePriceTxt();
        }
    }
}