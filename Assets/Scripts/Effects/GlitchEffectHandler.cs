using UnityEngine;
using UnityEngine.Rendering;
using static UnityEngine.Rendering.HighDefinition.CameraSettings;

[RequireComponent(typeof(Volume))]
public class GlitchEffectHandler : MonoBehaviour
{
    [SerializeField] private Material _material;

    public float strengthMultiplier = 1;

    private Volume _volume;
    private float _timer;
    private float _strength;

    private static GlitchEffectHandler _singleton;

    public static void SetStrength(float value) => _singleton._strength = value;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _singleton = this;
        _volume = GetComponent<Volume>();
    }

    // Update is called once per frame
    void Update()
    {
        _timer -= Time.deltaTime;
        if (_timer > 0)
            return;

        _volume.weight = _strength;
        _material.SetFloat("_OffsetStrength", _strength * 0.025f * strengthMultiplier);
        _material.SetFloat("_ChromaticAberationStrength", _strength * 0.015f * strengthMultiplier);
        _material.SetFloat("_Seed", Random.value * 5000000);
        _timer = Mathf.Clamp(Mathf.Pow(Random.value * 2 - 1, 10), 0.03f, 0.7f);
    }

    private void OnDestroy()
    {
        _volume.weight = 0;
        _material.SetFloat("_OffsetStrength", 0);
        _material.SetFloat("_ChromaticAberationStrength", 0);
        _material.SetFloat("_Seed", 0);
    }
}
