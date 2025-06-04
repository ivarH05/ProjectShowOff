using UnityEngine;

public class CameraPeekController : MonoBehaviour
{
    private float direction;

    public float distance = 0.2f;
    public float rotation = 15f;

    public float speed = 10;
    public void SetDirection(float direction) => this.direction = direction;


    // Update is called once per frame
    void Update()
    {
        if (!CanMove())
            direction = 0;
        Move(direction);
    }

    void Move(float direction)
    {
        transform.localPosition = new Vector3(Mathf.Lerp(transform.localPosition.x, direction * distance, Time.deltaTime * speed), 0, 0);
        Vector3 euler = new Vector3(0, 0, -direction * rotation);
        Quaternion q = Quaternion.Euler(euler);
        transform.localRotation = Quaternion.Lerp(transform.localRotation, q, Time.deltaTime * speed);
    }

    bool CanMove()
    {
        if (direction == 0)
            return true;

        return !Physics.Raycast(transform.position, transform.right * direction, 0.1f);
    }
}
