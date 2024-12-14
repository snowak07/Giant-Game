using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleEffectDestroy : MonoBehaviour
{
    private float startTime;
    private float totalDuration;

    // Start is called before the first frame update
    void Start()
    {
        ParticleSystem parts = GetComponent<ParticleSystem>();
        totalDuration = parts.main.duration;
        startTime = Time.time;
        Destroy(gameObject, startTime + totalDuration);
    }
}
