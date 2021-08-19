using System;
using System.Collections;
using System.Collections.Generic;
using FlipWebApps.BeautifulTransitions.Scripts.Transitions;
using FlipWebApps.BeautifulTransitions.Scripts.Transitions.Components.GameObject;
using UnityEngine;

namespace EES.Utilities
{
    public static class Utilities
    {

        public const int INVALID = -1;

        /// <summary>
        /// Delete all chield object under this transform
        /// </summary>
        /// <param name="_root">delete all GameObject under this transform</param>
        public static void DeleteAllChildGameObject(Transform _root)
        {
            if (_root == null) { return; }
            foreach (Transform child in _root)
            {
                GameObject.Destroy(child.gameObject);
            }
        }

        /// <summary>
        /// Set transform's parent and Normalize it
        /// </summary>
        public static void SetParentAndNormalize(Transform targetTransform, Transform parent)
        {
            targetTransform.SetParent(parent);
            NormalizeTransform(targetTransform.transform);
        }

        /// <summary>
        /// Set local position and rotation(eulerangles) to Vector3(0,0,0) and local scale to Vector3(1,1,1)
        /// </summary>
        public static void NormalizeTransform(Transform _transform)
        {
            _transform.localRotation = Quaternion.identity;
            _transform.localPosition = Vector3.zero;
            _transform.localScale = new Vector3(1, 1, 1);
        }

        /// <summary>
        /// Transition the object in (out) or you can add a action to invoke when it complete
        /// </summary>
        public static void Transition(GameObject inTarget, GameObject outTarget = null, Action onComplete = null)
        {
            inTarget.SetActive(true);
            if (outTarget)
                if (onComplete == null)
                    TransitionHelper.TransitionOut(outTarget, () => { outTarget.SetActive(false); });
                else
                    TransitionHelper.TransitionOut(outTarget, () => { outTarget.SetActive(false); onComplete(); });
            TransitionHelper.TransitionIn(inTarget);
        }

        /// <summary>
        /// Transition fade the object in (out) or you can add a action to invoke when it complete
        /// </summary>
        public static void TransitionFade(GameObject inTarget, GameObject outTarget = null, Action onComplete = null)
        {
            inTarget.SetActive(true);
            if (outTarget)
                if (onComplete == null)
                    TransitionHelper.TransitionOut(outTarget, () => { outTarget.SetActive(false); });
                else
                    TransitionHelper.TransitionOut(outTarget, () => { outTarget.SetActive(false); onComplete(); });
            inTarget.GetComponent<TransitionFade>().TransitionIn();
        }

        /// <summary>
        /// Transition out the object (will setActive = false), and you can add a action to invoke when it complete
        /// </summary>
        public static void TransitionOut(GameObject outTarget, Action onComplete = null)
        {
            if (onComplete == null)
                TransitionHelper.TransitionOut(outTarget, () => { outTarget.SetActive(false); });
            else
                TransitionHelper.TransitionOut(outTarget, () => { outTarget.SetActive(false); onComplete(); });
        }
    }
}