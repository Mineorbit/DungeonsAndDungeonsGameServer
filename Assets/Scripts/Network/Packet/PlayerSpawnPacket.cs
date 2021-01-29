﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Reflection;

public class PlayerSpawnPacket : Packet
{
    public PlayerSpawnPacket()
    {
        packetId = 8;
        types = new Type[9];
        types[0] = typeof(int);
        types[1] = typeof(float);
        types[2] = typeof(float);
        types[3] = typeof(float);
        types[4] = typeof(bool);

        types[5] = typeof(float);
        types[6] = typeof(float);
        types[7] = typeof(float);
        types[8] = typeof(float);
        content = new object[9];

    }
    public PlayerSpawnPacket(int localId, Vector3 position,Quaternion rotation,bool doInput)
    {
        packetId = 8;
        types = new Type[9];
        types[0] = typeof(int);
        types[1] = typeof(float);
        types[2] = typeof(float);
        types[3] = typeof(float);
        types[4] = typeof(bool);

        types[5] = typeof(float);
        types[6] = typeof(float);
        types[7] = typeof(float);
        types[8] = typeof(float);
        content = new object[9];
        content[0] = localId;
        content[1] = position.x;
        content[2] = position.y;
        content[3] = position.z;
        content[4] = doInput;
        content[5] = rotation.x;
        content[6] = rotation.y;
        content[7] = rotation.z;
        content[8] = rotation.w;
    }
    public void OnReceive()
    {

    }
}
