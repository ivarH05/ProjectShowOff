using UnityEngine;

public class GlitchEffectController : MonoBehaviour
{
    [SerializeField] float maxDistance;
    [SerializeField] Transform enemy;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        float distance = Vector3.Distance(transform.position, enemy.position);
        distance = distance / maxDistance;

        float value = 1 - Mathf.Clamp01(distance - 0.5f);
        GlitchEffectHandler.SetStrength(value);
    }
}
