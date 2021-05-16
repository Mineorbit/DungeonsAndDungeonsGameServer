using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using com.mineorbit.dungeonsanddungeonscommon;

public class PlayLogic : MonoBehaviour
{

    long levelId = 0;
    public static PlayLogic current;
    
    
    public void Awake()
    {
        if (current != null) Destroy(this);
        current = this;
    }

    
    //Called on Victory
    

    public static void PrepareRound(Transform t)
    {
        t.gameObject.AddComponent<PlayLogic>();


        //Set Level As Selected
        // = LevelManager.GetSelectedLevel();

        LevelMetaData levelMetaData = LevelDataManager.instance.localLevels[0];

        if (levelMetaData == null)
        {
            ServerManager.instance.performAction(ServerManager.GameAction.EndGame);
        }
        LevelDataManager.Load(levelMetaData,Level.InstantiateType.Default);

        Level.instantiateType = Level.InstantiateType.Play;

        NetworkManagerHandler.RequestPrepareRound();
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
        /*

        */
        //and Spawn Players in Positions
        for (int i = 0;i<4;i++)
        {
            Debug.Log("Spawn "+i);
            Vector3 spawn = PlayerManager.playerManager.GetSpawnLocation(i);
            Debug.Log(spawn);
            PlayerManager.playerManager.SpawnPlayer(i, spawn);
        }

        LevelManager.StartRound(resetDynamic: false);

        NetworkManagerHandler.RequestStartRound();

    }

    public static void EndRound()
    {
        //Despawn Players
        for (int i = 0; i < 4; i++)
        {
            PlayerManager.playerManager.DespawnPlayer(i);
        }

        LevelManager.EndRound(resetDynamic: true);

        if (PlayLogic.current != null)
        {
            Destroy(PlayLogic.current);
        }
    }




}
