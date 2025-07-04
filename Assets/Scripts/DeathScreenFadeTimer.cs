using GameManagement;
using UnityEngine;

public class DeathScreenFadeTimer : MonoBehaviour
{
    float timer = 0;
    public SceneFader sceneTransitioner;
    public string targetScene = "Week9Daytime";
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        if (timer > 3)
        {
            sceneTransitioner.TransitionScene(targetScene);
        }
    }
}
