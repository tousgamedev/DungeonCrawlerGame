using System;
using UnityEngine;

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
    [SerializeField] private AudioClipName fallScreamSound = AudioClipName.None;
    [SerializeField] [Range(0,1f)] private float fallScreamVolume = 0.6f;

    public void PlayWalkSound()
    {
        AudioManager.Instance.PlaySoundAtPoint(footStepSound, transform.position, footStepVolume);
    }

    public void PlayClimbSound()
    {
        AudioManager.Instance.PlaySoundAtPoint(climbingSound, transform.position, climbingVolume);
    }
    
    public void PlayLandingSound()
    {
        AudioManager.Instance.PlaySoundAtPoint(landingSound, transform.position, landingVolume);
    }

    public void PlayFallYellSound()
    {
        AudioManager.Instance.PlaySoundAtPoint(fallScreamSound, transform.position, fallScreamVolume);
    }

    public void PlayBumpSound()
    {
        AudioManager.Instance.PlaySoundAtPoint(obstacleBump, transform.position, obstacleBumpVolume);
    }
}
