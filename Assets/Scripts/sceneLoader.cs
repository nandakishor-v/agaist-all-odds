using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class sceneLoader : MonoBehaviour
{
    public void LoadGame()
    {
        if(SceneManager.GetActiveScene().buildIndex==2)
            Destroy(FindObjectOfType<GameSession>().gameObject);
        SceneManager.LoadScene(1);
    }
    public void QuitGame()
    {
        Application.Quit();
        Debug.Log("Quit");
    }
}
