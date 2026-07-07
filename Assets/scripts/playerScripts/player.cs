using System.Collections;
using System.Threading.Tasks;
using TMPro;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

public class player : MonoBehaviour
{
    [SerializeField] private float mouseSensitivity = 0.2f;

    private float xRotation;
    private float yRotation;

    private int groundContacts = 0;

    public float speed = 5f;
    public int hp = 100;
    private hpbar script;
    public float money = 1000;
    TMP_Text balance;
    int oldhp = 100;
    GameObject damage;
    GameObject colideObject;
    bool isGrounded;
    public float jumpForce = 30f;
    Rigidbody rb;
    public string work;
    GameObject[] inventory = new GameObject[8];
    GameObject sugar;
    public int selectedSlot = 0;
    GameObject hand;
    GameObject item;
    GameObject lastitem;
    CanvasGroup novell;
    GameObject nullObject;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        script = GameObject.Find("hpbar").GetComponent<hpbar>();
        balance = GameObject.Find("money").GetComponent<TextMeshProUGUI>();
        damage = GameObject.Find("damage");
        damage.SetActive(false);
        colideObject = GameObject.Find("colide object");
        rb = transform.parent.GetComponent<Rigidbody>();
        hand = GameObject.Find("hand");
        
        novell = GameObject.Find("novell").GetComponent<CanvasGroup>();
        Debug.Log(novell.name);
        nullObject = GameObject.Find("null");
        for (int i = 0; i < 8; i++)
        {
            inventory[i] = nullObject;
            Debug.Log("slot " + i);
        }
        inventory[0] = Resources.Load<GameObject>("Prefabs/sugar");

    }

    void Update()
    {
        
        if (inventory[selectedSlot] != null)
        {
            item = inventory[selectedSlot];
        }

        if (item != null && item != nullObject)
        {
            if (lastitem == null || lastitem.name != $"{item.name}(Clone)")
            {
                Destroy(lastitem);
                float scaleX = item.transform.localScale.x;
                float scaleY = item.transform.localScale.y;
                float scaleZ = item.transform.localScale.z;
                Debug.Log(scaleX + " " + scaleY + " " + scaleZ);
                GameObject itemN = Instantiate(inventory[selectedSlot], hand.transform.Find("item"));
                itemN.transform.localPosition = Vector3.zero;
                itemN.transform.localRotation = Quaternion.identity;
                itemN.transform.localScale = new Vector3(scaleX, scaleY, scaleZ);
                lastitem = itemN;
            }
        }
        else
        {
            Destroy(lastitem);
            Destroy(item);
            lastitem = null;
        }
        if (work == "loader" && GameObject.Find("punktA(Clone)") == null)
        {
            Debug.Log("������� ������");
            Instantiate(Resources.Load<GameObject>("Prefabs/punktA"), new Vector3(-3, -1, 5), Quaternion.identity);
            Instantiate(Resources.Load<GameObject>("Prefabs/punktB"), new Vector3(2, -1, 2), Quaternion.identity);
        }
        Vector2 mousePos = Mouse.current.position.ReadValue();
        script.fill = hp;
        balance.text = money + "$";
        if (Keyboard.current.wKey.isPressed)
        {
            transform.parent.position += Camera.main.transform.forward * Time.deltaTime * speed;
        }
        if (Keyboard.current.sKey.isPressed)
        {
            transform.parent.position -= Camera.main.transform.forward * Time.deltaTime * speed;
        }
        if (Keyboard.current.aKey.isPressed)
        {
            transform.parent.position -= Camera.main.transform.right * Time.deltaTime * speed;
        }
        if (Keyboard.current.dKey.isPressed)
        {
            transform.parent.position += Camera.main.transform.right * Time.deltaTime * speed;
        }
        if (Keyboard.current.spaceKey.wasPressedThisFrame && isGrounded == true)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
        if (Keyboard.current.leftShiftKey.isPressed)
        {
            speed = 15f;
        }
        if (Keyboard.current.leftShiftKey.wasReleasedThisFrame)
        {
            speed = 5f;
        }
        if (transform.parent.position.y <= -40f)
        {
            hp -= 10;
        }
        if (Keyboard.current.digit1Key.wasPressedThisFrame)
        {
            selectedSlot = 0;
        }
        if (Keyboard.current.digit2Key.wasPressedThisFrame)
        {
            selectedSlot = 1;
        }
        if (Keyboard.current.digit3Key.wasPressedThisFrame)
        {
            selectedSlot = 2;
        }
        if (Keyboard.current.digit4Key.wasPressedThisFrame)
        {
            selectedSlot = 3;
        }
        if (Keyboard.current.digit5Key.wasPressedThisFrame)
        {
            selectedSlot = 4;
        }
        if (Keyboard.current.digit6Key.wasPressedThisFrame)
        {
            selectedSlot = 5;
        }
        if (Keyboard.current.digit7Key.wasPressedThisFrame)
        {
            selectedSlot = 6;
        }
        if (Keyboard.current.digit8Key.wasPressedThisFrame)
        {
            selectedSlot = 7;
        }
        
        Vector2 mouseDelta = Mouse.current.delta.ReadValue() * mouseSensitivity;

        yRotation += mouseDelta.x;
        xRotation -= mouseDelta.y;

        xRotation = Mathf.Clamp(xRotation, -80f, 80f);

        Camera.main.transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        transform.parent.rotation = Quaternion.Euler(0f, yRotation, 0f);

        if (hp <= 0)
        {
            transform.parent.position = new Vector3(0, 0, 0);
            transform.parent.rotation = Quaternion.Euler(0, 0, 0);
            hp = 100;
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }
        if (oldhp > hp)
        {
            oldhp = hp;
            StartCoroutine(ShowDamageScreen());
        }
    }
    void FixedUpdate()
    {

    }
    IEnumerator ShowDamageScreen()
    {
        damage.SetActive(true);
        yield return new WaitForSeconds(0.1f);
        damage.SetActive(false);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("ground"))
        {
            groundContacts++;
            isGrounded = true;
        }

        if (other.CompareTag("damage"))
        {
            hp -= 10;
        }

        if (other.CompareTag("money add"))
        {
            money += 100;
        }

        if (other.CompareTag("money minus"))
        {
            money -= 100;
        }

        if (other.CompareTag("robert"))
        {
            novell.alpha = 1;
        }

        if (other.CompareTag("work"))
        {
            work = other.gameObject.name;
            Debug.Log("work: " + work);
        }

        if (other.gameObject.name == "punktA(Clone)")
        {
            inventory[2] = Resources.Load<GameObject>("Prefabs/box");
            selectedSlot = 2;
        }

        if (other.gameObject.name == "punktB(Clone)")
        {
            if (inventory[2] == Resources.Load<GameObject>("Prefabs/box"))
            {
                inventory[2] = nullObject;
                money += 10;
            }
        }
    }
    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("ground"))
        {
            groundContacts--;

            if (groundContacts <= 0)
            {
                groundContacts = 0;
                isGrounded = false;
            }
        }

        if (other.CompareTag("robert"))
        {
            novell.alpha = 0;
        }
    }

}
