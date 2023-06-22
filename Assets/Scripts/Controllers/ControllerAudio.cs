using System;
using UnityEngine;
using UnityEngine.Serialization;

public class ControllerAudio : MonoBehaviour
{
    [Header("Walking")]
    [SerializeField] private AudioClipName footStepSound = AudioClipName.Footstep;
    [SerializeField] [Range(0,1f)] private float footStepVolume = 0.6f;
    [Header("Climbing")]
    [SerializeField] private AudioClipName climbingSound = AudioClipName.Footstep;
    [SerializeField] [Range(0,1f)] private float climbingVolume = 0.6f;
    [Header("Obstacle Bump")]
    [SerializeField] private AudioClipName obstacleBump = AudioClipName.WallBump;
    [SerializeField] [Range(0,1f)] private float obstacleBumpVolume = 0.6f;
    [Header("Falling")]
    [SerializeField] private AudioClipName landingSound = AudioClipName.Footstep;
    [SerializeField] [Range(0,1f)] private float landingVolume = 0.6f;
    [SerializeField] private AudioClipName fallYellSound = AudioClipName.None;
    [SerializeField] [Range(0,1f)] private float fallYellVolume = 0.6f;

    private bool doFallYell = true;

    public void PlayMovementSound(bool isClimbing = false)
    {
        if (isClimbing)
        {
            PlayClimbSound();
        }
        else
        {
            PlayWalkSound();
        }
    }
    
    private void PlayWalkSound()
    {
        AudioManager.Instance.PlaySoundAtPoint(footStepSound, transform.position, footStepVolume);
    }

    private void PlayClimbSound()
    {
        AudioManager.Instance.PlaySoundAtPoint(climbingSound, transform.position, climbingVolume);
    }
    
    public void PlayLandingSound()
    {
        doFallYell = true;
        AudioManager.Instance.PlaySoundAtPoint(landingSound, transform.position, landingVolume);
    }

    public void PlayFallYellSound()
    {
        if (!doFallYell)
            return;
        
        AudioManager.Instance.PlaySoundAtPoint(fallYellSound, transform.position, fallYellVolume);
        doFallYell = false;
    }

    public void PlayBumpSound()
    {
        AudioManager.Instance.PlaySoundAtPoint(obstacleBump, transform.position, obstacleBumpVolume);
    }
}
