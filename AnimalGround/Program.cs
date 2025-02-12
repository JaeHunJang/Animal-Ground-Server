﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FreeNet;

namespace TheAnimalKingdom
{
    class Program
    {
        static List<CGameUser> userlist;
        public static CGameServer game_main = new CGameServer();

        static void Main(string[] args)
        {
            userlist = new List<CGameUser>();

            CNetworkService service = new CNetworkService(true);
            // 콜백 매소드 설정.
            service.session_created_callback += on_session_created;
            // 초기화.
            service.initialize(10000, 1024);
            service.listen("0.0.0.0", 27015, 100);


            Console.WriteLine("Started!");
            while (true)
            {
                string input = Console.ReadLine();
                //Console.Write(".");
                System.Threading.Thread.Sleep(1000);
            }

            Console.ReadKey();
        }


        static void on_session_created(CUserToken token)
        {
            CGameUser user = new CGameUser(token);
            lock (userlist)
            {
                userlist.Add(user);
            }
        }


        public static void remove_user(CGameUser user)
        {
            lock (userlist)
            {
                userlist.Remove(user);
                game_main.user_disconnected(user);
            }
        }


        public static int get_concurrent_user_count()
        {
            return userlist.Count;
        }
/*
        public static CGameUser find_user(string player_name)
        {
            lock (userlist)
            {
                for(int i = 0; i < get_concurrent_user_count(); i++)
                {
                    return (userlist[i].player.player_name == player_name) ? userlist[i] : null; 
                }
                return null;
            }
        }*/
        public static CGameUser find_user(string player_name)
        {
            List<CGameUser> copy;
            copy = userlist;
            for (int i = 0; i < get_concurrent_user_count(); i++)
            {
                Console.WriteLine("search : "+copy[i].player.player_name);
                if (copy[i].player.player_name.Equals(player_name))
                {
                    return copy[i];
                }
            }

            return null;
        }
    }
}
