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

        float valueVolumeMaster;
        float valueVolumeMusic;
        float valueVolumeSFX;
        bool valueFullScreen;

        void Start()
        {
            //load options, else use default values (taken from UI or default values in inspector)
            valueVolumeMaster = SaveManager.GetFloat(fileName, "masterVolume", takeFromUI && volumeMasterSlider ? volumeMasterSlider.value : volumeMasterDefault);
            valueVolumeMusic = SaveManager.GetFloat(fileName, "musicVolume", takeFromUI && volumeMusicSlider ? volumeMusicSlider.value : volumeMusicDefault);
            valueVolumeSFX = SaveManager.GetFloat(fileName, "sfxVolume", takeFromUI && volumeSFXSlider ? volumeSFXSlider.value : volumeSFXDefault);
            valueFullScreen = SaveManager.GetBool(fileName, "fullScreen", takeFromUI && fullScreenToggle ? fullScreenToggle.isOn : fullScreenDefault);

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
                SaveManager.Save(fileName);
        }

        #region private API

        void SetInGame()
        {
            //set volumes
            AudioListener.volume = valueVolumeMaster;
            if (SoundManager.instance) SoundManager.instance.SetVolumeMusic(valueVolumeMusic);
            if (SoundManager.instance) SoundManager.instance.SetVolumeSFX(valueVolumeSFX);

            //set full screen
            Screen.fullScreen = valueFullScreen;
        }

        void UpdateUI()
        {
            //audio slider and text
            if (volumeMasterSlider) volumeMasterSlider.SetValueWithoutNotify(valueVolumeMaster);
            if (volumeMasterText) volumeMasterText.text = (valueVolumeMaster * 100).ToString("F0") + "%";
            if (volumeMusicSlider) volumeMusicSlider.SetValueWithoutNotify(valueVolumeMusic);
            if (volumeMusicText) volumeMusicText.text = (valueVolumeMusic * 100).ToString("F0") + "%";
            if (volumeSFXSlider) volumeSFXSlider.SetValueWithoutNotify(valueVolumeSFX);
            if (volumeSFXText) volumeSFXText.text = (valueVolumeSFX * 100).ToString("F0") + "%";

            //toggle
            if (fullScreenToggle) fullScreenToggle.SetIsOnWithoutNotify(valueFullScreen);
        }

        #endregion

        #region UI events

        public void OnSetVolumeMaster(float value)
        {
            //save
            valueVolumeMaster = value;
            SaveManager.SetFloat(fileName, "masterVolume", value, saveOnDisable == false);

            //update UI and set in game
            UpdateUI();
            SetInGame();
        }

        public void OnSetVolumeMusic(float value)
        {
            //save
            valueVolumeMusic = value;
            SaveManager.SetFloat(fileName, "musicVolume", value, saveOnDisable == false);

            //update UI and set in game
            UpdateUI();
            SetInGame();
        }

        public void OnSetVolumeSFX(float value)
        {
            //save
            valueVolumeSFX = value;
            SaveManager.SetFloat(fileName, "sfxVolume", value, saveOnDisable == false);

            //update UI and set in game
            UpdateUI();
            SetInGame();
        }

        public void OnSetFullScreen(bool value)
        {
            //save
            valueFullScreen = value;
            SaveManager.SetBool(fileName, "fullScreen", value, saveOnDisable == false);

            //update UI and set in game
            UpdateUI();
            SetInGame();
        }

        #endregion
    }
}