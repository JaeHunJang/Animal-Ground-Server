using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FreeNet;

namespace TheAnimalKingdom.UserState
{
    class CUserLobbyState : IUserState
    {
        CGameUser owner;
        public CUserLobbyState(CGameUser owner)
        {
            this.owner = owner;
            
        }

        void IUserState.on_message(FreeNet.CPacket msg)
        {
            PROTOCOL protocol = (PROTOCOL)msg.pop_protocol_id();
            switch (protocol)
            {
                case PROTOCOL.ENTER_GAME_ROOM_REQ:
                    Program.game_main.matching_req(this.owner);
                    break;
                case PROTOCOL.ENTER_GAME_ROOM_REQ2:
                    Program.game_main.matching_req2(this.owner);
                    break;
                case PROTOCOL.ENTER_GAME_ROOM_REQ3:
                    invite_room_start(msg);
                    break;
                case PROTOCOL.INVITE_ROOM_CREATE: //친구초대방 생성
                    Program.game_main.matching_req3(this.owner);
                    this.owner.battle_room.board = msg.pop_int32();
                    this.owner.battle_room.invite_room_check.Add(this.owner.player);
                    break;
                case PROTOCOL.ENTER_GAME_ROOM_QUIT:
                    this.owner.player.removed();
                    break;
                case PROTOCOL.SERVER_CONNECT:
                    this.owner.enter_lobby(msg.pop_string());
                    break;
                case PROTOCOL.INVITE_FRIEND_REQ:
                    invite_friend(msg);
                    break;
                case PROTOCOL.INVITE_FRIEND_ACK:
                    invite_room();
                    break;
                case PROTOCOL.DENY_FRIEND:
                    deny_friend(msg);
                    break;
                case PROTOCOL.ROOM_REMOVED:
                    this.owner.quit_room(this.owner.player.player_index);
                    break;
                case PROTOCOL.INVITE_ROOM_QUIT_REQ:
                    invite_room_quit();
                    break;
            }
        }
        /// <summary>
        /// 초대한 방 나가기
        /// </summary>
        /// <param name="msg"></param>
        void invite_room_quit()
        {
            this.owner.battle_room.remove_player(this.owner.player); //거절을 보낸 자신을 삭제함으로서 초대 취소
            this.owner.battle_room.clear_received_protocol();
            this.owner.battle_room.invite_room_check.Remove(this.owner.player);
            invite_room_ready();
            CPacket reply = CPacket.create((short)PROTOCOL.INVITE_ROOM_QUIT_ACK);
            reply.push(this.owner.player.player_name);
            this.owner.battle_room.broadcast(reply);
            this.owner.quit_room(this.owner.player.player_index); //방뺌
        }

        /// <summary>
        /// 방에 있는 인원 ready 상태 체크
        /// </summary>
        /// <param name="protocol"></param>
        void invite_room_ready()
        {
            CPacket ready = CPacket.create((short)PROTOCOL.INVITE_ROOM_READY); //준비한 유저 알려주기
           
            ready.push(this.owner.battle_room.get_player_count());//인원수
            this.owner.battle_room.invite_room_check.ForEach(player =>
            {
                ready.push(player.player_name);
                ready.push(this.owner.battle_room.is_received(player.player_index, PROTOCOL.ENTER_GAME_ROOM_REQ3) ? 1 : 2);
            });
            this.owner.battle_room.broadcast(ready);
        }

        /// <summary>
        /// 게임 ready 상태 변경
        /// </summary>
        /// <param name="msg"></param>
        void invite_room_start(CPacket msg)
        {
            Console.WriteLine("aaaaa"+this.owner.player.player_index);
            this.owner.battle_room.removed_protocol(this.owner.player.player_index);
            this.owner.battle_room.checked_protocol(this.owner.player.player_index, PROTOCOL.ENTER_GAME_ROOM_REQ3);
            this.owner.change_state(USER_STATE_TYPE.PLAY);//유저 타입 변경
            invite_room_ready();
            if (this.owner.battle_room.get_player_count() >= 4)
            {
                if (this.owner.battle_room.all_received(PROTOCOL.ENTER_GAME_ROOM_REQ3))
                {
                    this.owner.battle_room.invite_room_check.Clear();
                    this.owner.battle_room.get_players().ForEach(player =>
                    {
                        CPacket reply = CPacket.create((short)PROTOCOL.START_LOADING); //게임 시작 메시지 보내기
                        Console.WriteLine(player.player_index + player.player_name);
                    reply.push((int)player.player_index);  // 본인의 플레이어 인덱스를 알려준다.
                    player.send(reply);
                    });
                }
            }
        }

