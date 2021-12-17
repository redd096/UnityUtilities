﻿using UnityEngine;
//using NaughtyAttributes;
using redd096.Attributes;

namespace redd096.GameTopDown2D
{
    [AddComponentMenu("redd096/.GameTopDown2D/Feedbacks/Weapon Range Feedback")]
    public class WeaponRangeFeedback : MonoBehaviour
    {
        [Header("Necessary Components - default get in parent")]
        [SerializeField] WeaponRange weaponRange;

        [Header("On Instantiate Bullet")]
        [SerializeField] InstantiatedGameObjectStruct gameObjectOnInstantiateBullet = default;
        [SerializeField] ParticleSystem particlesOnInstantiateBullet = default;
        [SerializeField] AudioClass audioOnInstantiateBullet = default;

        [Header("On Shoot - main barrel by default is transform")]
        [SerializeField] Transform mainBarrel = default;
        [SerializeField] InstantiatedGameObjectStruct gameObjectOnShoot = default;
        [SerializeField] ParticleSystem particlesOnShoot = default;
        [SerializeField] AudioClass audioOnShoot = default;

        [Header("On Shoot Camera Shake")]
        [SerializeField] bool cameraShake = true;
        [CanEnable("cameraShake")] [SerializeField] bool customShake = false;
        [CanEnable("cameraShake", "customShake")] [SerializeField] float shakeDuration = 1;
        [CanEnable("cameraShake", "customShake")] [SerializeField] float shakeAmount = 0.7f;

        [Header("On Press Attack - barrel by default is transform")]
        [SerializeField] Transform barrelOnPress = default;
        [SerializeField] InstantiatedGameObjectStruct gameObjectOnPress = default;
        [SerializeField] ParticleSystem particlesOnPress = default;
        [SerializeField] AudioClass audioOnPress = default;

        //deactive on release attack
        GameObject instantiatedGameObjectOnPress;
        ParticleSystem instantiatedParticlesOnPress;
        AudioSource instantiatedAudioOnPress;        

        void OnEnable()
        {
            //get references
            if(weaponRange == null) weaponRange = GetComponentInParent<WeaponRange>();
            if (mainBarrel == null) mainBarrel = transform;
            if (barrelOnPress == null) barrelOnPress = transform;

            //add events
            if (weaponRange)
            {
                weaponRange.onInstantiateBullet += OnInstantiateBullet;
                weaponRange.onShoot += OnShoot;
                weaponRange.onPressAttack += OnPressAttack;
                weaponRange.onReleaseAttack += OnReleaseAttack;
            }
        }

        void OnDisable()
        {
            //remove events
            if (weaponRange)
            {
                weaponRange.onInstantiateBullet -= OnInstantiateBullet;
                weaponRange.onShoot -= OnShoot;
                weaponRange.onPressAttack -= OnPressAttack;
                weaponRange.onReleaseAttack -= OnReleaseAttack;
            }
        }

        #region private API

        void OnInstantiateBullet(Transform barrel)
        {
            //instantiate vfx and sfx
            GameObject instantiatedGameObject = InstantiateGameObjectManager.instance.Play(gameObjectOnInstantiateBullet, barrel.position, barrel.rotation);
            ParticleSystem instantiatedParticles = ParticlesManager.instance.Play(particlesOnInstantiateBullet, barrel.position, barrel.rotation);
            SoundManager.instance.Play(audioOnInstantiateBullet, barrel.position);

            //set parent to vfx
            if (instantiatedGameObject)
                instantiatedGameObject.transform.SetParent(barrel);
            if (instantiatedParticles)
                instantiatedParticles.transform.SetParent(barrel);
        }

        void OnShoot()
        {
            //instantiate vfx and sfx
            GameObject instantiatedGameObject = InstantiateGameObjectManager.instance.Play(gameObjectOnShoot, mainBarrel.position, mainBarrel.rotation);
            ParticleSystem instantiatedParticles = ParticlesManager.instance.Play(particlesOnShoot, mainBarrel.position, mainBarrel.rotation);
            SoundManager.instance.Play(audioOnShoot, mainBarrel.position);

            //set parent to vfx
            if (instantiatedGameObject)
                instantiatedGameObject.transform.SetParent(mainBarrel);
            if (instantiatedParticles)
                instantiatedParticles.transform.SetParent(mainBarrel);

            //camera shake
            if (cameraShake && CameraShake.instance)
            {
                //custom or default
                if (customShake)
                    CameraShake.instance.StartShake(shakeDuration, shakeAmount);
                else
                    CameraShake.instance.StartShake();
            }
        }

        void OnPressAttack()
        {
            //instantiate vfx and sfx
            instantiatedGameObjectOnPress = InstantiateGameObjectManager.instance.Play(gameObjectOnPress, barrelOnPress.position, barrelOnPress.rotation);
            instantiatedParticlesOnPress = ParticlesManager.instance.Play(particlesOnPress, barrelOnPress.position, barrelOnPress.rotation);
            instantiatedAudioOnPress = SoundManager.instance.Play(audioOnPress, barrelOnPress.position);

            //set parents
            if (instantiatedGameObjectOnPress)
                instantiatedGameObjectOnPress.transform.SetParent(barrelOnPress);
            if (instantiatedParticlesOnPress)
                instantiatedParticlesOnPress.transform.SetParent(barrelOnPress);
            if (instantiatedAudioOnPress)
                instantiatedAudioOnPress.transform.SetParent(barrelOnPress);
        }

        void OnReleaseAttack()
        {
            //destroy vfx and sfx instantiated on press
            if (instantiatedGameObjectOnPress)
                Pooling.Destroy(instantiatedGameObjectOnPress);
            if (instantiatedParticlesOnPress)
                Pooling.Destroy(instantiatedParticlesOnPress.gameObject);
            if (instantiatedAudioOnPress)
                Pooling.Destroy(instantiatedAudioOnPress.gameObject);
        }

        #endregion
    }
}