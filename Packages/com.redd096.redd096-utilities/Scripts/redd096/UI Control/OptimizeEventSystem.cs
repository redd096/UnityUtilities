using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem.UI;
#endif

namespace redd096
{
    [AddComponentMenu("redd096/UI Control/Optimize Event System")]
    public class OptimizeEventSystem : MonoBehaviour
    {
        #region variables

        enum EEventSystemToUse { GetComponent, UseCurrent }

        [Header("GetComponent from this gameObject or use EventSystem.current?")]
        [SerializeField] EEventSystemToUse eventSystemToUse = EEventSystemToUse.GetComponent;

        [Header("Start from this Menu")]
        [SerializeField] GameObject startMenu = default;

        [Header("For every menu, from which object start?")]
        [SerializeField] GameObject[] firstSelectedGameObjects = default;

        [Header("When one of these objects is active, can navigate only in its menu")]
        [SerializeField] GameObject[] overrideObjects = default;

        [Header("Can't navigate to these objects")]
        [SerializeField] List<GameObject> notNavigables = new List<GameObject>();

        EventSystem _eventSystem;
        EventSystem eventSystem
        {
            get
            {
                //use EventSystem.current
                if (eventSystemToUse == EEventSystemToUse.UseCurrent) 
                    return EventSystem.current;

                //or GetComponent
                if (_eventSystem == null)
                    _eventSystem = GetComponent<EventSystem>();
                return _eventSystem;
            }
        }

        GameObject selected;
        GameObject lastSelected;
        GameObject currentOverrideObjectActive;

        List<GameObject> previousMenu = new List<GameObject>();
        GameObject currentMenu;

        #endregion

        private void Start()
        {
            //set current menu
            currentMenu = startMenu;
        }
        
        private void LateUpdate()
        {
            if (eventSystem == null)
                return;

            //set current selected and current override object active
            selected = eventSystem.currentSelectedGameObject;
            currentOverrideObjectActive = GetCurrentOverrideObject();

            //can't close override menu
            if (currentOverrideObjectActive == null)
            {
                //if press cancel button come back to old menu
                if (
                (eventSystem.currentInputModule is StandaloneInputModule && Input.GetButtonDown(((StandaloneInputModule)eventSystem.currentInputModule).cancelButton))              //old input system
#if ENABLE_INPUT_SYSTEM
                || (eventSystem.currentInputModule is InputSystemUIInputModule && ((InputSystemUIInputModule)eventSystem.currentInputModule).cancel.action.WasPressedThisFrame())   //new input system
#endif
                )
                {
                    if (BackToOldMenu())
                        return;
                }
            }

            //if there is something selected and active
            if (selected && selected.activeInHierarchy)
            {
                //check if there is an override object active (can navigate only in its menu)
                CheckOverride();

                //check if selected a not navigable object
                CheckNotNavigables();

                //if != from last selected, set last selected
                if (lastSelected != selected)
                    lastSelected = selected;
            }
            //else if selected nothing or is not active
            else
            {
                //if is active an override object, select it
                if (SetOverride())
                    return;

                //else, if last selected is active, select it
                //else check which firstSelectedGameObject is active, and select it
                SetFirstObject();
            }

            //if selected something not active, select null
            if (selected && selected.activeInHierarchy == false)
                eventSystem.SetSelectedGameObject(null);
        }

        GameObject GetCurrentOverrideObject()
        {
            //if is active an override object, return it
            if (overrideObjects != null)
            {
                foreach (GameObject overrideObj in overrideObjects)
                {
                    if (overrideObj && overrideObj.activeInHierarchy)
                    {
                        return overrideObj;
                    }
                }
            }

            return null;
        }

        void SetSelectedGameObject(GameObject go)
        {
            eventSystem.SetSelectedGameObject(go);
            selected = go;
        }


        #region selected and active

        void CheckOverride()
        {
            //if an override is active
            if (currentOverrideObjectActive &&
                (selected == null || selected.transform.parent != currentOverrideObjectActive.transform.parent))    //if selected something out of its menu
            {
                //if last selected was in override menu, select it - otherwise select override object
                if (lastSelected && lastSelected.activeInHierarchy && lastSelected.transform.parent == currentOverrideObjectActive.transform.parent)
                    SetSelectedGameObject(lastSelected);
                else
                    SetSelectedGameObject(currentOverrideObjectActive);
            }
        }

        void CheckNotNavigables()
        {
            if (notNavigables == null || notNavigables.Count <= 0)
                return;

            //if selected a not navigable object
            if (notNavigables.Contains(selected))
            {
                //back to last selected or select null
                if (lastSelected && lastSelected.activeInHierarchy)
                    SetSelectedGameObject(lastSelected);
                else
                    SetSelectedGameObject(null);
            }
        }

        #endregion

        #region selected nothing or not active

        bool SetOverride()
        {
            //if is active an override object, select it
            if (currentOverrideObjectActive)
            {
                SetSelectedGameObject(currentOverrideObjectActive);
                return true;
            }

            return false;
        }

        void SetFirstObject()
        {
            //if last selected is active, select it
            if (lastSelected && lastSelected.activeInHierarchy)
            {
                SetSelectedGameObject(lastSelected);
            }
            //else check which firstSelectedGameObject is active, and select it
            else
            {
                if (firstSelectedGameObjects == null || firstSelectedGameObjects.Length <= 0)
                    return;

                foreach (GameObject firstSelect in firstSelectedGameObjects)
                {
                    if (firstSelect && firstSelect.activeInHierarchy)
                    {
                        SetSelectedGameObject(firstSelect);
                        break;
                    }
                }
            }
        }

        #endregion

        #region menu

        /// <summary>
        /// Deactive current menu and active old one
        /// </summary>
        bool BackToOldMenu()
        {
            if (previousMenu != null && previousMenu.Count > 0 && currentMenu != null)
            {
                GameObject lastMenu = previousMenu[previousMenu.Count - 1];

                //reactive last previous menu and deactive current
                lastMenu.SetActive(true);
                currentMenu.SetActive(false);

                //set this as current menu and remove from previous menu list
                currentMenu = lastMenu;
                previousMenu.Remove(lastMenu);

                return true;
            }

            return false;
        }

        /// <summary>
        /// Active new menu and deactive old one
        /// </summary>
        public void ChangeMenu(GameObject newMenu)
        {
            //check if back to last previous menu
            if (previousMenu != null && previousMenu.Count > 0)
            {
                GameObject lastMenu = previousMenu[previousMenu.Count - 1];

                if (newMenu == lastMenu)
                {
                    BackToOldMenu();
                    return;
                }
            }

            //else, active new menu and deactive current
            newMenu.SetActive(true);
            if (currentMenu != null)
                currentMenu.SetActive(false);

            //add current menu to previous
            if (previousMenu != null && currentMenu != null)
                previousMenu.Add(currentMenu);

            //and set new one as current
            currentMenu = newMenu;
        }

        /// <summary>
        /// Deactive current menu and active old one. This function is used by buttons in scene, because return void instead of bool
        /// </summary>
        public void BackMenu()
        {
            BackToOldMenu();
        }

        #endregion
    }
}