using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheAnimalKingdom
{
    //보드의 타일 구성
    public struct Tile
    {
        //타일의 인덱스
        public int tile_num;

        //타일에 깔린 카드
        public int move_num;

        //byte 배열 크기 4
        public int[] players; //4bit 체계 계산해서

        public int this_num;

    }

    public struct Unit
    {
        //선택한 플레이어 인덱스
        public int player_index;

        //선택된 유닛 모델
        public int unit_num;

        //현재 위치한 타일 위치
        public int position;
    }

    public struct Item
    {
        //사용한 플레이어 인덱스
        public int player_index;

        //사용된 아이템 번호
        public int unit_num;

    }

    public struct UserInfo
    {
        public int player_index;
        public string nickName;
        public int charNum;
    }

}
