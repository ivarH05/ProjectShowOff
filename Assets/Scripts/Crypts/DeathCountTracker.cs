using DialogueSystem;
using UnityEngine;

public class DeathCountTracker : MonoBehaviour
{
    [SerializeField] private DialogueConditionFlag[] flags;

    private static DeathCountTracker _singleton;
    private static int deathCount;

    private static bool died = false;

    public void Increment()
    {
        if (died)
            return;

        died = true;
        if(deathCount < flags.Length)
            flags[deathCount].Set(true);
        deathCount++;
    }

    public void Reenable()
    {
        died = false;
    }
}
