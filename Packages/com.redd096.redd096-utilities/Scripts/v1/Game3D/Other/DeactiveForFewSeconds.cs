using UnityEngine;

namespace redd096.v1.Game3D
{
    /// <summary>
    /// Deactive for few seconds this character. This is used for example for RotationComponent, you don't want to press play and see already your camera looking to the floor. 
    /// </summary>
    [AddComponentMenu("redd096/v1/Game3D/Other/Deactive For Few Seconds")]
    public class DeactiveForFewSeconds : MonoBehaviour
    {
        [SerializeField] bool lockMouseOnAwake = true;
        [SerializeField] float delayBeforeEnableComponents = 0.5f;
        [SerializeField] MonoBehaviour[] componentsToDisable;

        private void Awake()
        {
            //lock mouse if necessary
            if (lockMouseOnAwake)
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }

            //disable every component
            foreach (var v in componentsToDisable)
            {
                if (v)
                    v.enabled = false;
            }

            //re-enable after few seconds
            Invoke(nameof(ToggleComponents), delayBeforeEnableComponents);
        }

        void ToggleComponents()
        {
            foreach (var v in componentsToDisable)
            {
                if (v)
                    v.enabled = true;
            }
        }
    }
}