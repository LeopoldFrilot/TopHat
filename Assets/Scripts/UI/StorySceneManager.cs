using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StorySceneManager : MonoBehaviour
{
    [SerializeField] private int endOfStorySceneToLoad;
    [SerializeField] private List<StoryScene> storyScenes = new();
    private int index = 0;

    private void Start()
    {
        LoadScene(index);
        GameWizard.Instance.audioHub.SetMusic(Help.Audio.context);
    }

    private void LoadScene(int index)
    {
        for (int i = 0; i < storyScenes.Count; i++)
        {
            storyScenes[i].gameObject.SetActive(i == index);
        }
    }

    public void ProgressScene()
    {
        index++;
        if (index < storyScenes.Count)
        {
            GameWizard.Instance.audioHub.PlayClip(Help.Audio.contextPageFlip);
            LoadScene(index);
        }
        else
        {
            GameWizard.Instance.sceneStatics.startFightSceneWithSmartAI = true;
            SceneManager.LoadScene(endOfStorySceneToLoad);
        }
    }
}