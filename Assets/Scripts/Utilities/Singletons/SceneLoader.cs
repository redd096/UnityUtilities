namespace redd096
{
    using UnityEngine;
    using UnityEngine.SceneManagement;

    [AddComponentMenu("redd096/Singletons/Scene Loader")]
    public class SceneLoader : Singleton<SceneLoader>
    {
        /// <summary>
        /// Resume time and hide cursor
        /// </summary>
        public void ResumeGame()
        {
            //hide pause menu and enable player input
            GameManager.instance.uiManager.PauseMenu(false);
            GameManager.instance.player.enabled = true;

            //hide cursor and set timeScale to 1
            Utility.LockMouse(CursorLockMode.Locked);
            Time.timeScale = 1;
        }

        /// <summary>
        /// Pause time and show cursor
        /// </summary>
        public void PauseGame()
        {
            //show pause menu and disable player input
            GameManager.instance.uiManager.PauseMenu(true);
            GameManager.instance.player.enabled = false;

            //show cursor and stop time
            Utility.LockMouse(CursorLockMode.None);
            Time.timeScale = 0;
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
        public void RestartGame()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        /// <summary>
        /// Load new scene by name
        /// </summary>
        public void LoadNewScene(string scene)
        {
            //show cursor and set timeScale to 1
            Utility.LockMouse(CursorLockMode.None);
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
            Utility.LockMouse(CursorLockMode.None);
            Time.timeScale = 1;

            //load next scene
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        }
    }
}