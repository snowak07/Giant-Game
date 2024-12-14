using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosionHandler : MonoBehaviour
{
    public void Explode()
    {
        float explosionEmitterDuration = 0;
        float explosionParticleDuration = 0;
        ParticleSystem[] explosionEffects = GetComponentsInChildren<ParticleSystem>();
        foreach (ParticleSystem effect in explosionEffects)
        {
            if (explosionEmitterDuration < effect.main.duration)
            {
                explosionEmitterDuration = effect.main.duration;
            }
        
            if (explosionParticleDuration < effect.main.startLifetime.constantMax)
            {
                explosionParticleDuration = effect.main.startLifetime.constantMax;
            }
        
            Destroy(effect, effect.main.duration + effect.main.startLifetime.constantMax);
            effect.Play();
        }
        
        Destroy(gameObject, explosionEmitterDuration);
    }
}
