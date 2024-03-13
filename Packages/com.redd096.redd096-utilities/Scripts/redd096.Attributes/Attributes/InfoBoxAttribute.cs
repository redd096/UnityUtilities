using System;

namespace redd096.Attributes
{
    /// <summary>
    /// Show help box above property. It's the same as HelpBoxAttribute
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = true, Inherited = true)]
    public class InfoBoxAttribute : HelpBoxAttribute
    {
        public InfoBoxAttribute(string message, EMessageType messageType = EMessageType.Info) : base(message, messageType)
        {
        }
    }
}