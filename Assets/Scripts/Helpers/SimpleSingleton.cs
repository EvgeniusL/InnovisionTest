using UnityEngine;
using System;

namespace Innovision.Helpers
{
    public class SimpleSingleton<T> where T : class
    {
        private static T _instance;

        public static T Instance
        {
            get
            {
                if (_instance == null)
                    CreateInstance();

                return _instance;
            }
        }

        private static void CreateInstance()
        {
            if (_instance == null)
            {
                Type t = typeof(T);
                _instance = (T)Activator.CreateInstance(t, true);
            }
        }
    }
}