using System.Collections;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    [Header("Item Groups")]
    [SerializeField] 
    GameObject[] _targetPrefabs; 
    [SerializeField]
    GameObject[] _avoidPrefabs;

    [Header("Spawn Settings")]
    [SerializeField]
    float _spawnInterval = 0.9f; //생성주기
    [SerializeField]
    float _targetSpawnChance = 70f; //target이 나올 확률(나머지는 avoid)

    [SerializeField]
    float _mapRangeX = 25f;
    float _mapRangeZ = 18f;

    private Transform _playerTransform; //플레이어 정보를 담아둠.

    void Start()
    {
        GameObject player = GameObject.FindWithTag("Player");
        if (player != null)
            _playerTransform = player.transform;
        else
            Debug.LogWarning("Spawner: 'Player'태그를 찾을 수 없어 랜덤 모드로만 작동");

        StartCoroutine(SpawnRoutine());
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            Debug.Log("2000개 집중 스폰 테스트 시작");
            for (int i = 0; i < 2000; i++)
            {
                SpawnItem();
            }
        }
    }

    void StartSpawnRountine()
    {
        StartCoroutine(SpawnRoutine());
    }

    IEnumerator SpawnRoutine()
    {
        while (true)
        {
            SpawnItem();
            yield return new WaitForSeconds(_spawnInterval);
        }
    }

    //아이템 스폰
    void SpawnItem()
    {
        //스폰될 종류(avoid vs target)의 확률 계산
        //refac 가능성있음.
        float itemTypeChance = Random.Range(0f, 100f);
        GameObject selectedPrefab = null;

        if(itemTypeChance <= _targetSpawnChance)
        {
            //target그룹 중 랜덤
            if (_targetPrefabs.Length > 0)
                selectedPrefab = _targetPrefabs[Random.Range(0, _targetPrefabs.Length)];
        }
        else
        {
            //avoid그룹 중 랜덤
            if (_avoidPrefabs.Length > 0)
                selectedPrefab = _avoidPrefabs[Random.Range(0, _avoidPrefabs.Length)];
        }

        if (selectedPrefab == null)
            return;

        //스폰 위치 계산
        float finalX, finalZ;
        float chance = Random.Range(0f, 100f); //가능성

        //40% 확률로 플레이어 주변 5m이내에 스폰
        if (_playerTransform != null && chance < 40f)
        {
            finalX = Random.Range(_playerTransform.position.x - 5f, _playerTransform.position.x + 5f);
            finalZ = Random.Range(_playerTransform.position.z - 5f, _playerTransform.position.z + 5f);
        }
        //60% 확률로 맵 전체에 랜덤하게 스폰
        else
        {
            finalX = Random.Range(-_mapRangeX, _mapRangeX);
            finalZ = Random.Range(-_mapRangeZ, _mapRangeZ);
        }
            
        Vector3 spawnPos = new Vector3(finalX, 17f, finalZ);

        GameObject item = PoolManager.Instance.GetItem(selectedPrefab); //PoolManager > GetItem
        item.transform.position = spawnPos;
        item.transform.rotation = Quaternion.identity;
    }

}
