using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ParticleManager : MonoSingleton<ParticleManager>
{
    [SerializeField] private ParticleSystem _mortarParticle;
    [SerializeField] private ParticleSystem _mineParticle;

    [Space, SerializeField] private float _mortarDuration;
    [SerializeField] private float _mineDuration;

    private List<ParticleSystem> _mortarPool = new List<ParticleSystem>();
    private List<ParticleSystem> _minePool = new List<ParticleSystem>();

    public void PlaceMortarParticle(Vector3 position)
    {
        if (_mortarPool.Count > 0)
        {
            var last = _mortarPool.Last();
            _mortarPool.RemoveAt(_mortarPool.Count - 1);

            StartCoroutine(MortarTimer(last,position, _mortarDuration));
        }
        else
        {
            StartCoroutine(MortarTimer(Instantiate(_mortarParticle),position, _mortarDuration));
        }
    }

    public void PlaceMineParticle(Vector3 position)
    {
        if (_minePool.Count > 0)
        {
            var last = _minePool.Last();
            _minePool.RemoveAt(_minePool.Count - 1);

            StartCoroutine(MineTimer(last,position, _mineDuration));
        }
        else
        {
            StartCoroutine(MineTimer(Instantiate(_mineParticle),position, _mineDuration));
        }
    }

    private IEnumerator MortarTimer(ParticleSystem particle,Vector3 position, float duration)
    {
        PlayAndSetPositionParticle(particle, position);
        
        yield return new WaitForSeconds(duration);
        _mortarPool.Add(particle);
    }
    
    private IEnumerator MineTimer(ParticleSystem particle,Vector3 position, float duration)
    {
        PlayAndSetPositionParticle(particle, position);
        
        yield return new WaitForSeconds(duration);
        _minePool.Add(particle);
    }

    private void PlayAndSetPositionParticle(ParticleSystem particle,Vector3 position)
    {
        particle.transform.position = position;
        particle.Play();
        
        if (particle.TryGetComponent(out AudioSource audioSource))
        {
            audioSource.Play();
        }
    }
}