namespace redd096
{
    using UnityEngine;

    public static class Particles
    {

        #region private API

        static ParticleSystem SpawnParticles(ParticleSystem particleEffect, Vector3 position, Quaternion rotation)
        {
            //return null if no particle
            if (particleEffect == null)
                return null;

            //instantiate
            ParticleSystem instantiated = Object.Instantiate(particleEffect);

            instantiated.transform.position = position;
            instantiated.transform.rotation = rotation;

            //return particle instantiated
            return instantiated;
        }

        static ParticleSystem SpawnParticles(ParticleSystem particleEffect, Vector3 position, Quaternion rotation, float delayBeforeDestroy)
        {
            //instantiate
            ParticleSystem instantiated = SpawnParticles(particleEffect, position, rotation);

            //destroy after few seconds
            if (instantiated != null)
                Object.Destroy(instantiated.gameObject, delayBeforeDestroy);

            //return particle instantiated
            return instantiated;
        }

        #endregion

        /// <summary>
        /// Instantiate particle effect in position and rotation
        /// </summary>
        public static ParticleSystem Instantiate(ParticleSystem particleEffect, Vector3 position, Quaternion rotation)
        {
            return SpawnParticles(particleEffect, position, rotation);
        }

        /// <summary>
        /// Instantiate particle effect in position and rotation. 
        /// Destroy particle after delay
        /// </summary>
        public static ParticleSystem Instantiate(ParticleSystem particleEffect, Vector3 position, Quaternion rotation, float delayBeforeDestroy)
        {
            return SpawnParticles(particleEffect, position, rotation, delayBeforeDestroy);
        }

        /// <summary>
        /// Instantiate particle effect in position and looking at direction
        /// </summary>
        public static ParticleSystem Instantiate(ParticleSystem particleEffect, Vector3 position, Vector3 direction)
        {
            //rotation = look at direction
            Quaternion rotation = Quaternion.LookRotation(direction);

            return SpawnParticles(particleEffect, position, rotation);
        }

        /// <summary>
        /// Instantiate particle effect in position and looking at direction. 
        /// Destroy particle after delay
        /// </summary>
        public static ParticleSystem Instantiate(ParticleSystem particleEffect, Vector3 position, Vector3 direction, float delayBeforeDestroy)
        {
            //rotation = look at direction
            Quaternion rotation = Quaternion.LookRotation(direction);

            return SpawnParticles(particleEffect, position, rotation, delayBeforeDestroy);
        }

        /// <summary>
        /// Instantiate particle effect in position and looking at direction. 
        /// upDirection is the up vector when look at direction
        /// </summary>
        public static ParticleSystem Instantiate(ParticleSystem particleEffect, Vector3 position, Vector3 direction, Vector3 upDirection)
        {
            //rotation = look at direction
            Quaternion rotation = Quaternion.LookRotation(direction, upDirection);

            return SpawnParticles(particleEffect, position, rotation);
        }

        /// <summary>
        /// Instantiate particle effect in position and looking at direction. 
        /// upDirection is the up vector when look at direction. 
        /// Destroy particle after delay
        /// </summary>
        public static ParticleSystem Instantiate(ParticleSystem particleEffect, Vector3 position, Vector3 direction, Vector3 upDirection, float delayBeforeDestroy)
        {
            //rotation = look at direction
            Quaternion rotation = Quaternion.LookRotation(direction, upDirection);

            return SpawnParticles(particleEffect, position, rotation, delayBeforeDestroy);
        }
    }
}