using System.Collections;
using System.Collections.Generic;
using UnityEngine;



[System.Serializable]
public class Tutorial
{
    public string tutorialKey;
    public bool completed;
    public UiPopin ui;
}


public class TutorialManager : Singleton<TutorialManager>
{
    [SerializeField] public List<Tutorial> tutorials;

    private IEnumerator Start()
    {
        // Check if tutorials are marked as completed in save data
        var saveData = SaveService.Load();
        var tutorialsCompleted = new List<string>(saveData.TutorialsCompleted);
        foreach (var tutorial in tutorials)
        {
            if (tutorialsCompleted.Contains(tutorial.tutorialKey))
            {
                tutorial.completed = true;
            }
        }
        // Play the first tutorial at start
        yield return new WaitUntil(() => SceneInitializer.Instance.isInitialized); 
        Play("World 1 - Gray");
    }


    /// <summary>
    ///     Plays the tutorial with the specified key if not already completed.
    /// </summary>
    /// <param name="tutorialKey"></param>
    public void Play(string tutorialKey)
    {
        var tutorial = tutorials.Find(t => t.tutorialKey == tutorialKey);
        if (tutorial != null && tutorial.completed == false)
        {
            UiManager.Instance.pauseMenu.Close();
            tutorial.ui.Open();
            MarkTutorialCompleted(tutorialKey);
        }
    }

    /// <summary>
    ///     Marks the tutorial with the specified key as completed.
    /// </summary>
    /// <param name="tutorialKey"></param>
    public void MarkTutorialCompleted(string tutorialKey)
    {
        var tutorial = tutorials.Find(t => t.tutorialKey == tutorialKey);
        if (tutorial != null && tutorial.completed == false)
        {
            tutorial.completed = true;
        }

        // Save completion in save data
        var saveData = SaveService.Load();
        var tutorialsCompleted = new List<string>(saveData.TutorialsCompleted) { tutorialKey };
        saveData.TutorialsCompleted = tutorialsCompleted.ToArray();
        SaveService.Save(saveData);
    }
}
