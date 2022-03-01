using System.Reflection;
using System.Linq;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace redd096.Attributes
{
	public static class ReflectionUtility
	{
		#region return serialez property field

#if UNITY_EDITOR

		/// <summary>
		/// Return field of this property, used for example to get or set a value without know the type
		/// </summary>
		/// <param name="property"></param>
		/// <returns></returns>
		public static FieldInfo GetField(this SerializedProperty property)
		{
			//find field using property.name
			foreach (FieldInfo field in property.serializedObject.targetObject.GetFields())
				if (field.Name.Equals(property.name, System.StringComparison.Ordinal))
					return field;

			return null;
		}

#endif

		#endregion

		#region return first

		/// <summary>
		/// Get Field by name
		/// </summary>
		/// <param name="property"></param>
		/// <returns></returns>
		public static FieldInfo GetField(this object targetObject, string fieldName)
		{
			foreach (FieldInfo field in targetObject.GetFields())
				if (field.Name.Equals(fieldName, System.StringComparison.Ordinal))
					return field;

			return null;
		}

		/// <summary>
		/// Get Method by name
		/// </summary>
		/// <param name="property"></param>
		/// <returns></returns>
		public static MethodInfo GetMethod(this object targetObject, string methodName)
		{
			foreach (MethodInfo method in targetObject.GetMethods())
				if (method.Name.Equals(methodName, System.StringComparison.Ordinal))
					return method;

			return null;
		}

		/// <summary>
		/// Get Property by name
		/// </summary>
		/// <param name="property"></param>
		/// <returns></returns>
		public static PropertyInfo GetProperty(this object targetObject, string propertyName)
		{
			foreach (PropertyInfo property in targetObject.GetProperties())
				if (property.Name.Equals(propertyName, System.StringComparison.Ordinal))
					return property;

			return null;
		}

		#endregion

		#region return array

		/// <summary>
		/// Return every method in this object
		/// </summary>
		/// <param name="targetObject"></param>
		/// <returns></returns>
		public static MethodInfo[] GetMethods(this object targetObject)
		{
			return targetObject.GetType().GetMethods(BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.DeclaredOnly);
		}

		/// <summary>
		/// Return every field in this object
		/// </summary>
		/// <param name="targetObject"></param>
		/// <returns></returns>
		public static FieldInfo[] GetFields(this object targetObject)
		{
			return targetObject.GetType().GetFields(BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.DeclaredOnly);
		}

		/// <summary>
		/// Return every property in this object
		/// </summary>
		/// <param name="targetObject"></param>
		/// <returns></returns>
		public static PropertyInfo[] GetProperties(this object targetObject)
		{
			return targetObject.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.DeclaredOnly);
		}

		#endregion

		#region generic

		/// <summary>
		/// Return type of a list
		/// </summary>
		/// <param name="listType"></param>
		/// <returns></returns>
		public static System.Type GetListElementType(System.Type listType)
		{
			//if generic, get first argument type
			if (listType.IsGenericType)
			{
				return listType.GetGenericArguments()[0];
			}
			//else get list type
			else
			{
				return listType.GetElementType();
			}
		}

		/// <summary>
		/// Return default value for every parameter inside a method
		/// </summary>
		/// <param name="method"></param>
		/// <returns></returns>
		public static object[] GetDefaultParameters(this MethodInfo method)
		{
			//foreach parameter, return default value
			return method.GetParameters().Select(p => p.DefaultValue).ToArray();
		}

		/// <summary>
		/// Return if this method has 0 parameters, or if there are parameters but all optionals
		/// </summary>
		/// <param name="method"></param>
		/// <returns></returns>
		public static bool HasZeroParameterOrOnlyOptional(this MethodInfo method)
		{
			return method.GetParameters().All(p => p.IsOptional);
		}

		#endregion
	}
}