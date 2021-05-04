using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;
using com.mineorbit.dungeonsanddungeonscommon;

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
        public static State Play = new State("Play",3);
        public static State GameOver = new State("GameOver",4);
    }

    public class GameAction : CustomEnum
    {
        public GameAction(string val, int card) : base(val, card)
        {
            Value = val;
            cardinal = card;
        }


        public static GameAction GoLive = new GameAction("GoLive",0);
        public static GameAction Prepare = new GameAction("GoLive",1);
        public static GameAction StartGame = new GameAction("GoLive",2);
        public static GameAction EndGame = new GameAction("GoLive",3);
        public static GameAction WinGame = new GameAction("GoLive",4);
        public static GameAction CancelGame = new GameAction("GoLive",5);
    }
    FSM<State, GameAction> serverState;


    //Networking
    public Server server;

    //Settings
    public bool Local = true;
    string password = "Test";


    void Start()
    {
        NetworkLevelObject.enableNetworking = true;
        if(instance==null)
        {
            instance = this;
        }else if(instance!=this)
        {
            Destroy(this);
        }
        SetupFSM();
        serverState.Move(GameAction.Prepare);
    }


    void SetupLogic()
    {

    }

   
    void SetupServer()
    {
        server = new Server();
    }

    

    public void AddClient(int localId, Client c)
    {
        PlayerManager.playerManager.Add(localId,"Test", true);
    }


    public void RemoveClient(int localid)
    {

        server.Disconnect(localid);
        PlayerManager.playerManager.Remove(localid);
        
    }


    public State GetState()
    {
        return serverState.state;
    }
    void SetupFSM()
    {
        serverState = new FSM<State, GameAction>();
        serverState.state = State.Setup;
        Action<GameAction> actSetup = x => {

            Debug.Log("Setting up");

            GameLogic.PrepareRound(this.transform);

            SetupServer();
            Debug.Log("Setup done");
            serverState.Move(GameAction.GoLive);

        };
        Action<GameAction> actLive = x => {
            Debug.Log("Opening Socket");
            server.Start();
        };


        Action<GameAction> actStartGame = x => {



            Debug.Log("Starting Round, no new connections");
            server.StopListen();
            // GameReadyPacket answerPacket = new GameReadyPacket(4,true);
            // Server.SendPacketToAll(answerPacket);
            GameLogic.current.StartRound();

        };
        Action<GameAction> actDropGame = x => {
            Debug.Log("Restarting");
            GameLogic.ClearRound();
            server.Start();

        };
        Action<GameAction> actCancel = x => {
            Debug.Log("Game canceled");

            LevelManager.Clear();

            GameLogic.EndRound();

            GameLogic.PrepareRound(this.transform);
            SpawnPlayersInLobby();
            server.Start();

        };
        Action<GameAction> actWin = x => {


            //WinPacket packet = new WinPacket();
            //Server.SendPacketToAll(packet);

            LevelManager.Clear();


            SpawnPlayersInLobby();
            server.Start();
            GameLogic.EndRound();
            GameLogic.PrepareRound(this.transform);

            foreach(Player p in PlayerManager.playerManager.players)
            {
                if(p!=null)
                {
                    //p.SendLevelList();
                }
            }

        };

        serverState.transitions.Add(new Tuple<State, GameAction>(State.Setup, GameAction.Prepare), new Tuple<Action<GameAction>, State>(actSetup, State.Prepare));

        serverState.transitions.Add(new Tuple<State,GameAction>(State.Prepare,GameAction.GoLive),new Tuple<Action<GameAction>,State>(actLive,State.Lobby));
        serverState.transitions.Add(new Tuple<State, GameAction>(State.Lobby, GameAction.StartGame), new Tuple<Action<GameAction>, State>(actStartGame, State.Play));
        serverState.transitions.Add(new Tuple<State, GameAction>(State.Play, GameAction.EndGame), new Tuple<Action<GameAction>, State>(actDropGame, State.Lobby));
        serverState.transitions.Add(new Tuple<State, GameAction>(State.Play, GameAction.WinGame), new Tuple<Action<GameAction>, State>(actWin, State.Lobby));
        serverState.transitions.Add(new Tuple<State, GameAction>(State.Play, GameAction.CancelGame), new Tuple<Action<GameAction>, State>(actCancel, State.Lobby));
    }

    void SpawnPlayersInLobby()
    {
        for (int i = 0; i < 4; i++)
            PlayerManager.playerManager.SpawnPlayer(i, new Vector3(i * 8, 0, 0));
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

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.L))
        {
            Debug.Log(server.ToString());
        }
    }

    void OnDisable()
    {
        Debug.Log("Server stopping");
        Stop();
    }
    

}
