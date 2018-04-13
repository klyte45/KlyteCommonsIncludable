using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using Klyte.Commons.Utils;
using System.Reflection.Emit;
using System.Security.Permissions;
using System.Runtime.CompilerServices;
using System.Threading;
using Klyte.Harmony;
using ColossalFramework;
using UnityEngine;
using System.Diagnostics;

namespace Klyte.Commons.Extensors
{
    public sealed class RedirectorUtils
    {
        public static readonly BindingFlags allFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly | BindingFlags.GetField | BindingFlags.GetProperty;
    }
    public abstract class Redirector<T> : MonoBehaviour where T : Redirector<T>
    {
        #region Class Base
        private readonly HarmonyInstance harmony = HarmonyInstance.Create("com.klyte.transportlinemanager." + typeof(T).Name);
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
        protected MethodInfo semiPreventDefaultMI = typeof(Redirector<T>).GetMethod("semiPreventDefault", allFlags);

        public void Awake()
        {
            AwakeBody();
            instance = this;
        }

        public abstract void AwakeBody();
        public abstract void doLog(string text, params object[] param);

        public static readonly BindingFlags allFlags = RedirectorUtils.allFlags;

        public void AddRedirect(MethodInfo oldMethod, MethodInfo newMethodPre, MethodInfo newMethodPost = null)
        {
            GetHarmonyInstance().Patch(oldMethod, newMethodPre != null ? new HarmonyMethod(newMethodPre) : null, newMethodPost != null ? new HarmonyMethod(newMethodPost) : null, null);
        }
    }
}

