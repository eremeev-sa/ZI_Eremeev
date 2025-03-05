using System.Globalization;

List<char> originalLetters = new List<char>{
	'А', 'Б', 'В', 'Г', 'Д', 'Е', 'Ё', 'Ж', 'З','И', 'Й', 'К', 'Л', 'М', 'Н', 'О', 'П', 'Р','С', 'Т', 'У', 'Ф', 'Х', 'Ц', 'Ч', 'Ш', 'Щ', 'Ы', 'Ь', 'Э', 'Ю', 'Я', ' ',
};

List<char> encryptedLetters = new List<char>{
	'Я', 'Ю', 'Э', 'Ь', 'Ы', 'Щ', 'Ш', 'Ч', 'Ц', 'Х', 'Ф','У', 'Т', 'С', 'Р', 'П','О', 'Н', 'М', 'Л', 'К','Й', 'И','З', 'Ж', 'Ё','Е', 'Д', 'Г', 'В', 'Б','А', '_',
};

Console.Write("Введите текст: ");
string text = Console.ReadLine()?.ToUpper() ?? string.Empty;

static string EncrypText(string text, List<char> originalLetters, List<char> encryptedLetters)
{
	char[] encryptedText = new char[text.Length];

	for (int i = 0; i < text.Length; i++)
	{
		char currentChar = text[i];
		int index = originalLetters.IndexOf(currentChar);
		encryptedText[i] = encryptedLetters[index];
	}

	return new string(encryptedText);
}

string text2 = EncrypText(text, originalLetters, encryptedLetters);

Console.WriteLine($"Исходный текст: {text}");
Console.WriteLine($"Зашифрованный текст: {text2}");


