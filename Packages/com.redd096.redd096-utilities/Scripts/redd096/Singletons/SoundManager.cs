using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace redd096
{
    [AddComponentMenu("redd096/Singletons/Sound Manager")]
    public class SoundManager : Singleton<SoundManager>
    {
        //we don't need AudioSource prefab and we don't need different prefab for different situations, but we can instantiate a single default audio source.
        //we create the functions for AudioData, where everything in the audio source (2d, 3d, etc..) is setted by AudioData
        //and we create functions where user can pass AudioClip instead of AudioData, but must set also other variables, like in MoreMountains with infinite optional variables
        
        //we have to edit FeedbackRedd096 probably
        //and also OptionsManager have calls on OldSoundManager
        //we can edit also ParticlesManager and InstantiateGameObjectManager ??

        //we have to create an AudioClass to set AudioData and Element in inspector

        public void UpdateAudioSourcesVolume()
        {

        }
    }
}