using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioFW : MonoBehaviour {
    // how to use:
    // put sound effects in their own objects under SFX
    // then anywhere in the code, call 'AudioFW.Play(id)'
    // where id is the name of the sound effect object.

    Dictionary<string, AudioSource> sfx = new Dictionary<string, AudioSource>();

    public static void Play(string id) {
        instance.PlayImpl(id);
    }

    void PlayImpl(string id) {
        if (!sfx.ContainsKey(id)) {
            Debug.LogError("No sound with ID " + id);
            return;
        }
        sfx[id].PlayOneShot(sfx[id].clip);
    }

    static public AudioFW instance {
        get {
            if (!_instance) {
                var a = GameObject.FindObjectsOfType<AudioFW>();
                if (a.Length == 0)
                    Debug.LogError("No AudioFW in scene");
                else if (a.Length > 1)
                    Debug.LogError("Multiple AudioFW in scene");
                _instance = a[0];
            }
            return _instance;
        }
    }
    static AudioFW _instance;

    void FindAudioSources() {
        var audioSources = transform.Find("SFX").GetComponentsInChildren<AudioSource>();
        foreach (var a in audioSources) {
            sfx.Add(a.name, a);
        }
    }

    void Awake() {
        FindAudioSources();
    }

    void Update() {
        if (Input.GetKeyDown(KeyCode.A))
            DebugPrint();
    }

    void DebugPrint() {
        string s = "";
        foreach (var id in sfx.Keys)
            s += id + " ";
        print(s);
    }
}
