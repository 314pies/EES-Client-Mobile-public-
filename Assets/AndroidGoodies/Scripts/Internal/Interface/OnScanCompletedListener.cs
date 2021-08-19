
#if UNITY_ANDROID
namespace DeadMosquito.AndroidGoodies.Internal
{
	using System;
	using UnityEngine;

	class OnScanCompletedListener : AndroidJavaProxy
	{
		private const string InterfaceSignature = "android.media.MediaScannerConnection$OnScanCompletedListener";

		private readonly Action<string, AndroidJavaObject> _onScanCompleted;

		public OnScanCompletedListener(Action<string, AndroidJavaObject> onScanCompleted)
			: base(InterfaceSignature)
		{
			_onScanCompleted = onScanCompleted;
		}

		public void onScanCompleted(String path, AndroidJavaObject uri)
		{
			if (uri.IsJavaNull())
			{
				Debug.LogWarning("Scannning file " + path + " failed");
			}

			if (_onScanCompleted != null)
			{
				GoodiesSceneHelper.Queue(() => _onScanCompleted(path, uri));
			}
		}
	}
}
#endif