using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using com.mineorbit.dungeonsanddungeonscommon;

public class GameLogic : MonoBehaviour
{

    long levelId = 0;
    public static GameLogic current;
    
    
    public void Awake()
    {
        if (current != null) Destroy(this);
        current = this;
    }

    
    //Called on Victory
    public static void EndRound()
    {
        //Despawn Players
        for(int i = 0;i<4;i++)
        {
            PlayerManager.playerManager.DespawnPlayer(i);
        }

        if (GameLogic.current != null)
        {
            Destroy(GameLogic.current);
        }
    }

    public static void PrepareRound(Transform t)
    {
        t.gameObject.AddComponent<GameLogic>();
        //Hier war mal ein player spawn ist aber falsch per se
    }

    public static void ClearRound()
    {
        EndRound();
        LevelManager.Clear();

        for (int i = 0; i < 4; i++)
        {
            ServerManager.instance.RemoveClient(i);
        }
    }

    public void StartRound()
    {
        //Reset Player Data if exists
        for (int i = 0; i < 4; i++)
        {
            if (PlayerManager.playerManager.players[i] != null)
                PlayerManager.playerManager.SpawnPlayer(i, PlayerManager.playerManager.GetSpawnLocation(i));
        }

        //Set Level As Selected
        LevelMetaData levelMetaData = null;
        // = LevelManager.GetSelectedLevel();


        if (levelMetaData == null)
        {
            ServerManager.instance.performAction(ServerManager.GameAction.EndGame); 
        }
        else
        { 

            LevelDataManager.Load(levelMetaData);

            //Send LevelData

            //and Spawn Players in Positions
            for (int i = 0;i<4;i++)
            {
            //Level.currentLevel.SendChunkAt(Level.currentLevel.spawn[i].transform.position, i);
            PlayerManager.playerManager.SpawnPlayer(i, PlayerManager.playerManager.GetSpawnLocation(i));
            }
        }

    }
    
    
   
    
}
