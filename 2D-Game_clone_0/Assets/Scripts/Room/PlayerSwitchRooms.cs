using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Mirror;
using UnityEngine.UI;

namespace Mirror.Examples.MultipleAdditiveScenes
{
    public class PlayerSwitchRooms : NetworkBehaviour
    {
        public Button buttonMove;
        // Start is called before the first frame update
        void Start()
        {
            // buttonMove.onClick.AddListener(MoveToRoom);
        }

        // Update is called once per frame
        void Update()
        {

        }
        public void ButtonClick()
        {
            if (isLocalPlayer)
            {
                SceneManager.MoveGameObjectToScene(GameObject.Find("Player").GetComponent<ScriptForMooving>().localPlayer, SceneManager.GetSceneByName("LobbyScene"));
                CmdMoveToRoom(NetworkClient.connection.connectionId);
            }
        }
        [Command]
        public void CmdMoveToRoom(int myConnId)
        {
            if (isLocalPlayer)
            {
                RpcMoveToRoom(myConnId);
            }
        }

        [ClientRpc]
        public void RpcMoveToRoom(int connectionId)
        {
            if (!isLocalPlayer)
            {
                GameObject RemotePlayer = NetworkIdentity.spawned[netId].gameObject;
                SceneManager.MoveGameObjectToScene(GameObject.Find("Player").GetComponent<ScriptForMooving>().localPlayer, SceneManager.GetSceneByName("LobbyScene"));
            }
        }
        //public void MoveToRoom()
        //{
        //    if (isLocalPlayer)
        //    {
        //       SceneManager.MoveGameObjectToScene(GameObject.Find("Player").GetComponent<ScriptForMooving>().localPlayer, SceneManager.GetSceneByName("LobbyScene"));

        //      RpcMoveToRoom();
        //  }
        // }
        //[ClientRpc]
        //public void RpcMoveToRoom()
        // {
        //     SceneManager.MoveGameObjectToScene(GameObject.Find("Player").GetComponent<ScriptForMooving>().localPlayer, SceneManager.GetSceneByName("LobbyScene"));
        // }
    }
}
