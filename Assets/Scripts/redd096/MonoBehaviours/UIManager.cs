using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace redd096
{
    [AddComponentMenu("redd096/MonoBehaviours/UI Manager")]
    public class UIManager : MonoBehaviour
    {
        [Header("Menu")]
        [SerializeField] float delayInputWhenOpenMenu = 0.3f;
        [SerializeField] GameObject pauseMenu = default;
        [SerializeField] GameObject endMenu = default;

        //delay input when open menu
        EventSystem eventSystem;
        Coroutine delayInputCoroutine;

        void Start()
        {
            //get references
            eventSystem = EventSystem.current;

            //by default, deactive menus
            PauseMenu(false);
            EndMenu(false);
        }

        #region open menu with input delay

        void OpenMenu(GameObject menu, bool active)
        {
            if (menu == null)
                return;

            //when active menu, deactive event system for a little time
            if (active)
            {
                if (eventSystem) eventSystem.enabled = false;

                //restart coroutine
                if (delayInputCoroutine != null) StopCoroutine(delayInputCoroutine);
                delayInputCoroutine = StartCoroutine(DelayInputCoroutine());
            }

            //active or deactive menu
            menu.SetActive(active);
        }

        IEnumerator DelayInputCoroutine()
        {
            //wait (real time, so if Time.timeScale is 0 it works anyway)
            float timeDelay = Time.realtimeSinceStartup + delayInputWhenOpenMenu;
            while (Time.realtimeSinceStartup < timeDelay)
                yield return null;

            //then re-enable event system
            if (eventSystem) eventSystem.enabled = true;
        }

        #endregion

        #region menu

        public void PauseMenu(bool active)
        {
            //active or deactive pause menu
            OpenMenu(pauseMenu, active);
        }

        public void EndMenu(bool active)
        {
            if (endMenu == null)
            {
                return;
            }

            //be sure to remove pause menu when active end menu
            if (active)
                PauseMenu(false);

            //active or deactive end menu
            OpenMenu(endMenu, active);
        }

        #endregion
    }
}