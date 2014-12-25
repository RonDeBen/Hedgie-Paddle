using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MusicMiddleware : MonoBehaviour {

    [System.Serializable]
    public class SoundEntry {
        public string soundName;
        public AudioClip sound;
        public AudioSource source;
    }
    private SoundEntry playEntry;
    public List<SoundEntry> sounds; 

    public void loopSound(string name){
        SoundEntry playEntry = sounds.Find(item => item.soundName == name);
        playEntry.source.Play();
    }

    public void loopSoundAfterTime(string name, float time) {
        SoundEntry playEntry = sounds.Find(item => item.soundName == name);

    }

    void Start() {
        foreach(SoundEntry sound in sounds){
            sound.source = gameObject.AddComponent<AudioSource>() as AudioSource;
            sound.source.clip = sound.sound;
        }
        playEntry = sounds.Find(item => item.soundName == "Very_Hedgie");
        playEntry.source.Play();
    }

    void Update(){
        Debug.Log("Song Time: " + playEntry.source.time + "\nGameTime" + Time.time);
    }
}
