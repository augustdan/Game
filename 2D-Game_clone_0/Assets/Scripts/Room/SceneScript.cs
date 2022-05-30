using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;

namespace Game {
   public class SceneScript : NetworkBehaviour
{
    public Button buttonMove;
    //public PlayerSwitchRooms Script;
    // Start is called before the first frame update
    void Start()
    {
        buttonMove.onClick.AddListener(MovePrefabToRoom);
    }

    // Update is called once per frame
    void Update()
    {

    }
    public void MovePrefabToRoom()
    {
            
        //Script.MoveToRoom();
            Debug.Log("Ñommand has been called");
        }
}

}