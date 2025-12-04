using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

[RequireComponent(typeof(UIDocument))]
public class UiPopin : UiController, IOpenable
{

    public bool isOpen { get; set; } = false;


    private void Start()
    {
        StartCoroutine(SetRot());
    }

    public void Open()
    {
        if (isOpen) return;
        Debug.Log("[UiPopin] Opening popin.");

        var popin = root.Q<VisualElement>("popin");
        popin.AddToClassList("open");
        popin.RemoveFromClassList("close");
        isOpen = true;
        
        Debug.Log("[UiPopin] Class list after adding 'open': " + string.Join(", ", popin.GetClasses()));
    }

    public void Close()
    {
        if (!isOpen) return;
        Debug.Log("[UiPopin] Closing popin.");
        
        var popin = root.Q<VisualElement>("popin");
        popin.AddToClassList("close");
        popin.RemoveFromClassList("open");
        isOpen = false;
    }
    


    /// <summary>
    ///  Rotate popin-inner based on mouse position
    /// </summary>
    private IEnumerator SetRot()
    {
//         string lastMousePos = "";
//         while (true)
//         {
//             yield return new WaitForEndOfFrame();        

//             Vector2 mousePos = InputHandlersManager.Instance.mousePosition;
//             VisualElement popinInner = root.Q<VisualElement>("popin-inner");
//             if (popinInner != null)
//             {
//                 // // check which quadrant the mouse is in
//                 // if (mousePos.x < Screen.width / 2f && mousePos.y < Screen.height / 2f)
//                 // {
//                 //     // top-left
//                 //     if (lastMousePos == "tl") continue;
//                 //     lastMousePos = "tl";
//                 //     popinInner.RemoveFromClassList("rotate-tr");
//                 //     popinInner.RemoveFromClassList("rotate-bl");
//                 //     popinInner.RemoveFromClassList("rotate-br");
//                 //     popinInner.AddToClassList("rotate-tl");
//                 //     popinInner.MarkDirtyRepaint();
//                 //     yield return new WaitForEndOfFrame();
//                 //     // uiDoc.enabled = false;
//                 //     yield return new WaitForEndOfFrame();
//                 //     // uiDoc.enabled = true;
//                 //     yield return new WaitForEndOfFrame();


//                 //     root.MarkDirtyRepaint();
//                 //     popinInner.style.display = DisplayStyle.None; 
//                 //     yield return new WaitForEndOfFrame();
//                 //     root.MarkDirtyRepaint();
//                 //     popinInner.style.display = DisplayStyle.Flex; 
//                 //     // popinInner.style.rotate = new StyleRotate(new Rotate(15f, new Vector3(-5f, 5f, 1f)));
//                 // }
//                 // else if (mousePos.x >= Screen.width / 2f && mousePos.y < Screen.height / 2f)
//                 // {
//                 //     // top-right
//                 //     if (lastMousePos == "tr") continue;
//                 //     lastMousePos = "tr";
//                 //     popinInner.RemoveFromClassList("rotate-tl");
//                 //     popinInner.RemoveFromClassList("rotate-bl");
//                 //     popinInner.RemoveFromClassList("rotate-br");
//                 //     popinInner.AddToClassList("rotate-tr");
//                 //     popinInner.MarkDirtyRepaint();
//                 //     yield return new WaitForEndOfFrame();
//                 //     // uiDoc.enabled = false;
//                 //     yield return new WaitForEndOfFrame();
//                 //     // uiDoc.enabled = true;
//                 //     yield return new WaitForEndOfFrame();


//                 //     root.MarkDirtyRepaint();
//                 //     popinInner.style.display = DisplayStyle.None; 
//                 //     yield return new WaitForEndOfFrame();
//                 //     root.MarkDirtyRepaint();
//                 //     popinInner.style.display = DisplayStyle.Flex; 
//                 //     // popinInner.style.rotate = new StyleRotate(new Rotate(15f, new Vector3(5f, 5f, 1f)));
//                 // }
//                 // else if (mousePos.x < Screen.width / 2f && mousePos.y >= Screen.height / 2f)
//                 // {
//                 //     // bottom-left
//                 //     if (lastMousePos == "bl") continue;
//                 //     lastMousePos = "bl";
//                 //     popinInner.RemoveFromClassList("rotate-tr");
//                 //     popinInner.RemoveFromClassList("rotate-tl");
//                 //     popinInner.RemoveFromClassList("rotate-br");
//                 //     popinInner.AddToClassList("rotate-bl");
//                 //     popinInner.MarkDirtyRepaint();
//                 //     yield return new WaitForEndOfFrame();
//                 //     // uiDoc.enabled = false;
//                 //     yield return new WaitForEndOfFrame();
//                 //     // uiDoc.enabled = true;
//                 //     yield return new WaitForEndOfFrame();


//                 //     root.MarkDirtyRepaint();
//                 //     popinInner.style.display = DisplayStyle.None; 
//                 //     yield return new WaitForEndOfFrame();
//                 //     root.MarkDirtyRepaint();
//                 //     popinInner.style.display = DisplayStyle.Flex; 
//                 //     // popinInner.style.rotate = new StyleRotate(new Rotate(15f, new Vector3(-5f, -5f, 1f)));
//                 // }
//                 // else if (mousePos.x >= Screen.width / 2f && mousePos.y >= Screen.height / 2f)
//                 // {
//                 //     // bottom-right
//                 //     if (lastMousePos == "br") continue;
//                 //     lastMousePos = "br";
//                 //     popinInner.RemoveFromClassList("rotate-tr");
//                 //     popinInner.RemoveFromClassList("rotate-tl");
//                 //     popinInner.RemoveFromClassList("rotate-bl");
//                 //     popinInner.AddToClassList("rotate-br");
//                 //     popinInner.MarkDirtyRepaint();
//                 //     yield return new WaitForEndOfFrame();
//                 //     // uiDoc.enabled = false;
//                 //     yield return new WaitForEndOfFrame();
//                 //     // uiDoc.enabled = true;
//                 //     yield return new WaitForEndOfFrame();


//                 //     root.MarkDirtyRepaint();
//                 //     popinInner.style.display = DisplayStyle.None; 
//                 //     yield return new WaitForEndOfFrame();
//                 //     root.MarkDirtyRepaint();
//                 //     popinInner.style.display = DisplayStyle.Flex; 
//                 //     // popinInner.style.rotate = new StyleRotate(new Rotate(15f, new Vector3(5f, -5f, 1f)));
//                 // }


// // popinInner.SetEnabled(false);
// // popinInner.SetEnabled(true);
// // popinInner.MarkDirtyRepaint();

//                 // THIS IS BUGGY IN UI TOOLKIT 
//                 // Vector2 centerScreen = new Vector2(Screen.width / 2f, Screen.height / 2f);
//                 // Vector2 direction = (mousePos - centerScreen).normalized * 10f;
//                 // Vector3 rotationAxis = new Vector3(direction.x * 5, -direction.y * 5, 1f);
//                 // float angle = 15f;
//                 // StyleRotate targetStyleRot = new StyleRotate(new Rotate(angle, rotationAxis));
//                 // popinInner.style.rotate = StyleKeyword.Null;
//                 // popinInner.style.rotate = targetStyleRot;
//             }

//         }

        yield return null;
    }

}