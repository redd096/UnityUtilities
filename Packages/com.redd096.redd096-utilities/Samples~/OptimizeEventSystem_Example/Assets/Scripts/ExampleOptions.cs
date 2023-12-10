using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace redd096.ExampleOptimizeEventSystem
{
    public class ExampleOptions : MonoBehaviour
    {
        [SerializeField] Slider masterVolumeSlider = default;
        [SerializeField] Slider musicVolumeSlider = default;
        [SerializeField] Slider sfxVolumeSlider = default;
        [Space]
        [SerializeField] TMP_Text masterVolumeText = default;
        [SerializeField] TMP_Text musicVolumeText = default;
        [SerializeField] TMP_Text sfxVolumeText = default;

        private void Start()
        {
            masterVolumeSlider.onValueChanged.AddListener(UpdateMasterVolume);
            musicVolumeSlider.onValueChanged.AddListener(UpdateMusicVolume);
            sfxVolumeSlider.onValueChanged.AddListener(UpdateSFXVolume);
        }

        public void UpdateMasterVolume(float value)
        {
            masterVolumeText.text = (value * 100).ToString("F0") + "%";
        }

        public void UpdateMusicVolume(float value)
        {
            musicVolumeText.text = (value * 100).ToString("F0") + "%";
        }

        public void UpdateSFXVolume(float value)
        {
            sfxVolumeText.text = (value * 100).ToString("F0") + "%";
        }
    }
}