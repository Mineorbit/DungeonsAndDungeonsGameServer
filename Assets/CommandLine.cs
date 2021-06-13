using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using com.mineorbit.dungeonsanddungeonscommon;

public class CommandLine : MonoBehaviour
{
    public TMPro.TMP_InputField commandLine;
    void Start()
    {
        
    }

    enum Command { Teleport , Spawn, Start, List, Get};

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Return))
        {
            string command = commandLine.text;


            string[] parts = command.Split(' ');
            Command com = Command.Teleport;
            switch(parts[0])
            {
                case "tel":
                    com = Command.Teleport;
                    break;
                case "tele":
                    com = Command.Teleport;
                    break;
                case "teleport":
                    com = Command.Teleport;
                    break;
                case "tp":
                    com = Command.Teleport;
                    break;
                case "tport":
                    com = Command.Teleport;
                    break;
                case "spawn":
                    com = Command.Spawn;
                    break;
                case "start":
                    com = Command.Start;
                    break;    
                case "list":
                    com = Command.List;
                    break;
                case "get":
                    com = Command.Get;
                    break;

            }

            if(com == Command.Teleport)
            {
                Vector3 position = new Vector3(float.Parse(parts[2]), float.Parse(parts[3]), float.Parse(parts[4]));
                PlayerManager.playerManager.GetPlayer(Int32.Parse(parts[1])).GetComponent<Player>().Teleport(position);
            }
            if(com == Command.Spawn)
            {
                Vector3 position = new Vector3(float.Parse(parts[2]), float.Parse(parts[3]), float.Parse(parts[4]));
                PlayerManager.playerManager.GetPlayer(Int32.Parse(parts[1])).GetComponent<Player>().Spawn(position,new Quaternion(0,0,0,0),true);
            }
            if (com == Command.List)
            { 
                if (LevelManager.currentLevelMetaData != null)
                Debug.Log(LevelManager.currentLevelMetaData);
                foreach (Player p in PlayerManager.playerManager.players)
                {
                    Debug.Log("Player "+p.localId+": "+p.name);
                }
            }

            if (com == Command.Get)
            {
                Debug.Log("Current InstantiateType: "+Level.instantiateType);
            }

            if (com == Command.Start)
            {
                ServerManager.instance.performAction(ServerManager.GameAction.PrepareGame);
            }

            commandLine.text = "";
        }
    }
}
