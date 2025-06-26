using UnityEngine;
using UnityEngine.UI;

public enum Expression { Default, Happy, Sad, Angry, Traumatized }

[CreateAssetMenu(fileName = "Character", menuName = "Dialogue/Character")]
public class Character : ScriptableObject
{
    public new string name;
    public Sprite sprite;
    [SerializeField]private ExpressionSpritePair[] expressions;

    public Sprite GetSprite(Expression expression)
    {
        for (int i = 0; i < expressions.Length; i++)
            if (expressions[i].expression == expression)
                return expressions[i].sprite;
        return sprite;
    }

    [System.Serializable]
    private struct ExpressionSpritePair
    {
        public Expression expression;
        public Sprite sprite;
    }
}
