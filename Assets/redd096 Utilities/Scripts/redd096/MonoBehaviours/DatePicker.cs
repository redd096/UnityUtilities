using System;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace redd096
{
    /// <summary>
    /// Calendar must have a GameObject (our calendar to open and close) with child:
    /// - (facoltative) header, with TMPro month text and buttons for NextMonth and PreviousMonth
    /// - a GridLayoutGroup with 7 columns.
    /// Firsts 7 child of GridLayoutGroup are 7 GameObjects with TMPro in self or child (unity button without button component). These are days of week (M,T,W,T,F,S,S)
    /// Buttons for every day, with TMPro in self or child (unity button). Text to show day of month and button to call OnResult event
    /// </summary>
    public class DatePicker : MonoBehaviour
    {
        [Header("Calendar to open and close")]
        [SerializeField] GameObject calendar = default;

        [Header("Month Header")]
        [SerializeField] TextMeshProUGUI monthText = default;
        [SerializeField] string monthFormat = "MMMM yyyy";

        [Header("Days of the week")]
        [SerializeField] DayOfWeek firstDayOfWeek = DayOfWeek.Monday;
        [SerializeField] GameObject[] daysOfWeek = default;

        [Header("Calendar")]
        [SerializeField] Button[] daysButtons = default;

        [Header("Not Clickable Days")]
        [SerializeField] bool disableDaysBeforeThanToday = true;
        [SerializeField] DayOfWeek[] disabledDaysOfTheWeek = new DayOfWeek[2] { DayOfWeek.Saturday, DayOfWeek.Sunday };

        [Header("Result")]
        [SerializeField] string dateFormat = "dd/MM/yyyy";
        [SerializeField] UnityEvent<string> onResult = default;

        [Space(10)]
        [Header("DEBUG - AutoGenerate DaysOfWeek and Calendar")]
        [SerializeField] Transform contentGridLayout = default;
        [SerializeField] Button buttonPrefab = default;

        DateTime monthToGenerate;

        public void OpenCalendar()
        {
            //set today as start month
            monthToGenerate = DateTime.Now.Date;
            GenerateCalendar();

            ActiveCalendar(true);
        }

        public void NextMonth()
        {
            //next month
            monthToGenerate = monthToGenerate.AddMonths(1);
            GenerateCalendar();
        }

        public void PreviousMonth()
        {
            //previous month
            monthToGenerate = monthToGenerate.AddMonths(-1);
            GenerateCalendar();
        }

        public void CloseCalendar()
        {
            ActiveCalendar(false);
        }

        #region generate calendar

        void ActiveCalendar(bool active)
        {
            if (calendar)
                calendar.SetActive(active);
        }

        void GenerateCalendar()
        {
            //set month text
            SetHeader();

            //show name of days of week
            UpdateDaysOfWeek();

            //create calendar from Monday to Sunday * number of rows
            UpdateDaysButtons();
        }

        void SetHeader()
        {
            //set month text
            if (monthText)
                monthText.text = monthToGenerate.ToString(monthFormat);
        }

        void UpdateDaysOfWeek()
        {
            if (daysOfWeek == null)
                return;

            //start from monday (or other day setted in inspector)
            DateTime firstDayOfCalendar = GetFirstDayOfCalendar();

            //update every gameObject with day of week name
            for (int i = 0; i < daysOfWeek.Length; i++)
            {
                DateTime dt = firstDayOfCalendar.AddDays(i);

                //only first char of day of week
                daysOfWeek[i].GetComponentInChildren<TextMeshProUGUI>().text = dt.ToString("dddd").Substring(0, 1).ToUpper();

                //set order in grid
                daysOfWeek[i].transform.SetSiblingIndex(i);
            }
        }

        void UpdateDaysButtons()
        {
            if (daysButtons == null)
                return;

            //start from monday (or other day setted in inspector)
            DateTime firstDayOfCalendar = GetFirstDayOfCalendar();

            //create calendar from Monday to Sunday * number of rows
            for (int i = 0; i < daysButtons.Length; i++)
            {
                DateTime dt = firstDayOfCalendar.AddDays(i);

                //set text and add event on click
                Button b = daysButtons[i];
                b.GetComponentInChildren<TextMeshProUGUI>().text = dt.Day.ToString();
                b.onClick.RemoveAllListeners();
                b.onClick.AddListener(() => OnClick(dt));

                //set order in grid
                b.transform.SetSiblingIndex(daysOfWeek != null ? daysOfWeek.Length + i : i);

                //check if button is enabled
                b.interactable = CanBeEnabledToday(dt);
                if (b.interactable)
                {
                    b.interactable = IsDayEnabled(dt);
                }
            }
        }

        DateTime GetFirstDayOfCalendar()
        {
            //move to first day of the month
            DateTime dt = new DateTime(monthToGenerate.Year, monthToGenerate.Month, 1);

            //if 1st day isn't the one setted in inspector, use previous month
            for (int i = 0; i < 7; i++)
            {
                dt = dt.AddDays(-1);
                if (dt.DayOfWeek == firstDayOfWeek)
                    break;
            }

            return dt;
        }
        #endregion

        #region checks to disable buttons

        bool CanBeEnabledToday(DateTime dt)
        {
            //if bool is disabled
            //else check if DateTime is greater or equal than today (DateTime.Now without check the hours)
            return disableDaysBeforeThanToday == false || dt.Date >= DateTime.Now.Date;
        }

        bool IsDayEnabled(DateTime dt)
        {
            //check every disabled day
            foreach (DayOfWeek dayOfWeek in disabledDaysOfTheWeek)
            {
                //if DateTime is one of these, then is disabled
                if (dt.DayOfWeek == dayOfWeek)
                {
                    return false;
                }
            }

            return true;
        }

        #endregion

        void OnClick(DateTime date)
        {
            //on button click, call event
            onResult?.Invoke(date.ToString(dateFormat));
        }

        /// <summary>
        /// editor only
        /// </summary>
        public void AutoGenerateCalendar()
        {
            //generate days of week
            daysOfWeek = new GameObject[7];
            for (int i = 0; i < daysOfWeek.Length; i++)
            {
                daysOfWeek[i] = Instantiate(buttonPrefab, contentGridLayout).gameObject;

                //remove button component
                if (Application.isPlaying)
                    Destroy(daysOfWeek[i].GetComponent<Button>());
                else
                    DestroyImmediate(daysOfWeek[i].GetComponent<Button>());
            }

            //create buttons for calendar from Monday to Sunday * number of rows (6 to be sure to show 31 days also when first day of the month is last day of week)
            daysButtons = new Button[7 * 6];
            for (int i = 0; i < daysButtons.Length; i++)
                daysButtons[i] = Instantiate(buttonPrefab, contentGridLayout);

            //set today as start month and update buttons texts
            monthToGenerate = DateTime.Now.Date;
            GenerateCalendar();
        }
    }

    #region editor
#if UNITY_EDITOR

    [CustomEditor(typeof(DatePicker), true)]
    public class DatePickerEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            if (GUILayout.Button("Generate Calendar"))
            {
                DatePicker datePicker = target as DatePicker;
                if (datePicker)
                    datePicker.AutoGenerateCalendar();
            }
        }
    }

#endif
    #endregion
}