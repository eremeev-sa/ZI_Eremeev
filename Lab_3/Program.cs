int A = 25, C = 37, T0 = 7, B = 256;

Console.Write("Введите строку открытого текста: ");
string strNach = Console.ReadLine();

string encryptedText = Encrypt(strNach, A, C, T0, B, out int[] gamma, out int[] kodascii, out int[] encryptedCodes);
Console.WriteLine("\nШифртекст: " + encryptedText + " ");

string decryptedText = Decrypt(encryptedText, A, C, T0, B, gamma, out int[] decryptedCodes);
Console.WriteLine("Дешифрованный текст: " + decryptedText + " ");

Console.WriteLine("\nТаблица шифрования/дешифрования:");
Console.WriteLine("---------------------------------------------------------------------------------");
Console.WriteLine("| Индекс | Символ | Код ASCII | Гамма | Код XOR | Символ (шифр.) |");
Console.WriteLine("-------------------------------------------------------------------------------");

for (int i = 0; i < strNach.Length; i++)
{
    // Здесь берем символы из шифрованного текста, которые были получены
    char encryptedChar = encryptedText[i];
    
    // Если символ шифртекста не печатный, выводим его как числовое значение в шестнадцатеричной форме
    string encryptedCharDisplay = IsPrintableChar(encryptedCodes[i]) 
        ? encryptedChar.ToString() 
        : $"0x{encryptedCodes[i]:X2}"; 

    Console.WriteLine($"| {i,6} | {strNach[i],6} | {kodascii[i],9} | {gamma[i],5} | {encryptedCodes[i],7} | {encryptedCharDisplay,15}|");
}
Console.WriteLine("-------------------------------------------------------------------");

static bool IsPrintableChar(int code)
{
    return code >= 32 && code <= 126;
}

static string Encrypt(string text, int A, int C, int T0, int B, out int[] gamma, out int[] kodascii, out int[] XOR)
{
    int numChar = text.Length;
    kodascii = new int[numChar];
    gamma = new int[numChar];
    XOR = new int[numChar];
    string encrypted = "";

    for (int i = 0; i < numChar; i++)
    {
        kodascii[i] = (int)text[i];
    }

    gamma[0] = T0;
    for (int i = 1; i < numChar; i++)
    {
        gamma[i] = (A * gamma[i - 1] + C) % B;
    }

    for (int i = 0; i < numChar; i++)
    {
        XOR[i] = (kodascii[i] + gamma[i]) % 256;
    }

    for (int i = 0; i < numChar; i++)
    {
        encrypted += (char)XOR[i];
    }

    return encrypted;
}

static string Decrypt(string text, int A, int C, int T0, int B, int[] gamma, out int[] decryptedCodes)
{
    int numChar = text.Length;
    int[] kodascii = new int[numChar];
    decryptedCodes = new int[numChar];
    string decrypted = "";

    for (int i = 0; i < numChar; i++)
    {
        kodascii[i] = (int)text[i];
    }

    for (int i = 0; i < numChar; i++)
    {
        decryptedCodes[i] = (kodascii[i] - gamma[i] + 256) % 256;
    }

    for (int i = 0; i < numChar; i++)
    {
        decrypted += (char)decryptedCodes[i];
    }

    return decrypted;
}

