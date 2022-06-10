using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class matchController : NetworkBehaviour
{
    public SyncList<string> matchID = new SyncList<string>();
    public SyncList<string> numOfready = new SyncList<string>();

    public SyncList<string> fullRooms = new SyncList<string>();
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (isServer)
        {
            //Get all players in game
            GameObject[] allPlayers = GameObject.FindGameObjectsWithTag("Player");

            
            foreach (GameObject player in allPlayers)
            {
                //Check if player's matchId has in matchController list and check if the list numOfready doesnt have a player name. If there isn't, add to a list
                if (matchID.Contains(player.GetComponent<playerStats>().currMatchId) && !numOfready[matchID.IndexOf(player.GetComponent<playerStats>().currMatchId)].Contains(player.gameObject.name))
                {
                    //We will check if any room has two players by creating a split of string (i think this will be the best way)... if there is 2 elements then removes a matchId from list
                    if (numOfready[matchID.IndexOf(player.GetComponent<playerStats>().currMatchId)] == "")
                    {
                        numOfready[matchID.IndexOf(player.GetComponent<playerStats>().currMatchId)] = player.gameObject.name;
                    }
                    else
                    {
                        numOfready[matchID.IndexOf(player.GetComponent<playerStats>().currMatchId)] = numOfready[matchID.IndexOf(player.GetComponent<playerStats>().currMatchId)] + "," + player.gameObject.name;
                    }
                    
                }
            }

            for (int i = 0; i < numOfready.Count; i++)
            {
                string[] splitArray = numOfready[i].Split(char.Parse(","));
                if(splitArray.Length == 2)
                {
                    //Remove the matchId from list if there is two players in room and the list numOfReady too to prevent a new player is added to it
                    matchID.RemoveAt(i);
                    numOfready.RemoveAt(i);
                }
            }
        }
        
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
            //Everytime a new matchId is added, will create a list to sync number of ready
            numOfready.Add("");
            GameObject.Find(playerName).GetComponent<playerStats>().currMatchId = _id;
            //Sorry, we need to get current instances -1 hehehe

        }
    }
    public void RemoveMatchId(string match)
    {
        matchID.Remove(match);
    }
}