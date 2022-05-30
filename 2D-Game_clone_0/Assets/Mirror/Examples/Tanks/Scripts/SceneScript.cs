using UnityEngine;
using UnityEngine.UI;
using Mirror;

// For your game, no need to use this namespace, its just for tank example modification  :)
namespace Mirror.Examples.Tanks
{

    public class SceneScript : NetworkBehaviour
    {
        [SyncVar(hook = nameof(OnReadyChanged))]
        public int readyStatus = 0;

        void OnReadyChanged(int _Old, int _New)
        {
            //this hook is fired when readyStatus is changed via client cmd or directly by host
            // you dont need to use _New, as the variable readyStatus is changed before hook is called

            if (readyStatus == 1)
            {
                statusText.text = "Server Says Go!";
                buttonReady.gameObject.SetActive(false);
            }
            // updating our canvas UI
            SetupScene();
        }

        public Tank playerScript; //    
        public Text statusText;
        public Button buttonReady, buttonDeath, buttonRespawn;


        private void Start()
        {
            //Make sure to attach these Buttons in the Inspector
            buttonReady.onClick.AddListener(ButtonReady);
            //you could choose to fully hide the server only buttons from clients, but for this guide we will show them to have less code involved
            buttonDeath.onClick.AddListener(ButtonDeath);
            buttonRespawn.onClick.AddListener(ButtonRespawn);
        }

        //[ServerCallback]
        public void ButtonReady()
        {
            // you can use the [ServerCallback] tag if server is only ever going to use the function, or do a check inside for  if( isServer ) { }
            if (isServer)
            {
                // optional checks to wait for full team, you can add this in connection joining callbacks, or via UI, upto you.
                //if (NetworkServer.connections.Count > 2)
                //{
                readyStatus = 1;
                //}
                //else
                //{
                //    playerStatusText.text = "Not enough players.";
                //}
            }
            else
            {
                statusText.text = "Server only feature";
            }
        }

        // For faster prototyping, we will have these as buttons, but eventually they will be in your raycast, or trigger code
        public void ButtonDeath()
        {
            playerScript.CmdPlayerStatus(true);
        }

        public void ButtonRespawn()
        {
            playerScript.CmdPlayerStatus(false);
        }

        public void SetupScene()
        {
            if (isServer == false)
            {
                buttonReady.interactable = false;
            }

            if (readyStatus == 0)
            {
                buttonRespawn.interactable = false;
                buttonDeath.interactable = false;
            }
            else if (playerScript)
            {
                 //quick check to make sure playerScript is set before checking its variables to prevent errors
               if (playerScript.isDead == true)
                {
                    buttonRespawn.interactable = true;
                    buttonDeath.interactable = false;
                }
                else if (playerScript.isDead == false)
                {
                   buttonRespawn.interactable = false;
                    buttonDeath.interactable = true;
                }
            }
        }
    }
}