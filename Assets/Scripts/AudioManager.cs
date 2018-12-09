using System;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour {

    public static AudioManager instance;

    public Sound[] sounds;

	// Use this for initialization
	void Awake () {

        if (instance == null)
            instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);

        foreach (Sound s in sounds)
        {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;
            s.source.volume = s.volume;
            s.source.pitch = s.pitch;
            s.source.loop = s.loop;
        }
	}

    public void play(string name)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if (s == null)
        {
            print(name + " is not a valid soundfile name.");
            return;
        }

        // Prevent allow the song from playing again if it already is.
        if (s.source.isPlaying && s.isSong)
            return;

        s.source.Play();
    }

    public void stop()
    {
        Sound s = Array.Find(sounds, sound => sound.name == "PaperCombat");
        if (s != null)
            s.source.Stop();
        s = Array.Find(sounds, sound => sound.name == "NihonMori");
        if (s != null)
            s.source.Stop();
        s = Array.Find(sounds, sound => sound.name == "RocktheDevil");
        if (s != null)
            s.source.Stop();

    }
}
