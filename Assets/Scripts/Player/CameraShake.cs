using UnityEngine;

public class CameraShake : MonoBehaviour
{
    private static CameraShake _singleton;
    [SerializeField] private AnimationCurve _shakeCurve;

    private float _time = 100;
    private float _duration = 1;
    private float _magnitude;


    void Start()
    {
        _singleton = this;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        float lerpedValue = Mathf.Clamp01(_time / _duration);
        transform.localEulerAngles = Random.insideUnitSphere * _magnitude * 50 * _shakeCurve.Evaluate(lerpedValue);
        transform.localPosition = Random.insideUnitSphere * _magnitude * _shakeCurve.Evaluate(lerpedValue);

        _time += Time.deltaTime;
    }

    public static void ShakeCamera(float duration, float magnitude)
    {
        _singleton._time = 0;
        _singleton._duration = duration;
        _singleton._magnitude = magnitude;
    }
}