        /// <summary>
        /// 초대 수락시 방에 입장 시키기
        /// </summary>
        /// <param name="msg"></param>
        void invite_room()
        {
            CPacket reply = CPacket.create((short)PROTOCOL.INVITE_ROOM);
            reply.push(this.owner.battle_room.board);
            for (int i = 0; i < this.owner.battle_room.get_player_count(); i++) //현재 있는놈들 인덱스 설정
            {
                this.owner.battle_room.get_players()[i].player_index = (byte)i;
            }
            this.owner.battle_room.invite_room_check.Add(this.owner.player); //나 방에 들어왔으요 체크
            
            reply.push(this.owner.battle_room.invite_room_check.Count); //방에 들어온 사람 수
            this.owner.battle_room.invite_room_check.ForEach(player =>
            {
                reply.push(player.player_index);
                reply.push(player.player_name);
                player.send(reply);
            });
            /*for (byte i = 0; i < this.owner.battle_room.invite_room_check.Count; i++) //방에 일단 초대된 모든 얘들중에
            {
                    reply.push(this.owner.battle_room.get_players()[i].player_index); //인덱스랑
            }
            this.owner.battle_room.broadcast(reply);*/
            this.owner.battle_room.clear_received_protocol();
            invite_room_ready();
        }

        /// <summary>
        /// 친구 초대 요청하기
        /// </summary>
        /// <param name="msg"></param>
        void invite_friend(CPacket msg)
        {
            string name = msg.pop_string();
            for (int i = 0; i < this.owner.battle_room.get_player_count(); i++)
            {
                if (this.owner.battle_room.get_players()[i].player_name.Equals(name))
                {
                    CPacket reply = CPacket.create((short)PROTOCOL.DENY_FRIEND_ACK);
                    reply.push(2); //이미 초대
                    this.owner.send(reply); //초대한 사람에게 거절 메시지 보냄
                    return;
                }
            }
            
            CGameUser invite = Program.find_user(name);

            if (invite != null)
            {
                if(invite.battle_room != null)
                {
                    CPacket reply = CPacket.create((short)PROTOCOL.DENY_FRIEND_ACK);
                    reply.push(4); //이미 게임중
                    this.owner.send(reply); //초대한 사람에게 거절 메시지 보냄
                }
                else if (this.owner.battle_room.get_player_count() < 4)
                {
                    this.owner.battle_room.add_player(invite.player); //방에 초대
                    invite.enter_room(this.owner.battle_room); //방 정보를 줌
                    CPacket reply = CPacket.create((short)PROTOCOL.INVITE_FRIEND);
                    reply.push(this.owner.player.player_name);
                    invite.send(reply);
                }
                else
                {
                    CPacket deny3 = CPacket.create((short)PROTOCOL.DENY_FRIEND_ACK);
                    deny3.push(3); //최대 인원
                    this.owner.send(deny3);
                }
            }
            else
            {
                CPacket deny1 = CPacket.create((short)PROTOCOL.DENY_FRIEND_ACK);
                deny1.push(1); //검색 실패
                this.owner.send(deny1);
            }

        }

        /// <summary>
        /// 초대 거절하기
        /// </summary>
        /// <param name="msg"></param>
        void deny_friend(CPacket msg)
        {
            string inviter_name = msg.pop_string();
            int flag = msg.pop_int32();
            CPacket deny0 = CPacket.create((short)PROTOCOL.DENY_FRIEND_ACK);
            switch (flag)
            {
                case 0:
                    deny0.push(0);//거절
                    break;
                case 1:
                    deny0.push(4);//게임중(랜덤매칭중 초대걸렷을때)
                    break;
            }

            this.owner.battle_room.get_players().ForEach(player =>
            {
                if (player.player_name.Equals(inviter_name))
                {
                    player.send(deny0); //초대한 사람에게 거절 메시지 보냄
                }
            });
            this.owner.battle_room.remove_player(this.owner.player); //거절을 보낸 자신을 삭제함으로서 초대 취소
            this.owner.quit_room(this.owner.player.player_index); //방뺌
        }
    }
}
