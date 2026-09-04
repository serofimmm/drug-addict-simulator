using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class startGame : MonoBehaviour
{
    public static string playerName;
    public static string gameIp;
    GameObject buttons;
    private GameObject multiplayerDialog;

    void Start()
    {
        buttons = GameObject.Find("buttons");
        multiplayerDialog = GameObject.Find("multiplayerDialog");
    }

    void Update()
    {
        
    }

    public void StartGame()
    {
       SceneManager.LoadScene("game");
        Debug.Log("Game started");
        startGame.playerName = GameObject.Find("name").transform.Find("Text Area").transform.Find("Text").GetComponent<TMP_Text>().text;
    }
    public void startMultiplayer()
    {
        GameObject.Find("buttons").GetComponent<CanvasGroup>().alpha = 0;
        GameObject.Find("buttons").GetComponent<CanvasGroup>().interactable = false;
        GameObject.Find("multiplayerDialog").GetComponent<CanvasGroup>().alpha = 0;
        GameObject.Find("multiplayerDialog").GetComponent<CanvasGroup>().interactable = false;
        GameObject.Find("buttons").GetComponent<CanvasGroup>().blocksRaycasts = false;
        GameObject.Find("loading").GetComponent<CanvasGroup>().alpha = 1;
        GameObject.Find("loading").GetComponent<CanvasGroup>().interactable = true;
        GameObject.Find("multiplayerDialog").GetComponent<CanvasGroup>().blocksRaycasts = false;
        SceneManager.LoadScene("multiplayer");
        startGame.playerName = GameObject.Find("name").transform.Find("Text Area").transform.Find("Text").GetComponent<TMP_Text>().text;
        startGame.gameIp = GameObject.Find("multiplayerDialog").transform.Find("ip").transform.Find("Text Area").transform.Find("Text").GetComponent<TMP_Text>().text;
    }

    public void exitGame()
    {
        SceneManager.LoadScene("menu");
    }
    public void exitMultiplayer()
    {
        SceneManager.LoadScene("menu");
        multipayerMenu();
    }

    public void multipayerMenu()
    {
        GameObject.Find("multiplayerDialog").GetComponent<CanvasGroup>().alpha = 1;
        GameObject.Find("multiplayerDialog").GetComponent<CanvasGroup>().interactable = true;
        GameObject.Find("multiplayerDialog").GetComponent<CanvasGroup>().blocksRaycasts = true;
        GameObject.Find("Camera").transform.position = new Vector3(-5.26f, 3.11f, -18.84f);
        GameObject.Find("buttons").GetComponent<CanvasGroup>().alpha = 0;
        GameObject.Find("buttons").GetComponent<CanvasGroup>().interactable = false;
        GameObject.Find("buttons").GetComponent<CanvasGroup>().blocksRaycasts = false;
        GameObject.Find("loading").GetComponent<CanvasGroup>().blocksRaycasts = false;
    }
    public void backHome()
    {
        GameObject.Find("multiplayerDialog").GetComponent<CanvasGroup>().alpha = 0;
        GameObject.Find("multiplayerDialog").GetComponent<CanvasGroup>().interactable = false;
        GameObject.Find("multiplayerDialog").GetComponent<CanvasGroup>().blocksRaycasts = false;
        GameObject.Find("Camera").transform.position = new Vector3(-6.81f, 2.81f, 9.38f);
        GameObject.Find("buttons").GetComponent<CanvasGroup>().alpha = 1;
        GameObject.Find("buttons").GetComponent<CanvasGroup>().interactable = true;
        GameObject.Find("buttons").GetComponent<CanvasGroup>().blocksRaycasts = true;
        GameObject.Find("loading").GetComponent<CanvasGroup>().blocksRaycasts = false;
    }
    public void EnableConstraint()
    {
        GameObject.Find("hand").GetComponent<ParentConstraint>().constraintActive = true;
    }
}
