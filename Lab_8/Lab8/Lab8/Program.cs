using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

class Program
{
    static List<string> PreprocessContainer(string text)
    {
        List<string> sentences = new List<string>();
        string currentSentence = "";
        
        for (int i = 0; i < text.Length; i++)
        {
            currentSentence += text[i];
            if (".!?".Contains(text[i]) && (i + 1 >= text.Length || text[i + 1] == ' '))
            {
                sentences.Add(currentSentence);
                currentSentence = "";
                while (i + 1 < text.Length && text[i + 1] == ' ') i++;
            }
        }
        if (!string.IsNullOrEmpty(currentSentence))
            sentences.Add(currentSentence);
        
        return sentences;
    }

    static string TextToBits(string text)
    {
        StringBuilder bits = new StringBuilder();
        foreach (char c in text)
        {
            bits.Append(Convert.ToString(c, 2).PadLeft(8, '0'));
        }
        return bits.ToString();
    }

    static string BitsToText(string bits)
    {
        StringBuilder text = new StringBuilder();
        for (int i = 0; i < bits.Length; i += 8)
        {
            if (i + 8 <= bits.Length)
            {
                string byteStr = bits.Substring(i, 8);
                text.Append((char)Convert.ToInt32(byteStr, 2));
            }
        }
        return text.ToString();
    }

    static void EmbedMessage(string containerFile, string messageFile, string outputFile)
    {
        string containerText = File.ReadAllText(containerFile, Encoding.UTF8);
        string message = File.ReadAllText(messageFile, Encoding.UTF8);

        List<string> sentences = PreprocessContainer(containerText);
        int capacity = sentences.Count - 1;

        message += "#";
        string messageBits = TextToBits(message);

        if (messageBits.Length > capacity)
        {
            Console.WriteLine($"Ошибка: сообщение ({messageBits.Length} бит) превышает емкость контейнера ({capacity} бит)");
            return;
        }

        StringBuilder result = new StringBuilder();
        for (int i = 0; i < sentences.Count; i++)
        {
            result.Append(sentences[i]);
            if (i < sentences.Count - 1)
            {
                if (i < messageBits.Length)
                {
                    result.Append(messageBits[i] == '0' ? "  " : " ");
                }
                else
                {
                    result.Append(" ");
                }
            }
        }

        File.WriteAllText(outputFile, result.ToString(), Encoding.UTF8);
        Console.WriteLine($"Сообщение зашифровано в {outputFile}");
    }

    static void ExtractMessage(string containerFile)
    {
        string text = File.ReadAllText(containerFile, Encoding.UTF8);
        List<string> sentences = PreprocessContainer(text);

        StringBuilder bits = new StringBuilder();
        int currentPosition = 0;
        for (int i = 0; i < sentences.Count - 1; i++)
        {
            int startIndex = text.IndexOf(sentences[i], currentPosition);
            if (startIndex == -1) break;

            startIndex += sentences[i].Length;

            int endIndex = text.IndexOf(sentences[i + 1], startIndex);
            if (endIndex == -1) break;

            int spaceCount = text.Substring(startIndex, endIndex - startIndex).Length;
            bits.Append(spaceCount == 1 ? "1" : "0");

            currentPosition = endIndex;
        }

        string markerBits = TextToBits("#");
        string messageBits = "";
        for (int i = 0; i <= bits.Length - markerBits.Length; i++)
        {
            if (i + markerBits.Length <= bits.Length && bits.ToString().Substring(i, markerBits.Length) == markerBits)
            {
                messageBits = bits.ToString().Substring(0, i);
                break;
            }
        }

        string message = BitsToText(messageBits);
        Console.WriteLine($"Извлеченное сообщение: {message}");
    }

    static void Main(string[] args)
    {
        if (args.Length < 1)
        {
            PrintUsage();
            return;
        }

        string command = args[0].ToLower();
        if (command == "embed" && args.Length == 4)
        {
            string containerFile = args[1];
            string messageFile = args[2];
            string outputFile = args[3];

            if (!File.Exists(containerFile) || !File.Exists(messageFile))
            {
                Console.WriteLine("Ошибка: один из файлов не найден");
                return;
            }
            EmbedMessage(containerFile, messageFile, outputFile);
        }
        else if (command == "extract" && args.Length == 2)
        {
            string containerFile = args[1];
            if (!File.Exists(containerFile))
            {
                Console.WriteLine("Ошибка: файл не найден");
                return;
            }
            ExtractMessage(containerFile);
        }
        else
        {
            PrintUsage();
        }
    }

    static void PrintUsage()
    {
        Console.WriteLine("Использование:");
        Console.WriteLine("  Для встраивания: Lab8.exe embed <container_file> <message_file> <output_file>");
        Console.WriteLine("  Для извлечения: Lab8.exe extract <container_file>");
    }
}