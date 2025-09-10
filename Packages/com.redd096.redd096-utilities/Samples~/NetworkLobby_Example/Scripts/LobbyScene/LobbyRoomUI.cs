using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace redd096.Examples.NetworkLobby
{
    /// <summary>
    /// List of rooms in Lobby. Use this to show room values and to connect on click
    /// </summary>
    [AddComponentMenu("redd096/Examples/NetworkLobby/Lobby RoomUI")]
    public class LobbyRoomUI : MonoBehaviour
    {
        [SerializeField] Button mainButton;
        [SerializeField] TMP_Text playersCountLabel;
        [SerializeField] TMP_Text roomNameLabel;

        private string roomID;
        private System.Action<string> onClick;

        private void Awake()
        {
            mainButton.onClick.AddListener(OnClick);
        }

        private void OnDestroy()
        {
            mainButton.onClick.RemoveListener(OnClick);
        }

        public void Init(int currentPlayersCount, int maxPlayersCount, string roomName, string roomID, System.Action<string> onClick)
        {
            playersCountLabel.text = $"{currentPlayersCount}/{maxPlayersCount}";
            roomNameLabel.text = roomName;

            this.roomID = roomID;
            this.onClick = onClick;
        }

        private void OnClick()
        {
            onClick?.Invoke(roomID);
        }
    }
}