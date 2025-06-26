using Daytime;
using DialogueSystem;
using Player;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(FromCameraSelectable))]
public class Church : MonoBehaviour
{
    public UnityEvent OnClickNight = new UnityEvent();

    FromCameraSelectable _selectable;

    private void OnEnable()
    {
        _selectable = GetComponent<FromCameraSelectable>();
        _selectable.OnClicked.AddListener(OnClick);
    }

    private void OnDisable()
    {
        _selectable.OnClicked.RemoveListener(OnClick);
    }

    public void OnClick()
    {
        if(TimeHandler.IsNight())
            OnClickNight.Invoke();
    }
}
