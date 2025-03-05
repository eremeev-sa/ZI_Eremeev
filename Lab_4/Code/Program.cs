using System.Security.Cryptography;
using System.Text;


Console.Write("Введите строку: ");
string inputString = Console.ReadLine();

// Генерация DES-ключа
byte[] key = GenerateDESKey();
string keyBinary = ToBinaryString(key);
string truncatedKeyBinary = TruncateKeyTo56Bits(keyBinary);

// Вывод начальных данных
Console.WriteLine("Введённая строка: " + inputString);
Console.WriteLine("Генераторный ключ: " + keyBinary);
Console.WriteLine("Ключ после перестановки PC-1: " + truncatedKeyBinary + " Размер ключа: 56");

// Генерация раундовых ключей
string[] roundKeys = GenerateRoundKeys(truncatedKeyBinary);
for (int i = 0; i < roundKeys.Length; i++)
{
    Console.WriteLine($"K-{i + 1} ключ: {roundKeys[i]}");
}

// Шифрование текста
byte[] encryptedBytes = Encrypt(inputString, key);
string encryptedText = Convert.ToBase64String(encryptedBytes);
Console.WriteLine("Зашифрованный текст (Base64): " + encryptedText);

// Расшифрование текста
string decryptedText = Decrypt(encryptedBytes, key);
Console.WriteLine("Расшифрованный текст: " + decryptedText);

static byte[] GenerateDESKey()
{
    using (DESCryptoServiceProvider des = new DESCryptoServiceProvider())
    {
        des.GenerateKey();
        return des.Key;
    }
}

static string ToBinaryString(byte[] bytes)
{
    StringBuilder binaryString = new StringBuilder();
    foreach (byte b in bytes)
    {
        binaryString.Append(Convert.ToString(b, 2).PadLeft(8, '0'));
    }
    return binaryString.ToString();
}

static string TruncateKeyTo56Bits(string keyBinary)
{
    // Удаляем каждый 8-й бит (контроль четности)
    StringBuilder truncatedKey = new StringBuilder();
    for (int i = 0; i < keyBinary.Length; i++)
    {
        if ((i + 1) % 8 != 0) // Пропускаем каждый 8-й бит
        {
            truncatedKey.Append(keyBinary[i]);
        }
    }
    return truncatedKey.ToString();
}

static string[] GenerateRoundKeys(string truncatedKeyBinary)
{
    // Делим ключ на две части
    string C = truncatedKeyBinary.Substring(0, 28);
    string D = truncatedKeyBinary.Substring(28, 28);

    // Массив сдвигов для каждого раунда
    int[] shifts = { 1, 1, 2, 2, 2, 2, 2, 2, 1, 2, 2, 2, 2, 2, 2, 1 };

    string[] roundKeys = new string[16];
    for (int i = 0; i < 16; i++)
    {
        C = LeftShift(C, shifts[i]);
        D = LeftShift(D, shifts[i]);
        roundKeys[i] = C + D; // Объединяем для раундового ключа
    }

    return roundKeys;
}

static string LeftShift(string input, int shift)
{
    return input.Substring(shift) + input.Substring(0, shift);
}

static byte[] Encrypt(string plainText, byte[] key)
{
    using (DESCryptoServiceProvider des = new DESCryptoServiceProvider())
    {
        des.Key = key;
        des.Mode = CipherMode.ECB; // Можно выбрать другой режим, например, CBC
        des.Padding = PaddingMode.PKCS7;

        ICryptoTransform encryptor = des.CreateEncryptor();
        byte[] plainBytes = Encoding.UTF8.GetBytes(plainText);
        return encryptor.TransformFinalBlock(plainBytes, 0, plainBytes.Length);
    }
}

static string Decrypt(byte[] cipherBytes, byte[] key)
{
    using (DESCryptoServiceProvider des = new DESCryptoServiceProvider())
    {
        des.Key = key;
        des.Mode = CipherMode.ECB; // Должен совпадать с режимом шифрования
        des.Padding = PaddingMode.PKCS7;

        ICryptoTransform decryptor = des.CreateDecryptor();
        byte[] plainBytes = decryptor.TransformFinalBlock(cipherBytes, 0, cipherBytes.Length);
        return Encoding.UTF8.GetString(plainBytes);
    }
}
