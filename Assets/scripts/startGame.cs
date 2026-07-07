using UnityEngine;

using UnityEngine.SceneManagement;

public class startGame : MonoBehaviour
{
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
    }
}
