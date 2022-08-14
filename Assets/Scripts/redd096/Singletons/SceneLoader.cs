﻿using UnityEngine;
using UnityEngine.SceneManagement;
using redd096.Attributes;

namespace redd096
{
    [AddComponentMenu("redd096/Singletons/Scene Loader")]
    public class SceneLoader : Singleton<SceneLoader>
    {
        public bool ChangeCursorLockMode = true;
        [EnableIf("ChangeCursorLockMode")] public CursorLockMode LockModeOnResume = CursorLockMode.Confined;

        /// <summary>
        /// Resume time and hide cursor
        /// </summary>
        public void ResumeGame()
        {
            //hide pause menu
            if (GameManager.instance && GameManager.instance.uiManager)
                GameManager.instance.uiManager.PauseMenu(false);

            //set timeScale to 1
            Time.timeScale = 1;

            //enable player input and hide cursor
            if (ChangeCursorLockMode) LockMouse(LockModeOnResume);
        }

        /// <summary>
        /// Pause time and show cursor
        /// </summary>
        public void PauseGame()
        {
            //show pause menu
            if (GameManager.instance && GameManager.instance.uiManager)
                GameManager.instance.uiManager.PauseMenu(true);

            //stop time
            Time.timeScale = 0;

            //disable player input and show cursor
            if (ChangeCursorLockMode) LockMouse(CursorLockMode.None);
        }

        /// <summary>
        /// Exit game (works also in editor)
        /// </summary>
        public void ExitGame()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }

        /// <summary>
        /// Reload this scene
        /// </summary>
        public void ReloadScene()
        {
            //show cursor and set timeScale to 1
            if (ChangeCursorLockMode) LockMouse(CursorLockMode.None);
            Time.timeScale = 1;

            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        /// <summary>
        /// Load scene by name
        /// </summary>
        public void LoadScene(string scene)
        {
            //show cursor and set timeScale to 1
            if (ChangeCursorLockMode) LockMouse(CursorLockMode.None);
            Time.timeScale = 1;

            //load new scene
            SceneManager.LoadScene(scene);
        }

        /// <summary>
        /// Load next scene in build settings
        /// </summary>
        public void LoadNextScene()
        {
            //show cursor and set timeScale to 1
            if (ChangeCursorLockMode) LockMouse(CursorLockMode.None);
            Time.timeScale = 1;

            //load next scene
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        }

        /// <summary>
        /// Open url
        /// </summary>
        public void OpenURL(string url)
        {
            Application.OpenURL(url);
        }

        /// <summary>
        /// Set lockState, and visible only when not locked
        /// </summary>
        public void LockMouse(CursorLockMode lockMode)
        {
            Cursor.lockState = lockMode;
            Cursor.visible = lockMode == CursorLockMode.None;
        }
    }
}