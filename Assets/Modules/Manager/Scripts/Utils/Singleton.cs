using UnityEngine;

namespace Dan.Manager
{
    public abstract class Singleton : MonoBehaviour
    {
        public static bool Quitting { get; private set; }
        private void OnApplicationQuit()
        {
            Quitting = true;
        }

        private void OnDestroy()
        {
            Quitting = true;
        }
    }

    public class Singleton<T> : Singleton where T : MonoBehaviour
    {
        /// <summary>
        /// Singleton instance
        /// </summary>
        private static T _instance;

        /// <summary>
        /// Access singleton instance through this propriety.
        /// </summary>
        public static T Instance
        {
            get
            {
                if (Quitting)
                {
                    return null;
                }

                if (_instance == null)
                {
                    _instance = FindObjectOfType<T>();
                    DontDestroyOnLoad(_instance.gameObject);
                }

                return _instance;
            }
        }

        /// <summary>
        /// Awake the singleton -> add instance to the same gameobject than this class
        /// </summary>
        protected virtual void Awake()
        {
            if (_instance == null)
            {
                _instance = Instance;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                // We have already an instance, we destroy this one
                if(this != _instance)
                    Destroy(gameObject);
            }
        }
    }
}