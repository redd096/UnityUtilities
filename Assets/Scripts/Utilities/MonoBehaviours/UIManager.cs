namespace redd096
{
    using UnityEngine;

    [AddComponentMenu("redd096/MonoBehaviours/UI Manager")]
    public class UIManager : MonoBehaviour
    {
        [SerializeField] GameObject pauseMenu = default;

        public void PauseMenu(bool active)
        {
            pauseMenu.SetActive(active);
        }
    }
}