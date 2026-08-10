using TMPro;
using UnityEngine;

using UnityEngine.SceneManagement;

public class startGame : MonoBehaviour
{
    public static string name;
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
        startGame.name = GameObject.Find("name").transform.Find("Text Area").transform.Find("Text").GetComponent<TMP_Text>().text;
    }
    public void startMultiplayer()
    {
        SceneManager.LoadScene("multiplayer");
        startGame.name = GameObject.Find("name").transform.Find("Text Area").transform.Find("Text").GetComponent<TMP_Text>().text;
    }
}
