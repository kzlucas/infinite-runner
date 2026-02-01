using System.Collections.Generic;
using System.Threading.Tasks;
using Components.DataServices;
using Components.Events;
using Components.UI.Scripts;
using Components.UI.Scripts.Controllers.BaseClasses;
using UnityEngine;

namespace Components.Tutorials
{
    public class TutorialManager : Singleton.Model<TutorialManager>, IInitializable, IGameService
    {

        [Header("Initialisation")]
        public int InitPriority => 0;
        public System.Type[] InitDependencies => null;



        [Header("Tutorial Data")]
        [SerializeField] public SO_Tutorials Tutorials;
        [SerializeField] public bool AllTutorialsCompleted = false;


        private void Start()
        {
            EventBus.Subscribe<BiomeChangedEvent>(OnBiomeChanged);
        }

        private void OnDestroy()
        {
            EventBus.Unsubscribe<BiomeChangedEvent>(OnBiomeChanged);
        }

        private void OnBiomeChanged(BiomeChangedEvent evt)
        {
            // Play "Tutorial Completed" when leaving tutorial biome
            if (evt.NewBiomeData.BiomeName != "World 0 - Tuto" && (PlayerPrefService.Load("SkipTutorials") == null))
            {
                Debug.Log("[TutorialManager] OnBiomeChanged! > " + evt.NewBiomeData.name );
                Play("Completed");
                PlayerPrefService.Save("SkipTutorials", "1");
                AllTutorialsCompleted = true;
            }
        }

        public async Task InitializeAsync()
        {
            AllTutorialsCompleted = false;

            if (PlayerPrefService.Load("SkipTutorials") == "1")
            {
                AllTutorialsCompleted = true;
                await Task.CompletedTask;
                return;
            }

            try
            {
                AllTutorialsCompleted = TutorialsCompleted();
                await Task.CompletedTask;
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"[TutorialManager] Initialization failed: {ex.Message}");
                throw;
            }
        }

        private void Update()
        {
            if (AllTutorialsCompleted)
            {
                return;
            }

            if (
                Components.Player.Utils.PlayerController
                && Components.Player.Utils.PlayerController.transform.position.z > 0f
                && !TutorialCompleted("ChangeLane"))
            {
                Play("ChangeLane");
            }

            if (
                Components.Player.Utils.PlayerController
                && Components.Player.Utils.PlayerController.transform.position.z > 140f
                && !TutorialCompleted("Jump"))
            {
                Play("Jump");
            }

            if (
                Components.Player.Utils.PlayerController
                && Components.Player.Utils.PlayerController.transform.position.z > 240f
                && !TutorialCompleted("Slide"))
            {
                Play("Slide");
            }

            if (
                Components.Player.Utils.PlayerController
                && Components.Player.Utils.PlayerController.transform.position.z > 340f
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
            var tutorial = Tutorials.Tutorials.Find(t => t.TutorialKey == tutorialKey);
            if (tutorial != null && tutorial.Completed == false)
            {
                UiRegistry.Instance.PauseMenu.Close();
                var ui = Instantiate(tutorial.UiGo);
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
            var tutorial = Tutorials.Tutorials.Find(t => t.TutorialKey == tutorialKey);
            if (tutorial != null && tutorial.Completed == false)
            {
                tutorial.Completed = true;
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
            var tutorial = Tutorials.Tutorials.Find(t => t.TutorialKey == tutorialKey);
            if (tutorial != null)
            {
                return tutorial.Completed;
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
            foreach (var tutorial in Tutorials.Tutorials)
            {
                tutorial.Completed = false;
                if (tutorialsCompleted.Contains(tutorial.TutorialKey))
                {
                    tutorial.Completed = true;
                }
            }
            return Tutorials.Tutorials.TrueForAll(t => t.Completed);
        }
    }
}

