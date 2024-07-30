using UnityEngine;

public class EffectsController : MonoBehaviour
{
    [field: Header("Explosion")]
    
    [field: SerializeField]
    public ParticleSystem ExplosionEffect { get; private set; }

    public void PlayExplosionEffect()
    {
        if (!ExplosionEffect) return;
        
        ExplosionEffect.Play();
    }
}
