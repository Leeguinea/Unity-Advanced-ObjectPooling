using UnityEngine;
using UnityEngine.Audio;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;
    private AudioSource _audioSource;

    [SerializeField] private AudioMixer _mainMixer;
    [SerializeField] private AudioSource _bgmSource;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            transform.SetParent(null);
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        _audioSource = GetComponent<AudioSource>();

        //만약 AudioSource가 없다면
        if (_audioSource == null)
        {
            _audioSource = gameObject.AddComponent<AudioSource>();
        }
    }

    public void PlayBGM(AudioClip clip)
    {
        if (_bgmSource == null || clip == null) return;

        // 이미 같은 노래가 나오고 있다면 다시 틀지 않음 (씬 전환 시 자연스러움 유지)
        if (_bgmSource.clip == clip && _bgmSource.isPlaying) return;

        _bgmSource.clip = clip;
        _bgmSource.Play();
    }


    public void PlaySFX(AudioClip clip)
    {
        if (clip != null && _audioSource != null)
        {
            Debug.Log($"[SoundManager] 재생 시도: {clip.name}");
            _audioSource.PlayOneShot(clip);
        }
    }

    public void SetVolume(string parameterName, float sliderValue)
    {
        if (_mainMixer == null)
        {
            Debug.LogWarning("Main Mixer가 연결되지 않았습니다!");
            return;
        }

        float dB = Mathf.Log10(Mathf.Clamp(sliderValue, 0.0001f, 1f)) * 20f;
        _mainMixer.SetFloat(parameterName, dB);
    }
}