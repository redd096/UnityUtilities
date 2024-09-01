using UnityEngine;

namespace redd096.v2.ComponentsSystem.Example
{
    /// <summary>
    /// Use this to rotate player based on AimComponent
    /// </summary>
    [System.Serializable]
    public class ExampleRotateCharacterFeedback : IComponentRD
    {
        [SerializeField] Transform objectToRotate;
        [SerializeField] bool useLerp;
        [SerializeField] float lerpSpeed = 2f;

        public IGameObjectRD Owner { get; set; }

        TopDownAimComponent aimComponent;

        public void Awake()
        {
            if (aimComponent == null && Owner.TryGetComponentRD(out aimComponent) == false)
                Debug.LogError($"Missing aimComponent on {GetType().Name}", Owner.transform.gameObject);
            if (objectToRotate == null)
                Debug.LogError($"Missing objectToRotate on {GetType().Name}", Owner.transform.gameObject);
        }

        public void Update()
        {
            //rotate only on Y axis, to look at aim direciton
            if (aimComponent != null && objectToRotate)
            {
                Quaternion lookDirection = Quaternion.LookRotation(aimComponent.AimDirectionInput, Vector3.up);

                if (useLerp)
                    objectToRotate.localRotation = Quaternion.Lerp(objectToRotate.localRotation, Quaternion.AngleAxis(lookDirection.eulerAngles.y, Vector3.up), Time.deltaTime * lerpSpeed);
                else
                    objectToRotate.localRotation = Quaternion.AngleAxis(lookDirection.eulerAngles.y, Vector3.up);
            }
        }
    }

}