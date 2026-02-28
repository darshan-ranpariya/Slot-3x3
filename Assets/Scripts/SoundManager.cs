using UnityEngine;
using UnityEngine.Audio;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; }

    [Header("Audio Mixer")]
    [SerializeField] private AudioMixer audioMixer;
    [SerializeField] private AudioMixerGroup musicGroup;
    [SerializeField] private AudioMixerGroup sfxGroup;

    [Header("Audio Sources")]
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource sfxSource;

    [Header("Audio Clips")]
    [SerializeField] private AudioClip backgroundMusic;
    [SerializeField] private AudioClip spinStartClip;
    [SerializeField] private AudioClip winClip;
    [SerializeField] private AudioClip buttonClickClip;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        if (musicSource != null && musicGroup != null)
            musicSource.outputAudioMixerGroup = musicGroup;
            
        if (sfxSource != null && sfxGroup != null)
            sfxSource.outputAudioMixerGroup = sfxGroup;

        PlayMusic(backgroundMusic);
    }

    public void PlayMusic(AudioClip clip)
    {
        if (clip != null && musicSource != null)
        {
            musicSource.clip = clip;
            musicSource.loop = true;
            musicSource.Play();
        }
    }

    public void PlaySFX(AudioClip clip)
    {
        if (clip != null && sfxSource != null)
        {
            float originalPitch = sfxSource.pitch;
            sfxSource.pitch *= Random.Range(0.95f, 1.05f);
            sfxSource.PlayOneShot(clip);
            sfxSource.pitch = originalPitch;
        }
    }

    public void PlaySpinStart() => PlaySFX(spinStartClip);
    public void PlayWin() => PlaySFX(winClip);
    public void PlayButtonClick() => PlaySFX(buttonClickClip);

    public void SetMusicVolume(float volume)
    {
        if (audioMixer != null)
            audioMixer.SetFloat("MusicVolume", Mathf.Log10(Mathf.Clamp(volume, 0.0001f, 1f)) * 20);
    }

    public void SetSFXVolume(float volume)
    {
        if (audioMixer != null)
            audioMixer.SetFloat("SFXVolume", Mathf.Log10(Mathf.Clamp(volume, 0.0001f, 1f)) * 20);
    }
}
