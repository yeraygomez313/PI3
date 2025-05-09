using UnityEngine;

public class MusicManager : MonoBehaviour
{
    [SerializeField] private AudioClipList musicClips;
    private AudioSource musicSource;

    private void Awake()
    {
        musicSource = GetComponent<AudioSource>();
    }

    public void PlayMusic(string musicId)
    {
        //musicSource.Stop();
        //musicSource.clip = musicClips[musicId].audioClip;
        //musicSource.Play();
    }
}
