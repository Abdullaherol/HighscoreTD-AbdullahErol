using UnityEngine;

public class GridNode : MonoBehaviour
{
    public TowerData Tower;

    [SerializeField] private Animator _animator;

    public void ShowPlaceAble()
    {
        if(Tower != null) return;

        _animator.gameObject.SetActive(true);
    }

    public void HidePlaceAble()
    {
        _animator.gameObject.SetActive(false);
    }

    public void SetTower(TowerData tower)
    {
        Tower = tower;

        _animator.gameObject.SetActive(false);
    }
}