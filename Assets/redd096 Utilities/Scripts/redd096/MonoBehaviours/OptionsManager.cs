using UnityEngine;
using UnityEngine.UI;
using redd096.Attributes;

namespace redd096
{
    [AddComponentMenu("redd096/MonoBehaviours/OptionsManager")]
    public class OptionsManager : MonoBehaviour
    {
        [Header("UI Volume")]
        [SerializeField] Slider volumeMasterSlider = default;
        [SerializeField] Text volumeMasterText = default;
        [SerializeField] Slider volumeMusicSlider = default;
        [SerializeField] Text volumeMusicText = default;
        [SerializeField] Slider volumeSFXSlider = default;
        [SerializeField] Text volumeSFXText = default;

        [Header("UI Graphic")]
        [SerializeField] Toggle fullScreenToggle = default;

        [Header("Default Values - set custom or take from bars and toggles in Editor")]
        [SerializeField] bool takeFromUI = true;
        [HideIf("takeFromUI")][SerializeField] float volumeMasterDefault = 1;
        [HideIf("takeFromUI")][SerializeField] float volumeMusicDefault = 1;
        [HideIf("takeFromUI")][SerializeField] float volumeSFXDefault = 1;
        [HideIf("takeFromUI")][SerializeField] bool fullScreenDefault = true;

        [Header("Optimization")]
        [SerializeField] bool saveOnDisable = true;

        private string fileName = "Options";

        float masterVolume;
        float musicVolume;
        float sfxVolume;
        bool fullScreenEnabled;

        void Start()
        {
            //load options, else use default values (taken from UI or default values in inspector)
            masterVolume = SaveManager.GetFloat(fileName, "masterVolume", takeFromUI && volumeMasterSlider ? volumeMasterSlider.value : volumeMasterDefault);
            musicVolume = SaveManager.GetFloat(fileName, "musicVolume", takeFromUI && volumeMusicSlider ? volumeMusicSlider.value : volumeMusicDefault);
            sfxVolume = SaveManager.GetFloat(fileName, "sfxVolume", takeFromUI && volumeSFXSlider ? volumeSFXSlider.value : volumeSFXDefault);
            fullScreenEnabled = SaveManager.GetBool(fileName, "fullScreenEnabled", takeFromUI && fullScreenToggle ? fullScreenToggle.isOn : fullScreenDefault);

            //update UI and set in game
            UpdateUI();
            SetInGame();
        }

        void OnEnable()
        {
            //add events
            if (volumeMasterSlider) volumeMasterSlider.onValueChanged.AddListener(OnSetVolumeMaster);
            if (volumeMusicSlider) volumeMusicSlider.onValueChanged.AddListener(OnSetVolumeMusic);
            if (volumeSFXSlider) volumeSFXSlider.onValueChanged.AddListener(OnSetVolumeSFX);

            if (fullScreenToggle) fullScreenToggle.onValueChanged.AddListener(OnSetFullScreen);
        }

        void OnDisable()
        {
            //remove events
            if (volumeMasterSlider) volumeMasterSlider.onValueChanged.RemoveListener(OnSetVolumeMaster);
            if (volumeMusicSlider) volumeMusicSlider.onValueChanged.RemoveListener(OnSetVolumeMusic);
            if (volumeSFXSlider) volumeSFXSlider.onValueChanged.RemoveListener(OnSetVolumeSFX);

            if (fullScreenToggle) fullScreenToggle.onValueChanged.RemoveListener(OnSetFullScreen);

            //save on disk On Disable
            if (saveOnDisable)
                SaveManager.Save();
        }

        #region private API

        void SetInGame()
        {
            //set volumes
            AudioListener.volume = masterVolume;
            if (SoundManager.instance) SoundManager.instance.SetVolumeMusic(musicVolume);
            if (SoundManager.instance) SoundManager.instance.SetVolumeSFX(sfxVolume);

            //set full screen
            Screen.fullScreen = fullScreenEnabled;
        }

        void UpdateUI()
        {
            //audio slider and text
            if (volumeMasterSlider) volumeMasterSlider.SetValueWithoutNotify(masterVolume);
            if (volumeMasterText) volumeMasterText.text = (masterVolume * 100).ToString("F0") + "%";
            if (volumeMusicSlider) volumeMusicSlider.SetValueWithoutNotify(musicVolume);
            if (volumeMusicText) volumeMusicText.text = (musicVolume * 100).ToString("F0") + "%";
            if (volumeSFXSlider) volumeSFXSlider.SetValueWithoutNotify(sfxVolume);
            if (volumeSFXText) volumeSFXText.text = (sfxVolume * 100).ToString("F0") + "%";

            //toggle
            if (fullScreenToggle) fullScreenToggle.SetIsOnWithoutNotify(fullScreenEnabled);
        }

        #endregion

        #region UI events

        public void OnSetVolumeMaster(float value)
        {
            //save
            masterVolume = value;
            SaveManager.SetFloat(fileName, "masterVolume", value, saveOnDisable == false);

            //update UI and set in game
            UpdateUI();
            SetInGame();
        }

        public void OnSetVolumeMusic(float value)
        {
            //save
            musicVolume = value;
            SaveManager.SetFloat(fileName, "musicVolume", value, saveOnDisable == false);

            //update UI and set in game
            UpdateUI();
            SetInGame();
        }

        public void OnSetVolumeSFX(float value)
        {
            //save
            sfxVolume = value;
            SaveManager.SetFloat(fileName, "sfxVolume", value, saveOnDisable == false);

            //update UI and set in game
            UpdateUI();
            SetInGame();
        }

        public void OnSetFullScreen(bool value)
        {
            //save
            fullScreenEnabled = value;
            SaveManager.SetBool(fileName, "fullScreenEnabled", value, saveOnDisable == false);

            //update UI and set in game
            UpdateUI();
            SetInGame();
        }

        #endregion
    }
}