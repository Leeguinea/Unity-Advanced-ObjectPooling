using UnityEngine;

public class PlayerSensor : MonoBehaviour
{
    [SerializeField]
    private PlayerController _player;

    private void Awake()
    {
        if (_player == null)
        {
            _player = GetComponentInParent<PlayerController>();

            if (_player == null)
            {
                Debug.LogError("부모 오브젝트에 PlayerController가 없습니다! 확인해 주세요.");
            }
        }
    }

    //머리에 물체가 닿였는지 감지만 하는 역할.
    private void OnTriggerEnter(Collider other)
    {
        // 만약 Awake에서도 못 찾았다면 한 번 더 체크
        if (_player == null)
        {
            Debug.LogError("PlayerSensor: PlayerController를 찾을 수 없습니다! 부모 오브젝트를 확인하세요.");
            return;
        }

        if (other.CompareTag("Target") || other.CompareTag("Avoid"))
        {
            _player.HandleCollection(other.gameObject);
        }
    }
}
