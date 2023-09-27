using System.Collections;

using UnityEngine;

public class CameraShake : MonoBehaviour
{
    [SerializeField]
    private Transform _cameraTransform;

    private Vector3 _originalCameraPos;

    [SerializeField]
    private float _shakeIntensity = 0.25f;
    [SerializeField]
    private float _shakeDuration = 0.5f;
    [SerializeField]
    private float _shakeElapse = 0;
    // Start is called before the first frame update
    void Start()
    {
        _originalCameraPos = _cameraTransform.position;
    }

    // Update is called once per frame


    public void CameraMovement()
    {
        StartCoroutine(CameraMovementRoutine());
    }

    IEnumerator CameraMovementRoutine()
    {
        while (_shakeElapse <= _shakeDuration)
        {
            _cameraTransform.position = _originalCameraPos + Random.insideUnitSphere * _shakeIntensity;
            _shakeElapse += Time.deltaTime;
            yield return 0;
        }
        
        if (_shakeElapse >= _shakeDuration)
        {
            _cameraTransform.position = _originalCameraPos;
            _shakeElapse = 0;
        }
    }
   
}
