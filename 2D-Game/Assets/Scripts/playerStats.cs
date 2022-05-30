using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;

public class playerStats : NetworkBehaviour
{
    //this string will be filled when player creates or joins a match and sync to all players   
    [SyncVar]
    public string currMatchId;
    public Button btnHostRoom;
    public Button btnReady; 
    //here we can check if player is owner, and ask server for a new scene(room)
    public bool isOwner = false;   
    public bool isReady = false;
    public GameObject netManager;
    public int currMapInstance = 0;

    // Start is called before the first frame update
    void Start()
    {       
        if (isLocalPlayer)
        {
            btnHostRoom = GameObject.Find("Canvas/btnHost").GetComponent<Button>();
            btnHostRoom.onClick.AddListener(CmdAskNewRoom);

            btnReady = GameObject.Find("Canvas/ReadyBtn").GetComponent<Button>();
            btnReady.onClick.AddListener(checkReady);

            GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
            int i = 0;
            foreach (GameObject player in players)
            {
                gameObject.name = "Player" + i++;
            }            
        }
        netManager = GameObject.Find("NetworkManager");
    }

    // Update is called once per frame
    void Update()
    {
        if (isLocalPlayer)
        {
            if (isOwner)
            {
                //Here localPlayer will keep updating all remote players his currMatchId and currMapInstance, but only if owner is true
                CmdSyncMatchAndMapInstance(currMatchId, currMapInstance);
            }

            if (currMatchId != "")
            {
                CmdSendMyMatchId(currMatchId);
            }
            
        }        
    }

    void checkReady()
    {        
        //Check if is localPlayer and if is not empty the current Match Id of player
        if (isLocalPlayer)
        {
            if(currMatchId != "")
            {
                isReady = true;
                CmdSyncReady(isReady);
            }
            else
            {
                print("You cannot be ready without create or join a match");
            }
        }
    }

    //I forgot to set this [Command] tag, without it will not work
    [Command(requiresAuthority = false)]
    void CmdAskNewRoom()
    {        
        if (isServer)
        {
            GameObject.Find("MatchMakerController").GetComponent<matchController>().GetRandomMatchID(gameObject.name);
            CmdAskServerNewScene();
        }

        /*
        if (isLocalPlayer)
        {           
            isOwner = true;
            //if owner is true, and is LocalPlayer will ask server a new scene
            CmdAskServerNewScene();
        }*/
    }

    [Command]
    void CmdSendMyMatchId(string id)
    {
        RpcSendToAllMyId(id);
    }

    [ClientRpc]
    void RpcSendToAllMyId(string id)
    {
        if (!isLocalPlayer)
        {
            gameObject.GetComponent<playerStats>().currMatchId = id;
        }
    }

    [Command]
    void CmdSyncMatchAndMapInstance(string matchId, int mapInstance)
    {
        RpcSycnMatchIdAndMapInstance(matchId, mapInstance);
    }

    [ClientRpc]
    void RpcSycnMatchIdAndMapInstance(string matchId, int mapInstance)
    {
        //I forgot, we need to check from all players, we can use in this case a tag to create a search
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject player in players)
        {
            if (player.GetComponent<playerStats>().currMatchId == matchId)
            {
                player.GetComponent<playerStats>().currMapInstance = mapInstance;
            }
        }
            
    }

    [Command(requiresAuthority = false)]
    void CmdAskServerNewScene()
    {
        //When player is an owner of room (create a matchId) he will send a info to server asking a new scene to be added
        if (isServer)
        {
            //When server add a new scene, it will auto change instances value, i'll get this new value to set to player (owner)
            netManager.GetComponent<Mirror.Examples.MultipleAdditiveScenes.MultiSceneNetManager>().addNewScene();
            int roomInstaceInServer = netManager.GetComponent<Mirror.Examples.MultipleAdditiveScenes.MultiSceneNetManager>().instances;
            gameObject.GetComponent<playerStats>().currMapInstance = roomInstaceInServer;            
        }
        //RpcAddinstanceToClient();
    }

    [ClientRpc]
    void RpcAddinstanceToClient()
    {
        if (isLocalPlayer)
        {
            //When server add a new scene, it will auto change instances value, i'll get this new value to set to player (owner)
            netManager.GetComponent<Mirror.Examples.MultipleAdditiveScenes.MultiSceneNetManager>().addNewScene();
            int roomInstaceInServer = netManager.GetComponent<Mirror.Examples.MultipleAdditiveScenes.MultiSceneNetManager>().instances;
            gameObject.GetComponent<playerStats>().currMapInstance = roomInstaceInServer;
        }
    }

    [Command]
    void CmdSyncReady(bool rdy)
    {
        RpcSyncReady(rdy);
    }

    [ClientRpc]
    void RpcSyncReady(bool rdy)
    {
        if (!isLocalPlayer)
        {
            isReady = rdy;
        }
    }
    
   
}
