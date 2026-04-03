using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    [Header("Common (Title & Play)")]
    [SerializeField] private AudioClip _buttonClickClip;

    [Header("Panels")]
    [SerializeField] private GameObject _pasuePanel;
    [SerializeField] private GameObject _optionPanel;

    [Header("Audio Sliders")]
    [SerializeField] private Slider _bgmSlider;
    [SerializeField] private Slider _sfxSlider;

    private bool _isPaused = false;

    void Awake() => Instance = this;

    //초기 볼륨 설정은 SoundManager에게
    void Start()
    {
        //저장된 볼륨 값 불러오기 (값이 없으면 0.5f로 설정)
        float bgmVol = PlayerPrefs.GetFloat("BGM_Save", 0.5f);
        float sfxVol = PlayerPrefs.GetFloat("SFX_Save", 0.5f);

        //슬라이더 UI 위치를 저장된 값으로 옮기기
        if (_bgmSlider != null) _bgmSlider.value = bgmVol;
        if (_sfxSlider != null) _sfxSlider.value = sfxVol;

        SetBGMVolume(bgmVol);
        SetSFXVolume(sfxVol);
    }

    void Update()
    {
        if(_pasuePanel != null && Input.GetKeyDown(KeyCode.Escape)) 
        {
            if (_isPaused) Resume();
            else Pause();
        }
    }

    #region [Title & Play]
    
    //버튼 클릭음
    public void PlayClickSound()
    {
        if (SoundManager.Instance != null && _buttonClickClip)
            SoundManager.Instance.PlaySFX(_buttonClickClip);
    }

    //게임 종료
    public void OnClickExit()
    {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit(); 
        #endif
    }

    //타이틀 > 옵션 > 슬라이더에서 호출될 함수
    // SoundManager에 있는 SetVolume 함수를 호출하며 "BGMVolume"이라는 이름을 넘겨줌
    //배경음 조절
    public void SetBGMVolume(float volume)
    {
        SoundManager.Instance.SetVolume("BGMVolume", volume);
        PlayerPrefs.SetFloat("BGM_Save", volume);
    }

    // 효과음 조절
    public void SetSFXVolume(float volume)
    {
        SoundManager.Instance.SetVolume("SFXVolume", volume);
        PlayerPrefs.SetFloat("SFX_Save", volume);
    }

    public void LoadGameScene(string sceneName)
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(sceneName);
    }

    //옵션 창 열기
    public void OpenOption()
    {
        if (_optionPanel != null) _optionPanel.SetActive(true);
    }

    //옵션 창 닫기
    public void CloseOption()
    {
        if (_optionPanel != null) _optionPanel.SetActive(false);
    }

    #endregion


    #region [Play Scene Functions]

    //일시정지
    public void Pause()
    {
        SoundManager.Instance.PlaySFX(_buttonClickClip);
        _isPaused = true;
        if(_pasuePanel != null) _pasuePanel.SetActive(true);
        Time.timeScale = 0f;
    }

    //재개
    public void Resume()
    {
        _isPaused = false;
        if(_pasuePanel != null) _pasuePanel.SetActive(false);
        Time.timeScale = 1f;
    }

    //게임 종료
    public void ExitGame()
    {
        Time.timeScale = 1f;
        Debug.Log("게임 종료");
        Application.Quit();
    }    

    //게임 재시작
    public void OnClickRestart()
    {
        Time.timeScale = 1f;
        string currentSceneName = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene(currentSceneName);

        Debug.Log("Game Restart");
    }
    #endregion
}

