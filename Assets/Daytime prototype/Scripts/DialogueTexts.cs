using UnityEngine;

public class DialogueTexts : MonoBehaviour
{
    public string[] Architect;
    public string[] Artist;
    public string[] Lamplighter;
    public string[] Priest;

    public string[][] Dialogues;

    void Awake()
    {
        Architect = new string[]{
            "skibidi skibidi hakhakhak",
            "Second line",
            "whatever"
        };


       Artist = new string[]{
            "testing",
            "NPC@ 2 ashw"
        };
        
       Lamplighter = new string[]{
            "RAAAAAAAAAAAAAA",
            "NPC@ 2 ashw"
        };

        Priest = new string[]{
            "RAAAAAAAAAAAAAA",
            "NPC@ 2 ashw"
        };

        Dialogues = new string[][]{
            Architect, Artist, Lamplighter, Priest
        };
    }
}
