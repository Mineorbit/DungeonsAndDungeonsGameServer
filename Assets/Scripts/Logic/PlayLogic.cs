﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using com.mineorbit.dungeonsanddungeonscommon;
using NetLevel;
using UnityEngine.Events;

public class PlayLogic : MonoBehaviour
{

    public static PlayLogic current;
    UnityEvent winEvent = new UnityEvent();
    
    public void Awake()
    {
        if (current != null) Destroy(this);
        current = this;
    }



    static bool preparing = false;

    public static void PrepareRound(Transform t)
    {
        t.gameObject.AddComponent<PlayLogic>();


        //Set Level As Selected

        LevelMetaData levelMetaData = ServerManager.instance.selectedLevel;
        
        HttpManager.DownloadLevel(levelMetaData);

        if (levelMetaData == null)
        {
            ServerManager.instance.performAction(ServerManager.GameAction.EndGame);
        }

        LevelDataManager.Load(levelMetaData,Level.InstantiateType.Default);

        Level.instantiateType = Level.InstantiateType.Play;

        

        NetworkManagerHandler.RequestPrepareRound();

        for (int i = 0; i < 4; i++)
        {
            Debug.Log("Spawn " + i);
            Vector3 spawn = PlayerManager.playerManager.GetSpawnLocation(i);
            Debug.Log(spawn);
            PlayerManager.playerManager.SpawnPlayer(i, spawn);
        }
        preparing = true;
    }

    float eps = 0.5f;
    bool PlayersInSpawn()
    {
        for(int i = 0; i < 4;i++)
        {
            Player p = PlayerManager.GetPlayerById(i);
            
            if (p != null)
            {
                float dist = (p.transform.position - PlayerManager.playerManager.GetSpawnLocation(i)).magnitude;
                Debug.Log("Dist "+p.transform.position+ " "+PlayerManager.playerManager.GetSpawnLocation(i));
                if(dist > eps)
                {
                    return false;
                }
            }
        }
        return true;
    }


    public void Update()
    {
        if(preparing && PlayersInSpawn())
        {
            preparing = false;
            Invoke("StartRound",2f);
        }
    }



    public void StartRound()
    {
        Debug.Log("Starting Round");
        LevelManager.StartRound(resetDynamic: false);

        foreach (Player p in PlayerManager.playerManager.players)
        {
            if (p != null)
            {
                p.points = 0;
            }
        }
        PlayerGoal.GameWinEvent.AddListener(WinRound);

        NetworkManagerHandler.RequestStartRound();

    }


    //Called on Victory
    public static void WinRound()
    {
        current.winEvent.Invoke();
        NetworkManagerHandler.RequestWinRound();

        ServerManager.instance.performAction(ServerManager.GameAction.WinGame);

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


        ServerManager.instance.performAction(ServerManager.GameAction.EndGame);
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


}
