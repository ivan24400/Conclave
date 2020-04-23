using System;
using System.IO;
using System.Security.Cryptography;

namespace Conclave.Utils
{
    /*
     * A symmetric cryptography utility
     */
    /// <summary>
    /// AES is used as a primary algorithm for cryptographic operations
    /// </summary>
    internal static class Crypto
    {
        private static readonly byte[] Key = { 0x17, 0x41, 0x23, 0x31, 0x35, 0x43, 0x21, 0xaf, 0x4e, 0x5b, 0x7c, 0x8d, 0x1a, 0x3d, 0x67, 0x9e };
        private static readonly byte[] IV = { 0x8a, 0x25, 0xee, 0xf1, 0xa3, 0x8a, 0x71, 0xc3, 0xd2, 0xa1, 0x5e, 0x2f, 0x7e, 0x8d, 0x91, 0xf8 };
        internal static string Encrypt(string plainText)
        {
            // Check arguments.
            if (plainText == null || plainText.Length <= 0)
                throw new ArgumentNullException("plainText");

            byte[] encrypted;

            // Create an Aes object
            // with the specified key and IV.
            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = Key;
                aesAlg.IV = IV;

                // Create an encryptor to perform the stream transform.
                ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

                // Create the streams used for encryption.
                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                        {
                            //Write all data to the stream.
                            swEncrypt.Write(plainText);
                        }
                        encrypted = msEncrypt.ToArray();
                    }
                }
            }

            // Return the encrypted bytes from the memory stream.
            return Convert.ToBase64String(encrypted);
        }

        internal static string Decrypt(string cipherText)
        {
            byte[] cipherData = Convert.FromBase64String(cipherText);
            // Check arguments.
            if (cipherData == null || cipherData.Length <= 0)
                throw new ArgumentNullException("cipherText");

            // Declare the string used to hold
            // the decrypted text.
            string plaintext = null;

            // Create an Aes object
            // with the specified key and IV.
            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = Key;
                aesAlg.IV = IV;

                // Create a decryptor to perform the stream transform.
                ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

                // Create the streams used for decryption.
                using (MemoryStream msDecrypt = new MemoryStream(cipherData))
                {
                    using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                        {

                            // Read the decrypted bytes from the decrypting stream
                            // and place them in a string.
                            plaintext = srDecrypt.ReadToEnd();
                        }
                    }
                }
            }

            return plaintext;
        }
    }
}
