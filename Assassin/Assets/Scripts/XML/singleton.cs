using UnityEngine;

/// <summary>
/// Singleton Pattern, Inherit from this to make a class a singleton.
/// </summary>
/// <typeparam name="T"></typeparam>
public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T m_Instance;
    private static object threadLock = new object();
    private static bool shuttingDown = false;

    public static T Instance
    {
        get
        {
            if (shuttingDown)
            {
                Debug.LogWarning("[Singleton] Instance '" + typeof(T) +
                    "' already destroyed. Returning null.");
                return null;
            }

            lock (threadLock)
            {
                if (m_Instance == null)
                {
                    m_Instance = (T)FindObjectOfType(typeof(T));

                    if (!m_Instance)
                    {
                        var go = new GameObject();
                        m_Instance = go.AddComponent<T>();
                        go.name = typeof(T).ToString() + " (Singleton)";

                        DontDestroyOnLoad(go);
                    }
                    else
                    {
                        if (m_Instance.name != typeof(T).ToString() + " (Singleton)")
                        {
                            m_Instance.name = typeof(T).ToString() + " (Singleton)";
                            DontDestroyOnLoad(m_Instance.gameObject);
                        }
                    }
                }
                return m_Instance;
            }
        }
    }

    //private void OnApplicationQuit()
    //{
    //    shuttingDown = true;
    //}

    //private void OnDestroy()
    //{
    //    shuttingDown = true;
    //}
}