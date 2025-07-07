using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private int trainingModeSceneIndex;
    [SerializeField] private int demoModeSceneIndex;
    
    public void QuitGame()
    {
        Application.Quit();
    }

    public void StartTrainingMode()
    {
        SceneManager.LoadScene(trainingModeSceneIndex);
    }

    public void StartDemo()
    {
        SceneManager.LoadScene(demoModeSceneIndex);
    }
}