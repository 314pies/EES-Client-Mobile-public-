
#if UNITY_ANDROID
namespace DeadMosquito.AndroidGoodies.Internal
{
	using System;
	using UnityEngine;

	class DialogOnCancelListenerPoxy : AndroidJavaProxy
	{
		private readonly Action _onCancel;

		public DialogOnCancelListenerPoxy(Action onCancel)
			: base("android.content.DialogInterface$OnCancelListener")
		{
			_onCancel = onCancel;
		}

		void onCancel(AndroidJavaObject dialog)
		{
			GoodiesSceneHelper.Queue(_onCancel);
		}
	}
}
#endif