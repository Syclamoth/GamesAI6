using UnityEngine;
using System.Collections;
using System.Collections.Generic;
[System.Serializable]
public class CommandableSound {
	public string name;
	public bool trackPosition;
	public AudioClip[] sounds;
	
	public AudioClip GetSound() {
		return sounds[Random.Range(0, sounds.Length)];
	}
}

public class SoundEffectManager : MonoBehaviour {
	public CommandableSound[] allSounds;
	
	private Dictionary<string, CommandableSound> availableSounds;
	private AudioSource soundSource;
	
	void Awake() {
		RebuildSounds();
		if(audio != null) {
			soundSource = audio;
		} else {
			soundSource = gameObject.AddComponent<AudioSource>();
		}
	}
	
	private void RebuildSounds() {
		availableSounds = new Dictionary<string, CommandableSound>();
		foreach(CommandableSound curSound in allSounds) {
			// Need to cleanly manage duplicate names!
			availableSounds.Add(curSound.name, curSound);
		}
	}
	
	
	public void PlaySound(string name) {
		// check for existence first!
		AudioClip clipToPlay = availableSounds[name].GetSound();
	}
}
