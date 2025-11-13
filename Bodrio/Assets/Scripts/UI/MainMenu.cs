using UnityEngine;

public class MainMenu : MonoBehaviour
{
    public SceneFader sceneFader;

    public void PlayGame()
    {
        sceneFader.FadeToScene("SCN_Maze_Sewers");
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
