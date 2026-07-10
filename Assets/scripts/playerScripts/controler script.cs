using UnityEngine;
using UnityEngine.InputSystem;

public class controlerscript : MonoBehaviour
{
    void Start()
    {
        
    }

    void Update()
    {
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            transform.position += new Vector3(Mouse.current.delta.ReadValue().x, Mouse.current.delta.ReadValue().y, 0);
        }
    }
}
