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
            "A short haired girl bumps into you, her pencils and papers spreading across the ground from her bag. You help her gather it all",
            "Oh, h-hello... I-I am so so sorry.. I'm the architect of the city, I was just heading to the new project..",
            "The crypt? Yes, I do know about it, I was the one that designed it..",
            "I will give you a map for it, the priest of the city should also tell you more about it.."
        };


       Artist = new string[]{
            "The girl cheerfully approaches you, her basket of flowers spinning around in the air",
            "Hello, traveller! Oh, me? I'm an artist, I think the brushes give it away..",
            "I love painting anything, flowers, animals, people.. But especially the past, it fascinates me.."
        };
        
       Lamplighter = new string[]{
            "You make eye contact, the girl pulling her hat down to hide a yawn before talking",
            "Is it morning already.. Oh.. hello.. I stayed up all night, I want to go to sleep now..",
            "I hope.. I won't have nightmares when I do..",
            "Why..? Well, uh.. I have to light up all the lamps in the city for people to see at night..",
            "But.. It's sometimes so, so scary... I see these creatures.. and hear afraid screams.. But then.. I blink twice and it's gone..",
            "So.. I just blink, yawn, and go back to my job.."
        };

        Priest = new string[]{
            "A stern  look hits your direction, the man adjusting his robe. A forced smile forming on his face",
            "Hello, my child, what brings you around here?",
            "I have just finished blessing one of the bodies back down there.. Oh, how will they be missed..",
            "You felt something was fake in his voice.."
        };

        Dialogues = new string[][]{
            Architect, Artist, Lamplighter, Priest
        };
    }
}
