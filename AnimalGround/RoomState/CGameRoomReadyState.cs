using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheAnimalKingdom.RoomState
{
    using FreeNet;

    class CGameRoomReadyState : IState, IRoomState
    {
        CGameRoom room;

        public CGameRoomReadyState(CGameRoom room)
        {
            this.room = room;
            //this.room.state_manager.register_message_handler(this, PROTOCOL.ENTER_GAME_ROOM_REQ, this.on_ready_req);
        }


        void IState.on_enter()
        {
        }


        void IState.on_exit()
        {
        }
        
        void IRoomState.on_receive(PROTOCOL protocol, CPlayer owner, CPacket msg)
        {
            switch (protocol)
            {
                case PROTOCOL.LOADING_COMPLETED:
                    {
                        on_ready_req(owner, msg);
                    }
                    break;
                case PROTOCOL.INVITE_ROOM_UNREADY:
                    {
                        on_unready_req(owner, msg);
                    }
                    break;
            }
        }
        void on_unready_req(CPlayer sender, CPacket msg)
        {
            sender.removed(); //친구방 레디 취소
            this.room.removed_protocol(sender.player_index);
            this.room.checked_protocol(sender.player_index, PROTOCOL.INVITE_ROOM_UNREADY);
            CPacket ready = CPacket.create((short)PROTOCOL.INVITE_ROOM_READY); //준비한 유저 알려주기
            ready.push(this.room.get_player_count());//인원수
            this.room.get_players().ForEach(player =>
            {
                ready.push(player.player_name);
                ready.push(this.room.is_received(player.player_index, PROTOCOL.ENTER_GAME_ROOM_REQ3) ? 1 : 2);
            });
            this.room.broadcast(ready);
        }

        void on_ready_req(CPlayer sender, CPacket msg)
        {
            if (sender.player_name == msg.pop_string())
            {
                sender.player_character = msg.pop_int32();
            }
            
            if (!this.room.all_received(PROTOCOL.LOADING_COMPLETED))
            {
                return;
            }
            this.room.change_state(ROOM_STATE_TYPE.PLAY);

            //this.room.state_manager.change_state(CGameRoom.STATE.PLAY);
            game_start();
        }

        void game_start()
        {
            CPacket reply = CPacket.create((short)PROTOCOL.GAME_START);

            this.room.get_players().ForEach(player =>
            {
                reply.push((int)player.player_index);
                reply.push(player.player_name);
                reply.push(player.player_character);
                Console.WriteLine("UserINFO [" + player.player_index + "] / [" + player.player_name + "] / [" + player.player_character + "]");
            });

            this.room.broadcast(reply);
        }
    }
}
