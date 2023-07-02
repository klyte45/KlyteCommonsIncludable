using Klyte.Commons.Extensions;
using System;
using System.Collections.Generic;

namespace Klyte.Commons.Tools
{
    public static class ToolControllerUtil
    {
        public static T AddExtraToolToController<T>(this ToolController toolController)
            where T : ToolBase
        {
            if (toolController == null)
            {
                throw new ArgumentException("Tool controller not found!");
            }

            var tool = toolController.gameObject.AddComponent<T>();
            var fieldInfo = typeof(ToolController).GetField("m_tools", Patcher.allFlags);
            var tools = (ToolBase[])fieldInfo.GetValue(toolController);
            var initialLength = tools.Length;
            Array.Resize(ref tools, initialLength + 1);
            var dictionary =
                (Dictionary<Type, ToolBase>)typeof(ToolsModifierControl).GetField("m_Tools", Patcher.allFlags)
                    .GetValue(null);
            dictionary.Add(tool.GetType(), tool);
            tools[initialLength] = tool;

            fieldInfo.SetValue(toolController, tools);
            return tool;
        }
    }
}
