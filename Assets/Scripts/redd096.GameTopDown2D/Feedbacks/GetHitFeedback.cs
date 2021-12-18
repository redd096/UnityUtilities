using System.Collections.Generic;
using System.Collections;
using UnityEngine;
//using NaughtyAttributes;
using redd096.Attributes;

namespace redd096.GameTopDown2D
{
    [AddComponentMenu("redd096/.GameTopDown2D/Feedbacks/Get Hit Feedback")]
    public class GetHitFeedback : MonoBehaviour
    {
        [Header("Necessary Components - default get in parent")]
        [SerializeField] HealthComponent component;

        [Header("Blink - Default get component in children")]
        [SerializeField] bool blinkOnGetDamage = true;
        [CanEnable("blinkOnGetDamage")] [SerializeField] SpriteRenderer[] spritesToUse = default;
        [CanEnable("blinkOnGetDamage")] [SerializeField] Material blinkMaterial = default;
        [CanEnable("blinkOnGetDamage")] [SerializeField] float blinkDuration = 0.2f;

        [Header("On Get Damage")]
        [SerializeField] InstantiatedGameObjectStruct gameObjectOnGetDamage = default;
        [SerializeField] ParticleSystem particlesOnGetDamage = default;
        [SerializeField] AudioClass audioOnGetDamage = default;

        [Header("On Die")]
        [SerializeField] InstantiatedGameObjectStruct gameObjectOnDie = default;
        [SerializeField] ParticleSystem particlesOnDie = default;
        [SerializeField] AudioClass audioOnDie = default;

        Dictionary<SpriteRenderer, Material> savedMaterials = new Dictionary<SpriteRenderer, Material>();
        Coroutine blinkCoroutine;

        void Awake()
        {
            //save materials
            if (spritesToUse == null || spritesToUse.Length <= 0) spritesToUse = GetComponentsInChildren<SpriteRenderer>();
            foreach (SpriteRenderer sprite in spritesToUse)
                savedMaterials.Add(sprite, sprite.material);
        }

        void OnEnable()
        {
            //get references
            if(component == null) component = GetComponentInParent<HealthComponent>();
            if (spritesToUse == null || spritesToUse.Length <= 0) spritesToUse = GetComponentsInChildren<SpriteRenderer>();

            //add events
            if (component)
            {
                component.onGetDamage += OnGetDamage;
                component.onDie += OnDie;
            }
        }

        void OnDisable()
        {
            //remove events
            if (component)
            {
                component.onGetDamage -= OnGetDamage;
                component.onDie -= OnDie;
            }
        }

        #region private API

        void OnGetDamage()
        {
            //instantiate vfx and sfx
            InstantiateGameObjectManager.instance.Play(gameObjectOnGetDamage, transform.position, transform.rotation);
            ParticlesManager.instance.Play(particlesOnGetDamage, transform.position, transform.rotation);
            SoundManager.instance.Play(audioOnGetDamage, transform.position);

            //blink
            if (blinkCoroutine == null && blinkOnGetDamage)
                blinkCoroutine = StartCoroutine(BlinkCoroutine());
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

        void OnDie(HealthComponent whoDied)
        {
            //instantiate vfx and sfx
            InstantiateGameObjectManager.instance.Play(gameObjectOnDie, transform.position, transform.rotation);
            ParticlesManager.instance.Play(particlesOnDie, transform.position, transform.rotation);
            SoundManager.instance.Play(audioOnDie, transform.position);
        }

        #endregion
    }
}