using Microsoft.Unity.VisualStudio.Editor;
using System.Collections;
using System.Threading.Tasks;
using TMPro;
using Unity.VisualScripting;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.InputSystem;

public class player : MonoBehaviour
{
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
    public GameObject[] inventory = new GameObject[8];
    GameObject sugar;
    public int selectedSlot = 0;
    GameObject hand;
    GameObject item;
    GameObject lastitem;
    CanvasGroup novell;
    GameObject nullObject;
    void Start()
    {
        Cursor.lockState = CursorLockMode.Confined;
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

        if (GameObject.Find($"{inventory[selectedSlot].name}(Clone)") == null)
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
        if(work == "loader" && GameObject.Find("punktA(Clone)") == null)
        {
            Debug.Log("спавним поинты");
            Instantiate(Resources.Load<GameObject>("Prefabs/punktA"), new Vector3(-3, -1, 5), Quaternion.identity);
            Instantiate(Resources.Load<GameObject>("Prefabs/punktB"), new Vector3(2, 0, 2), Quaternion.identity);
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
        Camera.main.transform.rotation = Quaternion.Euler(Mathf.Lerp(60f, -60f, mousePos.y / Screen.height), mousePos.x, 0);
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
        Debug.Log("ok");
        if (other.CompareTag("damage"))
        {
            hp -= 10;
            isGrounded = true;
        }

        if (other.CompareTag("money add"))
        {
            money += 100;
            isGrounded = true;
        }

        if (other.CompareTag("money minus"))
        {
            money -= 100;
            isGrounded = true;
        }
        if (other.CompareTag("ground"))
        {
            isGrounded = true;
        }
        if(other.CompareTag("robert"))
        {
            novell.alpha = 1;
        }
        if (other.CompareTag("work"))
        {
            work = other.gameObject.name;
            Debug.Log("work: " + work);
        }
        if(other.gameObject.name == "punktA(Clone)")
        {
            inventory[2] = Resources.Load<GameObject>("Prefabs/box");
            selectedSlot = 2;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("ground") && other.CompareTag("money minus") && other.CompareTag("money add") && other.CompareTag("damage"))
        {
            isGrounded = false;
        }
        if (other.CompareTag("robert"))
        {
            novell.alpha = 0;
        }
    }

}
