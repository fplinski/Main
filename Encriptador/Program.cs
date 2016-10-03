﻿using System;
using System.Security.Cryptography;
using System.Text;
using System.IO;

namespace Encriptador
{
    class Program
    {
        static void Main(string[] args)
        {
            string input = "";
            Console.Write("Ingrese el valor a desencriptar: ");
            input = Console.ReadLine();

            string resultado = RijndaelSimple.Encriptar(input);

            Console.WriteLine("La contraseña desencriptada es: " + resultado);
            Console.ReadLine();
        }

        public static class RijndaelSimple
        {
            private const string passWordBase = "pass75dc@avz10";
            private const string saltValueBytes = "s@lAvz";
            private const string hashAlgorithm = "SHA1";
            private const string vectorInicial = "@1B2c3D4e5F6g7H8";
            private const int iteraciones = 10;
            private const int tamañodeLlave = 256;

            public static string Encriptar(string aEncriptar)
            {
                return Encriptar(aEncriptar, passWordBase, saltValueBytes, hashAlgorithm, iteraciones, vectorInicial, tamañodeLlave);
            }

            public static string Encriptar(string aEncriptar, string passBase, string saltValue, string hashAlgorithm, int passwordIterations, string initVector, int keySize)
            {

                byte[] initVectorBytes = Encoding.ASCII.GetBytes(initVector);
                byte[] saltValueBytes = Encoding.ASCII.GetBytes(saltValue);
                byte[] plainTextBytes = Encoding.UTF8.GetBytes(aEncriptar);

                PasswordDeriveBytes password = new PasswordDeriveBytes(passBase, saltValueBytes, hashAlgorithm, passwordIterations);

                byte[] keyBytes = password.GetBytes(keySize / 8);

                RijndaelManaged symmetricKey = new RijndaelManaged();
                symmetricKey.Mode = CipherMode.CBC;

                ICryptoTransform encryptor = symmetricKey.CreateEncryptor(keyBytes, initVectorBytes);

                MemoryStream memoryStream = new MemoryStream();

                CryptoStream cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write);

                cryptoStream.Write(plainTextBytes, 0, plainTextBytes.Length);

                cryptoStream.FlushFinalBlock();
                byte[] cipherTextBytes = memoryStream.ToArray();
                memoryStream.Close();
                cryptoStream.Close();
                string cipherText = Convert.ToBase64String(cipherTextBytes);
                return cipherText;
            }

            public static string Desencriptar(string textoEncriptado)
            {
                return Desencriptar(textoEncriptado, passWordBase, saltValueBytes, hashAlgorithm, iteraciones, vectorInicial, tamañodeLlave);
            }

            public static string Desencriptar(string textoEncriptado, string passBase, string saltValue, string hashAlgorithm, int passwordIterations, string initVector, int keySize)
            {
                try
                {
                    byte[] initVectorBytes = Encoding.ASCII.GetBytes(initVector);
                    byte[] saltValueBytes = Encoding.ASCII.GetBytes(saltValue);
                    byte[] cipherTextBytes = Convert.FromBase64String(textoEncriptado);

                    PasswordDeriveBytes password = new PasswordDeriveBytes(passBase, saltValueBytes, hashAlgorithm, passwordIterations);
                    byte[] keyBytes = password.GetBytes(keySize / 8);

                    RijndaelManaged symmetricKey = new RijndaelManaged();
                    symmetricKey.Mode = CipherMode.CBC;


                    ICryptoTransform decryptor = symmetricKey.CreateDecryptor(keyBytes, initVectorBytes);

                    MemoryStream memoryStream = new MemoryStream(cipherTextBytes);
                    CryptoStream cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read);

                    byte[] plainTextBytes = new byte[cipherTextBytes.Length];

                    int decryptedByteCount = cryptoStream.Read(plainTextBytes, 0, plainTextBytes.Length);

                    memoryStream.Close();

                    cryptoStream.Close();

                    string plainText = Encoding.UTF8.GetString(plainTextBytes, 0, decryptedByteCount);

                    return plainText;
                }
                catch (Exception ex)
                {
                    return "";
                }
            }
        }
    }
}
