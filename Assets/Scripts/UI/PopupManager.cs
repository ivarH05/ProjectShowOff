using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PopupManager : MonoBehaviour
{
    public GameObject PopupPrefab;
    public List<Image> instances = new List<Image>();
    public Vector3[] positions;
    public float heightOffset;

    public void Update()
    {
        for (int i = 0; i < instances.Count; i++)
            instances[i].transform.position = Camera.main.WorldToScreenPoint(positions[i]) + new Vector3(0, heightOffset, 0);
    }

    public void Recalculate(Vector3[] worldPositions, Sprite[] sprites)
    {
        int length = worldPositions.Length;
        if(length > instances.Count)
        {
            for (int i = instances.Count; i < length; i++)
                instances.Add(Instantiate(PopupPrefab, transform).GetComponent<Image>());
        }
        else
        {
            for (int i = length - 1; i < instances.Count; i++)
            {
                Destroy(instances[i]);
                instances.RemoveAt(i);
            }
        }

        for (int i = 0; i < sprites.Length && i < instances.Count; i++)
            instances[i].sprite = sprites[i];
        positions = worldPositions;
    }
}
