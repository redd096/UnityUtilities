using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem.UI;
#endif

namespace redd096.UIControl
{
    /// <summary>
    /// Autoselect UI to use keyboard and gamepad
    /// </summary>
    [AddComponentMenu("redd096/UIControl/Optimize Event System")]
    public class OptimizeEventSystem : MonoBehaviour
    {
        #region variables

        enum EEventSystemToUse { GetComponent, UseCurrent }

        [Header("GetComponent from this gameObject or use EventSystem.current?")]
        [SerializeField] EEventSystemToUse eventSystemToUse = EEventSystemToUse.GetComponent;

        [Header("Start from this Menu - navigate with ChangeMenu and BackMenu to update it")]
        [SerializeField] GameObject startMenu = default;

        [Header("Deselect objects when using mouse - at start keep everything disabled until first event")]
        [SerializeField] bool deselectWhenUseMouse = true;
        [SerializeField] bool waitFirstEventToSelect = true;

        [Header("For every menu, from which object start?")]
        [SerializeField] GameObject[] firstSelectedGameObjects = default;

        [Header("When one of these objects is active, can navigate only in objects with same parent")]
        [SerializeField] GameObject[] overrideObjects = default;

        [Header("Can't navigate to these objects")]
        [SerializeField] List<GameObject> notNavigables = new List<GameObject>();

        [Header("When move mouse, don't deselect if selecting these objects")]
        [SerializeField] List<GameObject> notDeselectObjects = new List<GameObject>();

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

        bool canSelect;
        bool isUsingMouse;
        bool isFirstUpdateMousePosition;
        Vector2 lastMousePosition;
        Vector2 mousePosition;

        #endregion

        private void Start()
        {
            //set current menu
            currentMenu = startMenu;

            //normally can select objects, but when waitFirstEventToSelect is enabled, we must wait a keyboard or pad event before select first object
            canSelect = !waitFirstEventToSelect;

            isFirstUpdateMousePosition = true;
        }

        private void LateUpdate()
        {
            if (eventSystem == null)
                return;

            //if first time to update mouse position, call UpdateMousePosition() double time, so both lastMousePosition and mousePosition will have same value
            if (isFirstUpdateMousePosition)
            {
                isFirstUpdateMousePosition = false;
                UpdateMousePosition();
            }

            //keep updated mouse position
            UpdateMousePosition();

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

            //==============================================

            //if previous can't select objects, check if now received a keyboard or pad event
            if (canSelect == false)
            {
                if (UsedKeyboardOrGamepad())
                {
                    canSelect = true;
                    isUsingMouse = false;
                }
            }

            //if using mouse, don't select anything
            if (deselectWhenUseMouse && CheckMouse())
            {
                canSelect = false;

                //if selecting something, deselect it
                if (selected && notDeselectObjects.Contains(selected) == false)
                    SetSelectedGameObject(null);

                return;
            }

            //==============================================

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
                SetSelectedGameObject(null);
        }

        #region public API

        public void SetSelectedGameObject(GameObject go)
        {
            eventSystem.SetSelectedGameObject(go);
            selected = go;
        }

        #endregion

        #region get current override object

        GameObject GetCurrentOverrideObject()
        {
            //if is active an override object, return it
            if (overrideObjects != null)
            {
                for (int i = 0; i < overrideObjects.Length; i++)
                {
                    if (overrideObjects[i] && overrideObjects[i].activeInHierarchy)
                    {
                        return overrideObjects[i];
                    }
                }
            }

            return null;
        }

        #endregion

        #region check used device

        bool UsedKeyboardOrGamepad()
        {
            if (eventSystem.currentInputModule is StandaloneInputModule standaloneInput)
            {
                return standaloneInput.input.GetButtonDown(standaloneInput.cancelButton) ||
                    standaloneInput.input.GetButtonDown(standaloneInput.submitButton) ||
                    !Mathf.Approximately(standaloneInput.input.GetAxisRaw(standaloneInput.horizontalAxis), 0.0f) ||
                    !Mathf.Approximately(standaloneInput.input.GetAxisRaw(standaloneInput.verticalAxis), 0.0f);
            }
#if ENABLE_INPUT_SYSTEM
            if (eventSystem.currentInputModule is InputSystemUIInputModule inputSystemUIInput)
            {
                return inputSystemUIInput.cancel.action.WasPressedThisFrame() ||
                    inputSystemUIInput.submit.action.WasPressedThisFrame() ||
                    inputSystemUIInput.move.action.WasPressedThisFrame();
            }
#endif
            return false;
        }

        void UpdateMousePosition()
        {
            if (eventSystem.currentInputModule is StandaloneInputModule standaloneInput)
            {
                lastMousePosition = mousePosition;
                mousePosition = standaloneInput.input.mousePosition;
            }
#if ENABLE_INPUT_SYSTEM
            if (eventSystem.currentInputModule is InputSystemUIInputModule inputSystemUIInput)
            {
                lastMousePosition = mousePosition;
                mousePosition = inputSystemUIInput.point.action.ReadValue<Vector2>();
            }
#endif
        }

        bool UsedMouse()
        {
            if (eventSystem.currentInputModule is StandaloneInputModule standaloneInput)
            {
                return (mousePosition - lastMousePosition).sqrMagnitude > 0.0f ||
                    standaloneInput.input.GetMouseButtonDown(0);
            }
#if ENABLE_INPUT_SYSTEM
            if (eventSystem.currentInputModule is InputSystemUIInputModule inputSystemUIInput)
            {
                return (mousePosition - lastMousePosition).sqrMagnitude > 0.0f || 
                    inputSystemUIInput.leftClick.action.WasPressedThisFrame();
            }
#endif

            return isUsingMouse;
        }

        bool CheckMouse()
        {
            //check if press any key
            if (isUsingMouse)
            {
                if (UsedKeyboardOrGamepad())
                    isUsingMouse = false;
            }
            //check if move or click with mouse
            else
            {
                if (UsedMouse())
                    isUsingMouse = true;
            }

            return isUsingMouse;
        }

        #endregion

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

                for (int i = 0; i < firstSelectedGameObjects.Length; i++)
                {
                    if (firstSelectedGameObjects[i] && firstSelectedGameObjects[i].activeInHierarchy)
                    {
                        SetSelectedGameObject(firstSelectedGameObjects[i]);
                        break;
                    }
                }
            }
        }

        #endregion

        #region menu - public API

        /// <summary>
        /// Deactive current menu and active old one
        /// </summary>
        protected virtual bool BackToOldMenu()
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
        public virtual void ChangeMenu(GameObject newMenu)
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
        public virtual void BackMenu()
        {
            BackToOldMenu();
        }

        #endregion
    }
}