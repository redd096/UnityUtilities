namespace redd096
{
    using UnityEngine;
    using UnityEngine.EventSystems;

    [AddComponentMenu("redd096/UI Control/Event System redd096")]
    public class EventSystemRedd096 : EventSystem
    {
        #region variables

        [Tooltip("For every menu, from what object start?")]
        [SerializeField] GameObject[] firstSelectedGameObjects = default;

        GameObject lastSelected;

        #endregion

        protected override void Update()
        {
            base.Update();

            GameObject selected = current.currentSelectedGameObject;

            //if there is something selected and active
            if (selected && selected.activeInHierarchy)
            {
                //if != from last selected, set last selected
                if(lastSelected != selected)
                    lastSelected = selected;
            }
            else
            {
                //else, if last selected is active, select it
                if(lastSelected && lastSelected.activeInHierarchy)
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