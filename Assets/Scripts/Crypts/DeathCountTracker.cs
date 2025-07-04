using DialogueSystem;
using UI;
using UnityEngine;
using UnityEngine.Rendering;

public class DeathCountTracker : MonoBehaviour
{
    [SerializeField] private DialogueConditionFlag[] flags;

    private static DeathCountTracker _singleton;
    private static int deathCount;

    private static bool died = false;

    private void Awake()
    {
        _singleton = this;
    }
    public void CallDeath()
    {
        UIManager.SetState<DeathUIState>();
        if (died)
            return;

        died = true;
        if(deathCount < flags.Length)
            flags[deathCount].Set(true);
        deathCount++;
    }

    public static void Death() => _singleton.CallDeath();

    public void Reenable()
    {
        died = false;
    }
}
