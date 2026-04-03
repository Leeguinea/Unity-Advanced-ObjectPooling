using UnityEngine;

/*
 * [용도]
 * 게임 시스템 관리
 * [역할]
 * 1. 싱글톤 패턴 사용 (Managers.Instance로 접근 가능)
 * 2. 
 */
public class Managers : MonoBehaviour
{
    static Managers _instance; //유일성
    public static Managers Instance { get { Init(); return _instance; } }

    InputManager _input = new InputManager(); //내부
    public static InputManager Input { get { return Instance._input; } } //외부, 프로퍼티(통로)

    void Start()
    {
        Init();
    }

    void Update()
    {
        _input.OnUpdate();
    }

    static void Init()
    {
        if(_instance == null)
        {
            GameObject go = GameObject.Find("@Managers");
            if(go == null) 
            {
                go = new GameObject { name = "@Managers" };
                go.AddComponent<Managers>();
            }
            DontDestroyOnLoad(go);
            _instance = go.GetComponent<Managers>();    
        }
    }
}
