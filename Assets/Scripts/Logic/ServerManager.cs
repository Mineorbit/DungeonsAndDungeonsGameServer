﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;
using com.mineorbit.dungeonsanddungeonscommon;
using NetLevel;

public class ServerManager : MonoBehaviour
{
    public static ServerManager instance;

    public class State : CustomEnum
    {
        public State(string val, int card) : base(val, card)
        {
            Value = val;
            cardinal = card;
        }


        public static State Setup = new State("Setup",0);
        public static State Prepare = new State("Prepare",1);
        public static State Lobby = new State("Lobby",2);
        public static State Load = new State("Load", 4);
        public static State Play = new State("Play",5);
        public static State GameOver = new State("GameOver",6);
    }

    public class GameAction : CustomEnum
    {
        public GameAction(string val, int card) : base(val, card)
        {
            Value = val;
            cardinal = card;
        }


        public static GameAction GoLive = new GameAction("GoLive",0);
        public static GameAction PrepareServer = new GameAction("Prepare", 1);
        public static GameAction PrepareGame = new GameAction("Prepare",2);
        public static GameAction StartGame = new GameAction("GoLive",3);
        public static GameAction EndGame = new GameAction("GoLive",4);
        public static GameAction WinGame = new GameAction("GoLive",5);
        public static GameAction CancelGame = new GameAction("GoLive",6);
    }
    public FSM<State, GameAction> serverState = new FSM<State, GameAction>();

    public GameObject playerStore;
    //Networking
    public Server server;

    //Settings
    public bool Local = true;


    public LevelMetaData selectedLevel;

    void Start()
    {
        //This is necessary
        NetworkHandler.isOnServer = true;
        
        GameConsole.Log("IS ON SERVER "+NetworkHandler.isOnServer);

        Level.instantiateType = Level.InstantiateType.Default;
        if (instance==null)
        {
            instance = this;
        }else if(instance!=this)
        {
            Destroy(this);
        }
        SetupFSM();
        serverState.Move(GameAction.PrepareServer);
        NetworkManager.lobbyRequestEvent.AddListener((x) => { Debug.Log("Selected Level: "+x.SelectedLevel); selectedLevel = x.SelectedLevel;  });
    }  
    void SetupServer()
    {
        server = new Server();
        
        lobbyLogic = gameObject.AddComponent<LobbyLogic>();
    }
    public void AddClient(int localId, Client c)
    {
        PlayerManager.playerManager.Add(localId,"Test", true);
    }

    public bool go;

    public void Update()
    {
        if (go)
        {
            go = false;
            instance.performAction(ServerManager.GameAction.PrepareGame);
        }
    }
    
    public void RemoveClient(int localid)
    {

        server.Disconnect(localid);
        PlayerManager.playerManager.Remove(localid);
        
        
    }


    public void OnDestroy()
    {
        if(Server.instance != null)
            Server.instance.DisconnectAll();
    }

    public State GetState()
    {
        return serverState.state;
    }


    private LobbyLogic lobbyLogic;
    void SetupFSM()
    {
        serverState.state = State.Setup;
        Action<GameAction> actSetup = x => {
            SetupServer();
            Debug.Log("Setup done");
            serverState.Move(GameAction.GoLive);

        };
        Action<GameAction> actLive = x => {
            Debug.Log("Server is ready");
            playerStore.SetActive(true);
            server.Start();
            NetworkManager.isConnected = true;
        };
        
        Action<GameAction> actPrepareGame = x =>
        {
            Debug.Log("Setting up");
            server.StopListen();
            playerStore.SetActive(false);
            PlayLogic.PrepareRound(this.transform);


        };



        Action<GameAction> actDropGame = x => {
            Debug.Log("Restarting");
            PlayLogic.ClearRound();
            server.StartListen();
        };

        Action<GameAction> actCancel = x => {
            Debug.Log("Game canceled");
            LevelManager.Clear();
            PlayLogic.EndRound();
            PlayLogic.PrepareRound(this.transform);
            server.StartListen();
        };

        Action<GameAction> actWin = x => {
            LevelManager.Clear();
            PlayLogic.EndRound();
            server.StartListen();
        };

        serverState.transitions.Add(new Tuple<State, GameAction>(State.Setup, GameAction.PrepareServer), new Tuple<Action<GameAction>, State>(actSetup, State.Prepare));

        serverState.transitions.Add(new Tuple<State,GameAction>(State.Prepare,GameAction.GoLive),new Tuple<Action<GameAction>,State>(actLive,State.Lobby));
        serverState.transitions.Add(new Tuple<State, GameAction>(State.Lobby, GameAction.PrepareGame), new Tuple<Action<GameAction>, State>(actPrepareGame, State.Play));

        serverState.transitions.Add(new Tuple<State, GameAction>(State.Play, GameAction.EndGame), new Tuple<Action<GameAction>, State>(actDropGame, State.Lobby));
        serverState.transitions.Add(new Tuple<State, GameAction>(State.Play, GameAction.WinGame), new Tuple<Action<GameAction>, State>(actWin, State.Lobby));
        serverState.transitions.Add(new Tuple<State, GameAction>(State.Play, GameAction.CancelGame), new Tuple<Action<GameAction>, State>(actCancel, State.Lobby));
    }


    void Stop()
    {
        if(server != null)
        { 
            server.DisconnectAll();
            server.StopListen();
        }
    }

    public void performAction(GameAction action)
    {
        serverState.Move(action);
    }

    void OnDisable()
    {
        Debug.Log("Server stopping");
        Stop();
    }
    

}
