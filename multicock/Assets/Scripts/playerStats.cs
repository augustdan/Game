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
    [SyncVar]
    public int next = 0;
    public Button btnHostRoom;
    public Button btnReady;
    public Button btnFind;
    //here we can check if player is owner, and ask server for a new scene(room)
    public bool isOwner = false;
    public bool isReady = false;
    public bool roomIsFool = false;
    public bool isFinding = false;
    public GameObject netManager;
    public int currMapInstance = 0;
    
    public List<GameObject> playerInMyRoom = new List<GameObject>();

    



    // Start is called before the first frame update
    void Start()
    {
        if (isLocalPlayer)
        {
            btnHostRoom = GameObject.Find("Canvas/btnHost").GetComponent<Button>();
            btnHostRoom.onClick.AddListener(CmdAskNewRoom);

            btnReady = GameObject.Find("Canvas/ReadyBtn").GetComponent<Button>();
            btnReady.onClick.AddListener(checkReady);

            btnFind = GameObject.Find("Canvas/FindBtn").GetComponent<Button>();
            btnFind.onClick.AddListener(ChangeIsFindingVariable);

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
                //Send to all players you are owner of room

                //this line was commented out by duyz
                //RpcSetOwner();
            }


            if (isFinding)
            {
                Debug.Log("Finding");
                CmdFindRoom();
            }


            //Check if currMatchId is not empty
            if (currMatchId != "")
            {
                CmdSendMyMatchId(currMatchId);
                //Create a list of all player in game
                GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
                //foreach is to create a loop get/filter all players and set inside list
                foreach (GameObject player in players)
                {
                    //After filter each player (get all gameObject that has tag Player, we need to check what we need
                    //First we need to check if this gameObject found with tag Player has the same matchId I have (check remote player got from list and check with my currMatchid
                    //if this matchId is like mine, then do somthing
                    if (player.GetComponent<playerStats>().currMatchId == gameObject.GetComponent<playerStats>().currMatchId)
                    {
                        //now we gonna create a new list to fill all gameObjects that has same matchId i have
                        //my mistake, we need to check if this gameObject is not in list, if is not in list, then add
                        if (!playerInMyRoom.Contains(player))
                        {
                            playerInMyRoom.Add(player);
                        }

                        /*if (players.Length > 1)
                        {
                            //playerInMyRoom.Remove(player);
                            roomIsFool = true;
                            CmdCheckPlayersInRoom();
                            //RemoveMatch();

                            //GameObject.Find("MatchMakerController").GetComponent<matchController>().matchID.Remove(currMatchId);
                            //GameObject.Find("MatchMakerController").GetComponent<matchController>().fullRooms.Add(currMatchId);
                        }*/

                    }
                    //check if player is in list but change the matchId
                    if (player.GetComponent<playerStats>().currMatchId != gameObject.GetComponent<playerStats>().currMatchId && playerInMyRoom.Contains(player))
                    {
                        playerInMyRoom.Remove(player);
                        playerInMyRoom[0].GetComponent<playerStats>().isOwner = true;
                        playerInMyRoom[0].GetComponent<playerStats>().CmdSetNewOwner();
                    }
                    
                    /*if(!isLocalPlayer && playerInMyRoom.Contains(player) && player.GetComponent<playerStats>().isOwner == true)
                    {
                        gameObject.GetComponent<playerStats>().isOwner = false;
                    }*/
                }
            }
            
        }
    }


    //this is my code

    [Command]
    void CmdCheckPlayersInRoom()
    {
        if(playerInMyRoom.Count > 1)
        {
            //GameObject.Find("MatchMakerController").GetComponent<matchController>().matchID.Remove(currMatchId);
            //GameObject.Find("MatchMakerController").GetComponent<matchController>().fullRooms.Add(currMatchId);
            //GameObject.Find("MatchMakerController").GetComponent<matchController>().RemoveMatchID(currMatchId);
            RpcCheckPlayersInRoom();
        }
        
    }
    [ClientRpc]
    void RpcCheckPlayersInRoom()
    {
        if(playerInMyRoom.Count > 1)
        {

            playerInMyRoom[1].GetComponent<playerStats>().currMatchId = null;
            currMapInstance = 0;
            
            CmdFindRoom();
        }
    }
    void ChangeIsFindingVariable()
    {
        //Check if is localPlayer and if is not empty the isFindingVariable of player
        /*if (isLocalPlayer)
        {
                isFinding = !isFinding;
                CmdVariableFind(isFinding);   
        }
        else
        {
            Debug.Log("Cant");
        }*/

        //look if there is an available room
        if (isLocalPlayer)
        {
            //Get an INT of matchIds in synclist. Remeber that matchID list is create when Player hosts a room, if nobody hosts a room, the value will be 0
            int availableRooms = GameObject.Find("MatchMakerController").GetComponent<matchController>().matchID.Count;
            if (availableRooms > 0)
            {
                //if there is an available room, then join.. Always enter in the last available room, but you can change it to your want
                currMatchId = GameObject.Find("MatchMakerController").GetComponent<matchController>().matchID[availableRooms-1];
            }
            else
            {
                print("There is no available room. You must click in host room to create a room.. Creating a new one");
                CmdAskNewRoom();
            }
        }
    }
    [Command]
    void CmdVariableFind(bool isFindingVariable)
    {
        RpcVariableFind(isFindingVariable);
    }
    [ClientRpc]
    void RpcVariableFind(bool isFindingVariable)
    {
        if (!isLocalPlayer)
        {
            isFinding = isFindingVariable;
        }
    }
    [Command(requiresAuthority = false)]
    void CmdFindRoom()
    {
        Debug.Log("Next variable was changed");
        //currMatchId = GameObject.Find("MatchMakerController").GetComponent<matchController>().matchID[0];
        isFinding = false;

        //currMapInstance = GameObject.Find("MatchMakerController").GetComponent<matchController>().matchID[next];
        if (currMatchId != "")
        {
           // RpcFindRoom();
        }

    }
    [ClientRpc]
    void RpcFindRoom()
    {
        if(currMatchId != "")
        {
            isFinding = false;
        }
            //Debug.Log("Next variable was changed");
            //currMatchId = GameObject.Find("MatchMakerController").GetComponent<matchController>().matchID[next];
        
       
    }

    [Command]
    void RemoveMatch()
    {
        GameObject.Find("MatchMakerController").GetComponent<matchController>().RemoveMatchId(currMatchId);
    }

    //end of my code


    [Command]
    void CmdSetNewOwner()
    {
        RpcSetOwner();
    }

    void checkReady()
    {
        //Check if is localPlayer and if is not empty the current Match Id of player
        if (isLocalPlayer)
        {
            if (currMatchId != "")
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
        isOwner = true;
        if (isServer)
        {
            GameObject.Find("MatchMakerController").GetComponent<matchController>().GetRandomMatchID(gameObject.name);
            CmdAskServerNewScene();
        }        
    }

    [ClientRpc]
    void RpcSetOwner()
    {
        //Check if another player sent a CMD saying it is owner
        if(!isLocalPlayer)
        {
            isOwner = true;
        }        
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
                if(player == gameObject)
                {
                    isOwner = true;
                }
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