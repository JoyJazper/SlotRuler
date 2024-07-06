namespace JoyKit.Utility
{
    using UnityEngine;

    internal class CoroutineStarter : MonoBehaviour
    {
        private static CoroutineStarter instance;

        internal static CoroutineStarter Instance
        {
            get
            {
                if (instance == null)
                {
                    GameObject obj = new GameObject("CoroutineStarter");
                    instance = obj.AddComponent<CoroutineStarter>();
                }
                return instance;
            }
        }

        private void OnDestroy()
        {
            StopAllCoroutines();
            instance = null;
        }

        // Debug error data
        /*internal void DED(string data)
        {
            Debug.LogError("ERNOS : " + data);
        }*/
    }

}
