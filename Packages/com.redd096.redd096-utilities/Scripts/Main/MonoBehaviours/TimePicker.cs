using System.Collections.Generic;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using redd096.UIControl;

namespace redd096
{
    /// <summary>
    /// Get a text mesh pro Dropdown and makes it show hours to select
    /// </summary>
    [AddComponentMenu("redd096/Main/MonoBehaviours/Time Picker")]
    public class TimePicker : MonoBehaviour
    {
        [Header("Dropdown to use - else can instantiate objects in ScrollView")]
        [SerializeField] TMP_Dropdown dropdown = default;
        [SerializeField] bool instantiateInsteadOfUseDropdown = false;
        [SerializeField] GameObject prefab = default;
        [SerializeField] ScrollSnap scrollSnap = default;

        [Header("On Awake")]
        [Tooltip("On awake set DateTime.Now as selected date and default result, to generate default dropdown and have result to read without open dropdown")][SerializeField] bool setDefaultOnAwake = false;

        [Header("Hours to show in dropdown")]
        [SerializeField] string hoursFormat = "HH:mm";
        [Tooltip("Step between hours, for example: 08:00, 08:30, 09:00 and so on")][SerializeField] int minutesStep = 30;

        [Header("Not Clickable Hours")]
        [Tooltip("This only if selected date is today or earlier")][SerializeField] bool disableHoursBeforeNow = true;
        [Tooltip("If enabled, can for example disable hours if there isn't at least one hour of difference from now")][SerializeField] int additiveHoursAfterNow = 1;
        [Tooltip("Disable every hours before of this time")][SerializeField] string startHours = "08:00";
        [Tooltip("Disable every hours after of this time")][SerializeField] string endHours = "17:00";

        [Header("Result")]
        public string ResultStringFormat = "HH:mm";
        public string ResultString = "12:30";
        public DateTime Result = DateTime.Now;

        [Header("Events")]
        public UnityEvent<string> OnResultString = default;
        public UnityEvent<DateTime> OnResult = default;

        public TMP_Dropdown Dropdown => dropdown;
        public ScrollSnap Scroll => scrollSnap;
        public List<string> ShowedHours => showedHours;
        public List<DateTime> SavedHours => savedHours;

        DateTime date = DateTime.Today;                         //selected date (or date to use)
        DateTime now = DateTime.Now;                            //DateTime.Now + additiveHours
        DateTime start = DateTime.Now;                          //startHours
        DateTime end = DateTime.Now;                            //endHours

        DateTime selectedDate = DateTime.Now.Date;              //default date to use. Changed only when call SetSelectedDate
        List<string> showedHours = new List<string>();          //hours showed in dropdown (hours found with GetPossibleHoursForDropdown). These are changed only when call SetHoursInDropdown
        List<DateTime> savedHours = new List<DateTime>();       //hours showed in dropdown (hours found with GetPossibleHoursForDropdown) as DateTime. These are changed only when call SetHoursInDropdown

        private void Awake()
        {
            //checks
            if (instantiateInsteadOfUseDropdown == false && dropdown == null)
                Debug.LogError("Miss dropdown");
            else if (instantiateInsteadOfUseDropdown && (prefab == null || scrollSnap == null))
                Debug.LogError("Miss prefab to instantiate or scrollSnap");

            if (setDefaultOnAwake)
            {
                //set today as default date
                selectedDate = DateTime.Now.Date;

                //set today as default result
                Result = DateTime.Now;
            }

            //register to dropdown event
            if (dropdown)
                dropdown.onValueChanged.AddListener(OnClick);

            //generate default hours
            SetHoursInDropdown();
        }

        private void OnDestroy()
        {
            //unregister from dropdown event
            if (dropdown)
                dropdown.onValueChanged.RemoveListener(OnClick);
        }

        #region public API

        /// <summary>
        /// Used to update selected date by script
        /// </summary>
        /// <param name="date"></param>
        public void SetSelectedDate(DateTime date)
        {
            selectedDate = date;

            //update also hours in dropdown
            SetHoursInDropdown();
        }

        /// <summary>
        /// Check if with this date there are hours in dropdown
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public bool ThereAreHoursThisDate(DateTime date)
        {
            //update list and check number of elements in list
            GetPossibleHoursForDropdown(date, out List<DateTime> dtHours, out List<string> stringHours);
            return dtHours.Count > 0;
        }

        /// <summary>
        /// Check if with selected date, there is this hour in the list
        /// </summary>
        /// <param name="hour"></param>
        /// <returns></returns>
        public bool IsSelectableThisHour(DateTime hour)
        {
            //update list
            GetPossibleHoursForDropdown(selectedDate, out List<DateTime> dtHours, out List<string> stringHours);

            //and check if contains this hour in the list
            return dtHours.Contains(date.Add(hour.TimeOfDay));
        }

