using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using redd096.Attributes;

namespace redd096.GameTopDown2D
{
    [AddComponentMenu("redd096/.GameTopDown2D/Feedbacks/Get Hit Feedback")]
    public class GetHitFeedback : MonoBehaviour
    {
        [Header("Necessary Components - default get in parent")]
        [SerializeField] HealthComponent healthComponent;

        [Header("Blink - Default get component in children")]
        [SerializeField] bool blinkOnGetDamage = true;
        [EnableIf("blinkOnGetDamage")][SerializeField] SpriteRenderer[] spritesToUse = default;
        [EnableIf("blinkOnGetDamage")][SerializeField] Material blinkMaterial = default;
        [EnableIf("blinkOnGetDamage")][SerializeField] float blinkDuration = 0.2f;

        [Header("On Get Damage")]
        [SerializeField] InstantiatedGameObjectStruct gameObjectOnGetDamage = default;
        [SerializeField] ParticleSystem particlesOnGetDamage = default;
        [SerializeField] AudioClass audioOnGetDamage = default;

        [Header("On Get Damage Gamepad Vibration")]
        [SerializeField] bool gamepadVibration = false;
        [EnableIf("gamepadVibration")][SerializeField] bool customVibration = false;
        [EnableIf("gamepadVibration", "customVibration")][SerializeField] float vibrationDuration = 0.1f;
        [EnableIf("gamepadVibration", "customVibration")][SerializeField] float lowFrequency = 0.5f;
        [EnableIf("gamepadVibration", "customVibration")][SerializeField] float highFrequency = 0.8f;

        [Header("On Die (instantiate every element in array)")]
        [SerializeField] InstantiatedGameObjectStruct[] gameObjectsOnDie = default;
        [SerializeField] ParticleSystem[] particlesOnDie = default;
        [SerializeField] AudioClass[] audiosOnDie = default;

        [Header("On Get Health")]
        [SerializeField] FeedbackStructRedd096 feedbackOnGetHealth = default;

        [Header("Stop Time On Get Damage")]
        [SerializeField] bool stopTimeOnGetDamage = false;
        [EnableIf("stopTimeOnGetDamage")][SerializeField] float timeScaleToSet = 0.0f;
        [EnableIf("stopTimeOnGetDamage")][SerializeField] float timeBeforeResetTime = 0.1f;

        [Header("Popup On Get Damage")]
        [SerializeField] PopupText popupPrefab = default;
        Pooling<PopupText> poolPopup = new Pooling<PopupText>();

        Camera cam;
        Character selfCharacter;
        Dictionary<SpriteRenderer, Material> savedMaterials = new Dictionary<SpriteRenderer, Material>();
        Coroutine blinkCoroutine;

        Coroutine stopTimeCoroutine;

        void Awake()
        {
            //get camera reference
            if (popupPrefab)
                cam = Camera.main;

            //save materials
            if (spritesToUse == null || spritesToUse.Length <= 0) spritesToUse = GetComponentsInChildren<SpriteRenderer>();
            foreach (SpriteRenderer sprite in spritesToUse)
                savedMaterials.Add(sprite, sprite.material);
        }

        void OnEnable()
        {
            //get references
            if (healthComponent == null) healthComponent = GetComponentInParent<HealthComponent>();
            if (spritesToUse == null || spritesToUse.Length <= 0) spritesToUse = GetComponentsInChildren<SpriteRenderer>();
            if (selfCharacter == null) selfCharacter = GetComponentInParent<Character>();

            //add events
            if (healthComponent)
            {
                healthComponent.onGetDamage += OnGetDamage;
                healthComponent.onDie += OnDie;
                healthComponent.onGetHealth += OnGetHealth;
            }
        }

        void OnDisable()
        {
            //remove events
            if (healthComponent)
            {
                healthComponent.onGetDamage -= OnGetDamage;
                healthComponent.onDie -= OnDie;
                healthComponent.onGetHealth -= OnGetHealth;
            }
        }

