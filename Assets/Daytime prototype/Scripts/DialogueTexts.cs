using UnityEngine;

public class DialogueTexts : MonoBehaviour
{
    public string[] NPC1;
    public string[] NPC2;

    public string[][] Dialogues;

    void Awake()
    {
        NPC1 = new string[]{
            "skibidi skibidi hakhakhak",
            "Second line",
            "whatever"
        };


        NPC2 = new string[]{
            "testing",
            "NPC@ 2 ashw"
        };

        Dialogues = new string[][]{
            NPC1, NPC2
        };
    }
}