        /// <summary>
        /// Check if with this date, there is this hour in the list
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public bool IsSelectableThisDateTime(DateTime dt)
        {
            //update list
            GetPossibleHoursForDropdown(dt, out List<DateTime> dtHours, out List<string> stringHours);

            //and check if contains this hour in the list
            return dtHours.Contains(dt);
        }

        /// <summary>
        /// Update. Maybe it's been an hour and now the result is different, or someone called SetHour
        /// </summary>
        public void UpdateResult()
        {
            SetHoursInDropdown();
        }

        /// <summary>
        /// Set Result variable
        /// </summary>
        /// <param name="newHour"></param>
        public void SetHour(DateTime newHour)
        {
            Result = newHour;
            ResultString = Result.ToString(ResultStringFormat);
        }

        #endregion

        #region private API

        void SetHoursInDropdown()
        {
            GetPossibleHoursForDropdown(selectedDate, out savedHours, out showedHours);

            //update dropdown using selected date
            if (dropdown)
            {
                dropdown.ClearOptions();
                dropdown.AddOptions(showedHours);

                //set value
                int index = GetIndexStartTime();
                if (dropdown.value != index)
                    dropdown.value = index;         //set value
                else
                    OnClick(index);                 //if value isn't changed, call anyway to update result
            }
            //else instantiate objects in scrollview
            else if (instantiateInsteadOfUseDropdown)
            {
                //remove previous childs
                for (int i = scrollSnap.Content.childCount - 1; i >= 0; i--)
                {
                    Destroy(scrollSnap.Content.GetChild(i).gameObject);
                }

                //instantiate new ones
                for (int i = 0; i < showedHours.Count; i++)
                {
                    GameObject go = Instantiate(prefab, scrollSnap.Content);
                    go.GetComponentInChildren<TextMeshProUGUI>().text = showedHours[i];
                }

                //set value
                int index = GetIndexStartTime();
                scrollSnap.ScrollToItem(index); //TODO it's not updating because is turned off
                OnClick(index);
            }

        }

        int GetIndexStartTime()
        {
            DateTime currentTime = Result;
            DateTime nearestTime = start;
            int nearestIndex = 0;

            for (int i = 0; i < savedHours.Count; i++)
            {
                //if greater than nearest one, but not greater than current time (to show from last selected item - default is DateTime.Now)
                if (savedHours[i].TimeOfDay > nearestTime.TimeOfDay && savedHours[i].TimeOfDay <= currentTime.TimeOfDay)
                {
                    nearestTime = savedHours[i];
                    nearestIndex = i;
                }
            }

            return nearestIndex;
        }

        void GetPossibleHoursForDropdown(DateTime dateToUse, out List<DateTime> dtHours, out List<string> stringHours)
        {
            //parse inspector variables
            DateTime.TryParse(startHours, out start);
            DateTime.TryParse(endHours, out end);
            date = dateToUse.Date;                                                  //be sure date has only date and not TimeOfDay
            now = DateTime.Now.Add(new TimeSpan(additiveHoursAfterNow, 0, 0));      //now + additive hours

            //generate hours in dropdown
            dtHours = new List<DateTime>();
            stringHours = new List<string>();
            for (DateTime dt = date.Add(start.TimeOfDay); dt.TimeOfDay <= end.TimeOfDay; dt = dt.Add(new TimeSpan(0, minutesStep, 0)))
            {
                //only if enabled
                if (CanBeEnabledNow(dt) && IsInsideHoursRange(dt))
                {
                    dtHours.Add(dt);
                    stringHours.Add(dt.ToString(hoursFormat));
                }
            }
        }

        #endregion

        #region check to disable hours

        bool CanBeEnabledNow(DateTime dt)
        {
            //bool is disabled
            //or DateTime is greater than now + additive hours
            return disableHoursBeforeNow == false || dt > now;
        }

        bool IsInsideHoursRange(DateTime dt)
        {
            //check DateTime is between Start and End
            return dt.TimeOfDay >= start.TimeOfDay && dt.TimeOfDay <= end.TimeOfDay;
        }

        #endregion

        void OnClick(int index)
        {
            if (savedHours.Count > index)
            {
                //parse selected value
                DateTime dt = savedHours[index];
                SetHour(dt);

                //on select value, call event
                OnResult?.Invoke(Result);
                OnResultString?.Invoke(ResultString);
            }
        }
    }
}