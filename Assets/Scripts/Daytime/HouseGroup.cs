using Daytime;
using System.Collections.Generic;
using UnityEngine;

public class HouseGroup : MonoBehaviour
{
    SelectableHouse[] houses = new SelectableHouse[0];
    void Start()
    {
        houses = GetComponentsInChildren<SelectableHouse>();
        Recalculate();
    }

    public void Recalculate()
    {
        for (int i = 0; i < houses.Length; i++)
            houses[i].RecalculateDialogue();
    }
}
