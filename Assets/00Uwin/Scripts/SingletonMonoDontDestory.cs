using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Module.Nst.Pattern
{
    public class SingletonMonoDontDestory<T> : MonoBehaviour where T : MonoBehaviour
    {
        private static object syncLock = new object();
        protected static bool applicationIsQuitting;
        protected static T instance;

        public static T Instance
        {
            get
            {
                lock (syncLock)
                {
                    if (applicationIsQuitting == true)
                    {
                        return null;
                    }
                    if (instance == null)
                    {
                        instance = (T)FindObjectOfType<T>();
                        Object[] instanceList = FindObjectsOfType(typeof(T));
                        if (instanceList.Length > 1)
                        {
                            Debug.LogError("[Singleton] Something went really wrong " +
                            " - there should never be more than 1 singleton!" +
                            " Reopenning the scene might fix it.");
                            return instance;
                        }
                        if (instance == null)
                        {
                            GameObject obj = new GameObject(typeof(T).Name);
                            instance = obj.AddComponent<T>();
                            DontDestroyOnLoad(obj);
                        }
                        else
                        {
                            Debug.Log("co san~:" + typeof(T).Name);
                        }
                       
                    }
                    return instance;
                }
            }
        }

        protected virtual void Awake()
        {
            if (instance == null)
            {
                instance = this as T;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        protected virtual void OnDestroy()
        {
           
        }
    }
}
