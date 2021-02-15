namespace redd096
{
    using UnityEngine;

    [AddComponentMenu("redd096/Singletons/Particles Manager")]
    public class ParticlesManager : Singleton<ParticlesManager>
    {
        Transform particlesParent;
        Transform ParticlesParent
        {
            get
            {
                if (particlesParent == null)
                    particlesParent = new GameObject("Particles Parent").transform;

                return particlesParent;
            }
        }

        /// <summary>
        /// Start particles at point and rotation
        /// </summary>
        public void Play(Pooling<ParticleSystem> pool, ParticleSystem prefab, Vector3 position, Quaternion rotation)
        {
            if (prefab == null)
                return;

            //instantiate at position and rotation, set parent
            ParticleSystem particles = pool.Instantiate(prefab, position, rotation);
            particles.transform.SetParent(ParticlesParent);

            //play
            particles.Play();
        }
    }
}