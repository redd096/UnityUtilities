using System.Collections;
using UnityEngine;
using redd096.v2.ComponentsSystem;

namespace redd096.Examples.ComponentsSystem
{
    /// <summary>
    /// On interact, open door. After few seconds, close it
    /// </summary>
    [AddComponentMenu("redd096/Examples/ComponentsSystem/Interactables/Example Door Interactable")]
    public class ExampleDoorInteractable : MonoBehaviour, ISimpleInteractable
    {
        [SerializeField] Transform objectToMove;
        [SerializeField] float movementSpeed = 2;
        [SerializeField] float closeAfterSeconds = 3f;
        [SerializeField] Vector2 openPosition;

        bool isDoingAnimation;

        public void Interact(IGameObjectRD interactor)
        {
            //do nothing if already open
            if (isDoingAnimation)
                return;

            isDoingAnimation = true;

            StartCoroutine(MovementAnimation());
        }

        private IEnumerator MovementAnimation()
        {
            //calculate duration open animation
            Vector2 startPosition = objectToMove.localPosition;
            float distance = Vector2.Distance(startPosition, openPosition);
            float timeDuration = distance / movementSpeed;

            //open animation
            float delta = 0;
            while (delta < 1)
            {
                delta += Time.deltaTime / timeDuration;
                objectToMove.localPosition = Vector2.Lerp(startPosition, openPosition, delta);
                yield return null;
            }

            //wait
            yield return new WaitForSeconds(closeAfterSeconds);

            //close animation
            delta = 0;
            while (delta < 1)
            {
                delta += Time.deltaTime / timeDuration;
                objectToMove.localPosition = Vector2.Lerp(openPosition, startPosition, delta);
                yield return null;
            }

            isDoingAnimation = false;
        }
    }
}