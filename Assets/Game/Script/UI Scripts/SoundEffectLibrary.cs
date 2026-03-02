using System.Collections.Generic;
using UnityEngine;
using System.Collections;
using System;

public class SoundEffectLibrary : MonoBehaviour
{
    [SerializeField] private SoundEffectGroup[] soundEffectGroups;
    private Dictionary<string, List<AudioClip>> soundDictionary;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
   private void Awake()
    {
        InitializeDictionary();
    }

    // Update is called once per frame
    private void InitializeDictionary()
    {
        soundDictionary = new Dictionary<string, List<AudioClip>>();
        foreach (SoundEffectGroup soundEffectgroup in soundEffectGroups)
        {
            soundDictionary[soundEffectgroup.name] = soundEffectgroup.audioClip;
        }
    }

    public AudioClip GetRandomClip(string name)
    {
        if (soundDictionary.ContainsKey(name))
        {
            List<AudioClip> clips = soundDictionary[name];
            if (clips.Count > 0)
            {
                int randomIndex = UnityEngine.Random.Range(0, clips.Count);
                return clips[randomIndex];
            }
        }
        return null;
    }
}

[System.Serializable]  //can be serialized by unity and show up in the inspector
public struct SoundEffectGroup
{
    public string name;
    public List<AudioClip> audioClip;
}