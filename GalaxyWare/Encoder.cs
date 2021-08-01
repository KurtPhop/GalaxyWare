using System;
using System.Security.Cryptography;
using System.Text;

namespace GalaxyWare
{
    public static class Encoder
    {
        public static string Encode(string data)
        {
            string text = Encoder.smethod_0();
            string text2 = null;
            while (text.Length < data.Length)
            {
                text += text;
            }
            for (int i = 0; i < data.Length; i++)
            {
                text2 += (data[i] ^ text[i]).ToString();
            }
            return text2;
        }

        private static string smethod_0()
        {
            return BitConverter.ToString(new MD5CryptoServiceProvider().ComputeHash(Encoding.ASCII.GetBytes(Environment.UserName))).Replace("-", string.Empty);
        }
    }
}
