using System;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using redd096.Attributes;

namespace redd096
{
    /// <summary>
    /// Calendar must have a GameObject (our calendar to open and close) with child:
    /// - (facoltative) header, with TMPro month text and buttons for NextMonth and PreviousMonth
    /// - a GridLayoutGroup with 7 columns.
    /// Firsts 7 child of GridLayoutGroup are 7 GameObjects with TMPro in self or child (unity button without button component). These are days of week (M,T,W,T,F,S,S)
    /// Buttons for every day, with TMPro in self or child (unity button). Text to show day of month and button to call OnResult event
    /// - (facoltative) button to come back to CurrentMonth
    /// 
    /// You can use AutoGenerate button to generate DaysOfWeek and every day Button
    /// </summary>
	[AddComponentMenu("redd096/Main/MonoBehaviours/Date Picker")]
    public class DatePicker : MonoBehaviour
    {
        [Header("Calendar to open and close")]
        [SerializeField] GameObject calendar = default;
        [SerializeField] bool closeOnAwake = true;
        [Tooltip("On awake set DateTime.Now as default result, to have result to read without open calendar")][SerializeField] bool setDefaultOnAwake = true;
        [EnableIf("setDefaultOnAwake")][Tooltip("If set default on awake, start from first day selectable")][SerializeField] bool findFirstDaySelectableOnAwake = true;
        [Tooltip("When call OpenCalendar, start from first day selectable")][SerializeField] bool findFirstDaySelectableOnOpen = true;

        [Header("Month Header")]
        [SerializeField] TextMeshProUGUI monthText = default;
        [SerializeField] string monthFormat = "MMMM yyyy";

        [Header("Days of the week")]
        [SerializeField] DayOfWeek firstDayOfWeek = DayOfWeek.Monday;
        [SerializeField] GameObject[] daysOfWeek = default;

        [Header("Calendar")]
        [SerializeField] Button[] daysButtons = default;

        [Header("Not Clickable Days")]
        [Tooltip("Disable yesterday and before")][SerializeField] bool disableDaysBeforeThanToday = true;
        [Tooltip("If bool enabled and TimePicker setted, disable also today if there are not selectable hours")][SerializeField] TimePicker timePicker = default;
        [Tooltip("Disable these days of the week")][SerializeField] DayOfWeek[] disabledDaysOfTheWeek = new DayOfWeek[2] { DayOfWeek.Saturday, DayOfWeek.Sunday };

        [Header("Result")]
        public string ResultStringFormat = "dd/MM/yyyy";
        public string ResultString = "19/12/2022";
        public DateTime Result = DateTime.Now;

        [Header("Events")]
        public UnityEvent<Button> OnShowSelectedDate = default;
        public UnityEvent<string> OnResultString = default;
        public UnityEvent<DateTime> OnResult = default;

        [Space(10)]
        [Header("DEBUG - AutoGenerate DaysOfWeek and Calendar")]
        [Tooltip("Parent where spawn")][SerializeField] Transform contentGridLayout = default;
        [Tooltip("Prefab for DaysOfWeek and CalendarButtons")][SerializeField] Button buttonPrefab = default;

        public GameObject Calendar => calendar;

        DateTime monthToGenerate = DateTime.Today;
        DateTime selectedDay = DateTime.Today;
        DateTime[] customNotClickableDays = null;

        private void Awake()
        {
            //set today as default result
            if (setDefaultOnAwake)
            {
                Result = DateTime.Now;

                //or find first day selectable
                if (findFirstDaySelectableOnAwake)
                {
                    SelectFirstSelectableDay();
                }

                OnClick(Result);
            }

            //set current date (to show from previous selected month - default is DateTime.Now) as start month
            selectedDay = Result.Date;
            monthToGenerate = Result.Date;
            GenerateCalendar();

            //close on awake
            if (closeOnAwake)
                CloseCalendar();
        }

        #region public API

        public void OpenCalendar()
        {
            //set current date (to show from previous selected month - default is DateTime.Now) as start month
            selectedDay = Result.Date;
            monthToGenerate = Result.Date;
            GenerateCalendar();
            if (findFirstDaySelectableOnOpen) SelectFirstSelectableDay();

            //and open calendar
            ActiveCalendar(true);
        }

