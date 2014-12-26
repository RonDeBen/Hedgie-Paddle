using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MusicMiddleware : MonoBehaviour {

    [System.Serializable]
    public class SoundEntry {
        public AudioClip sound;
        [HideInInspector]
        public AudioSource source;
        [HideInInspector]
        public bool looping = false;
        public float loopStartTime;
        public float loopEndTime;
    }
    private SoundEntry playEntry;
    public List<SoundEntry> sounds; 

    public void loopSound(string name, bool startAtBeginning){
        SoundEntry playEntry = sounds.Find(item => item.sound.name == name);
        if(!startAtBeginning)
            playEntry.source.time = playEntry.loopStartTime;
        playEntry.source.Play();
    }

    public void loopFromTime(string name, float startTime, bool startAtBeginning){
        SoundEntry playEntry = sounds.Find(item => item.sound.name == name);
        if(startAtBeginning)
            playEntry.source.time = startTime;
        playEntry.source.Play();
        playEntry.loopStartTime = startTime;
        playEntry.looping = true;
    }

    public void loopBetweenTimes(string name, float startTime, float endTime, bool startAtBeginning){
        SoundEntry playEntry = sounds.Find(item => item.sound.name == name);
        if(startAtBeginning)
            playEntry.source.time = startTime;
        playEntry.source.Play();
        playEntry.loopStartTime = startTime;
        playEntry.loopEndTime = endTime;
        playEntry.looping = true;
    }

    void Start() {
        foreach(SoundEntry sound in sounds){
            sound.source = gameObject.AddComponent<AudioSource>() as AudioSource;
            sound.source.clip = sound.sound;
            //sound.loopEndTime = sound.source.clip.length;
            sound.looping = false;
        }
        //loopSound("Very_Hedgie", true);
        //loopFromTime("Very_Hedgie", 9.419f, true);
    }

    void FixedUpdate(){
        foreach(SoundEntry sound in sounds){
            if(sound.looping){
                if(!sound.source.isPlaying || sound.source.time >= sound.loopEndTime - 0.01){
                    sound.source.Play();
                    sound.source.time = sound.loopStartTime;
                }
            }
        }
    }
}
