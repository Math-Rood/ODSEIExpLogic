using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameUIManager : MonoBehaviour
{
    public GameObject pausePanel;

    public void ResetPhase()
    {
        SceneManager.LoadScene("Test");
    }

    public void BackToMenu()
    {
        SceneManager.LoadScene("Menu");
    }

    public void PauseGame()
    {
        pausePanel.SetActive(true);
    }

    public void ResumeGame()
    {
        pausePanel.SetActive(false);
    }
}