using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class SoundManager : MonoBehaviour 
{
    public AudioSource MusicAudioSource;
    public AudioSource SoundEffectsAudioSource;

    [Space(10)]

    public float MusicVolume = 1f;
    public float SoundEffectsVolume = 0.5f;
    public float VictoryAndDefeatSoundVolume = 0.7f;

    [Space(10)]

    public AudioClip SelectSound;
    public AudioClip ButtonSound;

    public AudioClip VictorySound;
    public AudioClip DefeatSound;

    public AudioClip StartConstructionSound;
    public AudioClip FinishConstructionSound;

    public AudioClip StartDeconstructionSound;

    [Space(10)]

    public AudioClip[] MusicClips;

    List<AudioClip> usedMusicClips;
    List<AudioClip> unusedMusicClips;

    [HideInInspector]
    public static bool Muted { get; protected set; }

    bool stopMusic;

    bool startConstructionSoundPlayedThisFrame;

    void Start()
    {
        MusicAudioSource.volume = MusicVolume;
        SoundEffectsAudioSource.volume = SoundEffectsVolume;

        Button[] buttons = Resources.FindObjectsOfTypeAll<Button>();
        foreach(Button button in buttons)
        {
            button.onClick.AddListener(PlayButtonSound);
        }

        unusedMusicClips = new List<AudioClip>(MusicClips);
        usedMusicClips = new List<AudioClip>();
    }

    private void Update()
    {
        startConstructionSoundPlayedThisFrame = false;

        if (Muted || stopMusic) return;

        if (MusicClips.Length == 0) return;

        if ((MusicAudioSource.clip == null || MusicAudioSource.time >= MusicAudioSource.clip.length)
            && MusicAudioSource.isPlaying == false)
        {
            MusicAudioSource.clip = GetRandomUnusedClip();
            MusicAudioSource.Play();
        }
    }

    public void TurnSoundOn()
    {
        Muted = false;
        if (stopMusic == false) MusicAudioSource.UnPause();
    }

    public void TurnSoundOff()
    {
        Muted = true;
        MusicAudioSource.Pause();
        SoundEffectsAudioSource.Stop();
    }

    AudioClip GetRandomUnusedClip()
    {
        if (unusedMusicClips.Count == 0)
        {
            unusedMusicClips = usedMusicClips;
            usedMusicClips = new List<AudioClip>();
        }

        int randomIndex = UnityEngine.Random.Range(0, unusedMusicClips.Count);

        AudioClip result = unusedMusicClips[randomIndex];

        usedMusicClips.Add(result);
        unusedMusicClips.Remove(result);

        return result;
    }

    public void PlaySelectSound()
    {
        if (Muted) return;
        SoundEffectsAudioSource.PlayOneShot(SelectSound);
    }

    public void PlayButtonSound()
    {
        if (Muted) return;
        SoundEffectsAudioSource.PlayOneShot(ButtonSound);
    }

    public void PlayVictorySound()
    {
        if (Muted) return;

        stopMusic = true;
        MusicAudioSource.Stop();
        MusicAudioSource.volume = VictoryAndDefeatSoundVolume;
        MusicAudioSource.PlayOneShot(VictorySound);
    }

    public void PlayDefeatSound()
    {
        if (Muted) return;

        stopMusic = true;
        MusicAudioSource.Stop();
        MusicAudioSource.volume = VictoryAndDefeatSoundVolume;
        MusicAudioSource.PlayOneShot(DefeatSound);
    }

    public void PlayStartConstructionSound()
    {
        if (Muted || startConstructionSoundPlayedThisFrame) return;
        
        SoundEffectsAudioSource.PlayOneShot(StartConstructionSound);

        startConstructionSoundPlayedThisFrame = true;
    }

    public void PlayFinishConstructionSound()
    {
        if (Muted) return;
        SoundEffectsAudioSource.PlayOneShot(FinishConstructionSound);
    }

    public void PlayStartDeconstructionSound()
    {
        if (Muted) return;
        SoundEffectsAudioSource.PlayOneShot(StartDeconstructionSound);        
    }
}
