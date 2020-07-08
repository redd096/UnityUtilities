namespace redd096
{
    using UnityEngine;
    using UnityEngine.EventSystems;

    [AddComponentMenu("redd096/UI Control/Event System redd096")]
    public class EventSystemRedd096 : EventSystem
    {
        #region variables

        [Header("For every menu, from what object start?")]
        [SerializeField] GameObject[] firstSelectedGameObjects = default;

        [Header("When one of these objects is active, can navigate only in its menu")]
        [SerializeField] GameObject[] overrideObjects = default;

        [Header("Can't navigate to these objects")]
        [SerializeField] GameObject[] notInteractables = default;

        GameObject lastSelected;

        #endregion

        protected override void Update()
        {
            base.Update();

            GameObject selected = current.currentSelectedGameObject;

            //if there is something selected and active
            if (selected && selected.activeInHierarchy)
            {
                //if there are override objects
                if (overrideObjects != null && overrideObjects.Length > 0)
                {
                    foreach (GameObject overrideObject in overrideObjects)
                    {
                        //if active override, can't go to another menu
                        if (overrideObject && overrideObject.activeInHierarchy && selected.transform.parent != overrideObject.transform.parent)
                        {
                            //if last selected was in override menu, select it - otherwise select override object
                            if (lastSelected && lastSelected.activeInHierarchy && lastSelected.transform.parent == overrideObject.transform.parent)
                                current.SetSelectedGameObject(lastSelected);
                            else
                                current.SetSelectedGameObject(overrideObject);

                            break;
                        }
                    }
                }

                //if there are "not interactables"
                if (notInteractables != null && notInteractables.Length > 0)
                {
                    foreach (GameObject notInteractable in notInteractables)
                    {
                        //if selected a not interactable object
                        if (notInteractable != null && selected == notInteractable)
                        {
                            //back to last selected or select null
                            if (lastSelected && lastSelected.activeInHierarchy)
                                current.SetSelectedGameObject(lastSelected);
                            else
                                current.SetSelectedGameObject(null);

                            break;
                        }
                    }
                }

                //if != from last selected, set last selected
                if (lastSelected != selected)
                    lastSelected = selected;
            }
            //if selected nothing or is not active
            else
            {
                //if there are override objects
                if (overrideObjects != null && overrideObjects.Length > 0)
                {
                    //if is active an override object, select it
                    foreach (GameObject overrideObject in overrideObjects)
                    {
                        if (overrideObject && overrideObject.activeInHierarchy)
                        {
                            current.SetSelectedGameObject(overrideObject);
                            return;
                        }
                    }
                }

                //else, if last selected is active, select it
                if (lastSelected && lastSelected.activeInHierarchy)
                {
                    current.SetSelectedGameObject(lastSelected);
                }
                else
                {
                    //else check which firstSelectedGameObject is active, and select it
                    foreach (GameObject firstSelect in firstSelectedGameObjects)
                    {
                        if (firstSelect && firstSelect.activeInHierarchy)
                        {
                            current.SetSelectedGameObject(firstSelect);
                            break;
                        }
                    }
                }
            }

            //if selected something not active, select null
            if (selected && selected.activeInHierarchy == false)
                current.SetSelectedGameObject(null);
        }
    }
}