using UnityEngine;

namespace redd096.Game3D
{
    /// <summary>
    /// Play sounds when grab, release, etc...
    /// </summary>
    [AddComponentMenu("redd096/.Game3D/Feedbacks/DraggableJoint Feedbacks")]
    public class DraggableJointFeedbacks : MonoBehaviour
    {
        [SerializeField] DraggableJointInteractable draggable;
        [SerializeField] GetDeltaPosition draggableDelta;
        [Space]
        [Tooltip("When start drag object, if the objects was close")][SerializeField] AudioClip[] onOpenAudio;
        [Tooltip("When moving heavy the object to completely open position (delta near the 1)")][SerializeField] AudioClip[] onOpenHeavyPositive;
        [Tooltip("When moving heavy the object to completely open but in opposite position (delta near the -1)")][SerializeField] AudioClip[] onOpenHeavyNegative;
        [Tooltip("When moving softly the object to completely close position (delta near the 0)")][SerializeField] AudioClip[] onCloseLightAudio;
        [Tooltip("When moving heavy the object to completely close position (delta near the 0)")][SerializeField] AudioClip[] onCloseHeavyAudio;
        [Space]
        [Tooltip("An audio to play when the object is moving. The audio is modified by the velocity")][SerializeField] AudioClip[] loopAudio;
        [Space]
        [Tooltip("When start drag object")][SerializeField] AudioClip[] onGrabAudio;
        [Tooltip("When stop drag object, if it was release softly")][SerializeField] AudioClip[] onReleaseAudio;
        [Tooltip("When stop grab object, if it was throwed")][SerializeField] AudioClip[] onThrowAudio;
        [Tooltip("When the joint is destroyed because the user is too distant from the handle")][SerializeField] AudioClip[] onTooMuchDistanceDestroyJoint;

        bool previousWasClosed;
        bool previousWasTotallyOpen;
        float previousVelocity;
        float previousAngularVelocity;

        const float velocityThrow = 8f;
        const float velocityHeavy = 1f;
        AudioSource loopAudioSource;

        private void Awake()
        {
            //be sure to have components
            if (draggable == null && TryGetComponent(out draggable) == false)
                Debug.LogError("Miss DraggableJointInteractable on " + name, gameObject);
            if (draggableDelta == null && TryGetComponent(out draggableDelta) == false)
                Debug.LogError("Miss GetDeltaPosition on " + name, gameObject);

            //set vars
            GetCurrentDelta(out bool nowIsClosed, out bool nowIsTotallyOpen, out bool openIsPositive);
            previousWasClosed = nowIsClosed;
            previousWasTotallyOpen = nowIsTotallyOpen;

            //start loop audio
            if (loopAudio != null && loopAudio.Length > 0)
            {
                loopAudioSource = new GameObject("Loop Audio", typeof(AudioSource)).GetComponent<AudioSource>();
                SoundManager.instance.Play(loopAudio, draggable.Rb.position, 0f, loopAudioSource, loop: true, enable3D: true, maxDistance: 10);
                loopAudioSource.transform.SetParent(draggable.Rb.transform);
                loopAudioSource.transform.localPosition = Vector3.zero;
            }

            //add events
            if (draggable)
            {
                draggable.onInteract += OnInteract;
                draggable.onDismiss += OnDismiss;
                draggable.onTooMuchDistance += OnTooMuchDistance;
            }
        }

        private void OnDestroy()
        {
            //remove events
            if (draggable)
            {
                draggable.onInteract -= OnInteract;
                draggable.onDismiss -= OnDismiss;
                draggable.onTooMuchDistance -= OnTooMuchDistance;
            }
        }

