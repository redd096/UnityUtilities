using System.Collections.Generic;

namespace redd096.ComponentsSystem
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

        //public void SetBlackboardElement(string key, object value)
        //{
        //    //add element if not inside blackboard
        //    if (Blackboard.ContainsKey(key) == false)
        //    {
        //        Blackboard.Add(key, value);
        //    }
        //    //else set it
        //    else
        //    {
        //        Blackboard[key] = value;
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

        //public void RemoveBlackboardElement(string key)
        //{
        //    //remove element from blackboard
        //    if (Blackboard.ContainsKey(key))
        //        Blackboard.Remove(key);

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

        //public T GetBlackboardElement<T>(string key)
        //{
        //    //return element from blackboard
        //    if (Blackboard.ContainsKey(key))
        //    {
        //        return (T)Blackboard[key];
        //    }

        //    //if there is no key, return null
        //    return default;
        //}
    }
}