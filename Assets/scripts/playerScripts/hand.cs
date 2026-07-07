using UnityEngine;

public class hand : MonoBehaviour
{
    public Transform cameraTransform;
    public Transform handmain;

    public Vector3 offset;

    void LateUpdate()
    {
        handmain.position = cameraTransform.position + cameraTransform.TransformDirection(offset);
        handmain.rotation = cameraTransform.rotation;
    }
}

