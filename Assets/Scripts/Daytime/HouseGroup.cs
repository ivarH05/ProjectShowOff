using Daytime;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HouseGroup : MonoBehaviour
{
    SelectableHouse[] houses = new SelectableHouse[0];
    public PopupManager popupManager;
    void Start()
    {
        houses = GetComponentsInChildren<SelectableHouse>();
        Recalculate();
    }

    public void Recalculate()
    {
        for (int i = 0; i < houses.Length; i++)
            houses[i].RecalculateDialogue();

        var popupData = GetPopups();
        popupManager.Recalculate(popupData.vectors, popupData.sprites);
    }

    public (Vector3[] vectors, Sprite[] sprites) GetPopups()
    {
        List<Vector3> popups = new List<Vector3>();
        List<Sprite> sprites = new List<Sprite>();
        for (int i = 0; i < houses.Length; i++)
            if (houses[i].enabled)
            {
                popups.Add(houses[i].transform.position);
                sprites.Add(houses[i].popupImage);
            }
        return (popups.ToArray(), sprites.ToArray());
    }
}
