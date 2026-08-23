using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using TMPro.Examples;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class player : MonoBehaviour
{
    [SerializeField] private float mouseSensitivity = 0.2f;

    private float xRotation;
    private float yRotation;

    private int groundContacts = 0;
    Vector3[] redPoints = {new Vector3(29.67535f, -1.5f, 17.06004f), new Vector3(24, -1.5f, 7.93f), new Vector3(26.96f, -1.5f, 29.29f)};
    Vector3[] greenPoints = { new Vector3(12.39252f, -1.3f, 27.86517f), new Vector3(12.39252f, -1.3f, 18.97f), new Vector3(12.39252f, -1.3f, 11.52f), new Vector3(12.39252f, -1.3f, 5.22f) };
    private Dictionary<string, GameObject> spawnedPlayers = new Dictionary<string, GameObject>();
    public float speed = 5f;
    public int hp = 100;
    private hpbar script;
    public float money = 10;
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
    float sendTimer = 0f;
    RectTransform hangryEl;
    float startHangry;
    GameObject mapObject;
    GameObject menu;
    bool lockedPlayer = false;
    Dictionary<string, int> smokedcigarettes = new Dictionary<string, int>();
    int ids = 0;
    Animator walkAnimation;
    int destroyBox = 0;
    float mapSizeX = 151.856f;
    float mapSizeZ = 95.52282f;
    TcpClient client;
    NetworkStream stream;
    string playerName;
    string jsonChunk;
    bool positionInit = false;
    string messsegeForMUltiplayer = null;


    private void Awake()
    {
        Application.runInBackground = true;
        if (SceneManager.GetActiveScene().name == "menu")
        {
            this.enabled = false;
            return;
        }
    }
    IEnumerator Start()
    {
        if (SceneManager.GetActiveScene().name == "menu") yield return null;
        playerName = startGame.playerName.Replace("\u200B", "").Replace("\uFEFF", "").Trim();
        Cursor.visible = false;
        if(SceneManager.GetActiveScene().name == "multiplayer")
        {
            client = new TcpClient("localhost", 8080);
        }
        script = GameObject.Find("hpbar").GetComponent<hpbar>();
        balance = GameObject.Find("money").GetComponent<TextMeshProUGUI>();
        damage = GameObject.Find("damage");
        damage.SetActive(false);
        colideObject = GameObject.Find("colide object");
        rb = transform.parent.GetComponent<Rigidbody>();
        hand = GameObject.Find("hand");
        head = GameObject.Find("head");
        menu = GameObject.Find("menu");
        menu.SetActive(false);
        novell = GameObject.Find("novell").GetComponent<CanvasGroup>();
        Debug.Log(novell.name);
        nullObject = GameObject.Find("null");
        hangryEl = GameObject.Find("hangryBar").GetComponent<RectTransform>();
        startHangry = hangryEl.anchoredPosition.x;
        walkAnimation = transform.parent.GetComponent<Animator>();
        Debug.Log(startHangry);
        stream = client.GetStream();
        for (int i = 0; i < 8; i++)
        {
            inventory[i] = new InventoryItem();
            inventory[i].prefab = nullObject;
        }
        StartCoroutine(hangrytick());
        if(SceneManager.GetActiveScene().name == "multiplayer")
        {
            StartCoroutine(SendPostRequest("{\"type\":\"join\",\"session\":\"" + playerName + "\"}"));
            yield return StartCoroutine(initMUltiplayer());
            string dataS = null;
            string message = messsegeForMUltiplayer;
            string[] m = message.Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
            if (message != null)
            {
                Debug.Log("обработка");
                
                if (message != null)
                {
                    JObject data = JObject.Parse(message);
                    string type = data["type"]?.ToString();
                    if (type == "update")
                    {
                        List<Player> players = data["data"].ToObject<List<Player>>();
                        while((data["data"] is JArray array && array.Count == 0))
                        {
                            players = JObject.Parse(message)["data"].ToObject<List<Player>>();
                        }
                        foreach (Player player in players)
                        {
                            Debug.Log("имя "+playerName);
                            if (playerName != null)
                            {
                                Debug.Log("длина на сервере " + player.Id.Length + " на локале " + playerName.Length);
                                if (player.Id == playerName)
                                {
                                    rb.position = new Vector3(player.X, player.Y, player.Z);
                                    Debug.Log("игрок с севреа " + player.X + " " + player.Y + " " + player.Z);
                                    rb.rotation = Quaternion.Euler(player.RotX, player.RotY, player.RotZ);
                                    positionInit = true;
                                }
                                else
                                {
                                    GameObject p = Instantiate(Resources.Load<GameObject>("Prefabs/network player"), new Vector3(player.X, player.Y, player.Z), Quaternion.identity);
                                    p.transform.rotation = Quaternion.Euler(player.RotX, player.RotY, player.RotZ);
                                    p.name = player.Id;
                                    spawnedPlayers[player.Id] = p;
                                }
                            }
                        }
                    }
                }
            }
        }
        
    }

    void Update()
    {
        if (SceneManager.GetActiveScene().name == "menu") return;
        for 
            (int i = 0; i < 8; i++)
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
                for (int i = 0; i <= smokedcigarettes[inventory[selectedSlot].id]; i++)
                {
                    Destroy(GameObject.Find("cigarette" + i));
                }

                if (Mouse.current.leftButton.wasPressedThisFrame)
                {
                    Destroy(GameObject.Find("cigarette" + smokedcigarettes[inventory[selectedSlot].id]));
                    smokedcigarettes[inventory[selectedSlot].id]++;
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
        if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
        {
            Ray ray = transform.Find("head").Find("head in").Find("Main Camera").GetComponent<Camera>().ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
            GameObject obj;
            string nam = "";
            if (Physics.Raycast(ray, out RaycastHit hit, 100f))
            {
                Vector3 lookPosition = hit.point;
                obj = hit.transform.gameObject;
                nam = hit.transform.name;
            }
            if(SceneManager.GetActiveScene().name == "multiplayer")
            {
                var data = new
                {
                    type = "atack",
                    session = playerName,
                    x = transform.parent.position.x.ToString(CultureInfo.InvariantCulture),
                    y = transform.parent.position.y.ToString(CultureInfo.InvariantCulture),
                    z = transform.parent.position.z.ToString(CultureInfo.InvariantCulture),
                    name = nam

                };
                string json = JsonConvert.SerializeObject(data);
                StartCoroutine(SendPostRequest(json));
            }
        }
        if (SceneManager.GetActiveScene().name != "multiplayer")
        {
            if (transform.parent.position.y <= -40f)
            {
                hp -= 1;
            }
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
        if(Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            if(menu.GetComponent<CanvasGroup>().alpha == 0)
            {
                menu.GetComponent<CanvasGroup>().alpha = 1;
                Cursor.visible = true;
                lockedPlayer = true;
                menu.transform.gameObject.SetActive(true);
            }
            else
            {
                menu.GetComponent<CanvasGroup>().alpha = 0;
                Cursor.visible = false;
                lockedPlayer = false;
                menu.transform.gameObject.SetActive(false);
            }
            
            
        }
        if (SceneManager.GetActiveScene().name == "multiplayer")
        {
            var data = new
            {
                session = playerName,
                type = "slot",
                slot = selectedSlot
            };
            StartCoroutine(SendPostRequest(JsonConvert.SerializeObject(data)));
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
            if (SceneManager.GetActiveScene().name == "game")
            {
                hp = 100;
                for (int i = 0; i < 8; i++)
                {
                    inventory[i] = new InventoryItem();
                    inventory[i].prefab = nullObject;
                }
            }
            transform.parent.rotation = Quaternion.Euler(0, 0, 0);
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            transform.parent.position = new Vector3(0, 0, 0);
        }
        if (oldhp > hp)
        {
            oldhp = hp;
            StartCoroutine(ShowDamageScreen());
        }
    }
    void FixedUpdate()
    {
        if (SceneManager.GetActiveScene().name == "menu") return;
        Vector3 move = Vector3.zero;
        sendTimer += Time.fixedDeltaTime;
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
        if (sendTimer >= 0.1f)
        {
            sendTimer = 0f;
            if(SceneManager.GetActiveScene().name == "multiplayer")
            {
                var pos = new
                {
                    type = "move",
                    session = playerName,
                    x = transform.parent.position.x,
                    y = transform.parent.position.y,
                    z = transform.parent.position.z,
                    rot = transform.parent.eulerAngles.y
                };

                StartCoroutine(SendPostRequest(JsonConvert.SerializeObject(pos)));
                string message = getMessage();
                if (message != null)
                {
                    if (jsonChunk != null)
                    {
                        message = jsonChunk + message;
                        jsonChunk = null;
                    }
                    if (!message.EndsWith("\n"))
                    {
                        jsonChunk = message;
                        return;
                    }
                    foreach (string i in message.Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries))
                    {
                        JObject data = JObject.Parse(i);
                        string type = data["type"]?.ToString();

                        if (type == "update")
                        {
                            List<Player> players = data["data"].ToObject<List<Player>>();
                            HashSet<string> names = new HashSet<string>();
                            foreach (Player player in players)
                            {
                                if (player.Id != playerName)
                                {
                                    names.Add(player.Id);
                                    if (!spawnedPlayers.ContainsKey(player.Id) || spawnedPlayers[player.Id] == null)
                                    {
                                        Debug.Log("1. Начинаем спавн игрока: " + player.Id);
                                        GameObject p = Instantiate(Resources.Load<GameObject>("Prefabs/network player"), new Vector3(player.X, player.Y, player.Z), Quaternion.identity);
                                        p.transform.rotation = Quaternion.Euler(player.RotX, player.RotY, player.RotZ);
                                        p.name = player.Id;
                                        GameObject item1 = p.transform.Find("Armature").gameObject;
                                        GameObject item2 = item1.transform.Find("туловице").gameObject;
                                        GameObject item3 = item2.transform.Find("Bone.001").gameObject;
                                        GameObject item4 = item3.transform.Find("Bone.002").gameObject;
                                        GameObject item5 = item4.transform.Find("hand").gameObject;
                                        GameObject item6 = item5.transform.Find("item").gameObject;
                                        
                                        spawnedPlayers[player.Id] = p;
                                        Debug.Log("2. Успешно записано в словарь: " + player.Id);
                                    }
                                    else
                                    {
                                        Debug.Log(spawnedPlayers[player.Id].name);
                                        spawnedPlayers[player.Id].transform.position = new Vector3(player.X, player.Y, player.Z);
                                        spawnedPlayers[player.Id].transform.rotation = Quaternion.Euler(player.RotX, player.RotY, player.RotZ);
                                    }
                                    
                                }
                                else
                                {
                                    hp = player.hp;
                                    for (int n = 0; n < player.inventory.Length; n++)
                                    {
                                        if (player.inventory[n] != null)
                                        {
                                            InventoryItem inv = new InventoryItem();
                                            inv.prefab = Resources.Load<GameObject>("Prefabs/" + player.inventory[n].prefab);
                                            inv.image = Resources.Load<Sprite>("sprites/" + player.inventory[n].image);
                                            inv.id = player.inventory[n].id;
                                            if (player.inventory[n].prefab == "cigarettes")
                                            {
                                                if (!smokedcigarettes.ContainsKey(inv.id))
                                                {
                                                    smokedcigarettes[inv.id] = 1;
                                                }
                                            }
                                            inventory[n] = inv;
                                        }
                                        else
                                        {
                                            inventory[n].prefab = new GameObject();
                                        }
                                    }
                                    money = player.money;

                                }
                            }
                            List<string> diskonected = spawnedPlayers.Keys.Except(names).ToList();
                            foreach (string name in diskonected)
                            {
                                if (spawnedPlayers[name] != null)
                                {
                                    Destroy(spawnedPlayers[name]);
                                }
                                spawnedPlayers.Remove(name);
                            }
                            
                        }
                    }
                }
            }
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
                        if(SceneManager.GetActiveScene().name == "multiplayer")
                        {
                            var data = new
                            {
                                session = playerName,
                                type = "buy",
                                item = "sugar"
                            };
                            StartCoroutine(SendPostRequest(JsonConvert.SerializeObject(data)));
                        }
                        else
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
                        }
                    });
                    novell.transform.Find("Button2").GetComponent<UnityEngine.UI.Button>().onClick.AddListener(() =>
                    {
                        if(SceneManager.GetActiveScene().name == "multiplayer")
                        {
                            var data = new
                            {
                                session = playerName,
                                type = "buy",
                                item = "cigarettes"
                            };
                            StartCoroutine(SendPostRequest(JsonConvert.SerializeObject(data)));
                        }
                        else
                        {
                            if (money >= 5)
                            {
                                money -= 5;
                                for (int i = 0; i < 8; i++)
                                {
                                    if (inventory[i].prefab == nullObject)
                                    {
                                        int id = randomID();
                                        InventoryItem cigarette = new InventoryItem();
                                        cigarette.prefab = Resources.Load<GameObject>("Prefabs/cigarettes");
                                        cigarette.id = Guid.NewGuid().ToString();
                                        cigarette.image = Resources.Load<Sprite>("sprites/cigarete");
                                        inventory[i] = cigarette;
                                        smokedcigarettes[cigarette.id] = 1;
                                        break;
                                    }
                                }
                            }
                        }  
                    });
                });
                Cursor.visible = true;
            }

        }
        if(other.gameObject != null)
        {
            if(SceneManager.GetActiveScene().name == "multiplayer")
            {
                var data = new
                {
                    session = playerName,
                    type = "triger",
                    obj = other.gameObject.name
                };
                StartCoroutine(SendPostRequest(JsonConvert.SerializeObject(data)));

            }
        }

        if (other.CompareTag("work"))
        {
            work = other.gameObject.name;
            Debug.Log("work: " + work);
        }

        if (other.gameObject.name == "punktA(Clone)")
        {
            if(SceneManager.GetActiveScene().name == "game")
            {
                int count = 0;
                foreach (InventoryItem n in inventory)
                {
                    if (n != null && n.prefab != nullObject)
                    {
                        if (n.prefab.name == "box(Clone)")
                        {
                            count++;
                        }
                    }
                }
                if (count < 5)
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
            else
            {
                speed -= 0.5f;
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
    IEnumerator SendPostRequest(string json)
    {
        if (client != null && client.Connected && stream != null && stream.CanWrite)
        {
            byte[] data = Encoding.UTF8.GetBytes(json + "\n");
            stream.Write(data);
            yield return null;
        }
    }

    private string getMessage()
    {
        if (stream == null)
        {
            Debug.Log("stream равен NULL!");
            return null;
        }

        if (!stream.CanRead)
        {
            Debug.Log("Сокет закрыт.");
            return null;
        }

        if (!stream.DataAvailable)
        {
            return null;
        }
        try
        {
            byte[] buffer = new byte[4096];
            int bytesRead = stream.Read(buffer, 0, buffer.Length);
            if (bytesRead > 0)
            {
                return Encoding.UTF8.GetString(buffer, 0, bytesRead);
            }
        }
        catch(System.Exception ex)
        {
            Debug.Log("ошыбка получения "+ex.Message);
            return null;
        }
        Debug.Log("ошыбка получения 2");
        return null;
    }
    IEnumerator initMUltiplayer()
    {
        while (string.IsNullOrEmpty(playerName))
        {
            yield return null;
        }

        while (client == null || !client.Connected)
        {
            yield return null;
        }

        if (stream == null)
        {
            stream = client.GetStream();
        }

        string message = null;

        while (message == null)
        {
            message = getMessage();
            yield return null;
        }
        messsegeForMUltiplayer = message;
        Debug.Log("INIT получил пакет: " + message);
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
    public InventoryItem(GameObject prefab = null, string id = null, Sprite image = null)
    {
        this.prefab = prefab;
        this.id = id;
        this.image = image;
    }
    public override bool Equals(object obj)
    {
        if (obj is InventoryItem other)
        {
            return this.id == other.id;
        }
        return false;
    }

    public override int GetHashCode()
    {
        return id.GetHashCode(); 
    }
}

public class NetInventoryItem
{
    public string prefab;
    public string id;
    public string image;
}

public class Player
{
    public string Id { get; set; }
    public float X { get; set; }
    public float Y { get; set; }
    public float Z { get; set; }
    public float RotX { get; set; }
    public float RotY { get; set; }
    public float RotZ { get; set; }
    public int hp { get; set; }
    public NetInventoryItem[] inventory { get; set; } = new NetInventoryItem[8];
    public int selectedSlot { get; set; } = 0;
    public string work { get; set; }
    public string toach { get; set; }
    public float money { get; set; } = 10;
}
