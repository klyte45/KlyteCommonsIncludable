using ColossalFramework.UI;
using HarmonyLib;
using Klyte.Commons.Extensions;
using Klyte.Commons.Interfaces;
using Klyte.Commons.Utils;
using System;
using static Klyte.Commons.Extensions.Patcher;

namespace Klyte.Commons.Redirectors
{
    public class UIViewRedirector : Patcher, IPatcher
    {
        [HarmonyPatch(typeof(UIView), "Start")]
        [HarmonyPostfix]
        public static void AfterStart()
        {
            System.Collections.Generic.List<Type> impls = ReflectionUtils.GetInterfaceImplementations(typeof(IViewStartActions), typeof(UIViewRedirector));
            foreach (Type impl in impls)
            {
                var inst =impl.GetConstructor(new Type[0])?.Invoke(new object[0]) as IViewStartActions;
                inst?.OnViewStart();
            }
        }
    }
}