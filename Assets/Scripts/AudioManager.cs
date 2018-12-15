using System;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour {

    public static AudioManager instance;

    public Sound[] sounds;

	// Use this for initialization
	void Awake () {

        // Check if an instance already exists, deletes itself otherwise
        if (instance == null)
            instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }

        // Game object persists through level loading
        DontDestroyOnLoad(gameObject);

        // Assign properties for each sound
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
        // Search for sound with the given name and play it
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if (s == null)
        {
            print(name + " is not a valid soundfile name.");
            return;
        }

        // Prevent the song from playing again if it already is.
        if (s.source.isPlaying && s.isSong)
            return;

        s.source.Play();
    }

    // Stops game music when not in Fighting scene
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
