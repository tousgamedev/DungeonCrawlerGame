using UnityEngine;

public class ControllerAudio : MonoBehaviour
{
    [Header("Walking")]
    [SerializeField] private AudioClipName footStepSound = AudioClipName.Footstep;
    [SerializeField] [Range(0,1f)] private float footStepVolume = 0.6f;
    [Header("Wall Bump")]
    [SerializeField] private AudioClipName wallBump = AudioClipName.WallBump;
    [SerializeField] [Range(0,1f)] private float wallBumpVolume = 0.6f;
    [Header("Falling")]
    [SerializeField] private AudioClipName landingSound = AudioClipName.Footstep;
    [SerializeField] [Range(0,1f)] private float landingVolume = 0.6f;
    [SerializeField] private AudioClipName fallScreamSound = AudioClipName.None;
    [SerializeField] [Range(0,1f)] private float fallScreamVolume = 0.6f;

    public void PlayWalkSound(Vector3 position)
    {
        AudioManager.Instance.PlaySoundAtPoint(footStepSound, position, footStepVolume);
    }

    public void PlayLandingSound(Vector3 position)
    {
        AudioManager.Instance.PlaySoundAtPoint(landingSound, position, landingVolume);
    }

    public void PlayFallScreamSound(Vector3 position)
    {
        AudioManager.Instance.PlaySoundAtPoint(fallScreamSound, position, fallScreamVolume);
    }

    public void PlayBumpSound(Vector3 position)
    {
        AudioManager.Instance.PlaySoundAtPoint(wallBump, position, wallBumpVolume);
    }
}
