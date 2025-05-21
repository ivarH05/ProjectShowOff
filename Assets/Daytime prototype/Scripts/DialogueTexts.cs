using UnityEngine;

public class DialogueTexts : MonoBehaviour
{
    public string[] NPC1;
    public string[] NPC2;

    public string[][] Dialogues;

    void Awake()
    {
        NPC1 = new string[]{
            "Text is fundamental to visual novels, and generally quite important to storytelling-based games. This text may consist of dialogue labeled with the character that is saying it, and narration, which does not have a speaker. (For convenience, we will lump both dialogue and narration together as dialogue",
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