        #region private API

        void OnGetDamage(Character whoHit, Vector2 hitPoint)
        {
            //rotation
            Vector2 direction = ((Vector2)transform.position - hitPoint).normalized;
            Quaternion rotation = Quaternion.LookRotation(Vector3.forward, Quaternion.AngleAxis(90, Vector3.forward) * direction);   //Forward keep to Z axis. Up use X instead of Y (AngleAxis 90) and rotate to direction

            //when rotate to opposite direction (from default), rotate 180 updown
            if (direction.x < 0)
            {
                rotation *= Quaternion.AngleAxis(180, Vector3.right);
            }

            //instantiate vfx and sfx
            InstantiateGameObjectManager.instance.Play(gameObjectOnGetDamage, transform.position, transform.rotation);
            ParticlesManager.instance.Play(particlesOnGetDamage, transform.position, rotation);
            SoundManager.instance.Play(audioOnGetDamage, transform.position);

            //blink
            if (blinkCoroutine == null && blinkOnGetDamage)
                blinkCoroutine = StartCoroutine(BlinkCoroutine());

            //gamepad vibration
            if (selfCharacter && selfCharacter.CharacterType == Character.ECharacterType.Player)
            {
                if (gamepadVibration && GamepadVibration.instance)
                {
                    //custom or default
                    if (customVibration)
                        GamepadVibration.instance.StartVibration(vibrationDuration, lowFrequency, highFrequency);
                    else
                        GamepadVibration.instance.StartVibration();
                }
            }

            //stop time on get damage
            if (stopTimeOnGetDamage && selfCharacter && selfCharacter.CharacterType == Character.ECharacterType.Player)
            {
                if (stopTimeCoroutine != null)
                    StopCoroutine(stopTimeCoroutine);

                stopTimeCoroutine = StartCoroutine(StopTimeCoroutine());
            }

            //popup on get damage
            if (popupPrefab && cam)
            {
                PopupText popup = poolPopup.Instantiate(popupPrefab);
                popup.GetComponentInChildren<Canvas>().worldCamera = cam;
                popup.Init(transform.position);//cam.WorldToScreenPoint(transform.position);
            }
        }

        IEnumerator BlinkCoroutine()
        {
            //set blink material, wait, then back to saved material
            foreach (SpriteRenderer sprite in savedMaterials.Keys)
                sprite.material = blinkMaterial;

            yield return new WaitForSeconds(blinkDuration);

            foreach (SpriteRenderer sprite in savedMaterials.Keys)
                sprite.material = savedMaterials[sprite];

            blinkCoroutine = null;
        }

        void OnDie(HealthComponent whoDied, Character whoHit)
        {
            //instantiate vfx and sfx
            foreach (InstantiatedGameObjectStruct go in gameObjectsOnDie)
                InstantiateGameObjectManager.instance.Play(go, transform.position, transform.rotation);         //instantiate every element in array

            foreach (ParticleSystem particle in particlesOnDie)
                ParticlesManager.instance.Play(particle, transform.position, transform.rotation);               //instantiate every element in array

            foreach (AudioClass audio in audiosOnDie)
                SoundManager.instance.Play(audio, transform.position);                                          //instantiate every element in array
        }

        void OnGetHealth()
        {
            //instantiate vfx and sfx
            feedbackOnGetHealth.InstantiateFeedback(transform);
        }

        IEnumerator StopTimeCoroutine()
        {
            //set time scale
            Time.timeScale = timeScaleToSet;

            //wait
            float time = Time.realtimeSinceStartup + timeBeforeResetTime;
            while (time > Time.realtimeSinceStartup)
            {
                yield return null;

                //if (GameManager.instance && GameManager.instance.levelManager)
                //{
                //    //if set pause, stop this coroutine
                //    if (GameManager.instance.levelManager.LevelState == LevelManager.ELevelState.Pause)
                //        yield break;
                //}
            }

            //reset time
            Time.timeScale = 1;
        }

        #endregion
    }
}