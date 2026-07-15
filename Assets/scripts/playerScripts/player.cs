using System.Collections;
using System.Collections.Generic;
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
    public int hangry = 100;
    int speedhangry = 5;
    GameObject head;
    RectTransform hangryEl;
    float startHangry;
    Dictionary<string, int> smokedcigarettes = new Dictionary<string, int>();
    int ids = 0;

    void Start()
    {
        
        script = GameObject.Find("hpbar").GetComponent<hpbar>();
        balance = GameObject.Find("money").GetComponent<TextMeshProUGUI>();
        damage = GameObject.Find("damage");
        damage.SetActive(false);
        colideObject = GameObject.Find("colide object");
        rb = transform.parent.GetComponent<Rigidbody>();
        hand = GameObject.Find("hand");
        head = GameObject.Find("head");
        novell = GameObject.Find("novell").GetComponent<CanvasGroup>();
        Debug.Log(novell.name);
        nullObject = GameObject.Find("null");
        hangryEl = GameObject.Find("hangryBar").GetComponent<RectTransform>();
        startHangry = hangryEl.anchoredPosition.x;
        Debug.Log(startHangry);
        for (int i = 0; i < 8; i++)
        {
            inventory[i] = nullObject;
            Debug.Log("slot " + i);
        }
        inventory[0] = Resources.Load<GameObject>("Prefabs/sugar");
        inventory[1] = Resources.Load<GameObject>("Prefabs/cigarettes");
        int id = randomID();
        inventory[1].name = "cigarettes-"+id;
        smokedcigarettes["cigarettes-" + id] = 1;
        StartCoroutine(hangrytick());
    }

    void Update()
    {
        item = inventory[selectedSlot];

        if (item != null && item != nullObject)
        {
            if (lastitem == null || lastitem.name != $"{item.name}(Clone)")
            {
                Destroy(lastitem);
                Debug.Log(inventory);
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
            inventory[selectedSlot] = nullObject;
        }
        if (work == "loader" && GameObject.Find("punktA(Clone)") == null)
        {
            Debug.Log("������� ������");
            Instantiate(Resources.Load<GameObject>("Prefabs/punktA"), new Vector3(-3, -1, 5), Quaternion.identity);
            Instantiate(Resources.Load<GameObject>("Prefabs/punktB"), new Vector3(2, -1, 2), Quaternion.identity);
        }
        if(inventory[selectedSlot] != nullObject)
        {
            if (inventory[selectedSlot].tag == "cigarettes")
            {
                Debug.Log("держыш в руках");
                for (int i = 1; i <= smokedcigarettes[inventory[selectedSlot].name]; i++)
                {
                    
                    Destroy(GameObject.Find("cigarette" + i));
                }

                if (Mouse.current.leftButton.wasPressedThisFrame)
                {
                    Destroy(inventory[selectedSlot].transform.Find("cigarette" + smokedcigarettes));
                    smokedcigarettes[inventory[selectedSlot].name]++;
                }
            }
        }
            Vector2 mousePos = Mouse.current.position.ReadValue();
        script.fill = hp;
        balance.text = money + "$";
        float value = startHangry - (605 - hangry);
        hangryEl.anchoredPosition = new Vector2(value, hangryEl.anchoredPosition.y);
        Debug.Log(Mathf.Lerp(startHangry - 100, startHangry, hangry / 100f));
        
        if (Keyboard.current.spaceKey.wasPressedThisFrame && isGrounded == true)
        {
            speedhangry = 2;
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            speedhangry = 5;
        }
        if (Keyboard.current.leftShiftKey.isPressed)
        { 
            speed = 15f;
            speedhangry = 2;
        }
        if (Keyboard.current.leftShiftKey.wasReleasedThisFrame)
        {
            speed = 5f;
            speedhangry = 5;
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

        head.transform.localRotation = Quaternion.Euler(0f, 0f, -xRotation);
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
        Vector3 move = Vector3.zero;

        Vector3 forward = Camera.main.transform.forward;
        Vector3 right = Camera.main.transform.right;

        forward.y = 0;
        right.y = 0;

        forward.Normalize();
        right.Normalize();


        if (Keyboard.current.wKey.isPressed)
        {
            move += forward;
        }
        if (Keyboard.current.sKey.isPressed)
        {
            move -= forward;
        }
        if (Keyboard.current.aKey.isPressed)
        {
            move -= right;
        }
        if (Keyboard.current.dKey.isPressed)
        {
            move += right;
        }

        transform.parent.position += move.normalized * speed * Time.fixedDeltaTime;
    }
    IEnumerator ShowDamageScreen()
    {
        damage.SetActive(true);
        yield return new WaitForSeconds(0.1f);
        damage.SetActive(false);
    }

    IEnumerator hangrytick()
    {
        while(hangry > 0)
        {
            yield return new WaitForSeconds(speedhangry);
            hangry--;
        }
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

        if (other.CompareTag("shop"))
        {
            if (other.gameObject.name == "robert")
            {
                novell.alpha = 1;
                novell.transform.Find("text").GetComponent<TextMeshProUGUI>().text = "Robert: привет я Роберт продавец запрещеных вещей, хотите ли вы чтото купить?";
                novell.transform.Find("Button1").GetComponentInChildren<TextMeshProUGUI>().text = "Да";
                novell.transform.Find("Button2").GetComponentInChildren<TextMeshProUGUI>().text = "Нет";
                novell.transform.Find("Button2").GetComponent<UnityEngine.UI.Button>().onClick.AddListener(() =>
                {
                    novell.alpha = 0;
                });
                novell.transform.Find("Button1").GetComponent<UnityEngine.UI.Button>().onClick.AddListener(() =>
                {
                    novell.transform.Find("text").GetComponent<TextMeshProUGUI>().text = "Robert: отлично, у меня есть сахар и сигареты, что вы хотите купить?";
                    novell.transform.Find("Button1").GetComponentInChildren<TextMeshProUGUI>().text = "Сахар 20$/грам";
                    novell.transform.Find("Button2").GetComponentInChildren<TextMeshProUGUI>().text = "Сигареты 5$";
                    novell.transform.Find("Button1").GetComponent<UnityEngine.UI.Button>().onClick.AddListener(() =>
                    {
                        if (money >= 20)
                        {
                            money -= 20;
                            inventory[0] = Resources.Load<GameObject>("Prefabs/sugar");
                        }
                    });
                    novell.transform.Find("Button2").GetComponent<UnityEngine.UI.Button>().onClick.AddListener(() =>
                    {
                        if (money >= 5)
                        {
                            money -= 5;
                            for(int i = 0; i < 8; i++)
                            {
                                if (inventory[i] == nullObject)
                                {
                                    int id = randomID();
                                    inventory[i] = Resources.Load<GameObject>("Prefabs/cigarettes");
                                    inventory[i].name = "cigarettes-" + id;
                                    smokedcigarettes["cigarettes-" + id] = 1;
                                    break;
                                }
                            }
                        }
                    });
                });
                Cursor.visible = true;
            }  
        }

        if (other.CompareTag("work"))
        {
            work = other.gameObject.name;
            Debug.Log("work: " + work);
        }

        if (other.gameObject.name == "punktA(Clone)")
        {
            for (int i = 0; i < 8; i++)
            {
                if (inventory[i] == nullObject)
                {
                    inventory[i] = Resources.Load<GameObject>("Prefabs/box");
                    selectedSlot = i;
                    break;
                }
            }
        }

        if (other.gameObject.name == "punktB(Clone)")
        {
            for (int i = 0; i < 8; i++)
            {
                if (inventory[i] == Resources.Load<GameObject>("Prefabs/box"))
                {
                    inventory[i] = nullObject;
                    money += 10;
                    break;
                }
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

        if (other.CompareTag("shop"))
        {
            novell.alpha = 0;
            Cursor.visible = false;
        }
    }
    int randomID()
    {
        return ids++;
    }

}
