using System;
using System.Security.Cryptography;
using System.Text;

class Program
{
	static void Main()
	{
		Console.Write("Введите строку открытого текста: ");
		string plaintext = Console.ReadLine() ?? string.Empty;

		byte[] iv = GenerateIV();
		Console.WriteLine("Сгенерированный IV (в двоичном виде): " + ToBinaryString(iv));

		byte[] key = GenerateKey();
		Console.WriteLine("Сгенерированный ключ (в двоичном виде): " + ToBinaryString(key));

		byte[] encrypted = EncryptDES(plaintext, key, iv);
		string encryptedText = Convert.ToBase64String(encrypted);
		Console.WriteLine("Шифртекст (в base64): " + encryptedText);

		string decrypted = DecryptDES(Convert.FromBase64String(encryptedText), key, iv);
		Console.WriteLine("Дешифрованный текст: " + decrypted);
	}

	static byte[] GenerateIV()
	{
		int A = 25, C = 37, T0 = 7, B = 256;
		byte[] iv = new byte[8];
		int gamma = T0;
		for (int i = 0; i < iv.Length; i++)
		{
			gamma = (A * gamma + C) % B;
			iv[i] = (byte)gamma;
		}
		return iv;
	}

	static byte[] GenerateKey()
	{
		byte[] key = new byte[8];
		RandomNumberGenerator.Fill(key);
		return key;
	}

	static byte[] EncryptDES(string plaintext, byte[] key, byte[] iv)
	{
		using (var des = DES.Create())
		{
			des.Key = key;
			des.IV = iv;
			des.Mode = CipherMode.CBC;
			des.Padding = PaddingMode.PKCS7;

			byte[] plaintextBytes = Encoding.UTF8.GetBytes(plaintext);

			using (var encryptor = des.CreateEncryptor())
			{
				return encryptor.TransformFinalBlock(plaintextBytes, 0, plaintextBytes.Length);
			}
		}
	}

	static string DecryptDES(byte[] ciphertext, byte[] key, byte[] iv)
	{
		using (var des = DES.Create())
		{
			des.Key = key;
			des.IV = iv;
			des.Mode = CipherMode.CBC;
			des.Padding = PaddingMode.PKCS7;

			using (var decryptor = des.CreateDecryptor())
			{
				byte[] decryptedBytes = decryptor.TransformFinalBlock(ciphertext, 0, ciphertext.Length);
				return Encoding.UTF8.GetString(decryptedBytes);
			}
		}
	}

	static string ToBinaryString(byte[] byteArray)
	{
		StringBuilder binaryString = new StringBuilder(byteArray.Length * 8);
		foreach (byte b in byteArray)
		{
			binaryString.Append(Convert.ToString(b, 2).PadLeft(8, '0') + " ");
		}
		return binaryString.ToString().Trim();
	}
}
