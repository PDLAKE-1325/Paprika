using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; }

    [Header("Audio Sources")]
    [SerializeField] private AudioSource bgmAudioSource;
    [SerializeField] private AudioSource sfxAudioSource;

    [Header("Volume Settings")]
    [Range(0f, 1f)] public float bgmVolume = 0.7f;
    [Range(0f, 1f)] public float sfxVolume = 1f;

    private AudioClip currentBgmClip;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            if (bgmAudioSource == null)
            {
                bgmAudioSource = gameObject.AddComponent<AudioSource>();
                bgmAudioSource.loop = true;
                bgmAudioSource.playOnAwake = false;
            }

            if (sfxAudioSource == null)
            {
                sfxAudioSource = gameObject.AddComponent<AudioSource>();
                sfxAudioSource.loop = false;
                sfxAudioSource.playOnAwake = false;
            }
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        bgmAudioSource.volume = bgmVolume;
        sfxAudioSource.volume = sfxVolume;
    }

    public void PlaySfx(AudioClip clip, float volumeScale = 1f)
    {
        if (clip != null)
        {
            sfxAudioSource.PlayOneShot(clip, sfxVolume * volumeScale);
        }
    }

    public void PlayBgm(AudioClip clip)
    {
        if (clip == null) return;

        if (currentBgmClip == clip && bgmAudioSource.isPlaying)
        {
            return;
        }

        currentBgmClip = clip;
        bgmAudioSource.clip = clip;
        bgmAudioSource.Play();
    }

    // 0-1
    public void SetBgmVolume(float volume)
    {
        bgmVolume = Mathf.Clamp01(volume);
        bgmAudioSource.volume = bgmVolume;
    }

    // 0-1
    public void SetSfxVolume(float volume)
    {
        sfxVolume = Mathf.Clamp01(volume);
        sfxAudioSource.volume = sfxVolume;
    }

    public void PauseBgm()
    {
        if (bgmAudioSource.isPlaying)
        {
            bgmAudioSource.Pause();
        }
    }

    public void ResumeBgm()
    {
        if (!bgmAudioSource.isPlaying && currentBgmClip != null)
        {
            bgmAudioSource.UnPause();
        }
    }

    public void StopBgm()
    {
        bgmAudioSource.Stop();
        currentBgmClip = null;
    }
}