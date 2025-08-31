using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class MusicManager : MonoBehaviour
{
    public static MusicManager Instance;
    public List<AudioClip> musicClips; // Lista de canciones
    public List<AudioClip> hearthClips; // Lista de canciones
    public float fadeTime = 2f;        // Tiempo de transición en segundos

    private AudioSource audioSource;
    private AudioSource hearthAudioSource;
    public int currentTrack = 0;
    private Coroutine transitionCoroutine;

    private void Awake()
    {
        Instance = this;
    }
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        hearthAudioSource = transform.GetChild(0).GetComponent<AudioSource>();
        if (musicClips.Count > 0)
        {
            audioSource.clip = musicClips[currentTrack];
            hearthAudioSource.clip = hearthClips[currentTrack];
            audioSource.Play();
            hearthAudioSource.Play();
        }
    }

    public void NextSong()
    {
        if (musicClips.Count == 0) return;

        currentTrack = (currentTrack + 1) % musicClips.Count;
        PlaySong(currentTrack);
    }

    public void PlaySong(int index)
    {
        if (index < 0 || index >= musicClips.Count) return;

        currentTrack= index;

        if (transitionCoroutine != null)
            StopCoroutine(transitionCoroutine);

        transitionCoroutine = StartCoroutine(SwitchTrack(index));
    }

    IEnumerator SwitchTrack(int newIndex)
    {
        // Fade out
        float startVolume = 1;
        for (float t = 0; t < fadeTime; t += Time.deltaTime)
        {
            audioSource.volume = Mathf.Lerp(startVolume, 0, t / fadeTime);
            hearthAudioSource.volume = Mathf.Lerp(startVolume, 0, t / fadeTime);
            yield return null;
        }
        audioSource.volume = 0;
        audioSource.Stop();

        // Switch and fade in
        audioSource.clip = musicClips[newIndex];
        hearthAudioSource.clip = hearthClips[newIndex];
        audioSource.Play();
        hearthAudioSource.Play();

        for (float t = 0; t < fadeTime; t += Time.deltaTime)
        {
            audioSource.volume = Mathf.Lerp(0, startVolume, t / fadeTime);
            hearthAudioSource.volume = Mathf.Lerp(0, startVolume, t / fadeTime);
            yield return null;
        }
        audioSource.volume = startVolume;
    }
}
