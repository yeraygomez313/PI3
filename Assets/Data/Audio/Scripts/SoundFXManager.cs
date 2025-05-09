using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using UnityEngine;

public class SoundFXManager : MonoBehaviour
{
    public static SoundFXManager Instance { get; private set; }

    [SerializeField] private GameObject soundFXObject;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else Destroy(gameObject);
    }

    public void PlaySoundFX(AudioClip audioClip, Vector3 spawnPosition, float volume, float pitch)
    {
        float clampedVolume = Mathf.Clamp01(volume);
        float clampedPitch = Mathf.Clamp(pitch, -3, 3);

        AudioSource audioSource = Instantiate(soundFXObject, spawnPosition, Quaternion.identity, gameObject.transform).GetComponent<AudioSource>();
        audioSource.clip = audioClip;
        audioSource.volume = clampedVolume;
        audioSource.pitch = clampedPitch;
        audioSource.Play();

        float clipLength = audioSource.clip.length;
        Destroy(audioSource, clipLength);
    }
}

[Serializable]
public class AudioClipPlus
{
    [SerializeField] private AudioClipPlusSettings settings;
    private float volume => settings.Volume;
    private float pitch => settings.Volume;
    [SerializeField] private AudioClip audioClip;

    public void PlayAtPoint(Vector3 position)
    {
        if (audioClip == null) return;
        SoundFXManager.Instance.PlaySoundFX(audioClip, position, volume, pitch);
    }
}

[Serializable]
public class AudioClipList
{
    [SerializeField] private AudioClipPlusSettings settings;
    private float volume => settings.Volume;
    private float pitch => settings.Volume;
    [SerializeField] private List<AudioClip> audioClipList = new();

    public void PlayAtPointRandom(Vector3 position)
    {
        if (audioClipList.Count == 0) return;
        SoundFXManager.Instance.PlaySoundFX(audioClipList[UnityEngine.Random.Range(0, audioClipList.Count)], position, volume, pitch);
    }

    public void PlayAtPoint(int audioClipIndex, Vector3 position)
    {
        if (audioClipList.Count == 0) return;
        int clampedIndex = Mathf.Clamp(audioClipIndex, 0, audioClipList.Count - 1);
        SoundFXManager.Instance.PlaySoundFX(audioClipList[audioClipIndex], position, volume, pitch);
    }
}

[Serializable]
public struct AudioClipPlusSettings
{
    [Range(0, 1)] public float Volume;
    [Range(-3, 3)] public float Pitch;

    public AudioClipPlusSettings(float volume, float pitch)
    {
        Volume = volume;
        Pitch = pitch;
    }
}

[Serializable]
public struct MusicClip
{
    [Range(0, 1)] public float Volume;
    [Range(-3, 3)] public float Pitch;
    public AudioClip audioClip;
}
