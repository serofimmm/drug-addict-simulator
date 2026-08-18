using TMPro;
using UnityEngine;

using UnityEngine.SceneManagement;

public class startGame : MonoBehaviour
{
    public static string playerName;
    void Start()
    {
        
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
        SceneManager.LoadScene("multiplayer");
        startGame.playerName = GameObject.Find("name").transform.Find("Text Area").transform.Find("Text").GetComponent<TMP_Text>().text;
    }

    public void exitGame()
    {
        SceneManager.LoadScene("menu");
    }
}
