using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Mirror.Examples.MultipleAdditiveScenes
{
    public class GetLocalPlayer : NetworkBehaviour
    {

        public static GetLocalPlayer localPlayerM;
        public GameObject matchMakerController;

        //This script you will set in a local player in the gameObject of DefineLocalPlayer script   


        private bool ready = false;
        public Button readyButton;
        public GameObject netManager;
        public int x = 0;
        public bool localPlayer = false;

        void Start()
        {
            if (isLocalPlayer)
            {
                //GameObject.Find("Player").GetComponent<ScriptForMooving>().localPlayer = gameObject;
                netManager = GameObject.Find("NetworkManager");
                localPlayer = true;

                matchMakerController = GameObject.Find("MatchMakerController");
            }
            readyButton = GameObject.Find("Canvas").GetComponent<buttonSelector>().button;
            Button buttonReady = readyButton.GetComponent<Button>();
            buttonReady.onClick.AddListener(ChangeReadyState);
            
        }
        public override void OnStartServer()
        {
            netManager = GameObject.Find("NetworkManager");
            matchMakerController = GameObject.Find("MatchMakerController");
            readyButton = GameObject.Find("Canvas").GetComponent<buttonSelector>().button;
            Button buttonReady = readyButton.GetComponent<Button>();
            buttonReady.onClick.AddListener(ChangeReadyState);
        }
        void Update()
        {
            if (ready == true && isLocalPlayer == true && x == 0)
            {
                transform.position = new Vector3(0, -20, 0);
                x = 1;
            }
        }

        public void ChangeReadyState()
        {
            //Debug.Log("Check Line");
            if (localPlayer)
            {
                
                ready = true;
                Debug.Log("Ready true");
                //here, when player is ready you will create (call) another game scene (this function only works in server-side)
                //netManager.GetComponent<Mirror.Examples.MultipleAdditiveScenes.MultiSceneNetManager>().addNewScene();
                //ChangePlayerScene();
                StartCoroutine(setPlayerToNewScene());
                //
                //to be more organized, i will set this script to playerStats

            }

           // CmdAddToList();

        }

        IEnumerator setPlayerToNewScene()
        {
            yield return new WaitForSeconds(2f);
            
            ChangePlayerScene();
        }
        
        [Command]
        public void ChangePlayerScene(NetworkConnectionToClient conn = null)
        {
            netManager.GetComponent<Mirror.Examples.MultipleAdditiveScenes.MultiSceneNetManager>().changeScene(conn, gameObject.GetComponent<playerStats>().currMapInstance);

            /*
            if (isLocalPlayer)
            {
                
                gameObject.transform.position = new Vector3(0, 0, 0);
                
            }*/
        }
        




        [Command]
        public void CmdAddToList()
        {
            //MatchMakerController.instance.matchIDs.Add(matchID);
            
           
        }
        
        

    }
}
