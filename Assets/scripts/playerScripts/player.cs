using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
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
    Dictionary<InventoryItem, int> smokedcigarettes = new Dictionary<InventoryItem, int>();
    int ids = 0;
    float mapSizeX = 151.856f;
    float mapSizeZ = 95.52282f;

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
                Debug.Log(scaleX + " " + scaleY + " " + scaleZ);
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
            Collider col = mapObject.GetComponent<Collider>();

            Vector3 min = col.bounds.min;
            Vector3 max = col.bounds.max;
            float randomX1 = UnityEngine.Random.Range(min.x, max.x);
            float randomZ1 = UnityEngine.Random.Range(min.z, max.z);

            float randomX2 = UnityEngine.Random.Range(min.x, max.x);
            float randomZ2 = UnityEngine.Random.Range(min.z, max.z);
            while (Vector3.Distance(new Vector3(randomX2, -1.5f, randomZ2), new Vector3(randomX1, -1.5f, randomZ1)) >= 20)
            {
                randomX2 = UnityEngine.Random.Range(min.x, max.x);
                randomZ2 = UnityEngine.Random.Range(min.z, max.z);
            }
            
            Instantiate(Resources.Load<GameObject>("Prefabs/punktA"), new Vector3(randomX1, -1.5f, randomZ1), Quaternion.identity);
            Instantiate(Resources.Load<GameObject>("Prefabs/punktB"), new Vector3(randomX2, -1.5f, randomZ2), Quaternion.identity);
            Debug.Log("заспавнены на ("+randomX1+","+randomZ1+")(" + randomX2 + "," + randomZ2 + ")");
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
            if(mapObject == null)
            {
                mapObject = other.gameObject;
            }
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
                            InventoryItem sugar = new InventoryItem();
                            sugar.prefab = Resources.Load<GameObject>("Prefabs/sugar");
                            sugar.id = Guid.NewGuid().ToString();
                            sugar.image = Resources.Load<Sprite>("sprites/sugar");
                            inventory[0] = sugar;
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
                    break;
                }
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
public class InventoryItem
{
    public GameObject prefab;
    public string id;
    public Sprite image;
}
