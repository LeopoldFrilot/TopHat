using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private int trainingModeSceneIndex;
    [SerializeField] private int demoModeSceneIndex;

    private void Start()
    {
        GameWizard.Instance.audioHub.SetMusic(Help.Audio.mainMenu);
    }

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