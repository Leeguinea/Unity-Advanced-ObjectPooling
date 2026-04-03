using UnityEngine;
using System.Collections.Generic;

public class PoolManager : MonoBehaviour
{
    public static PoolManager Instance;

    [System.Serializable]
    public class Pool
    {
        public string name;          // 풀의 이름 (관리용)
        public GameObject prefab;    // 원본 프리팹
        public int size;             // 미리 생성할 개수
    }

    [Header("Pool Configurations")]
    [SerializeField]
    private List<Pool> _poolConfigs; // 인스펙터에서 11종을 각각 설정 가능

    // 실제 오브젝트들을 담아둘 창고
    private Dictionary<string, Queue<GameObject>> _pools = new Dictionary<string, Queue<GameObject>>();

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        InitPools();
    }

    // 게임 시작 시 모든 프리팹을 설정된 개수만큼 미리 생성 (Pre-warm)
    private void InitPools()
    {
        foreach (Pool pool in _poolConfigs)
        {
            if (pool.prefab == null) continue;

            string key = pool.prefab.name;
            if (!_pools.ContainsKey(key))
            {
                _pools.Add(key, new Queue<GameObject>());

                for (int i = 0; i < pool.size; i++)
                {
                    GameObject obj = CreateNewObject(pool.prefab);
                    _pools[key].Enqueue(obj);
                }
            }
        }
    }

    private GameObject CreateNewObject(GameObject prefab)
    {
        GameObject obj = Instantiate(prefab, transform);
        obj.name = prefab.name;
        obj.SetActive(false); // 생성 직후에는 비활성화
        return obj;
    }

    public GameObject GetItem(GameObject prefab)
    {
        string key = prefab.name;

        // 1. 만약 풀이 아예 없다면 새로 생성 (예외 방지)
        if (!_pools.ContainsKey(key))
        {
            _pools.Add(key, new Queue<GameObject>());
        }

        // 2. 풀에 사용 가능한 오브젝트가 있다면 꺼내기
        if (_pools[key].Count > 0)
        {
            GameObject obj = _pools[key].Dequeue();
            obj.SetActive(true);
            return obj;
        }

        // 3. 풀이 비어있다면 즉석에서 새로 생성해서 전달
        return CreateNewObject(prefab);
    }

    // 사용이 끝난 아이템을 다시 창고로 반납할 때 사용
    public void ReturnItem(GameObject obj)
    {
        string key = obj.name;
        if (_pools.ContainsKey(key))
        {
            obj.SetActive(false);
            _pools[key].Enqueue(obj);
        }
        else
        {
            Destroy(obj); // 혹시 모를 예외 시 삭제
        }
    }
}