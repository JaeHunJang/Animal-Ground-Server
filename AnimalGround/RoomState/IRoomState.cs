using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheAnimalKingdom.RoomState
{
    using FreeNet;
    public enum ROOM_STATE_TYPE
    {
        READY,
        PLAY
    }
    public interface IRoomState
    {
        void on_receive(PROTOCOL protocol, CPlayer owner, CPacket msg);
    }
}
