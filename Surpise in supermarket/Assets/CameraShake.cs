using UnityEngine;

public class CameraShake : MonoBehaviour
{
    public float shakeAmount = 5f;

    private Vector3 originalLocalPosition;
    private bool isShaking = false;

    void Start()
    {
        originalLocalPosition = transform.localPosition;
    }

    void LateUpdate()
    {
        if (isShaking)
        {
            Vector3 randomOffset = Random.insideUnitSphere * shakeAmount;
            transform.localPosition = originalLocalPosition + randomOffset;
        }
        else
        {
            transform.localPosition = originalLocalPosition;
        }
    }

    public void StartShake()
    {
        isShaking = true;
    }

    public void StopShake()
    {
        isShaking = false;
        transform.localPosition = originalLocalPosition;
    }
}