using System;

namespace TheAnimalKingdom
{
    /// <summary>
    /// 프로토콜 정의.
    /// 서버에서 클라이언트로 가는 패킷 : S -> C
    /// 클라이언트에서 서버로 가는 패킷 : C -> S
    /// </summary>
    public enum PROTOCOL : short
    {
        ////-------------------------------------
        //// 0 이하는 종료코드로 사용되므로 게임에서 쓰지 말것!!
        ////-------------------------------------
        BEGIN,


        ////-------------------------------------
        //// 로비 프로토콜.
        ////-------------------------------------
        //// C -> S 게임방(6*4) 입장 요청.
        SERVER_CONNECT,
                
        //// C -> S 친구 초대방 생성 요청.
        INVITE_ROOM_CREATE,  

        //// C -> S 친구 초대 요청.
        INVITE_FRIEND_REQ,  
        
        //// S -> C 친구 초대 요청(친구찾아서 요청 보냄).
        INVITE_FRIEND,

        //// C -> S 친구 초대 수락.
        INVITE_FRIEND_ACK,

        //// S -> C 친구 게임 방에 입장.
        INVITE_ROOM,

        //// C -> S 친구 초대 취소, 실패. 랜덤매칭중일때 거절 0, 그냥거절시 1
        DENY_FRIEND,

        //// C -> S 친구 초대 거절 0, 검색실패 1, 이미초대2, 최대인원3, 이미 게임중 4, 방폭파 5
        DENY_FRIEND_ACK,

        //// S -> C 친구 방 준비 상태 확인
        INVITE_ROOM_READY,
        
        //// C -> S 게임방 레디 취소
        INVITE_ROOM_UNREADY,

        //// C -> S 게임방(6*4) 입장 요청.
        ENTER_GAME_ROOM_REQ, 
        
        //// C -> S 게임방(4*4) 입장 요청.
        ENTER_GAME_ROOM_REQ2,

        //// C -> S 게임방(친구) 입장 요청.
        ENTER_GAME_ROOM_REQ3,

        //// C -> S 게임 매칭 취소
        ENTER_GAME_ROOM_QUIT,
        
        //// C -> S 게임 매칭 취소(친구용)
        INVITE_ROOM_QUIT_REQ,

        //// S -> C 게임 매칭 취소(친구용)
        INVITE_ROOM_QUIT_ACK,

        //// S -> C 매칭이 성공했다. 방에 입장하고 로딩을 시작해라.
        START_LOADING,

        //// C -> S 로딩이 끝났다.
        LOADING_COMPLETED,





        ////-------------------------------------
        //// 게임 프로토콜.
        ////-------------------------------------
        //// S -> C 게임을 시작해도 좋다.
        GAME_START,

        //// C -> S 게임 초기화.
        GAME_INIT,
        
        //// S -> C 게임 초기화.
        GAME_INITED,

        //// S -> C 말이 이동함.
        UNIT_MOVE,

        //// C -> S 말이 이동됨.
        UNIT_MOVED,

        //// C -> S 클라가 타일에 카드 배치 함.
        CARD_SET_REQ,

        //// S -> C 서버에서 타일에 배치된 카드 병합시킴.
        CARD_SET,

        //// C -> S 병합된 타일 정보 적용됨.
        CARD_SETTED,

        //// C -> S 클라가 아이템 사용 요청함.
        ITEM_USE_REQ,

        //// S -> C 서버에서 각 유저가 사용한 아이템정보 전달시킴.
        ITEM_USE,

        //// C -> S 클라에서 각 플레이어의 아이템 효과 적용함.
        ITEM_USED,

        //// C -> S 게임 종료.
        GAME_OVER,

        ////  ->   방이 삭제됨.
        ROOM_REMOVED,

        END
    }
}
