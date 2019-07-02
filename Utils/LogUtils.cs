using System;
using System.Linq;
using UnityEngine;

namespace Klyte.Commons.Utils
{
    public class LogUtils
    {
        #region Log Utils

        internal static void DoLog(string format, params object[] args)
        {
            try
            {
                if (KlyteCommonsMod.DebugMode)
                {
                    Debug.LogWarningFormat("KCv" + KlyteCommonsMod.Version + " " + format, args);
                }

            }
            catch
            {
                Debug.LogErrorFormat("KltUtils: Erro ao fazer log: {0} (args = {1})", format, args == null ? "[]" : string.Join(",", args.Select(x => x != null ? x.ToString() : "--NULL--").ToArray()));
            }
        }
        internal static void DoErrorLog(string format, params object[] args)
        {
            try
            {
                Console.WriteLine("KCv" + KlyteCommonsMod.Version + " " + format, args);
            }
            catch
            {
                Debug.LogErrorFormat("KltUtils: Erro ao fazer log: {0} (args = {1})", format, args == null ? "[]" : string.Join(",", args.Select(x => x != null ? x.ToString() : "--NULL--").ToArray()));
            }
        }
        #endregion
    }
}
