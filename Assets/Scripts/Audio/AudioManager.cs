using System.Collections.Generic;
using UnityEngine;

public class AudioManager : ManagerBase<AudioManager>
{
    private const string AudioFolder = "Audio";

    [SerializeField] private AudioSource effectsSource;
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource ambienceSource;

    private readonly Dictionary<AudioClipName, AudioClip> audioClips = new();

#pragma warning disable CS0108, CS0114
    private void Awake()
#pragma warning restore CS0108, CS0114
    {
        base.Awake();

        if (effectsSource != null && musicSource != null && ambienceSource != null)
            return;
        
        LogHelper.Report("Audio source is null!", LogType.Error, LogGroup.System);;
        Utilities.Destroy(gameObject);
    }

    private void LoadAudioClip(AudioClipName clipName)
    {
        var clip = Resources.Load<AudioClip>($"{AudioFolder}/{clipName}");
        if (clip == null)
        {
            LogHelper.Report($"Audio clip {clipName} not found!", LogType.Error, LogGroup.Audio);
            return;
        }

        if (!audioClips.TryAdd(clipName, clip))
        {
            LogHelper.Report($"Audio clip {clipName} is duplicate!", LogType.Warning, LogGroup.Audio);
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
            LogHelper.Report($"Audio clip {clipName} not found!", LogType.Error, LogGroup.Audio);
            return;
        }

        effectsSource.PlayOneShot(clip, volume);
    }

    public void PlaySoundAtPoint(AudioClipName clipName, Vector3 position, float volume = 1f)
    {
        if (!TryGetAudioClip(clipName, out AudioClip clip))
        {
            LogHelper.Report($"Audio clip {clipName} not found!", LogType.Error, LogGroup.Audio);
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