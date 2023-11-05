using B83.Image.BMP;
using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using TWT.Networking.Client;
using TWT.Utility;
using UniRx;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Game.Client
{
    public class Utility
    {
        public static string FormatCode(string text, int code_length, int number_code_part, int code_part_length)
        {
            text = text.Replace("-", "");
            text = text.Replace(" ", "");
            for (int i = text.Length; i < code_length - code_part_length; i++)
            {
                text = text.Insert(text.Length, " ");
            }
            for (int i = 1; i <= number_code_part; i++)
            {
                var index = code_part_length * i + (i - 1);
                text = text.Insert(index, "-");
            }
            return text.ToUpper();
        }

        public static Texture2D LoadTextureFromBMPImage(string filePath)
        {
            Texture2D tex = null;

            if (File.Exists(filePath))
            {
                BMPLoader bmpLoader = new BMPLoader();
                //bmpLoader.ForceAlphaReadWhenPossible = true; //Uncomment to read alpha too

                //Load the BMP data
                BMPImage bmpImg = bmpLoader.LoadBMP(filePath);

                //Convert the Color32 array into a Texture2D
                tex = bmpImg.ToTexture2D();
            }
            return tex;
        }

        public static async UniTask<Texture> DownloadTextureAsync(string url, string savePath)
        {
            try
            {
                if (Path.GetExtension(url).Equals(VRAssetPath.BMP, StringComparison.OrdinalIgnoreCase))
                {
                    await ObservableUnityWebRequest.DownloadFileAsObservable(url, savePath);

                    Texture2D texture2D = new Texture2D(1, 1);
                    texture2D = LoadTextureFromBMPImage(savePath);
                    return texture2D;
                }
                return await ObservableUnityWebRequest.GetTexture2DAsObservable(url);
            }
            catch (Exception e)
            {
                DebugExtension.Log(e.Message);
                return null;
            }
        }

        public static bool GetServerStatus()
        {
#if ADMIN
            var new_status = CheckServerStatus.Instance.serverStatus;
            if (new_status == (int)ServerStatus.CANT_JOIN)
            {
                return false;
            }
#endif
            return true;
        }

        // make up keys for our encryption (k1 = 16-byte IV, k2 = 32-byte key)
        static private string k1 = "3812372841823172";
        static private string k2 = "94329842394934723847348723984324";
        /**
* Encrypt a string with AES (Rijndael)
*/
        public static string Encrypt(string data)
        {
            if (String.IsNullOrEmpty(data))
            {
                return "";
            }

            // plug in the keys
            var aes = Rijndael.Create();
            aes.Padding = PaddingMode.PKCS7;
            aes.IV = Encoding.ASCII.GetBytes(k1);
            aes.Key = Encoding.ASCII.GetBytes(k2);

            // create our streams
            var rawData = Encoding.Unicode.GetBytes(data);
            var memoryStream = new MemoryStream();
            var cryptoStream = new CryptoStream(memoryStream, aes.CreateEncryptor(), CryptoStreamMode.Write);

            // perform the encryption
            cryptoStream.Write(rawData, 0, rawData.Length);
            cryptoStream.FlushFinalBlock();
            cryptoStream.Close();

            // pull our encrypted bytes out
            var encrypted = memoryStream.ToArray();
            memoryStream.Close();

            // convert to string, return
            return System.Convert.ToBase64String(encrypted);
        }

        /**
        * Decrypt a string using AES
*/
        public static string Decrypt(string data)
        {
            if (String.IsNullOrEmpty(data))
            {
                return "";
            }

            var rawData = System.Convert.FromBase64String(data);
            if (rawData.Length < 8)
            {
                throw new System.ArgumentException("Invalid input data");
            }

            // plug in the keys
            var aes = Rijndael.Create();
            aes.Padding = PaddingMode.PKCS7;
            aes.IV = Encoding.ASCII.GetBytes(k1);
            aes.Key = Encoding.ASCII.GetBytes(k2);

            // decrypt the data
            var memoryStream = new MemoryStream();
            var cryptoStream = new CryptoStream(memoryStream, aes.CreateDecryptor(), CryptoStreamMode.Write);
            cryptoStream.Write(rawData, 0, rawData.Length);
            cryptoStream.FlushFinalBlock();
            cryptoStream.Close();

            var decrypted = memoryStream.ToArray();
            memoryStream.Close();

            return Encoding.Unicode.GetString(decrypted);
        }
    }
}
