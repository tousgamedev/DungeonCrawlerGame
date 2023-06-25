using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public const string AudioFolder = "Audio";
    public static AudioManager Instance { get; private set; }

    [SerializeField] private AudioSource effectsSource;
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource ambienceSource;

    private readonly Dictionary<AudioClipName, AudioClip> audioClips = new();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
#if UNITY_EDITOR
            if (Application.isEditor)
            {
                DestroyImmediate(gameObject);
            }
#endif

            Destroy(gameObject);
            return;
        }

        if (effectsSource == null || musicSource == null || ambienceSource == null)
        {
            LogHelper.Report("Audio source is null!", LogGroup.System, LogType.Error);;
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    private void LoadAudioClip(AudioClipName clipName)
    {
        var clip = Resources.Load<AudioClip>($"{AudioFolder}/{clipName}");
        if (clip == null)
        {
            LogHelper.Report($"Audio clip {clipName} not found!", LogGroup.Audio, LogType.Error);
            return;
        }

        if (!audioClips.TryAdd(clipName, clip))
        {
            LogHelper.Report($"Audio clip {clipName} is duplicate!", LogGroup.Audio, LogType.Warning);
        }
    }

    private bool TryGetAudioClip(AudioClipName clipName, out AudioClip clip)
    {
        if (audioClips.TryGetValue(clipName, out clip))
            return true;

        LoadAudioClip(clipName);
        audioClips.TryGetValue(clipName, out clip);
        return clip != null;
    }

    public void PlaySound(AudioClipName clipName, float volume = 1f)
    {
        if (!TryGetAudioClip(clipName, out AudioClip clip))
        {
            LogHelper.Report($"Audio clip {clipName} not found!", LogGroup.Audio, LogType.Error);
            return;
        }

        effectsSource.PlayOneShot(clip, volume);
    }

    public void PlaySoundAtPoint(AudioClipName clipName, Vector3 position, float volume = 1f)
    {
        if (!TryGetAudioClip(clipName, out AudioClip clip))
        {
            LogHelper.Report($"Audio clip {clipName} not found!", LogGroup.Audio, LogType.Error);
            return;
        }

        AudioSource.PlayClipAtPoint(clip, position, volume);
    }

    public void StopSound()
    {
        effectsSource.Stop();
    }

    public void SetMasterVolume(float volume)
    {
        AudioListener.volume = volume;
    }

    public void SetMusicVolume(float volume)
    {
        musicSource.volume = volume;
    }

    public void SetEffectsVolume(float volume)
    {
        effectsSource.volume = volume;
    }

    public void SetAmbienceVolume(float volume)
    {
        ambienceSource.volume = volume;
    }
}