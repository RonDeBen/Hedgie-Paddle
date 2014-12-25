using UnityEngine;
using System.Collections;

public class SongMap : MonoBehaviour {

    [System.Serializable]
    public class SongEntry {
        public string songName;
        public AudioSource audio;
    }
	
}