        public void CurrentMonth()
        {
            //set today as start month
            monthToGenerate = DateTime.Now.Date;
            GenerateCalendar();
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

        public bool IsCalendarOpen()
        {
            if (calendar)
                return calendar.activeInHierarchy;

            return false;
        }

        /// <summary>
        /// Regenerate calendar. For example if someone called SetDate, or if is midnight and DisableDaysBeforeThanToday is true.
        /// </summary>
        public void UpdateResult()
        {
            //update selected day
            selectedDay = Result.Date;
            GenerateCalendar();
        }

        public bool IsEnabledThisDay(DateTime day)
        {
            return CanBeEnabledToday(day.Date) && IsDayEnabled(day.Date) && IsNotInCustomNotClickableDays(day.Date);
        }

        /// <summary>
        /// Set Result variable
        /// </summary>
        /// <param name="newDate"></param>
        public void SetDate(DateTime newDate)
        {
            Result = newDate;
            ResultString = Result.ToString(ResultStringFormat);
        }

        /// <summary>
        /// Set days not selectable
        /// </summary>
        /// <param name="notClickableDays"></param>
        public void SetCustomNotClickableDays(DateTime[] notClickableDays)
        {
            customNotClickableDays = notClickableDays;
        }

        /// <summary>
        /// Cycle days from today, until found a day not disabled
        /// </summary>
        public void SelectFirstSelectableDay()
        {
            DateTime day = DateTime.Now;
            for (int i = 0; i < 100; i++)                           //set a limit of 100 days, if not found select Today
            {
                if (timePicker) timePicker.SetSelectedDate(day);    //update time picker to check also if there are hours this day
                if (IsEnabledThisDay(day.Date))                     //check is enabled (using variables setted for not clicable days)
                    break;
                day = day.AddDays(1);                               //else check next day
            }
            Result = day;
        }

        #endregion

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
            string s = monthToGenerate.ToString(monthFormat);

            //set month text
            if (monthText)
                monthText.text = s[0].ToString().ToUpper() + s.Substring(1);
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
                b.interactable = CanBeEnabledToday(dt) && IsDayEnabled(dt) && IsNotInCustomNotClickableDays(dt);

                //select current day
                if (dt.Date == selectedDay.Date)
                {
                    b.Select();
                    OnShowSelectedDate?.Invoke(b);
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
                if (dt.DayOfWeek == firstDayOfWeek)
                    break;
                dt = dt.AddDays(-1);
            }

            return dt;
        }
        #endregion

        #region checks to disable buttons

        bool CanBeEnabledToday(DateTime dt)
        {
            //if time picker is null, don't check hours
            bool thereAreHoursToday = timePicker == null || timePicker.ThereAreHoursThisDate(dt);

            //if bool is disabled
            //or check if DateTime is greater or equal than today (DateTime.Now without check the hours) AND if there is a time picker, be sure it isn't after last possible hour
            return disableDaysBeforeThanToday == false || (dt.Date >= DateTime.Now.Date && thereAreHoursToday);
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

        bool IsNotInCustomNotClickableDays(DateTime dt)
        {
            //check every not clickable day
            if (customNotClickableDays != null)
            {
                foreach (DateTime day in customNotClickableDays)
                {
                    if (dt.Day == day.Day)
                        return false;
                }
            }

            return true;
        }

        #endregion

        void OnClick(DateTime dt)
        {
            //on button click, call event
            SetDate(dt);
            OnResult?.Invoke(Result);
            OnResultString?.Invoke(ResultString);
        }

        #region editor only

        /// <summary>
        /// editor only
        /// </summary>
        [Button]
        void AutoGenerateCalendar()
        {
            //remove old buttons
            for (int i = contentGridLayout.childCount - 1; i >= 0; i--)
            {
                if (Application.isPlaying)
                    Destroy(contentGridLayout.GetChild(i).gameObject);
                else
                    DestroyImmediate(contentGridLayout.GetChild(0).gameObject);
            }

            //generate days of week
            daysOfWeek = new GameObject[7];
            for (int i = 0; i < daysOfWeek.Length; i++)
            {
#if UNITY_EDITOR
                daysOfWeek[i] = (UnityEditor.PrefabUtility.InstantiatePrefab(buttonPrefab, contentGridLayout) as Button).gameObject;
#else
                daysOfWeek[i] = Instantiate(buttonPrefab, contentGridLayout).gameObject;
#endif

                //remove button component
                if (Application.isPlaying)
                    Destroy(daysOfWeek[i].GetComponent<Button>());
                else
                    DestroyImmediate(daysOfWeek[i].GetComponent<Button>());
            }

            //create buttons for calendar from Monday to Sunday * number of rows (6 to be sure to show 31 days also when first day of the month is last day of week)
            daysButtons = new Button[7 * 6];
            for (int i = 0; i < daysButtons.Length; i++)
            {
#if UNITY_EDITOR
                daysButtons[i] = UnityEditor.PrefabUtility.InstantiatePrefab(buttonPrefab, contentGridLayout) as Button;
#else
                daysButtons[i] = Instantiate(buttonPrefab, contentGridLayout);
#endif
            }

            //set today as start month and update buttons texts
            monthToGenerate = DateTime.Now.Date;
            GenerateCalendar();
        }

        #endregion
    }
}