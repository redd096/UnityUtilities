using System;

namespace redd096.Attributes
{
    /// <summary>
    /// Show a vector3 or vector2 as a drabble point in scene. It's the same as DraggablePointAttribute
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = true, Inherited = true)]
    public class HandlePointAttribute : DraggablePointAttribute
    {
    }
}