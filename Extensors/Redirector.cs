using System;
using System.Reflection;
using UnityEngine;
using System.Diagnostics;
using Harmony;
using System.Reflection.Emit;
using System.Collections.Generic;
using Klyte.Commons.Utils;

namespace Klyte.Commons.Extensors
{
    public sealed class RedirectorUtils
    {
        public static readonly BindingFlags allFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly | BindingFlags.GetField | BindingFlags.GetProperty;
    }

    public interface IRedirectable
    {
        Redirector RedirectorInstance { get; set; }
    }

    public class Redirector : MonoBehaviour
    {
        #region Class Base
        private HarmonyInstance m_harmony ;

        private readonly List<DynamicMethod> m_detourList = new List<DynamicMethod>();


        public void Awake()
        {
            m_harmony = HarmonyInstance.Create($"com.klyte.redirectors.{transform.parent?.GetType()?.Name??"default"}");
        }

        public HarmonyInstance GetHarmonyInstance()
        {
            return m_harmony;
        }
        #endregion

        public readonly static MethodInfo semiPreventDefaultMI = new Func<bool>(() =>
        {
            StackTrace stackTrace = new StackTrace();
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

        public void AddRedirect(MethodInfo oldMethod, MethodInfo newMethodPre, MethodInfo newMethodPost = null, MethodInfo transpiler = null)
        {
            m_detourList.Add(GetHarmonyInstance().Patch(oldMethod, newMethodPre != null ? new HarmonyMethod(newMethodPre) : null, newMethodPost != null ? new HarmonyMethod(newMethodPost) : null, transpiler != null ? new HarmonyMethod(transpiler) : null));
        }

        public void OnDestroy()
        {
            foreach (var patch in m_detourList)
            {
                foreach (var method in patch.GetHarmonyMethods())
                {
                    GetHarmonyInstance().Unpatch(patch.GetBaseDefinition(), method.method);

                }

            }
        }

        public void EnableDebug()
        {
            HarmonyInstance.DEBUG = true;
        }
        public void DisableDebug()
        {
            HarmonyInstance.DEBUG = false;
        }
    }
}

