using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EES.Utilities
{
    /// <summary>
    /// For using MonoBehaviour's feature in those classes which didn't derive MonoBehaviour. Need to be attached to an scene's GameObject before use
    /// </summary>
    ///             /// <example> 
    /// This sample shows how to use the <see cref="StaticMono"/> method to login.
    /// <code>
    /**        
using UnityEngine;
using IDWaterApp.ClientAPI; 

public class LoginExample
{
       public void Example()
       {
          //Start a Coroutine in non MonoBehaviour derived classed
          StaticMono.Instance.StartCoroutine(exampleCoroutine());
       }

        IEnumerator exampleCoroutine(){
            yield return new WaitForSeconds(8.7);
        }
}
*/
    /// </code>
    /// </example>

    public class StaticMono : MonoBehaviour
    {

        public static StaticMono Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = GameObject.FindGameObjectWithTag("StaticMono").GetComponent<StaticMono>();
                }

                return instance;
            }
        }
        private static StaticMono instance = null;

    }
}
