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

    enum Command { Teleport };

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

            }

            if(com == Command.Teleport)
            {
                Vector3 position = new Vector3(float.Parse(parts[2]), float.Parse(parts[3]), float.Parse(parts[4]));
                PlayerManager.playerManager.GetPlayer(Int32.Parse(parts[1])).GetComponent<Player>().Teleport(position);
            }

            commandLine.text = "";
        }
    }
}
