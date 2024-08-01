using System.Collections;
using UnityEngine;

namespace redd096.ComponentsSystem
{
    /// <summary>
    /// On interact, open door
    /// </summary>
    [AddComponentMenu("redd096/.ComponentsSystem/Interactables/Door Interactable")]
    public class DoorInteractable : MonoBehaviour, IInteractable
    {
        [SerializeField] Transform objectToMove;
        [SerializeField] float movementSpeed = 2;
        [SerializeField] float closeAfterSeconds = 3f;
        [SerializeField] Vector2 openPosition;

        bool isDoingAnimation;

        public void Interact(ICharacter interactor)
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