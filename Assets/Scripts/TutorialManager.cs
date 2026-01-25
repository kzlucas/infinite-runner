using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;


public class TutorialManager : Singleton<TutorialManager>, IInitializable
{
    public int initPriority => 0;
    public System.Type[] initDependencies => null;


    [SerializeField] public SO_Tutorials tutorials;
    [SerializeField] public bool tutorialsCompleted = false;
    public GameObject playerGo;



    public Task InitializeAsync()
    {
        try
        {
            playerGo = GameObject.FindWithTag("Player");
            tutorials = Resources.Load<SO_Tutorials>("ScriptableObjects/Tutorials Data");
            tutorialsCompleted = TutorialsCompleted();
            return Task.CompletedTask;
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"[TutorialManager] Initialization failed: {ex.Message}");
            throw;
        }
    }


    private void Update()
    {
        if (tutorialsCompleted)
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
        var tutorial = tutorials.Tutorials.Find(t => t.tutorialKey == tutorialKey);
        if (tutorial != null && tutorial.completed == false)
        {
            UiManager.Instance.pauseMenu.Close();
            var ui = Instantiate(tutorial.uiGo);
            ui.transform.SetParent(transform, false);
            IInitializable item = ui.GetComponent<IInitializable>();
            item.InitializeAsync();
            ui.GetComponent<UiPopin>().Open();
            MarkTutorialCompleted(tutorialKey);
        }
    }

    /// <summary>
    ///     Marks the tutorial with the specified key as completed.
    /// </summary>
    /// <param name="tutorialKey"></param>
    public void MarkTutorialCompleted(string tutorialKey)
    {
        var tutorial = tutorials.Tutorials.Find(t => t.tutorialKey == tutorialKey);
        if (tutorial != null && tutorial.completed == false)
        {
            tutorial.completed = true;
        }

        // Save completion in save data
        var saveData = SaveService.Load();
        var tutorialsCompleted = new List<string>(saveData.TutorialsCompleted) { tutorialKey };
        tutorialsCompleted = new List<string>(new HashSet<string>(tutorialsCompleted)); // rm duplicates
        saveData.TutorialsCompleted = tutorialsCompleted.ToArray();
        SaveService.Save(saveData);
    }


    /// <summary>
    ///     Checks if the tutorial with the specified key has been completed.
    /// </summary>
    /// <param name="tutorialKey"></param>
    public bool TutorialCompleted(string tutorialKey)
    {
        var tutorial = tutorials.Tutorials.Find(t => t.tutorialKey == tutorialKey);
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
        foreach (var tutorial in tutorials.Tutorials)
        {
            tutorial.completed = false;
            if (tutorialsCompleted.Contains(tutorial.tutorialKey))
            {
                tutorial.completed = true;
            }
        }
        return tutorials.Tutorials.TrueForAll(t => t.completed);
    }
}

