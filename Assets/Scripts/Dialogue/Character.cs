using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "Character", menuName = "Dialogue/Character")]
public class Character : ScriptableObject
{
    public new string name;
    public Sprite sprite;
}
