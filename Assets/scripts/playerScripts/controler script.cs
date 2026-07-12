using UnityEngine;
using UnityEngine.InputSystem;

public class controlerscript : MonoBehaviour
{
    public float x;
    public float y;

    Transform parent;

    void Start()
    {
        parent = transform.parent;

        if (!Application.isMobilePlatform)
        {
            parent.gameObject.SetActive(false);
            gameObject.SetActive(false);
        }
    }

    void Update()
    {
        Vector2 pos;

        if (Touchscreen.current != null && Touchscreen.current.primaryTouch.press.isPressed)
        {
            pos = Touchscreen.current.primaryTouch.position.ReadValue();

            Vector3 dir = pos - (Vector2)parent.position;

            transform.position = parent.position + Vector3.ClampMagnitude(dir, parent.localScale.x * 50);

            x = dir.x;
            y = dir.y;
        }
        else
        {
            transform.position = parent.position;
            x = 0;
            y = 0;
        }
    }
}