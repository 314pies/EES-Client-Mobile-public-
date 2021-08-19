using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using EES.Utilities;
#if UNITY_ANDROID
using DeadMosquito.AndroidGoodies;
#endif
#if UNITY_IPHONE
using SA.IOSNative.UIKit;
#endif

namespace EES.Native
{
    /// <summary>
    /// Collect some native system tools for android and iOS
    /// </summary>
    public class MobileNative
    {
        private static float lastTimePicked = 0;
        private static bool isDone = false;
        private static bool isCancel = false;
        
        /// <summary>
        /// Show the DatePicker on user's screen.
        /// </summary>
        /// <param name="defaultDate">The default date displayed in DatePicker</param>
        /// <param name="OnDatePicked">Return (year,month,day) in this action</param>
        /// <param name="OnDatePickCancel">Recover the date in this action</param>
        public static void OnPickDateClick(DateTime defaultDate, Action<int, int, int> OnDatePicked, Action OnDatePickCancel)
        {
#if UNITY_ANDROID
            isDone = false;
            isCancel = false;
            AGDateTimePicker.ShowDatePicker(defaultDate.Year, defaultDate.Month, defaultDate.Day, 
                (int year, int month, int day) =>
                {
                    Debug.Log("OnDatePicked");
                    StaticMono.Instance.StartCoroutine(DatePickerTimer(OnDatePicked, year, month, day));
                },
                () =>
                {
                    isCancel = true;
                    Debug.Log("Accessed OnDateCanceled");
                    OnDatePickCancel();
                }
            );
#endif

#if UNITY_IPHONE
            DateTimePicker.Show(DateTimePickerMode.Date, defaultDate, (dateTime) => {
                OnDatePicked(dateTime.Year, dateTime.Month, dateTime.Day);
            });
#endif
        }

        public static void OnPickTimeClick(DateTime defaultTime, Action<int, int> OnTimePicked, Action OnTimeCancel)
        {
#if UNITY_ANDROID
            isDone = false;
            isCancel = false;
            AGDateTimePicker.ShowTimePicker(defaultTime.Hour, defaultTime.Minute, 
                (int hour, int minute) =>
                {
                    Debug.Log("OnTimePicked");
                    StaticMono.Instance.StartCoroutine(TimePickerTimer(OnTimePicked, hour, minute));
                },
                () =>
                {
                    isCancel = true;
                    Debug.Log("Accessed OnTimeCanceled");
                    OnTimeCancel();
                }
            );
#endif

#if UNITY_IPHONE
            Debug.Log(defaultTime);
            DateTime defaultUtcTime = defaultTime.ToUniversalTime();
            Debug.Log(defaultUtcTime);
            DateTimePicker.Show(DateTimePickerMode.Time, defaultUtcTime,
                (dateTime) => {
                OnTimePicked(dateTime.Hour, dateTime.Minute);
            });
#endif
        }

        public static void OnRadioClick(int defaultSelectedItemIndex, string[] Items, Action<int> OnDoneClicked)
        {
#if UNITY_ANDROID
            int selectedItemIndex = defaultSelectedItemIndex;
            AGAlertDialog.ShowSingleItemChoiceDialog("選擇稱謂", Items, defaultSelectedItemIndex,
                index => selectedItemIndex = index,
                "OK",
                () => { OnDoneClicked(selectedItemIndex); }
            );
#endif
        }

        public static void OnPhoneCalled(string phoneNumber)
        {
#if UNITY_ANDROID
            AGDialer.OpenDialer(phoneNumber);
#endif
        }

        static IEnumerator DatePickerTimer(Action<int, int, int> OnDatePicked, int year, int month, int day)
        {
            yield return new WaitForSeconds(0.25f);
            if (!isCancel && !isDone)
            {
                isDone = true;
                Debug.Log("Accessed OnDatePicked");
                OnDatePicked(year, month, day);
            }
        }

        static IEnumerator TimePickerTimer(Action<int, int> OnTimePicked, int hour, int minute)
        {
            yield return new WaitForSeconds(0.25f);
            if (!isCancel && !isDone)
            {
                isDone = true;
                Debug.Log("Accessed OnTimePicked");
                OnTimePicked(hour, minute);
            }
        }

        public static void PhotoGallery(Action<Texture2D> OnImagePicked)
        {
#if UNITY_ANDROID
            AGGallery.PickImageFromGallery(
                (selectedImage) =>
                {
                    OnImagePicked(selectedImage.LoadTexture2D());

                    // Clean up
                    Resources.UnloadUnusedAssets();
                },
                (errorMessage) => AGUIMisc.ShowToast("圖片選擇失敗：" + errorMessage),
                ImageResultSize.Max512,
                true
                );
#endif 

#if UNITY_IPHONE
            Action<IOSImagePickResult> TargetDelegate = null;
            Action<IOSImagePickResult> ImagePickedDelegate = delegate (IOSImagePickResult result) {
                if (result.IsSucceeded)
                {
                    OnImagePicked(result.Image);
                }
                else
                {
                    IOSMessage.Create("錯誤", "載入圖片失敗");
                }
                IOSCamera.OnImagePicked -= TargetDelegate;
            };
            TargetDelegate = ImagePickedDelegate;
            IOSCamera.OnImagePicked += TargetDelegate;
            IOSCamera.Instance.PickImage(ISN_ImageSource.Library);
#endif
        }

    }
}