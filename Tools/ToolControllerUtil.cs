using System;
using System.Collections.Generic;
using System.Reflection;

namespace Klyte.Commons
{
    public static class ToolControllerUtil
    {
        public static T AddExtraToolToController<T>(this ToolController toolController)
            where T : ToolBase
        {
            if (toolController is null)
            {
                throw new ArgumentException("Tool controller not found!");
            }

            var tool = toolController.gameObject.AddComponent<T>();
            var fieldInfo = typeof(ToolController).GetField("m_tools", BindingFlags.Instance | BindingFlags.NonPublic);
            var tools = (ToolBase[])fieldInfo.GetValue(toolController);
            if (tools is null)
            {
                throw new Exception("m_tools is null!");
            }
            var initialLength = tools.Length;
            Array.Resize(ref tools, initialLength + 1);
            tools[initialLength] = tool;
            fieldInfo.SetValue(toolController, tools);
            ToolsModifierControl.GetCurrentTool<DefaultTool>();
            var dictionary =
                (Dictionary<Type, ToolBase>)typeof(ToolsModifierControl).GetField("m_Tools", BindingFlags.Static | BindingFlags.NonPublic)
                    .GetValue(null);
            dictionary[typeof(T)] = tool;

            return tool;
        }
    }
}
