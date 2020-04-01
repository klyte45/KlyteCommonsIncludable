using Harmony;
using Klyte.Commons.Utils;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Reflection.Emit;
using UnityEngine;

namespace Klyte.Commons.Extensors
{
    public sealed class RedirectorUtils
    {
        public static readonly BindingFlags allFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly | BindingFlags.GetField | BindingFlags.GetProperty;
    }

    public interface IRedirectable
    {
        Redirector RedirectorInstance { get; }
    }

    public class Redirector : MonoBehaviour
    {
        #region Class Base
        private static readonly HarmonyInstance m_harmony = HarmonyInstance.Create($"com.klyte.redirectors.{CommonProperties.Acronym}");
        private static readonly List<MethodInfo> m_patches = new List<MethodInfo>();

        private readonly List<DynamicMethod> m_detourList = new List<DynamicMethod>();


        public HarmonyInstance GetHarmonyInstance() => m_harmony;
        #endregion

        public static readonly MethodInfo semiPreventDefaultMI = new Func<bool>(() =>
        {
            var stackTrace = new StackTrace();
            StackFrame[] stackFrames = stackTrace.GetFrames();
            LogUtils.DoLog($"SemiPreventDefault fullStackTrace: \r\n {Environment.StackTrace}");
            for (int i = 2; i < stackFrames.Length; i++)
            {
                if (stackFrames[i].GetMethod().DeclaringType.ToString().StartsWith("Klyte."))
                {
                    return false;
                }
            }
            return true;
        }).Method;

        public static bool PreventDefault() => false;

        public void AddRedirect(MethodInfo oldMethod, MethodInfo newMethodPre, MethodInfo newMethodPost = null, MethodInfo transpiler = null)
        {

            LogUtils.DoLog($"Adding patch! {oldMethod}");
            m_detourList.Add(GetHarmonyInstance().Patch(oldMethod, newMethodPre != null ? new HarmonyMethod(newMethodPre) : null, newMethodPost != null ? new HarmonyMethod(newMethodPost) : null, transpiler != null ? new HarmonyMethod(transpiler) : null));
            m_patches.Add(oldMethod);
        }

        public static void UnpatchAll()
        {
            LogUtils.DoLog($"Unpatching all: {m_harmony.Id}");
            foreach (MethodInfo method in m_patches)
            {
                m_harmony.Unpatch(method, HarmonyPatchType.All, m_harmony.Id);
            }
            m_patches.Clear();
        }

        public void EnableDebug() => HarmonyInstance.DEBUG = true;
        public void DisableDebug() => HarmonyInstance.DEBUG = false;
    }
}

