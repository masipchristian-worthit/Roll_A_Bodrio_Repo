using UnityEngine;
using UnityEngine.SceneManagement;
public class PauseMenu : MonoBehaviour
{

    [Header("Referencias")]
    public GameObject pauseMenuUI;
    public PlayerController playerController;
    public SceneFader sceneFader;

    private bool isPaused = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (pauseMenuUI != null)
            pauseMenuUI.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.P))
        {
            if (isPaused)
                ResumeGame();
            else
                PauseGame();
        }
    }

    public void PauseGame()
    {

        Time.timeScale = 0f;
        isPaused = true;


        if (playerController != null)
            playerController.enabled = false;


        if (pauseMenuUI != null)
            pauseMenuUI.SetActive(true);


        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void ResumeGame()
    {
        isPaused = false;
        Time.timeScale = 1f;

        if (playerController != null)
            playerController.enabled = true;

        if (pauseMenuUI != null)
            pauseMenuUI.SetActive(false);

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

    }

    public void LoadMainMenu()
    {
        Time.timeScale = 1f;

        if (sceneFader != null)
        { 
            sceneFader.FadeToScene("SCN_MainMenu");
    }
    else
    {
    SceneManager.LoadScene("SCN_MainMenu");

        }
}

    public void RestartLevel()
    {
        Time.timeScale = 1f;

        if (sceneFader != null)
        {
            sceneFader.FadeToScene(SceneManager.GetActiveScene().name);
        }
        else
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }
  
    public void ExitToMainMenu()
    {
        Time.timeScale = 1f;

        if (sceneFader != null)
        {
            sceneFader.FadeToScene("SCN_MainMenu");

        }
        else
        {
            SceneManager.LoadScene("SCN_MainMenu");
      
    }
}
}
