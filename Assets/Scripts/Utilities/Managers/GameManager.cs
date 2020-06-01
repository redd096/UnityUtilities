namespace redd096
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    [AddComponentMenu("redd096/Managers/Game Manager")]
    public class GameManager : Singleton<GameManager>
    {
        //public Player player { get; private set; }

        protected override void SetDefaults()
        {
            //player = FindObjectOfType<Player>();
        }
    }
}