using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    private static MusicManager instance;

    private AudioSource introSource;
    private AudioSource loopSource;

    private AudioClip intro;
    private AudioClip loop;
    public AudioClip weapon_shoot;
    public AudioClip enemy_hit;
    public AudioClip house_damage;


    public static MusicManager Get()
    {
        return instance;
    }
    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);

        // Two sources so the loop can be scheduled precisely
        introSource = gameObject.AddComponent<AudioSource>();
        loopSource = gameObject.AddComponent<AudioSource>();

        introSource.playOnAwake = false;
        loopSource.playOnAwake = false;

        introSource.loop = false;
        loopSource.loop = true;
        SetVolume(0.3f);
    }

    private void Start()
    {
        intro = Resources.Load<AudioClip>("Sound/Music/snd_main_intro");
        loop = Resources.Load<AudioClip>("Sound/Music/snd_main_loop");
        weapon_shoot = Resources.Load<AudioClip>("Sound/Sfx/Weapon_Shoot");
        enemy_hit = Resources.Load<AudioClip>("Sound/Sfx/Enemy_Hit");
        house_damage = Resources.Load<AudioClip>("Sound/Sfx/Weapon_Shoot");
        
        
        intro.LoadAudioData();
        loop.LoadAudioData();
        weapon_shoot.LoadAudioData();
        enemy_hit.LoadAudioData();
        house_damage.LoadAudioData();

        PlayIntroThenLoop();
    }

    public void PlayIntroThenLoop()
    {
        if (intro == null || loop == null)
        {
            Debug.LogError("Intro or loop clip missing in Resources.");
            return;
        }

        introSource.clip = intro;
        loopSource.clip = loop;

        // Schedule precisely on the DSP clock
        double startTime = AudioSettings.dspTime + 0.1; // small lead time
        double introEndTime = startTime + (double)intro.samples / intro.frequency;

        introSource.PlayScheduled(startTime);
        introSource.SetScheduledEndTime(introEndTime);
        loopSource.PlayScheduled(introEndTime);
    }

    public void StopMusic()
    {
        introSource.Stop();
        loopSource.Stop();
    }

    public void SetVolume(float volume)
    {
        float v = Mathf.Clamp01(volume);
        introSource.volume = v;
        loopSource.volume = v;
    }
}