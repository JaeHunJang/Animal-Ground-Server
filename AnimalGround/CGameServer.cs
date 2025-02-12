﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheAnimalKingdom
{
    using FreeNet;
    using System.Threading;

    class CGameServer
    {
        // 게임방을 관리하는 매니저.
        public CGameRoomManager room_manager { get; private set; }
        public CGameRoomManager room_manager2 { get; private set; }
        public CGameRoomManager room_manager3 { get; private set; }

        // 매칭 대기 리스트.
        List<CGameUser> matching_waiting_users;
        List<CGameUser> matching_waiting_users2;
        //----------------------------------------------------------------

        public CGameServer()
        {
            this.room_manager = new CGameRoomManager();
            this.room_manager2 = new CGameRoomManager();
            this.room_manager3 = new CGameRoomManager();
            this.matching_waiting_users = new List<CGameUser>();
            this.matching_waiting_users2 = new List<CGameUser>();
        }


        /// <summary>
        /// 유저로부터 매칭 요청이 왔을 때 호출됨.
        /// </summary>
        /// <param name="user">매칭을 신청한 유저 객체</param>
        public void matching_req(CGameUser user)
        {
            // 대기 리스트에 중복 추가 되지 않도록 체크.
            if (this.matching_waiting_users.Contains(user))
            {
                return;
            }

            // 매칭 대기 리스트에 추가.
            this.matching_waiting_users.Add(user);

            // 4명이 모이면 매칭 성공.
            if (this.matching_waiting_users.Count == 4)
            {
                // 게임 방 생성.
                this.room_manager.create_room(this.matching_waiting_users[0], this.matching_waiting_users[1], this.matching_waiting_users[2], this.matching_waiting_users[3]);

                // 매칭 대기 리스트 삭제.
                this.matching_waiting_users.Clear();
            }
            else
            {
                // 매칭 인원이 모자를 경우 대기 메시지 전송.
                //CPacket msg = CPacket.create((short)PROTOCOL.ENTER_GAME_ROOM_ACK);
                //user.send(msg);
            }
        }

        /// <summary>
        /// 유저로부터 매칭 요청이 왔을 때 호출됨.
        /// </summary>
        /// <param name="user">매칭을 신청한 유저 객체</param>
        public void matching_req2(CGameUser user)
        {
            // 대기 리스트에 중복 추가 되지 않도록 체크.
            if (this.matching_waiting_users2.Contains(user))
            {
                return;
            }

            // 매칭 대기 리스트에 추가.
            this.matching_waiting_users2.Add(user);

            // 4명이 모이면 매칭 성공.
            if (this.matching_waiting_users2.Count == 4)
            {
                // 게임 방 생성.
                this.room_manager2.create_room(this.matching_waiting_users2[0], this.matching_waiting_users2[1], this.matching_waiting_users2[2], this.matching_waiting_users2[3]);

                // 매칭 대기 리스트 삭제.
                this.matching_waiting_users2.Clear();
            }
            else
            {
                // 매칭 인원이 모자를 경우 대기 메시지 전송.
                //CPacket msg = CPacket.create((short)PROTOCOL.ENTER_GAME_ROOM_ACK);
                //user.send(msg);
            }
        }
       
        /// <summary>
        /// 유저로부터 매칭 요청이 왔을 때 호출됨. (친구 초대용)
        /// </summary>
        /// <param name="user">매칭을 신청한 유저 객체</param>
        public void matching_req3(CGameUser user)
        {
                // 게임 방 생성.
                this.room_manager3.create_room(user);
        }


        /// <summary>
        /// 유저가 끊겼을 경우 매칭 대기 리스트에서 제거.
        /// </summary>
        /// <param name="user"></param>
		public void user_disconnected(CGameUser user)
        {
            if (this.matching_waiting_users.Contains(user))
            {
                this.matching_waiting_users.Remove(user);
            }
            if (this.matching_waiting_users2.Contains(user))
            {
                this.matching_waiting_users2.Remove(user);
            }
        }
    }
}
