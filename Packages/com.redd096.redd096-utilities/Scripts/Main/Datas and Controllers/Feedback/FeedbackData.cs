using redd096.Attributes;
using System.Collections.Generic;
using UnityEngine;

namespace redd096
{
    /// <summary>
    /// Scriptable object used to set audios, particles and so on
    /// </summary>
    [CreateAssetMenu(fileName = "FeedbackData", menuName = "redd096/Datas/Feedback Data")]
    public class FeedbackData : ScriptableObject
    {
        public Element[] Elements;

        private void OnValidate()
        {
            //if change volume in inspector, update in game
            if (Application.isPlaying)
            {
                if (SoundManager.instance)
                    SoundManager.instance.UpdateAudioSourcesVolume();
            }
        }

        /// <summary>
        /// Get element in array by name
        /// </summary>
        /// <param name="elementName"></param>
        /// <param name="showErrors"></param>
        /// <returns></returns>
        public Element GetElement(string elementName, bool showErrors = true)
        {
            if (Elements != null)
            {
                foreach (var element in Elements)
                    if (element.Name == elementName)
                        return element;
            }

            if (showErrors) Debug.LogError("Impossible to find: " + elementName);
            return null;
        }

        #region element classes

        [System.Serializable]
        public class Element
        {
            public string Name;
            [Tooltip("Use array to instantiate every single element, or just one picked random from it")] public bool InstantiateOneRandom;
            [Tooltip("When instantiated, set child of the object which instantiate it?")] public bool SetParent;
            public InstantiatedGameObjectStruct[] GameObjectsFeedback;
            public ParticleSystem[] ParticlesFeedback;
            public AudioData.Element[] AudiosFeedback;

            //updated every time call InstantiateFeedback
            List<GameObject> instantiatedGameObjects;
            List<ParticleSystem> instantiatedParticles;
            List<AudioSource> instantiatedAudios;

            /// <summary>
            /// Instantiate feedback and return first instantiated element for every list
            /// </summary>
            public void InstantiateFeedback(out GameObject outInstantiatedGameObject, out ParticleSystem outInstantiatedParticle, out AudioSource outInstantiatedAudio, Transform parent, bool worldPositionStays = true)
            {
                //instantiate
                InstantiateFeedback(parent, worldPositionStays);

                //out first vars
                outInstantiatedGameObject = instantiatedGameObjects.Count > 0 ? instantiatedGameObjects[0] : null;
                outInstantiatedParticle = instantiatedParticles.Count > 0 ? instantiatedParticles[0] : null;
                outInstantiatedAudio = instantiatedAudios.Count > 0 ? instantiatedAudios[0] : null;
            }

            /// <summary>
            /// Instantiate feedback and return every instantiated element
            /// </summary>
            public void InstantiateFeedback(out GameObject[] outInstantiatedGameObjects, out ParticleSystem[] outInstantiatedParticles, out AudioSource[] outInstantiatedAudios, Transform parent, bool worldPositionStays = true)
            {
                //instantiate
                InstantiateFeedback(parent, worldPositionStays);

                //out array vars
                outInstantiatedGameObjects = instantiatedGameObjects.ToArray();
                outInstantiatedParticles = instantiatedParticles.ToArray();
                outInstantiatedAudios = instantiatedAudios.ToArray();
            }

            /// <summary>
            /// Instantiate feedback and set position and rotation for every instantiated element
            /// </summary>
            public void InstantiateFeedback(Vector3 position, Quaternion rotation, Transform parent, bool worldPositionStays = true)
            {
                //instantiate
                InstantiateFeedback(parent, worldPositionStays);

                //set custom position and rotation
                instantiatedGameObjects.ForEach(x => { x.transform.position = position; x.transform.rotation = rotation; });
                instantiatedParticles.ForEach(x => { x.transform.position = position; x.transform.rotation = rotation; });
                instantiatedAudios.ForEach(x => { x.transform.position = position; x.transform.rotation = rotation; });
            }

            /// <summary>
            /// Instantiate feedback
            /// </summary>
            public void InstantiateFeedback(Transform parent, bool worldPositionStays = true)
            {
                //be sure lists are created
                if (instantiatedGameObjects == null) instantiatedGameObjects = new List<GameObject>();
                if (instantiatedParticles == null) instantiatedParticles = new List<ParticleSystem>();
                if (instantiatedAudios == null) instantiatedAudios = new List<AudioSource>();

                //clear lists
                instantiatedGameObjects.Clear();
                instantiatedParticles.Clear();
                instantiatedAudios.Clear();

                //instantiate single vfx and sfx from array
                if (InstantiateOneRandom)
                {
                    instantiatedGameObjects.Add(InstantiateGameObjectManager.instance.Play(GameObjectsFeedback, parent.position, parent.rotation));
                    instantiatedParticles.Add(ParticlesManager.instance.Play(ParticlesFeedback, parent.position, parent.rotation));
                    instantiatedAudios.Add(SoundManager.instance.Play(new AudioClass(AudiosFeedback[Random.Range(0, AudiosFeedback.Length)]), parent.position));
                }
                //else, instantiate every element from array
                else
                {
                    foreach (InstantiatedGameObjectStruct gameObjectStruct in GameObjectsFeedback)
                        instantiatedGameObjects.Add(InstantiateGameObjectManager.instance.Play(gameObjectStruct, parent.position, parent.rotation));

                    foreach (ParticleSystem particleSystem in ParticlesFeedback)
                        instantiatedParticles.Add(ParticlesManager.instance.Play(particleSystem, parent.position, parent.rotation));

                    foreach (AudioData.Element audio in AudiosFeedback)
                        instantiatedAudios.Add(SoundManager.instance.Play(new AudioClass(audio), parent.position));
                }

                //set parent if necessary
                if (SetParent)
                {
                    foreach (GameObject instantiated in instantiatedGameObjects)
                        if (instantiated)
                            instantiated.transform.SetParent(parent, worldPositionStays);

                    foreach (ParticleSystem instantiated in instantiatedParticles)
                        if (instantiated)
                            instantiated.transform.SetParent(parent, worldPositionStays);

                    foreach (AudioSource instantiated in instantiatedAudios)
                        if (instantiated)
                            instantiated.transform.SetParent(parent, worldPositionStays);
                }
            }
        }

        #endregion
    }
}