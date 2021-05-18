using System.Collections;
using System.Collections.Generic;
using com.mineorbit.dungeonsanddungeonscommon;
using UnityEngine;

public class LobbyLogic : MonoBehaviour
{
    bool[] ready = new bool[4];
    public void Start()
    {
        NetworkManager.readyEvent.AddListener((x) => { ready[x.Item1] = x.Item2;
            checkGo();
        });    
    }

    void CheckGo()
    {
        int c = 0;
        for(int i = 0; i<4;i++ )
        {
            if (PlayerManager.GetPlayerById(i) != null)
            {
                c++;
                if (!ready[i])
                    return;
            }
        }
        if(c>0)
            ServerManager.instance.performAction(ServerManager.GameAction.PrepareGame);
    }
    
}
