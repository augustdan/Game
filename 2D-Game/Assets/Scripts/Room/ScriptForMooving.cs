using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Mirror;


namespace Mirror.Examples.MultipleAdditiveScenes
{
    public class ScriptForMooving : NetworkBehaviour
    {
        public GameObject localPlayer;

        // Start is called before the first frame update
        void Start()
        {
            DontDestroyOnLoad(gameObject);
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}