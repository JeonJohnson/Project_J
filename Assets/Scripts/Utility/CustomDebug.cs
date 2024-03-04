using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
//using UnityEditor.Build.Content;
//using UnityEditor.PackageManager.UI;
using UnityEngine;

namespace Potato
{
	public enum LogType
	{
		Normal,
		Warning,
		Error,
		Input
	}

	public class Debug 
    {
        public static void Log(object message)
        {
            UnityEngine.Debug.Log(message);
			
			
			PotatoConsole.Instance?.Print(GetString(message));
        }
		public static void LogWarning(object message)
		{
			UnityEngine.Debug.LogWarning(message);

			PotatoConsole.Instance?.Print(GetString(message),LogType.Warning);
		}
		public static void LogError(object message)
		{
			UnityEngine.Debug.LogError(message);

			PotatoConsole.Instance?.Print(GetString(message), LogType.Error);
		}


		private static string GetString(object message)
		{
			if (message == null)
			{
				return "Null";
			}
			var formattable = message as IFormattable;
			if (formattable != null)
			{
				return formattable.ToString(null, CultureInfo.InvariantCulture);
			}
			else
			{
				return message.ToString();
			}
		}
	}

}