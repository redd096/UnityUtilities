using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace redd096.Examples.NetworkLobby
{
    /// <summary>
    /// List of players in Lobby. Use this to show color and name
    /// </summary>
    [AddComponentMenu("redd096/Examples/NetworkLobby/Lobby PlayerUI")]
    public class LobbyPlayerUI : MonoBehaviour
    {
        [SerializeField] Image playerImage;
        [SerializeField] TMP_Text playerNameLabel;

        public void Init(Color color, string playerName)
        {
            playerImage.color = color;
            playerNameLabel.text = playerName;
        }
    }
}