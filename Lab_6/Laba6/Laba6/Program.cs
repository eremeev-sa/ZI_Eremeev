using System;
using System.Security.Cryptography;
using System.Text;

class Program
{
	static void Main()
	{
		int A = 25, C = 37, T0 = 7, B = 256;
		byte[] C0 = GenerateIV(A, C, T0, B); // Генерация начального вектора

		Console.Write("Введите строку открытого текста: ");
		string plaintext = Console.ReadLine();

		// Генерация ключа
		byte[] key = Encoding.UTF8.GetBytes("12345678"); // 8 байт для DES

		Console.WriteLine("\nИзначальный ключ: " + ConvertToBinaryString(key));
		Console.WriteLine("Начальный вектор C0: " + ConvertToBinaryString(C0));

		// Шифрование
		byte[] encrypted = EncryptDES(plaintext, key, C0);
		Console.WriteLine("\nШифртекст (в base64): " + Convert.ToBase64String(encrypted));

		// Дешифрование
		string decrypted = DecryptDES(encrypted, key, C0);
		Console.WriteLine("Дешифрованный текст: " + decrypted);
	}

	static byte[] GenerateIV(int A, int C, int T0, int B)
	{
		byte[] iv = new byte[8]; // 64-битовый начальный вектор
		int gamma = T0;
		for (int i = 0; i < 8; i++)
		{
			gamma = (A * gamma + C) % B;
			iv[i] = (byte)gamma;
		}
		return iv;
	}

	static byte[] EncryptDES(string plaintext, byte[] key, byte[] iv)
	{
		using (DES des = DES.Create())
		{
			des.Key = key;
			des.IV = iv;
			des.Mode = CipherMode.CBC;
			des.Padding = PaddingMode.PKCS7;

			ICryptoTransform encryptor = des.CreateEncryptor();
			byte[] plaintextBytes = Encoding.UTF8.GetBytes(plaintext);
			return encryptor.TransformFinalBlock(plaintextBytes, 0, plaintextBytes.Length);
		}
	}

	static string DecryptDES(byte[] ciphertext, byte[] key, byte[] iv)
	{
		using (DES des = DES.Create())
		{
			des.Key = key;
			des.IV = iv;
			des.Mode = CipherMode.CBC;
			des.Padding = PaddingMode.PKCS7;

			ICryptoTransform decryptor = des.CreateDecryptor();
			byte[] plaintextBytes = decryptor.TransformFinalBlock(ciphertext, 0, ciphertext.Length);
			return Encoding.UTF8.GetString(plaintextBytes);
		}
	}

	static string ConvertToBinaryString(byte[] data)
	{
		StringBuilder binaryString = new StringBuilder();
		foreach (byte b in data)
		{
			binaryString.Append(Convert.ToString(b, 2).PadLeft(8, '0')).Append(" ");
		}
		return binaryString.ToString().Trim();
	}
}
