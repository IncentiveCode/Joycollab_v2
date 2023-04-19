using UnityEngine;

namespace Joycollab.v2
{
    public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
    {
        private static bool m_ShuttingDown = false;
        private static object m_Lock = new object();
        private static T m_singleton;
        public static T singleton
        {
            get
            {
                if (m_ShuttingDown)
                {
                    return null;
                }

                lock (m_Lock)
                {
                    if (m_singleton == null)
                    {
                        m_singleton = (T)FindObjectOfType(typeof(T));

                        if (m_singleton == null)
                        {
                            var singletonObject = new GameObject();
                            m_singleton = singletonObject.AddComponent<T>();
                            singletonObject.name = typeof(T).ToString() + " (singleton)";

                            DontDestroyOnLoad(singletonObject);
                        }
                    }

                    return m_singleton;
                }
            }
        }

        private void OnApplicationQuit()
        {
            m_ShuttingDown = true;
        }


        private void OnDestroy()
        {
            m_ShuttingDown = true;
        }
    }
}