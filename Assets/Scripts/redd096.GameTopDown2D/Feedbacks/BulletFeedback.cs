using UnityEngine;
using redd096.Attributes;

namespace redd096.GameTopDown2D
{
    [AddComponentMenu("redd096/.GameTopDown2D/Feedbacks/Bullet Feedback")]
    public class BulletFeedback : MonoBehaviour
    {
        [Header("Necessary Components - default get in parent")]
        [SerializeField] Bullet bullet;

        [Header("On Hit (also if move through this object)")]
        [SerializeField] InstantiatedGameObjectStruct gameObjectOnHit = default;
        [SerializeField] ParticleSystem particlesOnHit = default;
        [SerializeField] AudioClass audioOnHit = default;

        [Header("On Hit something that destroy bullet")]
        [SerializeField] InstantiatedGameObjectStruct gameObjectOnLastHit = default;
        [SerializeField] ParticleSystem particlesOnLastHit = default;
        [SerializeField] AudioClass audioOnLastHit = default;

        [Header("On AutoDestruction")]
        [SerializeField] InstantiatedGameObjectStruct gameObjectOnAutodestruction = default;
        [SerializeField] ParticleSystem particlesOnAutodestruction = default;
        [SerializeField] AudioClass audioOnAutodestruction = default;

        [Header("On Destroy (both autodestruction or hit)")]
        [SerializeField] InstantiatedGameObjectStruct gameObjectOnDestroy = default;
        [SerializeField] ParticleSystem particlesOnDestroy = default;
        [SerializeField] AudioClass audioOnDestroy = default;

        [Header("Camera Shake")]
        [SerializeField] bool cameraShakeOnHit = false;
        [SerializeField] bool cameraShakeOnHitSomethingThatDestroyBullet = false;
        [SerializeField] bool cameraShakeOnAutoDestruction = false;
        [SerializeField] bool cameraShakeOnDestroy = false;
        [EnableIf(EnableIfAttribute.EConditionOperator.OR, "cameraShakeOnHit", "cameraShakeOnHitSomethingThatDestroyBullet", "cameraShakeOnAutoDestruction", "cameraShakeOnDestroy")][SerializeField] bool customShake = false;
        [EnableIf(EnableIfAttribute.EConditionOperator.OR, "cameraShakeOnHit", "cameraShakeOnHitSomethingThatDestroyBullet", "cameraShakeOnAutoDestruction", "cameraShakeOnDestroy")][SerializeField] float shakeDuration = 1;
        [EnableIf(EnableIfAttribute.EConditionOperator.OR, "cameraShakeOnHit", "cameraShakeOnHitSomethingThatDestroyBullet", "cameraShakeOnAutoDestruction", "cameraShakeOnDestroy")][SerializeField] float shakeAmount = 0.7f;

        TrailRenderer trail;

        void OnEnable()
        {
            //get references
            if (bullet == null)
                bullet = GetComponentInParent<Bullet>();

            if (trail == null)
                trail = GetComponentInChildren<TrailRenderer>();

            //add events
            if (bullet)
            {
                bullet.onInit += OnInit;
                bullet.onHit += OnHit;
                bullet.onLastHit += OnLastHit;
                bullet.onAutodestruction += OnAutodestruction;
                bullet.onDie += OnDie;
            }
        }

        void OnDisable()
        {
            //remove events
            if (bullet)
            {
                bullet.onInit -= OnInit;
                bullet.onHit -= OnHit;
                bullet.onLastHit -= OnLastHit;
                bullet.onAutodestruction -= OnAutodestruction;
                bullet.onDie -= OnDie;
            }
        }

        private void OnInit()
        {
            //reset trail
            if (trail)
                trail.Clear();
        }

        void OnHit(GameObject hit)
        {
            //instantiate vfx and sfx
            InstantiateGameObjectManager.instance.Play(gameObjectOnHit, transform.position, transform.rotation);
            ParticlesManager.instance.Play(particlesOnHit, transform.position, transform.rotation);
            SoundManager.instance.Play(audioOnHit, transform.position);

            //camera shake
            if (cameraShakeOnHit && CameraShake.instance)
            {
                //custom or default
                if (customShake)
                    CameraShake.instance.StartShake(shakeDuration, shakeAmount);
                else
                    CameraShake.instance.StartShake();
            }
        }

        void OnLastHit(GameObject hit)
        {
            //instantiate vfx and sfx
            InstantiateGameObjectManager.instance.Play(gameObjectOnLastHit, transform.position, transform.rotation);
            ParticlesManager.instance.Play(particlesOnLastHit, transform.position, transform.rotation);
            SoundManager.instance.Play(audioOnLastHit, transform.position);

            //camera shake
            if (cameraShakeOnHitSomethingThatDestroyBullet && CameraShake.instance)
            {
                //custom or default
                if (customShake)
                    CameraShake.instance.StartShake(shakeDuration, shakeAmount);
                else
                    CameraShake.instance.StartShake();
            }
        }

        void OnAutodestruction()
        {
            //instantiate vfx and sfx
            InstantiateGameObjectManager.instance.Play(gameObjectOnAutodestruction, transform.position, transform.rotation);
            ParticlesManager.instance.Play(particlesOnAutodestruction, transform.position, transform.rotation);
            SoundManager.instance.Play(audioOnAutodestruction, transform.position);

            //camera shake
            if (cameraShakeOnAutoDestruction && CameraShake.instance)
            {
                //custom or default
                if (customShake)
                    CameraShake.instance.StartShake(shakeDuration, shakeAmount);
                else
                    CameraShake.instance.StartShake();
            }
        }

        void OnDie(Bullet bullet)
        {
            //instantiate vfx and sfx
            InstantiateGameObjectManager.instance.Play(gameObjectOnDestroy, transform.position, transform.rotation);
            ParticlesManager.instance.Play(particlesOnDestroy, transform.position, transform.rotation);
            SoundManager.instance.Play(audioOnDestroy, transform.position);

            //camera shake
            if (cameraShakeOnDestroy && CameraShake.instance)
            {
                //custom or default
                if (customShake)
                    CameraShake.instance.StartShake(shakeDuration, shakeAmount);
                else
                    CameraShake.instance.StartShake();
            }
        }
    }
}