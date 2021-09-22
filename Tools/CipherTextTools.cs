using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace WebApiPrototype2.Tools
{
    /// <summary>
    /// Clase estática que provee métodos para encriptar y desencriptar cadenas
    /// de texto en base a una llave simétrica privada.
    /// </summary>
    public static class CipherTextTools
    {
        #region -- CONSTANTES Y CAMPOS --------------------------------------->
        private const string WAP2_PRIVATE_KEY_ENVIRONMENT_VARIABLE = "wap2";
        #endregion

        #region -- MÉTODOS PARA ENCRIPTAR Y DESENCRIPTAR CADENAS DE TEXTO ---->
        /// <summary>
        /// Método para encriptar una cadena de texto plano
        /// dada en base a una llave simetrica privada.
        /// </summary>
        /// <param name="plainText"></param>
        /// <returns></returns>
        public static string CipherText(string plainText)
        {
            // Obtener la llave simétrica desde las variables de entorno.
            string encryptionKey = GetEncryptionKeyFromEnvironmentVariables();
            byte[] iv = new byte[16];
            byte[] array = null;
            if (encryptionKey != null)
            {
                using (Aes aes = Aes.Create())
                {
                    aes.Key = Encoding.UTF8.GetBytes(encryptionKey);
                    aes.IV = iv;

                    ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);

                    using (MemoryStream memoryStream = new MemoryStream())
                    {
                        using (CryptoStream cryptoStream = new CryptoStream((Stream)memoryStream, encryptor, CryptoStreamMode.Write))
                        {
                            using (StreamWriter streamWriter = new StreamWriter((Stream)cryptoStream))
                            {
                                streamWriter.Write(plainText);
                            }
                            array = memoryStream.ToArray();
                        }
                    }
                }
            }
            return Convert.ToBase64String(array);
        }

        /// <summary>
        /// Método para desencriptar una cadena de texto encriptada en base
        /// a una llave simétrica privada.
        /// </summary>
        /// <param name="cipherText"></param>
        /// <returns></returns>
        public static string DecipherText(string cipherText)
        {
            // Obtener la llave simétrica desde las variables de entorno.
            string encryptionKey = GetEncryptionKeyFromEnvironmentVariables();
            byte[] iv = new byte[16];
            byte[] buffer = Convert.FromBase64String(cipherText);

            using (Aes aes = Aes.Create())
            {
                aes.Key = Encoding.UTF8.GetBytes(encryptionKey);
                aes.IV = iv;
                ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);

                using (MemoryStream memoryStream = new MemoryStream(buffer))
                {
                    using (CryptoStream cryptoStream = new CryptoStream((Stream)memoryStream, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader streamReader = new StreamReader((Stream)cryptoStream))
                        {
                            return streamReader.ReadToEnd();
                        }
                    }
                }
            }
        }
        #endregion

        #region -- MÉTODOS DE SOPORTE ---------------------------------------->
        /// <summary>
        /// Método para obtener la llava simétrica de encriptado desde las va-
        /// riables de entorno del sistema operativo Windows.
        /// </summary>
        /// <returns></returns>
        private static string GetEncryptionKeyFromEnvironmentVariables()
        {
            // Verificar el sistema operativo.
            if (Environment.OSVersion.Platform == PlatformID.Win32NT)
            {
                string privateKeyString =
                    Environment
                    .GetEnvironmentVariable(WAP2_PRIVATE_KEY_ENVIRONMENT_VARIABLE);
                return privateKeyString;
            }
            return null;
        } 
        #endregion

    }
}
