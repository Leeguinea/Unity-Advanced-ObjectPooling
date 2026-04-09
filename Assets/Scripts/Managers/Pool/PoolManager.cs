using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class PoolManager : MonoBehaviour
{
    public static PoolManager Instance;
    float delay = 3f;

    [System.Serializable]
    public class Pool
    {
        public string name;          // 풀의 이름 (관리용)
        public GameObject prefab;    // 원본 프리팹
        public int size;             // 미리 생성할 개수
        public bool isGenerate = false; //코루틴중인지 확인  
    }

    [Header("Pool Configurations")]
    [SerializeField]
    private List<Pool> _poolConfigs; // 인스펙터에서 11종을 각각 설정 가능

    // 실제 오브젝트들을 담아둘 창고
    private Dictionary<int, Queue<GameObject>> _pools = new Dictionary<int, Queue<GameObject>>();
    private Dictionary<GameObject, PoolItem> _itemMap = new Dictionary<GameObject, PoolItem>();
    private Dictionary<int, Pool> _poolMap = new Dictionary<int, Pool>();

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

            int key = pool.prefab.GetInstanceID();

            //_pools
            if (!_pools.ContainsKey(key))
            {
                _pools.Add(key, new Queue<GameObject>());

                for (int i = 0; i < pool.size; i++)
                {
                    GameObject obj = CreateNewObject(pool.prefab);
                    obj.SetActive(false);
                    _pools[key].Enqueue(obj);
                }
            }

            //코루틴_실행중인지 정보
            if(!_poolMap.ContainsKey(key))
            {
                _poolMap.Add(key, pool);
            }
        }
    }

    //프리팹 분류
    private GameObject CreateNewObject(GameObject prefab)
    {
        GameObject obj = Instantiate(prefab);

        PoolItem item = obj.AddComponent<PoolItem>();
        item.myID = prefab.GetInstanceID();

        _itemMap.Add(obj, item);
        return obj;
    }


    //아이템 꺼내기 
    public GameObject GetItem(GameObject prefab)
    {
        int key = prefab.GetInstanceID();

        // 만약 풀이 아예 없다면 새로 생성 (예외 방지)
        if (!_pools.ContainsKey(key)) _pools.Add(key, new Queue<GameObject>());
        
        Pool p = _poolMap[key];

        if (p.isGenerate == false && _pools[key].Count == 0)
        {
            p.isGenerate = true;
            //에러 방지로 1개 생성 
            GameObject tempObj = CreateNewObject(prefab);
            tempObj.SetActive(true);
            _pools[key].Enqueue(tempObj);

            StartCoroutine(FillPoolRoutine(p, 100));
        }

        //아이템 꺼내기
        GameObject useObj = _pools[key].Dequeue();
        useObj.SetActive(true);

        StartCoroutine(AutoReturn(useObj, delay));
        return useObj;
    }

    //아이템 생성
    private IEnumerator FillPoolRoutine(Pool p, int count)
    {
        int key = p.prefab.GetInstanceID();

        for (int i = 0; i < count; i++)
        {
            GameObject obj = CreateNewObject(p.prefab);
            obj.SetActive(false);
            _pools[key].Enqueue(obj);

            if (i % 5 == 0)
                yield return null;
        }
        p.isGenerate = false;
    }

    // 사용된 오브젝트 반납
    public void ReturnItem(GameObject obj)
    {
        if(!obj.activeSelf) return;

        if (_itemMap.TryGetValue(obj, out PoolItem item))
        {
            int Key = item.myID;
            obj.SetActive(false);
            _pools[Key].Enqueue(obj);
        }
    }

    public IEnumerator AutoReturn(GameObject obj, float delay)
    {
        yield return new WaitForSeconds(delay);
        ReturnItem(obj);
    }

   
}