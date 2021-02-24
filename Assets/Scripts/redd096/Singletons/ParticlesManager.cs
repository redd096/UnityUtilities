namespace redd096
{
    using UnityEngine;

    [AddComponentMenu("redd096/Singletons/Particles Manager")]
    public class ParticlesManager : Singleton<ParticlesManager>
    {
        private Transform particlesParent;
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

            //instantiate (if didn't find deactivated, take first one in the pool)
            ParticleSystem particles = pool.Instantiate(prefab);
            if (particles == null && pool.PooledObjects.Count > 0)
                particles = pool.PooledObjects[0];

            //if still null, return
            if (particles == null)
                return;

            //set position, rotation and parent
            particles.transform.position = position;
            particles.transform.rotation = rotation;
            particles.transform.SetParent(ParticlesParent);

            //play
            particles.Play();
        }
    }
}