using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TMPro;
using TMPro.Examples;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class player : MonoBehaviour
{
    [SerializeField] private float mouseSensitivity = 0.2f;

    private float xRotation;
    private float yRotation;

    private int groundContacts = 0;
    Vector3[] redPoints = {new Vector3(29.67535f, -1.5f, 17.06004f), new Vector3(24, -1.5f, 7.93f), new Vector3(26.96f, -1.5f, 29.29f)};
    Vector3[] greenPoints = { new Vector3(12.39252f, -1.3f, 27.86517f), new Vector3(12.39252f, -1.3f, 18.97f), new Vector3(12.39252f, -1.3f, 11.52f), new Vector3(12.39252f, -1.3f, 5.22f) };
    public float speed = 5f;
    public int hp = 100;
    private hpbar script;
    public float money = 1000;
    TMP_Text balance;
    int oldhp = 100;
    GameObject damage;
    GameObject colideObject;
    bool isGrounded;
    public float jumpForce = 50f;
    Rigidbody rb;
    public string work;
    InventoryItem[] inventory = new InventoryItem[8];
    GameObject sugar;
    public int selectedSlot = 0;
    GameObject hand;
    InventoryItem item;
    GameObject lastitem;
    InventoryItem lastInvItem;
    CanvasGroup novell;
    GameObject nullObject;
    public int hangry = 100;
    int speedhangry = 5;
    GameObject head;
    RectTransform hangryEl;
    float startHangry;
    GameObject mapObject;
    bool lockedPlayer = false;
    Dictionary<InventoryItem, int> smokedcigarettes = new Dictionary<InventoryItem, int>();
    int ids = 0;
    Animator walkAnimation;
    int destroyBox = 0;
    float mapSizeX = 151.856f;
    float mapSizeZ = 95.52282f;

    void Start()
    {
        Cursor.visible = false;
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
        walkAnimation = transform.parent.GetComponent<Animator>();
        Debug.Log(startHangry);
        for (int i = 0; i < 8; i++)
        {
            inventory[i] = new InventoryItem();
            inventory[i].prefab = nullObject;
        }
        InventoryItem sugar = new InventoryItem();
        sugar.prefab = Resources.Load<GameObject>("Prefabs/sugar");
        sugar.id = Guid.NewGuid().ToString();
        sugar.image = Resources.Load<Sprite>("sprites/sugar");
        inventory[0] = sugar;
        InventoryItem cigarette = new InventoryItem();
        cigarette.prefab = Resources.Load<GameObject>("Prefabs/cigarettes");
        cigarette.id = Guid.NewGuid().ToString();
        cigarette.image = Resources.Load<Sprite>("sprites/cigarete");
        inventory[1] = cigarette;
        int id = randomID();
        
        smokedcigarettes[cigarette] = 1;
        StartCoroutine(hangrytick());
    }

    void Update()
    {
        for(int i = 0; i < 8; i++)
        {
            if(inventory[i].prefab != nullObject)
            {
                GameObject.Find("inventory").transform.Find("inv block" + i).GetComponent<Image>().sprite = inventory[i].image;
            }
            else
            {
                GameObject.Find("inventory").transform.Find("inv block" + i).GetComponent<Image>().sprite = null;
            }
        }
        item = inventory[selectedSlot];

        if (item != null && item.prefab != nullObject)
        {
            if (lastitem == null || lastInvItem != item)
            {
                Destroy(lastitem);
                Debug.Log(inventory);
                float scaleX = item.prefab.transform.localScale.x;
                float scaleY = item.prefab.transform.localScale.y;
                float scaleZ = item.prefab.transform.localScale.z;
                GameObject itemN = Instantiate(inventory[selectedSlot].prefab, hand.transform.Find("item"));
                itemN.transform.localPosition = Vector3.zero;
                itemN.transform.localRotation = Quaternion.identity;
                itemN.transform.localScale = new Vector3(scaleX, scaleY, scaleZ);
                lastitem = itemN;
                lastInvItem = item;
            }
        }
        else
        {
            Destroy(lastitem);
            lastitem = null;
            inventory[selectedSlot].prefab = nullObject;
        }
        if (work == "loader" && GameObject.Find("punktA(Clone)") == null)
        {
            Vector3 rand = greenPoints[UnityEngine.Random.Range(0, greenPoints.Length)];
            Instantiate(Resources.Load<GameObject>("Prefabs/punktA"), new Vector3(-0.2341937f, -1.3f, 19.39402f), Quaternion.identity);
            Instantiate(Resources.Load<GameObject>("Prefabs/punktB"), rand, Quaternion.identity);
            GameObject boxes = Instantiate(Resources.Load<GameObject>("Prefabs/boxes"), GameObject.Find("palet").transform);
            boxes.transform.position = new Vector3(GameObject.Find("palet").transform.position.x, GameObject.Find("palet").transform.position.y + 0.2614f
                , GameObject.Find("palet").transform.position.z);
            boxes.transform.localScale = new Vector3(0.5f, 6, 1.1f);
        }
        if (destroyBox == 22)
        {
            GameObject boxes = Instantiate(Resources.Load<GameObject>("Prefabs/boxes"), GameObject.Find("palet").transform);
            boxes.transform.position = new Vector3(GameObject.Find("palet").transform.position.x, GameObject.Find("palet").transform.position.y + 0.2614f
                , GameObject.Find("palet").transform.position.z);
            destroyBox = 0;
        }
        if(inventory[selectedSlot].prefab != nullObject)
        {
            if (inventory[selectedSlot].prefab.tag == "cigarettes")
            {
                Debug.Log("держыш в руках");
                for (int i = 0; i <= smokedcigarettes[inventory[selectedSlot]]; i++)
                {
                    Destroy(GameObject.Find("cigarette" + i));
                }

                if (Mouse.current.leftButton.wasPressedThisFrame)
                {
                    Destroy(GameObject.Find("cigarette" + smokedcigarettes[inventory[selectedSlot]]));
                    smokedcigarettes[inventory[selectedSlot]]++;
                }
            }
        }
            Vector2 mousePos = Mouse.current.position.ReadValue();
        script.fill = hp;
        balance.text = money + "$";
        float value = startHangry - (605 - hangry);
        hangryEl.anchoredPosition = new Vector2(value, hangryEl.anchoredPosition.y);

        if (Keyboard.current.spaceKey.wasPressedThisFrame && isGrounded == true)
        {
            speedhangry = 2;
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            speedhangry = 5;
        }
        if (Keyboard.current.leftShiftKey.isPressed)
        {
            if(speed == 5f)
            {
                speed += 15f;
                speedhangry = 2;
            }
        }
        if (Keyboard.current.leftShiftKey.wasReleasedThisFrame)
        {
            if(speed == 20f)
            {
                speed -= 15f;
                speedhangry = 5;
            }
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

        if (!lockedPlayer)
        {
            Vector2 mouseDelta = Mouse.current.delta.ReadValue() * mouseSensitivity;

            yRotation += mouseDelta.x;
            xRotation -= mouseDelta.y;

            xRotation = Mathf.Clamp(xRotation, -80f, 80f);

            head.transform.localRotation = Quaternion.Euler(0f, 0f, -xRotation);
            transform.parent.rotation = Quaternion.Euler(0f, yRotation, 0f);
        }
        if (hp <= 0)
        {
            transform.parent.position = new Vector3(0, 0, 0);
            transform.parent.rotation = Quaternion.Euler(0, 0, 0);
            hp = 100;
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            for (int i = 0; i < 8; i++)
            {
                inventory[i] = new InventoryItem();
                inventory[i].prefab = nullObject;
            }
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
            move += forward;
        }
        if (Keyboard.current.sKey.isPressed)
        {
            
            move -= forward;
        }
        if (Keyboard.current.aKey.isPressed)
        {
            move += forward;
            move -= right;
        }
        if (Keyboard.current.dKey.isPressed)
        {
            move += forward;
            move += right;
        }
        bool isMoving = move.sqrMagnitude > 0.01f;
        if (walkAnimation != null)
        {
            walkAnimation.SetBool("isWalking", isMoving);
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
        Debug.Log(other.tag);
        if (mapObject == null)
        {
            mapObject = other.gameObject;
        }
        if (other.CompareTag("ground"))
        {
            isGrounded = true;
            Debug.Log(isGrounded);
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
            lockedPlayer = true;
            if (other.gameObject.name == "robert")
            {
                novell.transform.Find("Button1").GetComponent<UnityEngine.UI.Button>().onClick.RemoveAllListeners();
                novell.transform.Find("Button2").GetComponent<UnityEngine.UI.Button>().onClick.RemoveAllListeners();
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
                            for (int i = 0; i < 8; i++)
                            {
                                if (inventory[i].prefab == nullObject)
                                {
                                    InventoryItem sugar = new InventoryItem();
                                    sugar.prefab = Resources.Load<GameObject>("Prefabs/sugar");
                                    sugar.id = Guid.NewGuid().ToString();
                                    sugar.image = Resources.Load<Sprite>("sprites/sugar");
                                    inventory[0] = sugar;
                                    break;
                                }
                            }
                            
                        }
                    });
                    novell.transform.Find("Button2").GetComponent<UnityEngine.UI.Button>().onClick.AddListener(() =>
                    {
                        if (money >= 5)
                        {
                            money -= 5;
                            for(int i = 0; i < 8; i++)
                            {
                                if (inventory[i].prefab == nullObject)
                                {
                                    int id = randomID();
                                    InventoryItem cigarette = new InventoryItem();
                                    cigarette.prefab = Resources.Load<GameObject>("Prefabs/cigarettes");
                                    cigarette.id = Guid.NewGuid().ToString();
                                    cigarette.image = Resources.Load<Sprite>("sprites/cigarete");
                                    inventory[i] = cigarette;
                                    smokedcigarettes[cigarette] = 1;
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
            int count = 0;
            foreach (InventoryItem n in inventory)
            {
                if(n != null && n.prefab != nullObject)
                {
                    if (n.prefab.name == "box(Clone)")
                    {
                        count++;
                    }
                }
            }
            if(count < 5)
            {
                for (int i = 0; i < 8; i++)
                {
                    if (inventory[i].prefab == nullObject)
                    {
                        InventoryItem box = new InventoryItem();
                        box.prefab = Resources.Load<GameObject>("Prefabs/box");
                        box.id = Guid.NewGuid().ToString();
                        box.image = Resources.Load<Sprite>("sprites/box");
                        inventory[i] = box;
                        selectedSlot = i;
                        speed -= 0.5f;
                        break;
                    }
                }

                Debug.Log("box (" + (22 - destroyBox) + ")");
                Destroy(GameObject.Find("boxes(Clone)").transform.Find("box (" + (22 - destroyBox) + ")").gameObject);
                destroyBox++;
            }
           
            
        }

        if (other.gameObject.name == "punktB(Clone)")
        {
            for (int i = 0; i < 8; i++)
            {
                if (inventory[i].prefab == Resources.Load<GameObject>("Prefabs/box"))
                {
                    inventory[i].prefab = nullObject;
                    money += 10;
                    speed += 0.5f;
                    Destroy(GameObject.Find("punktB(Clone)"));
                    Vector3 rand = greenPoints[UnityEngine.Random.Range(0, greenPoints.Length)];
                    Instantiate(Resources.Load<GameObject>("Prefabs/punktB"), rand, Quaternion.identity);
                    break;
                }
            }
            
        }
    }
    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("ground"))
        {
            isGrounded = false;
        }

        if (other.CompareTag("shop"))
        {
            novell.alpha = 0;
            Cursor.visible = false;
            lockedPlayer = false;
        }
    }
    int randomID()
    {
        return ids++;
    }

}
public class InventoryItem
{
    public GameObject prefab;
    public string id;
    public Sprite image;
}
