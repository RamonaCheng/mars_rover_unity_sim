using UnityEngine;

public class SCC_Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T m_Instance;
    private static readonly object m_Lock = new object();

    public static T Instance
    {
        get
        {
            lock (m_Lock)
            {
                if (m_Instance != null)
                    return m_Instance;

                // Search for existing instance (using the new API).
#if UNITY_2023_1_OR_NEWER
                m_Instance = (T)FindFirstObjectByType(typeof(T));
#else
                m_Instance = (T)FindObjectOfType(typeof(T));
#endif

                // Create new instance if one doesn't already exist.
                if (m_Instance != null) return m_Instance;

                // Need to create a new GameObject to attach the singleton to.
                var singletonObject = new GameObject();
                m_Instance = singletonObject.AddComponent<T>();
                singletonObject.name = typeof(T).ToString();

                // Make instance persistent.
                //DontDestroyOnLoad(singletonObject);

                return m_Instance;
            }
        }
    }
}
