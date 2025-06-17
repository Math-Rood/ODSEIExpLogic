using UnityEngine;
using UnityEngine.SceneManagement; 
using UnityEngine.UI; 

public class MenuManager : MonoBehaviour
{

    void Start()
    {
        
        Time.timeScale = 1f;

    }

    public void PlayGame()
    {
        
        SceneManager.LoadScene("Test"); 
    }

    public void QuitGame()
    {
        Debug.Log("Saindo do jogo...");
        #if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false; 
        #else
                Application.Quit();
        #endif
    }
}