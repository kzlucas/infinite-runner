using System.Collections.Generic;
using UnityEngine;



[System.Serializable]
public class Tutorial
{
    public string tutorialKey;
    public bool completed;
    public GameObject uiGo;
}

[CreateAssetMenu(fileName = "Create New Tutorials Data", menuName = "Tutorials Data/New Tutorials Data", order = 1)]
public class SO_Tutorials : ScriptableObject
{
    [SerializeField] private List<Tutorial> _tutorials;
    public List<Tutorial> Tutorials => _tutorials;
}