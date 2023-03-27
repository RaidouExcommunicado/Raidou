using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOver : MonoBehaviour
{
    public void Retry()
    {
        SceneManager.LoadScene("TestScene");
        Debug.Log("Retry");
    }

    public void Quit()
    {
        Application.Quit();
        Debug.Log("Quit");
    }
}
