using UnityEngine;
using TMPro;

/*
 * [역할]
 * 1. 플레이어가 일정 시간 동안 아이템을 먹지 않으면 점수를 차감함.
 * 2. 아이템 획득 시 플레이어에 의해 타이머가 초기화됨.
 * [참조]
 * PlayerController로부터 컴포넌트 참조를 획득하여 대상의 점수를 제어.
 */

public class PenaltySystem : MonoBehaviour
{
    [Header("Penalty Setting")]
    [SerializeField] float _penaltyCount = 5.0f;
    [SerializeField] int _penaltyDamage = 1;

    [SerializeField] 
    TextMeshProUGUI _penaltyTimerText;

    float _lastEatTime = 0f;
    float _timer = 0f;

    PlayerController _player;

    void Start()
    {
        _player = GetComponent<PlayerController>();
    }

    void Update()
    {
        if (Time.timeScale == 0) return;

        // 1. 마지막 식사 후 흐른 시간을 계속 더해줌
        _lastEatTime += Time.deltaTime;

        UpdateTimerUI();

        // _penaltyCount가 지났을 때만 실행
        if (_lastEatTime >= _penaltyCount)
        {
            _timer += Time.deltaTime;

            if (_timer >= 1.0f)
            {
                if(ScoreManager.Instance != null)
                {
                    ScoreManager.Instance.ChangeScore(-_penaltyDamage);
                }
                _timer = 0f;
            }
        }
    }

    void UpdateTimerUI()
    {
        if (_penaltyTimerText == null) return;

        // 남은 시간(전체 시간 - 흐른 시간)
        float remaining = _penaltyCount - _lastEatTime;
        if (remaining < 0) 
            remaining = 0;

        // 소수점 1자리까지 표시 (예: "Danger: 3.2s")
        _penaltyTimerText.text = $"{remaining:F1}s";

        // 시간이 다 되면 빨간색, 평소엔 흰색으로 강조!
        _penaltyTimerText.color = (remaining <= 0) ? Color.red : Color.white;
    }

    // 3. 아이템을 먹었을 때 외부에서 호출할 함수
    public void ResetPenaltyTimer()
    {
        _lastEatTime = 0f;
        _timer = 0f;
    }
} 