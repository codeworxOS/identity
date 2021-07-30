using System;
using System.Text;

namespace Codeworx.Identity.Internal
{
    public static class Base64UrlEncoding
    {
        private static char _base64Character62 = '+';
        private static char _base64Character63 = '/';
        private static char _base64PadCharacter = '=';
        private static char _base64UrlCharacter62 = '-';
        private static char _base64UrlCharacter63 = '_';

        public static string Decode(string arg)
        {
            return Encoding.UTF8.GetString(DecodeBytes(arg));
        }

        public static byte[] DecodeBytes(string str)
        {
            return UnsafeDecode(str);
        }

        public static string Encode(string arg)
        {
            return Encode(Encoding.UTF8.GetBytes(arg));
        }

        public static string Encode(byte[] inArray, int offset, int length)
        {
            return EncodeString(Convert.ToBase64String(inArray, offset, length));
        }

        public static string Encode(byte[] inArray)
        {
            return EncodeString(Convert.ToBase64String(inArray, 0, inArray.Length));
        }

        internal static string EncodeString(string str)
        {
            return UnsafeEncode(str);
        }

        private static unsafe byte[] UnsafeDecode(string str)
        {
            int mod = str.Length % 4;

            bool needReplace = false;
            int decodedLength = str.Length + ((4 - mod) % 4);

            for (int i = 0; i < str.Length; i++)
            {
                if (str[i] == _base64UrlCharacter62 || str[i] == _base64UrlCharacter63)
                {
                    needReplace = true;
                    break;
                }
            }

            if (needReplace)
            {
                string decodedString = new string(char.MinValue, decodedLength);
                fixed (char* dest = decodedString)
                {
                    int i = 0;
                    for (; i < str.Length; i++)
                    {
                        if (str[i] == _base64UrlCharacter62)
                        {
                            dest[i] = _base64Character62;
                        }
                        else if (str[i] == _base64UrlCharacter63)
                        {
                            dest[i] = _base64Character63;
                        }
                        else
                        {
                            dest[i] = str[i];
                        }
                    }

                    for (; i < decodedLength; i++)
                    {
                        dest[i] = _base64PadCharacter;
                    }
                }

                return Convert.FromBase64String(decodedString);
            }
            else
            {
                if (decodedLength == str.Length)
                {
                    return Convert.FromBase64String(str);
                }
                else
                {
                    string decodedString = new string(char.MinValue, decodedLength);
                    fixed (char* src = str)
                    {
                        fixed (char* dest = decodedString)
                        {
                            Buffer.MemoryCopy(src, dest, str.Length * 2, str.Length * 2);
                            dest[str.Length] = _base64PadCharacter;
                            if (str.Length + 2 == decodedLength)
                            {
                                dest[str.Length + 1] = _base64PadCharacter;
                            }
                        }
                    }

                    return Convert.FromBase64String(decodedString);
                }
            }
        }

        private static unsafe string UnsafeEncode(string str)
        {
            bool needReplace = false;
            int reductionSize = 0;
            if (str[str.Length - 1] == _base64PadCharacter)
            {
                reductionSize = 1;
            }

            if (str[str.Length - 2] == _base64PadCharacter)
            {
                reductionSize = 2;
            }

            int encodedLength = str.Length - reductionSize;
            for (int i = 0; i < str.Length; i++)
            {
                if (str[i] == _base64Character62 || str[i] == _base64Character63)
                {
                    needReplace = true;
                    break;
                }
            }

            if (needReplace)
            {
                string encodedString = new string(char.MinValue, encodedLength);
                fixed (char* dest = encodedString)
                {
                    for (int i = 0; i < encodedLength; i++)
                    {
                        if (str[i] == _base64Character62)
                        {
                            dest[i] = _base64UrlCharacter62;
                        }
                        else if (str[i] == _base64Character63)
                        {
                            dest[i] = _base64UrlCharacter63;
                        }
                        else
                        {
                            dest[i] = str[i];
                        }
                    }
                }

                return encodedString;
            }
            else
            {
                if (encodedLength == str.Length)
                {
                    return str;
                }
                else
                {
                    string encodedString = new string(char.MinValue, encodedLength);
                    fixed (char* src = str)
                    {
                        fixed (char* dest = encodedString)
                        {
                            Buffer.MemoryCopy(src, dest, encodedLength * 2, encodedLength * 2);
                        }
                    }

                    return encodedString;
                }
            }
        }
    }
}