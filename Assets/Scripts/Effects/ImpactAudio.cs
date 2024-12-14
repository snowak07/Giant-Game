using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImpactAudio : MonoBehaviour
{
    private bool audioCooldown = false;

    private void OnCollisionEnter(Collision collision)
    {
        if (!audioCooldown)
        {
            StartCoroutine(playImpactSound());
        }
    }

    IEnumerator playImpactSound()
    {
        if (!audioCooldown)
        {
            AudioSource audio = GetComponent<AudioSource>();
            audio.Play();
            audioCooldown = true;
            yield return new WaitForSeconds(audio.clip.length);
            audioCooldown = false;
        }
    }
}
