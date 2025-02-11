﻿
#if UNITY_ANDROID
namespace DeadMosquito.AndroidGoodies.Internal
{
	using UnityEngine;

	static class AndroidGoodiesExtensions
	{
		public static string GetClassName(this AndroidJavaObject ajo)
		{
			return ajo.GetJavaClass().Call<string>("getName");
		}

		public static string GetClassSimpleName(this AndroidJavaObject ajo)
		{
			return ajo.GetJavaClass().Call<string>("getSimpleName");
		}

		public static AndroidJavaObject GetJavaClass(this AndroidJavaObject ajo)
		{
			return ajo.CallAJO("getClass");
		}

		public static string JavaToString(this AndroidJavaObject ajo)
		{
			return ajo.Call<string>("toString");
		}

		#region AndroidJavaObject_Call_Proxy

		public static string CallStr(this AndroidJavaObject ajo, string methodName, params object[] args)
		{
			return ajo.Call<string>(methodName, args);
		}

		public static bool CallBool(this AndroidJavaObject ajo, string methodName, params object[] args)
		{
			return ajo.Call<bool>(methodName, args);
		}

		public static float CallFloat(this AndroidJavaObject ajo, string methodName, params object[] args)
		{
			return ajo.Call<float>(methodName, args);
		}

		public static int CallInt(this AndroidJavaObject ajo, string methodName, params object[] args)
		{
			return ajo.Call<int>(methodName, args);
		}

		public static long CallLong(this AndroidJavaObject ajo, string methodName, params object[] args)
		{
			return ajo.Call<long>(methodName, args);
		}

		public static string CallStaticStr(this AndroidJavaObject ajo, string methodName, params object[] args)
		{
			return ajo.CallStatic<string>(methodName, args);
		}

		public static AndroidJavaObject CallAJO(this AndroidJavaObject ajo, string methodName, params object[] args)
		{
			return ajo.Call<AndroidJavaObject>(methodName, args);
		}

		public static AndroidJavaObject CallStaticAJO(this AndroidJavaObject ajo, string methodName, params object[] args)
		{
			return ajo.CallStatic<AndroidJavaObject>(methodName, args);
		}

		#endregion

		#region AndroidJavaClass_Call_Proxy

		// GET STATIC
		public static string GetStaticStr(this AndroidJavaClass ajc, string fieldName)
		{
			return ajc.GetStatic<string>(fieldName);
		}

		public static bool GetStaticBool(this AndroidJavaClass ajc, string fieldName)
		{
			return ajc.GetStatic<bool>(fieldName);
		}

		public static int GetStaticInt(this AndroidJavaClass ajc, string fieldName)
		{
			return ajc.GetStatic<int>(fieldName);
		}

		// CALL STATIC
		public static string CallStaticStr(this AndroidJavaClass ajc, string methodName, params object[] args)
		{
			return ajc.CallStatic<string>(methodName, args);
		}

		public static int CallStaticInt(this AndroidJavaClass ajc, string methodName, params object[] args)
		{
			return ajc.CallStatic<int>(methodName, args);
		}

		public static long CallStaticLong(this AndroidJavaClass ajc, string methodName, params object[] args)
		{
			return ajc.CallStatic<long>(methodName, args);
		}

		public static bool CallStaticBool(this AndroidJavaClass ajc, string methodName, params object[] args)
		{
			return ajc.CallStatic<bool>(methodName, args);
		}

		public static AndroidJavaObject CallStaticAJO(this AndroidJavaClass ajc, string methodName, params object[] args)
		{
			return ajc.CallStatic<AndroidJavaObject>(methodName, args);
		}

		public static T AJCCallStaticOnce<T>(this string className, string methodName, params object[] args)
		{
			using (var ajc = new AndroidJavaClass(className))
			{
				return ajc.CallStatic<T>(methodName, args);
			}
		}

		public static AndroidJavaObject AJCCallStaticOnceAJO(this string className, string methodName, params object[] args)
		{
			return className.AJCCallStaticOnce<AndroidJavaObject>(methodName, args);
		}

		#endregion
	}
}
#endif