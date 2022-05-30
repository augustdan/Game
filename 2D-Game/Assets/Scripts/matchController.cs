using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class matchController : NetworkBehaviour
{    
    public SyncList<string> matchID = new SyncList<string>();
    public int numOfready = 0;

    public SyncList<string> room = new SyncList<string>();
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void GetRandomMatchID(string playerName)
    {
        //I will do server-side implementation. What we will do is create a function to server generates a matchId when player ask
        if (isServer)
        {
            string _id = string.Empty;
            for (int i = 0; i < 5; i++)
            {
                int random = UnityEngine.Random.Range(0, 36);
                if (random < 26)
                {
                    _id += (char)(random + 65);
                }
                else
                {
                    _id += (random - 26).ToString();
                }
            }
            Debug.Log($"Random Match ID: {_id}");
            //After generate a new matchId we will add to matchID list
            matchID.Add(_id);
            GameObject.Find(playerName).GetComponent<playerStats>().currMatchId = _id;
            //Sorry, we need to get current instances -1 hehehe
           
        }        
    }
}
