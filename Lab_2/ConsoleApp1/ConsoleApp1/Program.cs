Console.Write("Введите текст: ");
string inputText = Console.ReadLine().ToUpper();

string text = inputText.Replace(' ', '_');
List<char> list = new List<char>(text.ToCharArray());

while (list.Count < 17)
{
	list.Add('_');
}
    
    static string Encrypt(string text)
{
	char[] chars = text.ToCharArray();

	for (int i = 0; i < chars.Length - 1; i += 2)
	{
		char temp = chars[i];
		chars[i] = chars[i + 1];
		chars[i + 1] = temp;
	}

	return new string(chars);
}

static string Decrypt(string text)
{
	char[] chars = text.ToCharArray();

	for (int i = 0; i < chars.Length - 1; i += 2)
	{
		char temp = chars[i];
		chars[i] = chars[i + 1];
		chars[i + 1] = temp;
	}

	return new string(chars);
}

text = new string(list.ToArray());
Console.WriteLine($"Исходный текст: {text}");

string encryptedText = Encrypt(text);
Console.WriteLine($"Зашифрованный текст: {encryptedText}");

string decryptedText = Decrypt(encryptedText);
decryptedText = decryptedText.Replace('_', ' ');
Console.WriteLine($"Расшифрованный текст: {decryptedText}");