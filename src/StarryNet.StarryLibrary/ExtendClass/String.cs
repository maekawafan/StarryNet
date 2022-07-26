using StarryLibrary;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace StarryNet.StarryLibrary
{
    public static class StringEx
    {
        public static IEnumerator<string> SplitLine(this string value)
        {
            foreach (string line in value.Split('\n'))
            {
                yield return line;
            }
        }

        public static string GetLine(this string value, int index)
        {
            var lineIndex = GetLineIndex(value, index);
            if (lineIndex.index < 0 || lineIndex.length <= 0)
                return string.Empty;
            return value.Substring(lineIndex.index, lineIndex.length);
        }

        public static (int index, int length) GetLineIndex(this string value, int index)
        {
            if (index < 0 || index > value.Length)
                return (-1, -1);

            int resultIndex = Math.Max(0, value.Substring(0, index).LastIndexOf('\n') + 1);
            int length = value.Substring(index, value.Length - index).IndexOf('\n');
            if (length == -1)
                length = value.Length - resultIndex;
            else
                length += index + 1 - resultIndex;
            return (resultIndex, length);
        }

        public static string Fill(this char character, int count)
        {
            StringBuilder stringBuilder = new StringBuilder(count);
            for (int i = 0; i < count; i++)
                stringBuilder.Append(character);
            return stringBuilder.ToString();
        }

        public static string Fill(this string value, char character, int count)
        {
            StringBuilder stringBuilder = new StringBuilder(value);
            for (int i = 0; i < count; i++)
                stringBuilder.Append(character);
            return stringBuilder.ToString();
        }

        /// <summary>
        /// 문자열의 파스칼 케이스를 리턴합니다.
        /// Ex ) testMessage > TestMessage
        /// </summary>
        /// <param name="text">변환할 문자열</param>
        public static string ToPascalCase(this string text)
        {
            string result = text.Insert(0, text[0].ToString().ToUpper());
            return result.Remove(1, 1);
        }

        /// <summary>
        /// 문자열의 카멜 케이스를 리턴합니다.
        /// Ex ) TestMessage > testMessage
        /// </summary>
        /// <param name="text">변환할 문자열</param>
        public static string ToCamelCase(this string text)
        {
            string result = text.Insert(0, text[0].ToString().ToLower());
            return result.Remove(1, 1);
        }

        /// <summary>
        /// 문자열이 비어있는지 여부를 리턴합니다.
        /// </summary>
        /// <param name="text">비어있는지 여부를 검사할 문자열</param>
        static public bool IsEmpty(this string text)
        {
            return text.Length == 0;
        }

        static public bool IsNullOrEmpty(this string text)
        {
            return text == null || text.Length == 0;
        }

        /// <summary>
        /// 문자열을 특정 문자열로 채웁니다.
        /// </summary>
        /// <param name="count">채우는 문자 갯수</param>
        public static string Fill(this string text, int count)
        {
            string result = string.Empty;
            for (int i = 0; i < count; i++)
                result += text;
            return result;
        }

        /// <summary>
        /// 문자열의 왼쪽 일부를 잘라낸 문자열을 리턴합니다.
        /// </summary>
        /// <param name="length">잘라낼 길이</param>
        /// <returns></returns>
        static public string CropLeft(this string text, int length)
        {
            int realLength = Math.Min(text.Length, length);
            return text.Substring(realLength, text.Length - realLength);
        }

        /// <summary>
        /// 문자열의 오른쪽 일부를 잘라낸 문자열을 리턴합니다.
        /// </summary>
        /// <param name="length">잘라낼 길이</param>
        /// <returns></returns>
        static public string CropRight(this string text, int length)
        {
            int realLength = Math.Min(text.Length, length);
            return text.Substring(0, text.Length - realLength);
        }

        /// <summary>
        /// 문자열을 바이트 배열로 변환합니다.
        /// </summary>
        public static byte[] StringToByte(this string text)
        {
            return Encoding.UTF8.GetBytes(text);
        }

        /// <summary>
        /// 문자열에 문자가 포함되어 있는지 여부를 리턴합니다.
        /// </summary>
        public static bool Contains(this string text, char character)
        {
            return text.Contains(character.ToString());
        }

        /// <summary>
        /// 문자열을 특정 갯수로 나눕니다.
        /// </summary>
        public static string[] Divide(this string text, int count)
        {
            string[] result = new string[text.Length.CeilingDivide(count)];
            int index = 0;
            for (int i = 0; i < text.Length; i += count)
                result[index++] = text.Substring(i, count);
            return result;
        }

        public static string HexXOR(this string a, string b)
        {
            string value = a.Length > b.Length ? a : b;
            string key = a.Length > b.Length ? b : a;

            if (key.Length == 0)
                key = "0";

            StringBuilder stringBuilder = new StringBuilder();
            for (int i = 0; i < value.Length; i++)
            {
                int valueInt = Convert.ToInt32(value[i].ToString(), 16);
                int keyInt = Convert.ToInt32(key[i % key.Length].ToString(), 16);
                stringBuilder.Append((valueInt ^ keyInt).ToString("X"));
            }
            return stringBuilder.ToString();
        }

        public static string GetIP(this string address)
        {
            return address.Split(':')[0];
        }

        public static ushort GetPort(this string address)
        {
            return ushort.Parse(address.Split(':')[1]);
        }

        public static string GetIP(this EndPoint endPoint)
        {
            return endPoint.ToString().Split(':')[0];
        }

        public static ushort GetPort(this EndPoint endPoint)
        {
            return ushort.Parse(endPoint.ToString().Split(':')[1]);
        }
    }
}