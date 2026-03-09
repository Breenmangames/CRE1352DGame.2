using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;

public class SoundEffectManager : MonoBehaviour
{
    private static SoundEffectManager instance;

    private static AudioSource audioSource;
    private static SoundEffectLibrary soundEffectLibrary;
    [SerializeField] private Slider sfxSlider;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            audioSource = GetComponent<AudioSource>();
            soundEffectLibrary = GetComponent<SoundEffectLibrary>();
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        //sfxSlider.onValueChanged.AddListener(delegate { OnValueChanged(); });
    }
    public static void PlaySoundEffect(string name)
    {
            AudioClip audioClip = soundEffectLibrary.GetRandomClip(name);
            if (audioClip != null)
            {
                audioSource.PlayOneShot(audioClip);
            }
        
    }

    public static void SetVolume(float volume)
    {
        audioSource.volume = volume;
    }

    public void OnValueChanged()
    {
        SetVolume(sfxSlider.value);
    }
    // Update is called once per frame
}

