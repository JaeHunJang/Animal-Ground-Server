using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheAnimalKingdom
{
    /// <summary>
    /// 게임방들을 관리하는 룸매니저.
    /// </summary>
    public class CGameRoomManager
    {
        List<CGameRoom> rooms;

        public CGameRoomManager()
        {
            this.rooms = new List<CGameRoom>();
        }


        /// <summary>
        /// 매칭을 요청한 유저들을 넘겨 받아 게임 방을 생성한다.
        /// </summary>
        /// <param name="user1"></param>
        /// <param name="user2"></param>
        public void create_room(CGameUser user1, CGameUser user2, CGameUser user3, CGameUser user4)
        {
            // 게임 방을 생성하여 입장 시킴.
            CGameRoom battleroom = new CGameRoom(this);
            this.rooms.Add(battleroom);

            user1.enter_room(battleroom, 0);
            user2.enter_room(battleroom, 1);
            user3.enter_room(battleroom, 2);
            user4.enter_room(battleroom, 3);
            battleroom.enter_gameroom(user1.player, user2.player, user3.player, user4.player);
        }

        /// <summary>
        /// 친구 초대 매칭을 요청한 유저들을 넘겨 받아 게임 방을 생성한다.
        /// </summary>
        /// <param name="user1"></param>
        /// <param name="user2"></param>
        public void create_room(CGameUser user) //최초에 초대를 시작한 유저
        {
            // 게임 방을 생성하여 입장 시킴.
            CGameRoom battleroom = new CGameRoom(this);
            this.rooms.Add(battleroom);
            battleroom.add_player(user.player);
            user.enter_room(battleroom);
        }
        public void remove_room(CGameRoom room)
        {
            room.destroy();
            this.rooms.Remove(room);
        }
    }
}
