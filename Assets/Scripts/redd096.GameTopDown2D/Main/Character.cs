using UnityEngine;

namespace redd096.GameTopDown2D
{
    [AddComponentMenu("redd096/.GameTopDown2D/Main/Character")]
    public class Character : Redd096Main
    {
        public enum ECharacterType { Player, AI };
        public ECharacterType CharacterType = ECharacterType.AI;
    }
}