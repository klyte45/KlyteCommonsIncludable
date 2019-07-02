using System;
using System.Text;

namespace Klyte.Commons.Utils
{
    public class NumberingUtils
    {
        #region Numbering Utils
        public static string ToRomanNumeral(ushort value)
        {
            if (value < 0)
                throw new ArgumentOutOfRangeException("Please use a positive integer greater than zero.");

            StringBuilder sb = new StringBuilder();
            if (value >= 4000)
            {
                RomanizeCore(sb, (ushort)(value / 1000));
                sb.Append("·");
                value %= 1000;
            }
            RomanizeCore(sb, value);

            return sb.ToString();
        }
        private static ushort RomanizeCore(StringBuilder sb, ushort remain)
        {
            while (remain > 0)
            {
                if (remain >= 1000)
                {
                    sb.Append("Ⅿ");
                    remain -= 1000;
                }
                else if (remain >= 900)
                {
                    sb.Append("ⅭⅯ");
                    remain -= 900;
                }
                else if (remain >= 500)
                {
                    sb.Append("Ⅾ");
                    remain -= 500;
                }
                else if (remain >= 400)
                {
                    sb.Append("ⅭⅮ");
                    remain -= 400;
                }
                else if (remain >= 100)
                {
                    sb.Append("Ⅽ");
                    remain -= 100;
                }
                else if (remain >= 90)
                {
                    sb.Append("ⅩⅭ");
                    remain -= 90;
                }
                else if (remain >= 50)
                {
                    sb.Append("Ⅼ");
                    remain -= 50;
                }
                else if (remain >= 40)
                {
                    sb.Append("ⅩⅬ");
                    remain -= 40;
                }
                else if (remain >= 13)
                {
                    sb.Append("Ⅹ");
                    remain -= 10;
                }
                else
                {
                    switch (remain)
                    {
                        case 12:
                            sb.Append("Ⅻ");
                            break;
                        case 11:
                            sb.Append("Ⅺ");
                            break;
                        case 10:
                            sb.Append("Ⅹ");
                            break;
                        case 9:
                            sb.Append("Ⅸ");
                            break;
                        case 8:
                            sb.Append("Ⅷ");
                            break;
                        case 7:
                            sb.Append("Ⅶ");
                            break;
                        case 6:
                            sb.Append("Ⅵ");
                            break;
                        case 5:
                            sb.Append("Ⅴ");
                            break;
                        case 4:
                            sb.Append("Ⅳ");
                            break;
                        case 3:
                            sb.Append("Ⅲ");
                            break;
                        case 2:
                            sb.Append("Ⅱ");
                            break;
                        case 1:
                            sb.Append("Ⅰ");
                            break;
                    }
                    remain = 0;
                }
            }

            return remain;
        }
        public static string GetStringFromNumber(string[] array, int number)
        {
            int arraySize = array.Length;
            string saida = "";
            while (number > 0)
            {
                int idx = (number - 1) % arraySize;
                saida = "" + array[idx] + saida;
                if (number % arraySize == 0)
                {
                    number /= arraySize;
                    number--;
                }
                else
                {
                    number /= arraySize;
                }

            }
            return saida;
        }
        #endregion
    }
}
