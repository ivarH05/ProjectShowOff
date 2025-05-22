using GameManagement;
using UnityEngine;

public class BasicTutorial : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Cursor.lockState = CursorLockMode.None;
        for (int i = 0; i < PlayerManager.PlayerCount; i++)
            PlayerManager.GetPlayer(i).gameObject.SetActive(false);
    }

    public void Close()
    {
        for (int i = 0; i < PlayerManager.PlayerCount; i++)
            PlayerManager.GetPlayer(i).gameObject.SetActive(true);
        Cursor.lockState = CursorLockMode.Locked;
    }
}
