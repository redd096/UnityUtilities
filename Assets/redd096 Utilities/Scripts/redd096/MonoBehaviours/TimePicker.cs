using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;
using UnityEngine.Events;

namespace redd096
{
    /// <summary>
    /// Get a text mesh pro Dropdown and makes it show hours to select
    /// </summary>
    [AddComponentMenu("redd096/MonoBehaviours/Time Picker")]
    public class TimePicker : MonoBehaviour
    {
        [Header("Dropdown to use")]
        [SerializeField] TMP_Dropdown dropdown = default;
        [Tooltip("On awake set DateTime.Now as selected date and default result, to generate default dropdown and have result to read without open dropdown")][SerializeField] bool setDefaultOnAwake = true;

        [Header("Hours to show in dropdown")]
        [SerializeField] string hoursFormat = "HH:mm";
        [Tooltip("Step between hours, for example: 08:00, 08:30, 09:00 and so on")][SerializeField] int minutesStep = 30;

        [Header("Selected Date, to know if disable hours")]
        [SerializeField] string selectedDate = "16/12/2022";

        [Header("Not Clickable Hours")]
        [Tooltip("This only if selected date is today or earlier")][SerializeField] bool disableHoursBeforeNow = true;
        [Tooltip("If enabled, can for example disable hours if there isn't at least one hour of difference from now")][SerializeField] int additiveHoursAfterNow = 1;
        [Tooltip("Disable every hours before of this time")][SerializeField] string startHours = "08:00";
        [Tooltip("Disable every hours after of this time")][SerializeField] string endHours = "17:00";

        [Header("Result")]
        [SerializeField] string resultFormat = "HH:mm";
        [SerializeField] string result = "12:30";
        [SerializeField] UnityEvent<string> onResult = default;

        DateTime date;                                          //selected date (or date to use)
        DateTime now;                                           //DateTime.Now + additiveHours
        DateTime start;                                         //startHours
        DateTime end;                                           //endHours
        List<DateTime> hoursInDropdown = new List<DateTime>();  //hours showed in dropdown (hours found with GetPossibleHoursForDropdown)

        private void Awake()
        {
            if (setDefaultOnAwake)
            {
                //set today as default date
                selectedDate = DateTime.Now.ToString("dd/MM/yyyy");

                //set today as default result
                result = DateTime.Now.ToString(resultFormat);
            }

            //register to dropdown event
            dropdown.onValueChanged.AddListener(OnClick);

            //generate default hours
            SetHoursInDropdown(true);
        }

        private void OnDestroy()
        {
            //unregister from dropdown event
            if (dropdown)
                dropdown.onValueChanged.RemoveListener(OnClick);
        }

        /// <summary>
        /// Used to update selected date by script
        /// </summary>
        /// <param name="date"></param>
        public void SetSelectedDate(string date)
        {
            selectedDate = date;

            //update also hours in dropdown
            SetHoursInDropdown(true);
        }

        /// <summary>
        /// Check if with this date there are hours in dropdown
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public bool ThereAreHoursThisDate(DateTime date)
        {
            //update list and check number of elements in list
            return GetPossibleHoursForDropdown(date.ToString("dd/MM/yyyy")).Count > 0;
        }

        /// <summary>
        /// Check if with selected date, there is this hour in the list
        /// </summary>
        /// <param name="hour"></param>
        /// <returns></returns>
        public bool IsSelectableThisHour(DateTime hour)
        {
            //update list
            GetPossibleHoursForDropdown(selectedDate);

            //and check if contains this hour in the list
            return hoursInDropdown.Contains(date.Add(hour.TimeOfDay));
        }

        /// <summary>
        /// Check if with this date, there is this hour in the list
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public bool IsSelectableThisDateTime(DateTime dt)
        {
            //update list
            GetPossibleHoursForDropdown(dt.ToString("dd/MM/yyyy"));

            //and check if contains this hour in the list
            return hoursInDropdown.Contains(dt);
        }

        /// <summary>
        /// Used to force result. For example to set a default value before open dropdown (to get from a variable instead of set DateTime.Now in awake)
        /// </summary>
        public void SetResult(string result)
        {
            this.result = result;

            //update also hours in dropdown
            SetHoursInDropdown(false);
        }

        #region private API

        void SetHoursInDropdown(bool notify)
        {
            //update dropdown using selected date
            dropdown.ClearOptions();
            dropdown.AddOptions(GetPossibleHoursForDropdown(selectedDate));

            if (notify)
                dropdown.value = GetIndexStartTime();
            else
                dropdown.SetValueWithoutNotify(GetIndexStartTime());
        }

        int GetIndexStartTime()
        {
            DateTime currentTime = DateTime.Parse(result);
            DateTime nearestTime = start;
            int nearestIndex = 0;

            for (int i = 0; i < hoursInDropdown.Count; i++)
            {
                //if greater than nearest one, but not greater than current time (to show from last selected item - default is DateTime.Now)
                if (hoursInDropdown[i].TimeOfDay > nearestTime.TimeOfDay && hoursInDropdown[i].TimeOfDay <= currentTime.TimeOfDay)
                {
                    nearestTime = hoursInDropdown[i];
                    nearestIndex = i;
                }
            }

            return nearestIndex;
        }

        List<string> GetPossibleHoursForDropdown(string dateToUse)
        {
            //parse inspector variables
            DateTime.TryParse(startHours, out start);
            DateTime.TryParse(endHours, out end);
            DateTime.TryParse(dateToUse, out date);
            date = date.Date;                                                       //be sure date has only date and not TimeOfDay
            now = DateTime.Now.Add(new TimeSpan(additiveHoursAfterNow, 0, 0));      //now + additive hours

            //generate hours in dropdown
            hoursInDropdown = new List<DateTime>();
            List<string> hoursToShow = new List<string>();
            for (DateTime dt = date.Add(start.TimeOfDay); dt.TimeOfDay <= end.TimeOfDay; dt = dt.Add(new TimeSpan(0, minutesStep, 0)))
            {
                //only if enabled
                if (CanBeEnabledNow(dt) && IsInsideHoursRange(dt))
                {
                    hoursInDropdown.Add(dt);
                    hoursToShow.Add(dt.ToString(hoursFormat));
                }
            }

            return hoursToShow;
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
            //parse option from dropdown
            DateTime dt = DateTime.Parse(dropdown.options[index].text);
            result = dt.ToString(resultFormat);

            //on select value, call event
            onResult?.Invoke(result);
        }
    }
}