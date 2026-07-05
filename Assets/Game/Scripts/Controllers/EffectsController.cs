using UnityEngine;

public class EffectsController : MonoBehaviour
{
    [Header("Explosion")]
    
    [SerializeField] private ParticleSystem _explosionEffect;

    public void PlayExplosionEffect()
    {
        if (!_explosionEffect)
            return;
        
        _explosionEffect.Play();
    }
}
