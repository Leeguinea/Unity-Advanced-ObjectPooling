using UnityEngine;

//배경음악 씬마다 별도로 재생 가능하게 함.
public class BGMSetter : MonoBehaviour
{
    [SerializeField] private AudioClip _sceneBGM; //씬에서 재생할 음악

    void Start()
    {
        if (_sceneBGM == null)
        {
            Debug.LogWarning($"{gameObject.name}에 재생할 BGM이 등록되지 않았습니다!");
            return;
        }

        if (_sceneBGM != null && SoundManager.Instance != null)
            SoundManager.Instance.PlayBGM(_sceneBGM);
    }
}
