using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Mirror;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Game
{
    public class UIControllerOne : MonoBehaviour
    {
        // Start is called before the first frame update
        void Start()
        {
           
        }

        // Update is called once per frame
        

        public void CreateRoom()
        {
            NetworkManagerD networkManagerD = GameObject.Find("Manager").GetComponent<NetworkManagerD>();
            networkManagerD.CreateSubScenesForGame();
            
        }
        public void MoveToRoom()
        {
            Debug.Log("Has Moved To Room");
            SceneManager.MoveGameObjectToScene(gameObject, SceneManager.GetSceneByName("LobbyScene"));
        }
    }
}