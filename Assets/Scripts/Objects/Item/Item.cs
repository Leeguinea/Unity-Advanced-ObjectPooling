using UnityEngine;
using System.Collections;

public class Item : MonoBehaviour
{
    [SerializeField] private AudioClip _targetSound;
    [SerializeField] private AudioClip _AvoidSound;

    [Header("Life Settings")]
    [SerializeField] private float _lifeTime = 5.0f; // 5초 뒤 자동 소멸

    public enum ItemType { Target, Avoid }
    public ItemType type;

    private Rigidbody _rb;
    private bool _isReturned = false; // 중복 반납 방지용 플래그

    void Awake()
    {
        _rb = GetComponent<Rigidbody>();
    }

    private void OnEnable()
    {
        _isReturned = false;

        //물리 초기화: 느릿하게 떨어지는 문제 해결
        if (_rb != null)
        {
            _rb.linearVelocity = Vector3.zero;
            _rb.angularVelocity = Vector3.zero;
        }

        //자동 반납
        Invoke("ReturnToPool", _lifeTime);
    }

    private void OnDisable()
    {
        // 비활성화 시 예약된 Invoke 취소 (메모리 관리)
        CancelInvoke();
        CancelInvoke("ReturnToPool");
    }

    // 풀 매니저로 돌려보내는 통합 함수
    private void ReturnToPool()
    {
        if (_isReturned) return; // 이미 반납되었다면 무시

        _isReturned = true;
        PoolManager.Instance.ReturnItem(gameObject);
    }

    // 땅에 닿았을 때 
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground") && !_isReturned)
        {
            CancelInvoke("ReturnToPool");
            Invoke("ReturnToPool", 3.0f);
        }
    }

    // 플레이어가 먹었을 때
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !_isReturned)
        {
            AudioClip clipToPlay = (type == ItemType.Target) ? _targetSound : _AvoidSound;

            if (SoundManager.Instance != null && clipToPlay != null)
            {
                SoundManager.Instance.PlaySFX(clipToPlay);
            }

            ReturnToPool(); // 즉시 반납
        }
    }
}