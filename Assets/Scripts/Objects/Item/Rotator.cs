using UnityEngine;

public class Rotator : MonoBehaviour
{
    [SerializeField]
    Vector3 _rotationSpeed = new Vector3(0f, 0f, 90f);

    void Update()
    {
        transform.Rotate(_rotationSpeed * Time.deltaTime);
    }
}