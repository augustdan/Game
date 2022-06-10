using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

//you dont need to set a namespace for every script
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
        public bool isOwner = false;
        public bool hasStarted = false;
        public Rigidbody2D rb;
        public PlayerControllerD pc;

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
            //Get isOwner from playerStats
            if (isLocalPlayer)
            {
                isOwner = gameObject.GetComponent<playerStats>().isOwner;

                //Lest create a list checking all players with the same matchId and check who is owner and if owner has started a match
                //Needs to be in this script to after check send info to server to change scene
                GameObject[] playerInMyRoom = GameObject.FindGameObjectsWithTag("Player");

                foreach (GameObject playerinRoom in playerInMyRoom)
                {
                    //Get all players that has my same curr Match Id, check if is owner and if has started match
                    if (playerinRoom.GetComponent<playerStats>().currMatchId == gameObject.GetComponent<playerStats>().currMatchId && playerinRoom.GetComponent<playerStats>().isOwner && playerinRoom.GetComponent<GetLocalPlayer>().hasStarted)
                    {
                        //Send to server a message to change scene in client side
                        ChangeReadyState();
                    }
                }

                //keep sending data to all clients but we just want that a owner has a ag has Started as true
                if (hasStarted && isOwner)
                {
                    CmdSendToAllMatchStarted();                    
                }                                
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
                hasStarted = true;
                //
                //to be more organized, i will set this script to playerStats

                //and add this line. at all
                //transform.position = new Vector3(0, 3, 0);
                StartCoroutine(enableScript());
            }

            // CmdAddToList();

        } 
        
        IEnumerator enableScript()
        {
            yield return new WaitForSeconds(2f);
            CmdChangeRbAndPc();
            
        }
        [Command]
        void CmdChangeRbAndPc()
        {
           
            RpcChangeRbAndPc();
        }
        [ClientRpc]
        void RpcChangeRbAndPc()
        {
            pc.enabled = true;
            rb.simulated = true;
        }
        IEnumerator setPlayerToNewScene()
        {
            //Lets do it another way
            yield return new WaitForSeconds(2f);
            ChangePlayerScene();
        }

        [Command]
        public void ChangePlayerScene(NetworkConnectionToClient conn = null)
        {
            netManager.GetComponent<Mirror.Examples.MultipleAdditiveScenes.MultiSceneNetManager>().changeScene(conn, gameObject.GetComponent<playerStats>().currMapInstance);
            ready = false;
        }

        [Command]
        void CmdSendToAllMatchStarted()
        {
            RpcSendToAllMatchStarted();
        }

        [ClientRpc]
        void RpcSendToAllMatchStarted ()
        {            
            if (!isLocalPlayer)
            {
                gameObject.GetComponent<GetLocalPlayer>().hasStarted = true;
            }            
        }


        [Command]
        public void CmdAddToList()
        {
            //MatchMakerController.instance.matchIDs.Add(matchID);


        }



    }
}
