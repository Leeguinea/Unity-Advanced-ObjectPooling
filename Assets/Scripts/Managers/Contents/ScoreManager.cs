using UnityEngine;
using TMPro;

public class ScoreManager : MonoBehaviour
{
    //싱글톤 패턴
    public static ScoreManager Instance { get; private set; }

    [SerializeField] 
    private int _score = 0;

    [SerializeField] 
    private TextMeshProUGUI _scoreText;


    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    void Start()
    {
        UpdateScoreUI();
    }

    public int GetCurrentScore()
    {
        return _score;
    }

    public void ChangeScore(int amount)
    {
        _score += amount;
        UpdateScoreUI();

        //점수가 마이너스면 게임 오버 처리
        if (_score < 0)
        {
            GameManager gm = FindFirstObjectByType<GameManager>();
            if (gm != null)
            {
                gm.EndGame(false);
            }
            else
            {
                Debug.LogError("씬에 GameManager가 없습니다.");
            }
        }
    }

    public void SetScoreUIActive(bool active)
    {
        if(_scoreText != null)
            _scoreText.gameObject.SetActive(active);
    }

    void UpdateScoreUI()
    {
        if (_scoreText != null)
            _scoreText.text = $"Score: {_score}";
    }

    //최고 점수 업데이트 및 저장
    public void UpdateBestScore(int currentScore)
    {
        //기존에 저장된 최고점수 불러오기(기본 값은 0)
        int bestScore = PlayerPrefs.GetInt("BestScore", 0);

        if(currentScore > bestScore) 
        {
            //현재 점수가 더 높으면 새로 저장
            PlayerPrefs.SetInt("BestScore", currentScore);
            PlayerPrefs.Save(); //저장
            Debug.Log("최고점수갱신");
        }
    }


}
