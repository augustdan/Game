using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using Mirror;

namespace Mirror.Examples.MultipleAdditiveScenes
{
    public class MatchMakerController : NetworkBehaviour
    {
        public static MatchMakerController instance;

        
        public SyncList<String> matchIDs = new SyncList<String>();
       //public SyncList<GetLocalPlayer> = new SyncList<>();

        // Start is called before the first frame update
        void Start()
        {
            instance = this;
        }

        // Update is called once per frame
        void Update()
        {

        }
        public static string GetRandomMatchID()
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
            return _id;
        }
    }
    
}
