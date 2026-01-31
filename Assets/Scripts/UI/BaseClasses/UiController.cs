using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Components.Audio.Scripts;
using Components.ServiceLocator.Scripts;
using Components.UI.Scripts;
using UnityEngine;
using UnityEngine.UIElements;

[RequireComponent(typeof(UIDocument))]
public class UiController : MonoBehaviour, IInitializable
{
        
    [Header("Dependencies")]
    private UiRegistry UiRegistry => ServiceLocator.Get<UiRegistry>();


    [Header("Initialization")]
    public int initPriority => 1;
    public System.Type[] initDependencies => null;


    [Header("UI Document References")]
    protected VisualElement root;
    private PanelSettings panelSettings;
    public UIDocument uiDoc;
    [HideInInspector] public bool docReady = false;
    private List<Button> buttons = null;
    private List<Toggle> toggles = null;



    public async Task InitializeAsync()
    {
        StartCoroutine(AttachDocument());
    
        while (!docReady)
        {
            await Task.Yield(); 
        }
    }

    public virtual void OnDocReady(){}


    /// <summary>
    ///    Attaches the UIDocument and initializes UI elements.
    /// </summary>
    /// <returns></returns>
    public IEnumerator AttachDocument()
    {
        float timeout = 3f;
        uiDoc.enabled = true;

        while ((uiDoc == null || root == null || panelSettings == null) && timeout > 0f)
        {
            timeout -= Time.deltaTime;
            if (timeout <= 0f)
            {
                Debug.LogError("[UiController] AttachDocument() timed out. uiDoc: " + (uiDoc != null) + ", root: " + (root != null) + ", panelSettings: " + (panelSettings != null));
                yield break;
            }
            _AttachDocument();
            yield return null;
        }

        /*
         *
         * Remove Exit app button on WebGL
         */

        #if UNITY_WEBGL
        Button exitButton = root.Q<Button>("btn--exit");
        if(exitButton != null)
        {
            exitButton.RemoveFromHierarchy();
        }
        #endif


        /*
         *
         * Buttons click actions 
         */

        buttons = new List<Button>();
        root.Query<Button>(className: "action-button").ToList(buttons);
        foreach (Button button in buttons)
        {
            button.clickable = null; // clear existing handlers
            button.clicked += () => OnButtonClicked(button);
        }

        /*
         *
         * Toggles change actions 
         */

        toggles = new List<Toggle>();
        root.Query<Toggle>(className: "unity-toggle").ToList(toggles);
        foreach (Toggle toggle in toggles)
        {
            // Register handler
            toggle.RegisterValueChangedCallback((evt) => OnToggleChanged(toggle, evt.newValue));

            // Initialize toggle states 
            if (toggle.name == "settings--music")
                toggle.value = AudioManager.Instance.IsMusicEnabled;
            if (toggle.name == "settings--sfx")
                toggle.value = AudioManager.Instance.IsSfxEnabled;
        }
        
        // root.Focus(); // needed to ensure input works in WebGL builds
        docReady = true;
        root.style.display = DisplayStyle.Flex;

        OnDocReady();
    }

    private void OnButtonClicked(Button button)
    {
        // Assumes button names are in the format "btn--ActionName"
        if (!button.name.Contains("--"))
        {
            Debug.LogError("[UiController] Button name does not contain '--': " + button.name);
            return;
        }
        string actionName = button.name.ToString().Split("--")[1]; 

        if(actionName == "splash")
        {
            Debug.Log("[UiController] splash screen button clicked.");
            SceneLoader.Instance.Load("Splash Screen");
        }
        else if(actionName == "exit")
        {
            Debug.Log("[UiController] exit button clicked.");
            Application.Quit();
        }
        else if(actionName == "start")
        {
            Debug.Log("[UiController] start button clicked.");
            SceneLoader.Instance.Load("Game");
        }
        else if(actionName == "restart")
        {
            Debug.Log("[UiController] restart button clicked.");
            SceneLoader.Instance.ReloadCurrentScene();
        }
        else if(actionName == "resume")
        {
            Debug.Log("[UiController] resume button clicked.");
            UiRegistry.pauseMenu.Close();
        }
        else if(actionName == "pause")
        {
            Debug.Log("[UiController] pause button clicked.");
            UiRegistry.pauseMenu.Open();   
        }
        else
        {
            Debug.LogError("[UiController] Unhandled button action: " + actionName);
        }

        AudioManager.Instance.PlaySound("button-click");
    }



    private void OnToggleChanged(Toggle toggle, bool newValue)
    {
        // Assumes button names are in the format "btn--ActionName"
        if (!toggle.name.Contains("--"))
        {
            Debug.LogError("[UiController] Toggle name does not contain '--': " + toggle.name);
            return;
        }
        string actionName = toggle.name.ToString().Split("--")[1]; 

        if(actionName == "music")
        {
            AudioManager.Instance.UserSettings.SetMusicEnabled(newValue);
        }
        else if(actionName == "sfx")
        {
            AudioManager.Instance.UserSettings.SetSfxEnabled(newValue);
        }
        else
        {
            Debug.LogError("[UiController] Unhandled toggle action: " + actionName);
        }

        AudioManager.Instance.PlaySound("button-click");
    }


    public void _AttachDocument()
    {
        uiDoc =  GetComponent<UIDocument>();
        root = uiDoc.rootVisualElement;
        if(root != null) root.style.display = DisplayStyle.None;
        panelSettings = uiDoc.panelSettings;
    }


}