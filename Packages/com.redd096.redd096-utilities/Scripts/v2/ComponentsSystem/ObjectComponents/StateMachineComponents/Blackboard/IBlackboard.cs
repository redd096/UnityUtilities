using System.Collections.Generic;

namespace redd096.v2.ComponentsSystem
{
    public interface IBlackboard
    {
        /// <summary>
        /// Blackboard to save vars to use in differents states
        /// </summary>
        Dictionary<string, object> blackboard { get; set; }
        List<string> blackboardDebug { get; set; }

        System.Action<string> onSetBlackboardValue { get; set; }

        /// <summary>
        /// Add or Set element inside blackboard
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        void SetBlackboardElement(string key, object value);
        /// <summary>
        /// Remove element from blackboard
        /// </summary>
        /// <param name="key"></param>
        void RemoveBlackboardElement(string key);
        /// <summary>
        /// Get element from blackboard
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        T GetBlackboardElement<T>(string key);

        #region blackboard public API

        ///// <summary>
        ///// Add or Set element inside blackboard
        ///// </summary>
        ///// <param name="key"></param>
        ///// <param name="value"></param>
        //public void SetBlackboardElement(string key, object value)
        //{
        //    //add element if not inside blackboard
        //    if (blackboard.ContainsKey(key) == false)
        //    {
        //        blackboard.Add(key, value);
        //    }
        //    //else set it
        //    else
        //    {
        //        blackboard[key] = value;
        //    }

        //    //in editor, update debug blackboard
        //    if (Application.isEditor)
        //    {
        //        //add value in blackboard
        //        if (blackboardDebug.Contains(key) == false)
        //        {
        //            blackboardDebug.Add(key);
        //        }
        //    }

        //    //call event
        //    onSetBlackboardValue?.Invoke(key);
        //}

        ///// <summary>
        ///// Remove element from blackboard
        ///// </summary>
        ///// <param name="key"></param>
        //public void RemoveBlackboardElement(string key)
        //{
        //    //remove element from blackboard
        //    if (blackboard.ContainsKey(key))
        //        blackboard.Remove(key);

        //    //in editor, update debug blackboard
        //    if (Application.isEditor)
        //    {
        //        //remove value from blackboard
        //        if (blackboardDebug.Contains(key))
        //        {
        //            blackboardDebug.Remove(key);
        //        }
        //    }
        //}

        ///// <summary>
        ///// Get element from blackboard
        ///// </summary>
        ///// <typeparam name="T"></typeparam>
        ///// <param name="key"></param>
        ///// <returns></returns>
        //public T GetBlackboardElement<T>(string key)
        //{
        //    //return element from blackboard
        //    if (blackboard.ContainsKey(key))
        //    {
        //        return (T)blackboard[key];
        //    }

        //    //if there is no key, return null
        //    return default;
        //}

        #endregion
    }
}