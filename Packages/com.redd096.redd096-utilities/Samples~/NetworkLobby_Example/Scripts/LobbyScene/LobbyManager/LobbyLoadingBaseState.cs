using TMPro;
using UnityEngine;
using UnityEngine.UI;
using redd096.StateMachine;

namespace redd096.Examples.NetworkLobby
{
    /// <summary>
    /// Base loading animation
    /// </summary>
    public abstract class LobbyLoadingBaseState : IState<LobbyManagerSM>
    {
        [SerializeField] protected GameObject panel;
        [SerializeField] protected GameObject loadingObj;
        [SerializeField] protected GameObject errorObj;
        [Space]
        [SerializeField] protected TMP_Text loadingLabel;
        [SerializeField] protected Button backOnErrorButton;
        [SerializeField] protected TMP_Text errorLabel;

        //consts
        public const float LOADING_ANIMATION_DELAY = 0.5f;
        public const float MINIMUM_TIME_LOADING = 1f;

        public LobbyManagerSM StateMachine { get; set; }

        //example of defaultText: Loading<color=#0000>.</color><color=#0000>.</color><color=#0000>.</color>
        protected string defaultText;
        protected float animationTimer;
        protected float minTimer;

        public virtual void Enter()
        {
            //reset vars
            animationTimer = Time.time + LOADING_ANIMATION_DELAY;
            minTimer = Time.time + MINIMUM_TIME_LOADING;
            errorLabel.text = "";

            //register events
            backOnErrorButton.onClick.AddListener(OnClickBackOnError);

            //show loading
            ShowObj(showLoading: true);

            //show panel and save default text (with transparent points)
            panel.SetActive(true);
            defaultText = loadingLabel.text;
        }

        public virtual void UpdateState()
        {
            DoAnimation();

            //wait minimum time, then continue to call function
            if (Time.time > minTimer)
                UpdateAfterMinimumTime();
        }

        public virtual void Exit()
        {
            //unregister events
            backOnErrorButton.onClick.RemoveListener(OnClickBackOnError);

            //hide panel and reset text
            panel.SetActive(false);
            loadingLabel.text = defaultText;
        }

        protected void DoAnimation()
        {
            //animation: every few seconds add one point "." 
            //(by removing transparency to avoid resize text) 
            //and when reach last, restart
            if (Time.time > animationTimer)
            {
                animationTimer = Time.time + LOADING_ANIMATION_DELAY;

                if (loadingLabel.text.Contains("<color=#0000>"))
                {
                    //remove transparency
                    string t = loadingLabel.text;
                    t = t.Remove(t.IndexOf("<color=#0000>"), "<color=#0000>".Length);
                    t = t.Remove(t.IndexOf("</color>"), "</color>".Length);
                    loadingLabel.text = t;
                }
                else
                {
                    //if every dot is visible, return transparency to every dot
                    loadingLabel.text = defaultText;
                }
            }
        }

        /// <summary>
        /// Show LoadingObj or ErrorObj
        /// </summary>
        /// <param name="showLoading"></param>
        protected void ShowObj(bool showLoading)
        {
            loadingObj.SetActive(showLoading);
            errorObj.SetActive(showLoading == false);
        }

        protected abstract void UpdateAfterMinimumTime();

        protected abstract void OnClickBackOnError();
    }
}