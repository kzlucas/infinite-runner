using System.Collections.Generic;
using UnityEngine;

namespace Components.Tutorials
{

    [System.Serializable]
    public class Tutorial
    {
        public string TutorialKey;
        public bool Completed;
        public GameObject UiGo;
    }

    [CreateAssetMenu(fileName = "Create New Tutorials Data", menuName = "Tutorials Data/New Tutorials Data", order = 1)]
    public class SO_Tutorials : ScriptableObject
    {
        [SerializeField] private List<Tutorial> _tutorials;
        public List<Tutorial> Tutorials => _tutorials;
    }
}