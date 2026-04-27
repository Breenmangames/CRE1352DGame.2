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
    private void InitializeDictionary() // Method to initialize the sound dictionary by populating it with the sound effect groups defined in the inspector, allowing us to organize and access sound effects by name during gameplay
    {
        soundDictionary = new Dictionary<string, List<AudioClip>>(); // Initialize the sound dictionary to store lists of audio clips for each sound effect group, allowing us to organize and access sound effects by name during gameplay
        foreach (SoundEffectGroup soundEffectgroup in soundEffectGroups)
        {
            soundDictionary[soundEffectgroup.name] = soundEffectgroup.audioClip; // Add each sound effect group to the dictionary using the group's name as the key and the list of audio clips as the value, allowing us to organize and access sound effects by name during gameplay
        }
    }

    public AudioClip GetRandomClip(string name) //Method to retrieve a random audio clip from the sound dictionary based on the provided name, allowing us to play different variations of a sound effect during gameplay for added variety and immersion
    {
        if (soundDictionary.ContainsKey(name)) // Check if the sound dictionary contains the specified name as a key, allowing us to determine if there are sound effects available for that name before attempting to retrieve a clip
        {
            List<AudioClip> clips = soundDictionary[name]; // Retrieve the list of audio clips associated with the specified name from the sound dictionary, allowing us to access the available sound effects for that name during gameplay
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
public struct SoundEffectGroup // Struct to define a sound effect group, which consists of a name and a list of audio clips, allowing us to organize and access sound effects by name during gameplay
{
    public string name;
    public List<AudioClip> audioClip;
}