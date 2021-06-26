using System;
using System.Collections;
using System.Collections.Generic;
using com.mineorbit.dungeonsanddungeonscommon;
using UnityEngine;

public class LobbyLogic : MonoBehaviour
{
    public bool[] ready = new bool[4];
    public void Start()
    {
        NetworkManager.readyEvent.AddListener((x) => { ready[x.Item1] = x.Item2;
            CheckGo();
        });
    }


    private void Update()
    {
        if (ServerManager.instance.GetState() == ServerManager.State.Lobby)
        {
            SpawnPlayersInLobby();
        }
    }

    static void SpawnPlayersInLobby()
    {
        for (int i = 0; i < 4; i++)
        {
            Player p = PlayerManager.GetPlayerById(i);
            if(p != null)
            {
                Vector3 pos = LobbyPosition(i);
                if((p.transform.position - pos).magnitude > 1f )
                    PlayerManager.playerManager.SpawnPlayer(i, pos);
            }
        }
    }

    static Vector3 LobbyPosition(int i)
    {
        return new Vector3(i * 8, 6, 0);
    }
    
    
    void CheckGo()
    {
        MainCaller.Do( () =>
        {
            int c = 0;
            for(int i = 0; i<4;i++ )
            {
                if (PlayerManager.playerManager.players[i] != null)
                {
                    c++;
                    if (!ready[i])
                        return;
                }
            }

            if (c > 0)
            {
                ServerManager.instance.go = true;
                for (int i = 0; i < 4; i++)
                {
                    ready[i] = false;
                }
            }
        });
    }
    
}
