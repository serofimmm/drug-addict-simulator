using UnityEngine;

public class hpbar : MonoBehaviour
{
    public float fill = 100f;

    void Start()
    {

    }

    void Update()
    {

        transform.parent.localScale = new Vector3(fill/100*2, 1f, 1f);
        transform.parent.position = new Vector3((fill/100*2 - 1f) * 0.5f, transform.parent.position.y, transform.parent.position.z);
    }
}