using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;



[System.Serializable]
public class Tutorial
{
    public string tutorialKey;
    public bool completed;
    public UiPopin ui;
}

public class TutorialManager : Singleton<TutorialManager>, IInitializable
{
    public int initPriority => 0;
    public System.Type[] initDependencies => null;


    [SerializeField] public List<Tutorial> tutorials;
    [SerializeField] public bool tutorialsCompleted = false;
    public GameObject playerGo;



    public Task InitializeAsync()
    {
        playerGo = GameObject.FindWithTag("Player");
        tutorialsCompleted = TutorialsCompleted();
        return Task.CompletedTask;
    }


    private void Update()
    {
        if(tutorialsCompleted)
        {
            return;
        }

        if (
            playerGo
            && playerGo.transform.position.z > 10f
            && !TutorialCompleted("ChangeLane"))
        {
            Play("ChangeLane");
        }

        if (
            playerGo
            && playerGo.transform.position.z > 170f
            && !TutorialCompleted("Jump"))
        {
            Play("Jump");
        }

        if (
            playerGo
            && playerGo.transform.position.z > 270f
            && !TutorialCompleted("Slide"))
        {
            Play("Slide");
        }

        if (
            playerGo
            && playerGo.transform.position.z > 410f
            && !TutorialCompleted("Crystal"))
        {
            Play("Crystal");
        }
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


    /// <summary>
    ///     Checks if the tutorial with the specified key has been completed.
    /// </summary>
    /// <param name="tutorialKey"></param>
    public bool TutorialCompleted(string tutorialKey)
    {
        var tutorial = tutorials.Find(t => t.tutorialKey == tutorialKey);
        if (tutorial != null)
        {
            return tutorial.completed;
        }
        return false;
    }

    /// <summary>
    ///     Checks if all tutorials have been completed.
    /// </summary>
    private bool TutorialsCompleted()
    {
        var saveData = SaveService.Load();
        var tutorialsCompleted = new List<string>(saveData.TutorialsCompleted);
        foreach (var tutorial in tutorials)
        {
            if (tutorialsCompleted.Contains(tutorial.tutorialKey))
            {
                tutorial.completed = true;
            }
        }
        return tutorials.TrueForAll(t => t.completed);
    }
}

