using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class MusicManager : MonoBehaviour
{
    public List<AudioClip> musicClips; // Lista de canciones
    public float fadeTime = 2f;        // Tiempo de transición en segundos

    private AudioSource audioSource;
    private int currentTrack = 0;
    private Coroutine transitionCoroutine;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (musicClips.Count > 0)
        {
            audioSource.clip = musicClips[currentTrack];
            audioSource.Play();
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

        if (transitionCoroutine != null)
            StopCoroutine(transitionCoroutine);

        transitionCoroutine = StartCoroutine(SwitchTrack(index));
    }

    IEnumerator SwitchTrack(int newIndex)
    {
        // Fade out
        float startVolume = audioSource.volume;
        for (float t = 0; t < fadeTime; t += Time.deltaTime)
        {
            audioSource.volume = Mathf.Lerp(startVolume, 0, t / fadeTime);
            yield return null;
        }
        audioSource.volume = 0;
        audioSource.Stop();

        // Switch and fade in
        audioSource.clip = musicClips[newIndex];
        audioSource.Play();

        for (float t = 0; t < fadeTime; t += Time.deltaTime)
        {
            audioSource.volume = Mathf.Lerp(0, startVolume, t / fadeTime);
            yield return null;
        }
        audioSource.volume = startVolume;
    }
}
