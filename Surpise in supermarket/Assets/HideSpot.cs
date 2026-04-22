using UnityEngine;

public class HideSpot : MonoBehaviour
{
    public Transform enterPoint;
    public Transform hidePoint;

    public AudioSource hideAudioSource;
    public AudioClip hideEnterClip;
    public AudioClip hideExitClip;
    public float hideSoundVolume = 1f;

    [HideInInspector] public bool playerInRange = false;

    public void PlayEnterSound()
    {
        if (hideAudioSource != null && hideEnterClip != null)
        {
            hideAudioSource.PlayOneShot(hideEnterClip, hideSoundVolume);
        }
    }

    public void PlayExitSound()
    {
        if (hideAudioSource != null && hideExitClip != null)
        {
            hideAudioSource.PlayOneShot(hideExitClip, hideSoundVolume);
        }
    }
}