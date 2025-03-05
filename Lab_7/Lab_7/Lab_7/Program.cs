using System;
using System.Security.Cryptography;
using System.Text;

class Program
{
    // Значения для генерации случайных чисел
    static int A = 25, C = 37, T0 = 7, B = 256;

    static void Main(string[] args)
    {
        Console.WriteLine("Введите текст для шифрования:");
        string text = Console.ReadLine();

        // Генерируем начальный вектор C0 
        byte[] C0 = GenerateC0(8);
        Console.WriteLine("Начальный вектор C0: " + BitConverter.ToString(C0));

        // Ключ для DES
        byte[] key = new byte[8] { 0x13, 0x34, 0x57, 0x79, 0x9B, 0xBC, 0xDF, 0xF1 };
        Console.WriteLine("Ключ: " + BitConverter.ToString(key));
        
        int k = 8;

        // Шифруем текст
        string encryptedText = EncryptOFB(text, key, C0, k);
        Console.WriteLine("Зашифрованный текст: " + encryptedText);

        // Дешифруем текст
        string decryptedText = DecryptOFB(encryptedText, key, C0, k);
        Console.WriteLine("Расшифрованный текст: " + decryptedText);
    }

    // Метод для генерации начального вектора C0 с помощью ПСЧ из Лаб. №3
    static byte[] GenerateC0(int length)
    {
        byte[] C0 = new byte[length]; 
        int[] gamma = new int[length];

      
        gamma[0] = T0;
     
        for (int i = 1; i < length; i++)
        {
            gamma[i] = (A * gamma[i - 1] + C) % B;
        }
        
        for (int i = 0; i < length; i++)
        {
            C0[i] = (byte)gamma[i];
        }

        return C0;
    }

    // Метод для шифрования в режиме OFB
    static string EncryptOFB(string text, byte[] key, byte[] C0, int k)
    {
        byte[] inputBlock = (byte[])C0.Clone();
        byte[] textBytes = Encoding.ASCII.GetBytes(text);
        byte[] encryptedBytes = new byte[textBytes.Length];
        int kBytes = k / 8;

        // Создаём объект DES для шифрования
        using (DES des = DES.Create())
        {
            des.Key = key; 
            des.Mode = CipherMode.ECB; 
            des.Padding = PaddingMode.None; 

            ICryptoTransform encryptor = des.CreateEncryptor();
            
            for (int i = 0; i < textBytes.Length; i += kBytes)
            {
                byte[] outputBlock = new byte[8];
                encryptor.TransformBlock(inputBlock, 0, 8, outputBlock, 0);
                
                byte keystream = outputBlock[0];
                
                for (int j = 0; j < kBytes && (i + j) < textBytes.Length; j++)
                {
                    encryptedBytes[i + j] = (byte)(textBytes[i + j] ^ keystream);
                }

                inputBlock = (byte[])outputBlock.Clone();
            }
        }
        
        return BitConverter.ToString(encryptedBytes).Replace("-", "");
    }

    // Метод для дешифрования в режиме OFB
    static string DecryptOFB(string encryptedText, byte[] key, byte[] C0, int k)
    {
        byte[] encryptedBytes = new byte[encryptedText.Length / 2];
        for (int i = 0; i < encryptedBytes.Length; i++)
        {
            encryptedBytes[i] = Convert.ToByte(encryptedText.Substring(i * 2, 2), 16);
        }
        
        byte[] inputBlock = (byte[])C0.Clone();
        byte[] decryptedBytes = new byte[encryptedBytes.Length];
        int kBytes = k / 8; 
        
        using (DES des = DES.Create())
        {
            des.Key = key; 
            des.Mode = CipherMode.ECB; 
            des.Padding = PaddingMode.None;

            ICryptoTransform encryptor = des.CreateEncryptor();
            
            for (int i = 0; i < encryptedBytes.Length; i += kBytes)
            {
                byte[] outputBlock = new byte[8];
                encryptor.TransformBlock(inputBlock, 0, 8, outputBlock, 0);
                
                byte keystream = outputBlock[0]; // Первый байт — это наш keystream
                
                for (int j = 0; j < kBytes && (i + j) < encryptedBytes.Length; j++)
                {
                    decryptedBytes[i + j] = (byte)(encryptedBytes[i + j] ^ keystream);
                }
                
                inputBlock = (byte[])outputBlock.Clone();
            }
        }
        
        return Encoding.ASCII.GetString(decryptedBytes);
    }
}