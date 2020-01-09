using System;
using System.Collections;
using System.Collections.Generic;
using FreeNet;

public enum PLAYER_TYPE : byte
{
    HUMAN,
    AI
}

namespace TheAnimalKingdom
{
    public class CPlayer
    {
        public delegate void SendFn(CPacket msg);

        IPeer owner;

        public byte player_index { get; set; }
        public string player_name { get; set; }
        public int player_character { get; set; }

        public CPlayer(CGameUser user, byte player_index)
        {
            this.owner = user;
            this.player_index = player_index;
        }
        
        public CPlayer(CGameUser user, string player_name)
        {
            this.owner = user;
            this.player_name = player_name;
        }

        public void send(CPacket msg)
        {
            this.owner.send(msg);
        }
        
        public void removed()
        {
            ((CGameUser)this.owner).change_state(UserState.USER_STATE_TYPE.LOBBY);
        }

        public void disconnect()
        {
            this.owner.disconnect();
        }
        
    }
}
