using System;
using UnityEngine;
using UnityEngine.UI;

namespace Managers.Boost
{
    public class BoostManager : MonoSingleton<BoostManager>
    {
        public bool OnBoost;
        [SerializeField] private float _boostMultiplier;
        [SerializeField] private float _boostDuration;
        [SerializeField] private int _boostFillValue;

        [Space, SerializeField] private Image _boostImage;

        private float _currentTime;
        private int _currentBoost;

        private void Update()
        {
            BoostTimer();
        }

        private void BoostTimer()
        {
            if (!OnBoost) return;
            
            if (_currentTime < _boostDuration)
            {
                _currentTime += Time.deltaTime;

                _boostImage.fillAmount = 1 - (_currentTime / _boostDuration);
            }
            else
            {
                _currentTime = 0;
                _currentBoost = 0;
                OnBoost = false;
            }
        }

        public float GetBoostMultiplier()
        {
            return (OnBoost) ? _boostMultiplier : 1;
        }

        public void AddBoost()
        {
            if(OnBoost || _currentBoost >= _boostFillValue) return;
            
            _currentBoost++;
            UpdateBoostImage();
        }

        private void UpdateBoostImage()
        {
            var percentage = (float)_currentBoost / _boostFillValue;

            _boostImage.fillAmount = percentage;
        }

        public void ActivateBoost()
        {
            if(OnBoost || _currentBoost < _boostFillValue) return;

            OnBoost = true;
        }
    }
}