        private void FixedUpdate()
        {
            //set loop volume
            SetLoopVolume();

            //check closed actions ======================

            GetCurrentDelta(out bool nowIsClosed, out bool nowIsTotallyOpen, out bool openIsPositive);

            //update force when open 
            if (previousWasClosed == false && nowIsClosed == false)
            {
                previousVelocity = draggable.Rb.velocity.magnitude;
                previousAngularVelocity = draggable.Rb.angularVelocity.magnitude;
            }

            CheckCloseEvents(nowIsClosed);
            CheckOpenEvents(nowIsTotallyOpen, openIsPositive);
        }

        private void OnInteract()
        {
            //sound on grab
            SoundManager.instance.Play(onGrabAudio, draggable.Rb.position);

            //sound on open
            if (previousWasClosed)
                SoundManager.instance.Play(onOpenAudio, draggable.Rb.position);
        }

        private void OnDismiss()
        {
            //sound on throw or release
            SoundManager.instance.Play(draggable.Rb.velocity.magnitude > velocityThrow ? onThrowAudio : onReleaseAudio, draggable.Rb.position);
        }

        private void OnTooMuchDistance()
        {
            //sound when destroy joint for too much distance
            SoundManager.instance.Play(onTooMuchDistanceDestroyJoint, draggable.Rb.position);
        }

        #region private API

        void SetLoopVolume()
        {
            //set loop volume
            float volume;
            if (draggableDelta.Type == GetDeltaPosition.EType.Position)
                volume = draggable.Rb.velocity.magnitude;
            else
                volume = draggable.Rb.angularVelocity.magnitude / 3f;

            volume = Mathf.Clamp01(volume);
            loopAudioSource.volume = SoundManager.instance.GetCorrectVolume(AudioData.EAudioType.Sfx, Mathf.Lerp(loopAudioSource.volume, volume, 10 * Time.deltaTime));
        }

        private void GetCurrentDelta(out bool isClosed, out bool isTotallyOpen, out bool openIsPositive)
        {
            if (draggableDelta == null)
            {
                isClosed = false;
                isTotallyOpen = false;
                openIsPositive = false;
                return;
            }

            //set open or closed based on delta
            float delta = draggableDelta.GetDeltaClamped();
            isClosed = delta < 0.1f && delta > -0.1f;
            isTotallyOpen = delta > 0.9f || delta < -0.9f;
            openIsPositive = delta > 0;
        }

        private void CheckCloseEvents(bool nowIsClosed)
        {
            //when closed, do nothing
            if (previousWasClosed && nowIsClosed)
            {
                return;
            }

            //set no more closed
            if (previousWasClosed && nowIsClosed == false)
            {
                previousWasClosed = false;
                return;
            }

            //set when close again
            if (previousWasClosed == false && nowIsClosed)
            {
                previousWasClosed = true;

                //and play sound
                float velocityToUse = previousVelocity > previousAngularVelocity ? previousVelocity : previousAngularVelocity;
                if (velocityToUse > velocityHeavy)
                    SoundManager.instance.Play(onCloseHeavyAudio, draggable.Rb.position);
                else
                    SoundManager.instance.Play(onCloseLightAudio, draggable.Rb.position);

                return;
            }
        }

        private void CheckOpenEvents(bool nowIsTotallyOpen, bool openIsPositive)
        {
            //if was already totally open, do nothing
            if (previousWasTotallyOpen && nowIsTotallyOpen)
            {
                return;
            }

            //set no more totally open
            if (previousWasTotallyOpen && nowIsTotallyOpen == false)
            {
                previousWasTotallyOpen = false;
                return;
            }

            //set when totally open again
            if (previousWasTotallyOpen == false && nowIsTotallyOpen)
            {
                previousWasTotallyOpen = true;

                //and play sound
                float velocityToUse = previousVelocity > previousAngularVelocity ? previousVelocity : previousAngularVelocity;
                if (velocityToUse > velocityHeavy)
                {
                    if (openIsPositive)
                        SoundManager.instance.Play(onOpenHeavyPositive, draggable.Rb.position);
                    else
                        SoundManager.instance.Play(onOpenHeavyNegative, draggable.Rb.position);
                }

                return;
            }
        }

        #endregion
    }
}