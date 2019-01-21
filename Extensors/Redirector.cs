using System;
using System.Reflection;
using UnityEngine;
using System.Diagnostics;
using Klyte.Harmony;

namespace Klyte.Commons.Extensors
{
    public sealed class RedirectorUtils
    {
        public static readonly BindingFlags allFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly | BindingFlags.GetField | BindingFlags.GetProperty;
    }

    public abstract class Redirector : MonoBehaviour
    {
        public static readonly BindingFlags allFlags = RedirectorUtils.allFlags;
    }

    public abstract class Redirector<T> : Redirector where T : Redirector<T>
    {
        #region Class Base
        private readonly HarmonyInstance harmony = HarmonyInstance.Create("com.klyte.commons." + typeof(T).Name);
        private static Redirector<T> instance;

        public HarmonyInstance GetHarmonyInstance()
        {
            return harmony;
        }
        #endregion

        protected static bool semiPreventDefault()
        {
            StackTrace stackTrace = new StackTrace();
            StackFrame[] stackFrames = stackTrace.GetFrames();
            instance?.doLog($"SemiPreventDefault fullStackTrace: \r\n {Environment.StackTrace}");
            for (int i = 2; i < stackFrames.Length; i++)
            {
                if (stackFrames[i].GetMethod().DeclaringType.ToString().StartsWith("Klyte."))
                {
                    return false;
                }
            }
            return true;
        }
        protected MethodInfo semiPreventDefaultMI = typeof(T).GetMethod("semiPreventDefault", allFlags);

        public void Awake()
        {
            AwakeBody();
            instance = this;
        }

        public abstract void AwakeBody();
        public abstract void doLog(string text, params object[] param);


        public void AddRedirect(MethodInfo oldMethod, MethodInfo newMethodPre, MethodInfo newMethodPost = null)
        {
            GetHarmonyInstance().Patch(oldMethod, newMethodPre != null ? new HarmonyMethod(newMethodPre) : null, newMethodPost != null ? new HarmonyMethod(newMethodPost) : null, null);
        }
    }
}

