using Harmony;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using UnityEngine;

namespace Klyte.Commons.Utils
{
    public class LogUtils
    {
        #region Log Utils

        public static void DoLog(string format, params object[] args)
        {
            try
            {
                if (CommonProperties.DebugMode)
                {
                    Console.WriteLine($"{CommonProperties.Acronym}v" + CommonProperties.Version + " " + format, args);
                }

            }
            catch
            {
                Debug.LogErrorFormat($"{CommonProperties.Acronym}: Erro ao fazer log: {{0}} (args = {{1}})", format, args == null ? "[]" : string.Join(",", args.Select(x => x != null ? x.ToString() : "--NULL--").ToArray()));
            }
        }
        public static void DoErrorLog(string format, params object[] args)
        {
            try
            {
                Console.WriteLine($"{CommonProperties.Acronym}v" + CommonProperties.Version + " " + format, args);
            }
            catch
            {
                Debug.LogErrorFormat($"{CommonProperties.Acronym}: Erro ao fazer log: {{0}} (args = {{1}})", format, args == null ? "[]" : string.Join(",", args.Select(x => x != null ? x.ToString() : "--NULL--").ToArray()));
            }
        }

        public static void PrintMethodIL(List<CodeInstruction> inst)
        {
            if (CommonProperties.DebugMode)
            {
                int j = 0;
                LogUtils.DoLog($"TRANSPILLED:\n\t{string.Join("\n\t", inst.Select(x => $"{(j++).ToString("D8")} {x.opcode.ToString().PadRight(10)} {ParseOperand(inst, x.operand)}").ToArray())}");
            }
        }


        private static string ParseOperand(List<CodeInstruction> instr, object operand)
        {
            if (operand is null)
            {
                return null;
            }

            if (operand is Label lbl)
            {
                return "LBL: " + instr.Select((x, y) => Tuple.New(x, y)).Where(x => x.First.labels.Contains(lbl)).Select(x => $"{x.Second.ToString("D8")} {x.First.opcode.ToString().PadRight(10)} {ParseOperand(instr, x.First.operand)}").FirstOrDefault();
            }
            else
            {
                return operand.ToString();
            }
        }
        #endregion
    }
}
