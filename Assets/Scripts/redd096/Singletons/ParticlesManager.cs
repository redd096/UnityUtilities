﻿namespace redd096
{
    using System.Collections.Generic;
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
        Dictionary<ParticleSystem, Pooling<ParticleSystem>> poolingParticles = new Dictionary<ParticleSystem, Pooling<ParticleSystem>>();

        /// <summary>
        /// Start particles at point and rotation. Use specific pooling
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

        /// <summary>
        /// Start particles at point and rotation
        /// </summary>
        public void Play(ParticleSystem prefab, Vector3 position, Quaternion rotation)
        {
            if (prefab == null)
                return;

            //if this pooling is not in the dictionary, add it
            if (poolingParticles.ContainsKey(prefab) == false)
                poolingParticles.Add(prefab, new Pooling<ParticleSystem>());

            //use this manager's pooling, instead of a specific one
            Play(poolingParticles[prefab], prefab, position, rotation);
        }

        /// <summary>
        /// Start particles at point and rotation. Get one random from the array
        /// </summary>
        public void Play(ParticleSystem[] prefabs, Vector3 position, Quaternion rotation)
        {
            //do only if there are elements in the array
            if (prefabs.Length > 0)
            {
                Play(prefabs[Random.Range(0, prefabs.Length)], position, rotation);
            }
        }
    }
}