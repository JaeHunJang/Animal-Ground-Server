using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheAnimalKingdom.RoomState
{
    using FreeNet;

    class CGameRoomPlayState :IRoomState
    {

        const byte MAX_DISTANCE = 3;
        const byte MIN_DISTANCE = 1;

        const short MAX_TILE_SET = 2;
        const short MIN_TILE_SET = -2;

        readonly int[] CARDSET = { -2, -1, 0, 1, 2 };
        readonly int[] REWARD = { 100, 80, 60, 40 };

        CGameRoom room;
       
        //유닛이 정상적으로 움직였는지 검증하기 위한 배열
        int[] unit_move;

        //게임 보드판
        Tile[] gameboard;

        //Item items;
        List<Item> items;

        int max_tile;
        int move_round;
        int player_count;

        public CGameRoomPlayState(CGameRoom room)
        {
            this.room = room;
            
        }
        
        
        /// <summary>
        /// 각 유저가 전달한 프로토콜 로직 처리
        /// </summary>
        /// <param name="protocol"></param>
        /// <param name="owner"></param>
        /// <param name="msg"></param>
        void IRoomState.on_receive(PROTOCOL protocol, CPlayer owner, CPacket msg)
        {
            Console.WriteLine("recieve protocol : " + protocol);
            switch (protocol)
            {
                case PROTOCOL.CARD_SET_REQ:
                    {
                        set_card(owner, msg);
                    }
                    break;
                case PROTOCOL.CARD_SETTED:
                    {
                        setted_card(owner, msg);
                    }
                    break;
                case PROTOCOL.ITEM_USE_REQ:
                    {
                        use_item(owner, msg);
                    }
                    break;
                case PROTOCOL.ITEM_USED:
                    {
                        used_item(owner, msg);
                    }
                    break;
                case PROTOCOL.UNIT_MOVED:
                    {
                        moved_unit(owner, msg);
                    }
                    break;
                case PROTOCOL.GAME_INIT:
                    {
                        init_game(owner, msg);
                    }
                    break;
                case PROTOCOL.GAME_OVER:
                    {
                        game_over(owner, msg);
                    }
                    break;

            }
        }

        void init_game(CPlayer player, CPacket msg)
        {
            if (this.room.all_received(PROTOCOL.GAME_INIT))
            {
                this.max_tile = msg.pop_int32();
                this.move_round = msg.pop_int32();
                //초기화
                this.gameboard = new Tile[max_tile];
                player_count = room.get_player_count();
                this.unit_move = new int[player_count * move_round];
                //items = new Item[player_count];
                items = new List<Item>();
                for (int i = 0; i < unit_move.Length; i++)
                {
                    unit_move[i] = 0;
                }
                for (byte i = 0; i < max_tile; i++)
                {
                    gameboard[i].move_num = int.MaxValue;
                    gameboard[i].tile_num = i;
                    gameboard[i].players = new int[room.get_player_count()];
                    gameboard[i].this_num = int.MaxValue;
                }
                /*for (byte i = 0; i < this.room.get_player_count(); i++) //초기화
                {
                    Item init = new Item();
                    init.player_index = int.MinValue;
                    init.unit_num = int.MinValue;
                    items[i] = init;
                }*/

                CPacket reply = CPacket.create((short)PROTOCOL.GAME_INITED);
                this.room.broadcast(reply);
            }
        }

        /*
        /// <summary>
        /// 게임을 시작(게임 씬 로딩) 후 말을 선택한다.
        /// </summary>
        /// <param name="player"></param>
        public void select_unit(CPlayer player, CPacket msg)
        {
            //모든 유저가 로딩이 되었다면 말을 선택하라고 패킷을 보낸다.
            if (!this.room.all_received(PROTOCOL.UNIT_SELECT_REQ))
            {
                return;
            }
            CPacket reply = CPacket.create((short)PROTOCOL.UNIT_SELECT);
            //랜덤 유닛 모델 4개를 전송한다.
            shuffle_unit();
            for (int i = 0; i < players_unit.Length; i++)
            {
                reply.push(players_unit[i].unit_num);
                Console.Write("Unit Select : " + players_unit[i].unit_num + " / ");
            }
            Console.WriteLine();
            this.room.broadcast(reply);
        }*/
        /*
        /// <summary>
        /// 유닛을 선택한 후 상태 변경.
        /// </summary>
        /// <param name="player"></param>
        public void selected_unit(CPlayer player, CPacket msg)
        {

            //유닛 선택이 완료된 플레이어는 유닛을 선택된 스테이트로 변경해준다.
            //change_playerstate(player, PLAYER_STATE.UNIT_SELECTED);

            if (this.room.all_received(PROTOCOL.UNIT_SELECTED))
            {
                //캐릭터(유닛) 선택후 이동 명령 전송
                //move_unit();
            }
        }
        */
        /// <summary>
        /// 유닛을 랜덤 이동 완료시 상태 변경.
        /// </summary>
        /// <param name="player"></param>
        void moved_unit(CPlayer player, CPacket msg)
        {

            //이동 비교

            //유닛 이동이 클라이언트에서 완료된 플레이어의 스테이트를 변경해준다.
            if (this.room.all_received(PROTOCOL.UNIT_MOVED))
            {
                CPacket reply = CPacket.create((short)PROTOCOL.UNIT_MOVED);
                //각 클라의 유닛 이동이 정상적으로 실행되었음을 확인차 전송 (데이터 x)
                this.room.broadcast(reply);
            }


        }

        /// <summary>
        /// 카드 설정
        /// </summary>
        /// <param name="player"></param>
        /// <param name="msg"></param>
        void set_card(CPlayer player, CPacket msg)
        {

            int player_num = msg.pop_int32();
            change_gameboard(player_num, msg.pop_int32(), msg.pop_int32());
            change_gameboard(player_num, msg.pop_int32(), msg.pop_int32());


            if (this.room.all_received(PROTOCOL.CARD_SET_REQ))
            {
                //8개 다 받고 한번에 전송
                for (int i = 0; i < gameboard.Length; i++)
                {
                    gameboard[i].this_num = int.MaxValue;
                }
                setting_card();
            }


        }

        /// <summary>
        /// 캐릭터 이동.
        /// </summary>
        /// <param name="player"></param>
        void setted_card(CPlayer player, CPacket msg)
        {
            if (this.room.all_received(PROTOCOL.CARD_SETTED))
            {
                //이동 패킷 전송
                //move_unit();
            }
        }

        /// <summary>
        /// 아이템 사용.
        /// </summary>
        /// <param name="player"></param>
        void use_item(CPlayer player, CPacket msg)
        {
            Item item = new Item();
            item.player_index = msg.pop_int32();
            item.unit_num = msg.pop_int32();

            items.Add(item);
            //items[player.player_index] = item;

            if (this.room.all_received(PROTOCOL.ITEM_USE_REQ))
            {
                
                //응답 패킷 전송
                CPacket reply = CPacket.create((short)PROTOCOL.ITEM_USE);
                
                //아이템 사용 데이터(4개)를 push
                for (byte i = 0; i < this.room.get_player_count(); i++)
                {
                    if (items[i].player_index.Equals(int.MinValue+1))
                    {
                        reply.push(int.MinValue);
                        reply.push(int.MinValue);
                    }
                    //유닛이 정상적으로 움직였는지 검증하기 위해 값 저장
                    reply.push(items[i].player_index);
                    reply.push(items[i].unit_num);
                    Console.WriteLine("Item Use [" + items[i].player_index + "] : " + items[i].unit_num + " / ");
                }
                this.room.broadcast(reply);
                /*for(byte i = 0; i < items.Length; i++) //초기화
                {
                    Item init = new Item();
                    init.player_index = int.MinValue;
                    init.unit_num = int.MinValue;
                    items[i] = init;
                }*/
                items.Clear();
                Console.WriteLine("item Use broad");
            }

        }

        /// <summary>
        /// 아이템 사용 완료시 상태 변경.
        /// </summary>
        /// <param name="player"></param>
        void used_item(CPlayer player, CPacket msg)
        {
            if (this.room.all_received(PROTOCOL.ITEM_USED))
            {
                //이동 패킷 전송
                move_unit();
            }
        }

        /// <summary>
        /// 게임 끝
        /// </summary>
        /// <param name="player"></param>
        /// <param name="msg"></param>
        void game_over(CPlayer player, CPacket msg)
        {
            if (this.room.all_received(PROTOCOL.GAME_OVER))
            {
                this.room.get_players().ForEach(current_player => //현재 게임에 있는 플레이어만 결과 처리
                {
                    int player_index = msg.pop_int32();
                    int reward_index = msg.pop_int32();
                    for (byte i = 0; i < player_count; i++) //플레이어 인덱스로 찾기
                    {
                        if ((int)current_player.player_index == player_index)
                        {
                            int result = MySqlConnector.Instance.doNonQuery(
                                "update account2 set " +
                                "account_money_now = account_money_now + " + REWARD[reward_index] +
                                ", account_exp = account_exp  + " + REWARD[reward_index] +
                                " where account_nickname = '" + current_player.player_name + "'");
                            break;
                        }
                    }

                });
                this.room.remove_self();
                //this.room.destroy(1);
            }
            /*
            // 우승자 가리기.
            byte win_player_index = byte.MaxValue;


            CPacket reply = CPacket.create((short)PROTOCOL.GAME_OVER);
            reply.push(win_player_index);
            this.room.broadcast(reply);
            */
            //방 제거.
            //Program.game_main.room_manager.remove_room(this);
        }


        /// <summary>
        /// 유닛의 랜덤 모델 15가지중 4가지를 랜덤선택해 전송해준다.
        /// </summary>
        /// <param name="msg"></param>
       /* void shuffle_unit()
        {
            //유닛 모델 15가지를 랜덤하게 섞는다
            int[] list = random_int(MIN_UNIT_MODEL, MAX_UNIT_MODEL + 1);

            //섞인 모델 리스트 중 플레이어 숫자만큼 사용한다.
            for (int i = 0; i < players_unit.Length; i++)
            {
                players_unit[i].player_index = i;
                players_unit[i].unit_num = list[i];
            }
        }*/

        int[] random_int(int min, int max)
        {
            Random rand = new Random((int)DateTime.Now.Ticks);
            int[] list = Enumerable.Range(min, max).ToArray();
            int idx, old;
            for (int i = 0; i < max; i++)
            {
                idx = rand.Next(max);
                old = list[i];
                list[i] = list[idx];
                list[idx] = old;
            }
            return list;
        }

        void move_unit()
        {
            CPacket reply = CPacket.create((short)PROTOCOL.UNIT_MOVE);
            Random rand = new Random((int)DateTime.Now.Ticks);
            //캐릭터(유닛)의 개수(4개)를 이동시킬 만큼 push
            for (byte i = 0; i < unit_move.Length; i++)
            {
                //유닛이 정상적으로 움직였는지 검증하기 위해 값 저장
                unit_move[i] = rand.Next(MIN_DISTANCE, MAX_DISTANCE + 1);
                Console.WriteLine("Unit Move[" + i + "] : " + unit_move[i] + " / ");
                reply.push(unit_move[i]);
            }
            this.room.broadcast(reply);
            Console.WriteLine("unit Move broad");
        }

        void move_unit_check(int player_index, int a, int b, int c)
        {
            //players_unit
        }

        void change_gameboard(int player, int tile, int card)
        {
            if (card < 0)
            {
                return;
            }
            switch (player)
            {
                case 1:
                    this.gameboard[tile].players[3] = player;
                    break;
                case 2:
                    this.gameboard[tile].players[2] = player;
                    break;
                case 4:
                    this.gameboard[tile].players[1] = player;
                    break;
                case 8:
                    this.gameboard[tile].players[0] = player;
                    break;

            }
            if (gameboard[tile].this_num == 0)
            {
                return;
            }
            
            switch (card)
            {
                case 0:// -2
                case 1:// -1
                    this.gameboard[tile].move_num =
                        (CARDSET[card] + this.gameboard[tile].move_num) < MIN_TILE_SET ?
                        MIN_TILE_SET : CARDSET[card] + (this.gameboard[tile].move_num == int.MaxValue ? 0 : this.gameboard[tile].move_num);
                    break;

                case 2:// 0
                    this.gameboard[tile].move_num = CARDSET[card];
                    break;

                case 3:// 1
                case 4:// 2
                    this.gameboard[tile].move_num =
                        (CARDSET[card] + this.gameboard[tile].move_num) > MAX_TILE_SET ?
                        MAX_TILE_SET : CARDSET[card] + (this.gameboard[tile].move_num == int.MaxValue ? 0 : this.gameboard[tile].move_num);
                    break;
            }
            this.gameboard[tile].this_num = CARDSET[card];
        }

        void setting_card()
        {
            CPacket reply = CPacket.create((short)PROTOCOL.CARD_SET);
            foreach (Tile tile in gameboard)
            {
                reply.push(tile.players.Sum());
                reply.push(tile.tile_num);
                switch (tile.move_num + 2)
                {
                    case 0: reply.push(0); break;
                    case 1: reply.push(1); break;
                    case 2: reply.push(2); break;
                    case 3: reply.push(3); break;
                    case 4: reply.push(4); break;
                    default: reply.push(int.MaxValue); break;
                }

                Console.WriteLine("Gameboard : " + tile.players.Sum() + " / " + tile.tile_num + " / " + tile.move_num);

            }
            this.room.broadcast(reply);
            Console.WriteLine("card set broad");

        }

    }
}
