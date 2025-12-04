using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

[RequireComponent(typeof(UIDocument))]
public class UiController : MonoBehaviour
{
    protected VisualElement root;
    private PanelSettings panelSettings;
    public UIDocument uiDoc;
    [HideInInspector] public bool docReady = false;

    protected void OnEnable()
    {
        StartCoroutine(AttachDocument());
    }

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

        List<Button> buttons = new List<Button>();
        root.Query<Button>(className: "unity-button").ToList(buttons);

        /*
         *
         * Buttons click actions 
         */
        
        buttons.ForEach(
            (button) =>
            {
                button.clicked += () =>
                {
                    // Assumes button names are in the format "btn--ActionName"
                    if (!button.name.Contains("--"))
                    {
                        Debug.LogError("[UiController] Button name does not contain '--': " + button.name);
                        return;
                    }
                    string actionName = button.name.ToString().Split("--")[1]; 

                    Debug.Log("[UiController] Button pressed: " + actionName);
                    if(actionName == "splash")
                    {
                        Debug.Log("[UiController] splash screen button clicked.");
                        SceneLoader.Instance.Load("SplashScreen");
                    }
                    else if(actionName == "exit")
                    {
                        Debug.Log("[UiController] exit button clicked.");
                        Application.Quit();
                    }
                    else if(actionName == "restart")
                    {
                        Debug.Log("[UiController] restart button clicked.");
                        SceneLoader.Instance.ReloadCurrentScene();
                    }
                    else if(actionName == "resume")
                    {
                        Debug.Log("[UiController] resume button clicked.");
                        // GameManager.Instance.uiPauseMenu.ClosePauseMenu();
                    }
                    else if(actionName == "pause")
                    {
                        Debug.Log("[UiController] pause button clicked.");
                        // GameManager.Instance.uiPauseMenu.OpenPauseMenu();   
                    }
                    else if(actionName == "next")
                    {
                        // Debug.Log("[UiController] next button clicked.");
                        // int levelNumber = SceneLoader.Instance.GetCurrentLevelIndex();
                        // if(SceneLoader.Instance.SceneExists("Level " + (levelNumber + 1)))
                        //     SceneLoader.Instance.Load("Level " + (levelNumber + 1));
                        // else
                        //     SceneLoader.Instance.Load("SelectLevel");
                    }
                    else if(actionName == "select-level")
                    {
                        Debug.Log("[UiController] level-select button clicked.");
                        SceneLoader.Instance.Load("SelectLevel");
                    }
                    else if(actionName.StartsWith("level-"))
                    {
                        string levelNumberStr = actionName.Split("-")[1];
                        Debug.Log("[UiController] level button clicked: " + levelNumberStr);
                        SceneLoader.Instance.Load("Level " + levelNumberStr);
                    }
                    else
                    {
                        Debug.LogError("[UiController] Unhandled button action: " + actionName);
                    }

                    AudioManager.Instance.PlaySound("button-click");
                };

            }
        );


        docReady = true;
    }

    public void _AttachDocument()
    {
        uiDoc =  GetComponent<UIDocument>();
        root = uiDoc.rootVisualElement;
        panelSettings = uiDoc.panelSettings;
    }


}