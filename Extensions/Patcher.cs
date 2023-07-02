using HarmonyLib;
using Klyte.Commons.Utils;
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace Klyte.Commons.Extensions
{
	public class Patcher : MonoBehaviour
    {
        private const string HarmonyId = "com.klyte.redirectors.TLM";

        private static bool patched = false;

        public static BindingFlags allFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static | BindingFlags.GetField | BindingFlags.GetProperty | BindingFlags.SetProperty | BindingFlags.SetField;

        public interface IPatcher
        {
        }

        public static void PatchAll()
        {
            if (patched) return;

            Debug.Log("com.klyte.redirectors.TLM: Patching...");

            patched = true;
            GameObject m_topObj = GameObject.Find("k45_Patches") ?? new GameObject("k45_Patches");
            Type typeTarg = typeof(IPatcher);
            List<Type> instances = ReflectionUtils.GetInterfaceImplementations(typeTarg, typeTarg);
            try
            {
                foreach (Type t in instances)
                {
                    LogUtils.DoLog($"Patch: {t}");
                    m_topObj.AddComponent(t);

                }
            }
            finally
            {
                
            }
            // Apply your patches here!
            // Harmony.DEBUG = true;
            var harmony = new Harmony("com.klyte.redirectors.TLM");
            harmony.PatchAll(Assembly.GetExecutingAssembly());
        }

        public static void UnpatchAll()
        {
            if (!patched) return;

            var harmony = new Harmony(HarmonyId);
            harmony.UnpatchAll(HarmonyId);

            patched = false;

            UnityEngine.Debug.Log("com.klyte.redirectors.TLM: Reverted...");
        }
    }
}
