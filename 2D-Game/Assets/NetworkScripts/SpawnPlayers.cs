using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;

public class SpawnPlayers : MonoBehaviour
{
    public GameObject PlayerPrefab;
    //public float minX, minY, maxX, maxY;
    private void Start()
    {
        if (PlayerPrefab == null)
        {
            Debug.LogError("<Color=Red><a>Missing</a></Color> playerPrefab Reference. Please set it up in GameObject 'Game Manager'", this);
        }
        else
        {
            Debug.LogFormat("We are Instantiating LocalPlayer from {0}", Application.loadedLevelName);
            // we're in a room. spawn a character for the local player. it gets synced by using PhotonNetwork.Instantiate
            PhotonNetwork.Instantiate(this.PlayerPrefab.name, new Vector3(0f, 5f, 0f), Quaternion.identity, 0);
        }
    }
    //private void Awake ()
    //{
   //     GameCanvas.SetActive(true);
    //}
    //void SpawnPlayer()
   // {
    //    float randomValue = Random.Range(-1f, 1f);
    //    PhotonNetwork.Instantiate(PlayerPrefab.name, new Vector2(this.transform.position.x * randomValue, this.transform.position.y), Quaternion.identity, 0);
  //      GameCanvas.SetActive(false);
  //      SceneCamera.SetActive(true);
   // }

}
