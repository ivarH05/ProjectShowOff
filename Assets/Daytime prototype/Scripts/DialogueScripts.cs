using System;
using System.Collections;
using Daytime;
using TMPro;
using UnityEngine;

public class DialogueScripts : MonoBehaviour
{
    public DialogueTexts dialogueTexts;
    public DialogueRoot dialogueRoot;
    public TextMeshProUGUI textComponents;
    public float textSpeed = 0.3f;

    public MyEnum Character = new MyEnum();

    public enum MyEnum
    {
        Architect, Artist, Lamplighter, Priest
    }

    

    [SerializeField] int index;

    void Start()
    {
        dialogueRoot.OnDialogueEnable += StartDialouge;
    }
    public void StartDialouge()
    {
        index = 0;

        StartCoroutine(Typelines(dialogueTexts.Dialogues[(int)Character]));
    }

    public void NextLine()
    {
        if (index < dialogueTexts.Dialogues[(int)Character].Length - 1)
        {
            index++;
            StartCoroutine(Typelines(dialogueTexts.Dialogues[(int)Character]));
        }
        else
        {
            dialogueRoot.Disable();
        }
    }

    public void Next()
    {
        if (textComponents.text == dialogueTexts.Dialogues[(int)Character][index])
        {
            NextLine();
        }
        else
        {
            StopAllCoroutines();
            textComponents.text = dialogueTexts.Dialogues[(int)Character][index];
        }
    }

    IEnumerator Typelines(string[] LineArray)
    {
        textComponents.text = string.Empty;
        foreach (char c in LineArray[index])
        {
            textComponents.text += c;
            yield return new WaitForSeconds(textSpeed);
        }
    }

}


