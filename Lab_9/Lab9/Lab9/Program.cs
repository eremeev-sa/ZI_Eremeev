using System;
using System.IO;
using System.Text;

class Program
{
    static void Main(string[] args)
    {
        if (args.Length < 1)
        {
            PrintUsage();
            return;
        }

        string command = args[0].ToLower();

        try
        {
            switch (command)
            {
                case "embed":
                    if (args.Length < 5)
                    {
                        PrintUsage();
                        return;
                    }
                    Embed(args, args[1], args[2], args[3], args[4]);
                    break;

                case "extract":
                    if (args.Length < 3)
                    {
                        PrintUsage();
                        return;
                    }
                    Extract(args[1], args[2], args.Length > 3 ? int.Parse(args[3]) : 1);
                    break;

                default:
                    PrintUsage();
                    break;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка: {ex.Message}");
        }
    }

    static void PrintUsage()
    {
        Console.WriteLine("Использование:");
        Console.WriteLine("Lab9.exe embed <empty_file> <message_file> <output_file> <mode> [bits]");
        Console.WriteLine("Lab9.exe extract <container_file> <space_mode> [bits_per_line]");
        Console.WriteLine("Режимы: basic - 1/2 пробела, advanced - обычный/неразрывный пробел");
    }

    static void Embed(string[] args, string emptyFile, string messageFile, string outputFile, string mode)
    {
        
        string[] containerLines = File.ReadAllLines(emptyFile);
        string message = File.ReadAllText(messageFile);

    
        byte[] messageBytes = Encoding.ASCII.GetBytes(message);
        string bitString = "";
        foreach (byte b in messageBytes)
        {
            bitString += Convert.ToString(b, 2).PadLeft(8, '0');
        }

 
        for (int i = 0; i < containerLines.Length; i++)
        {
            containerLines[i] = containerLines[i].TrimEnd();
        }

   
        int capacity = mode.ToLower() == "basic" ? containerLines.Length : 
            containerLines.Length * (args.Length > 5 ? int.Parse(args[5]) : 1);
        
        Console.WriteLine($"Максимальная емкость контейнера: {capacity} бит");
        Console.WriteLine($"Размер сообщения: {bitString.Length} бит");
        
        if (bitString.Length > capacity)
        {
            Console.WriteLine("Ошибка: Сообщение слишком большое для контейнера");
            return;
        }
        
        string[] outputLines = new string[containerLines.Length];
        int bitIndex = 0;

        if (mode.ToLower() == "basic")
        {
            // Метод 1: 1 или 2 пробела
            for (int i = 0; i < containerLines.Length; i++)
            {
                if (bitIndex < bitString.Length)
                {
                    outputLines[i] = containerLines[i] + (bitString[bitIndex] == '1' ? " " : "  ");
                    bitIndex++;
                }
                else
                {
                    outputLines[i] = containerLines[i];
                }
            }
        }
        else if (mode.ToLower() == "advanced")
        {
            // Метод 2: обычный и неразрывный пробел
            int bitsPerLine = args.Length > 5 ? int.Parse(args[5]) : 1;
            
            for (int i = 0; i < containerLines.Length && bitIndex < bitString.Length; i++)
            {
                string spaces = "";
                for (int j = 0; j < bitsPerLine && bitIndex < bitString.Length; j++)
                {
                    spaces += bitString[bitIndex] == '1' ? "\u00A0" : " "; 
                    bitIndex++;
                }
                outputLines[i] = containerLines[i] + spaces;
            }
            
            // Копируем оставшиеся строки
            for (int i = bitIndex/bitsPerLine; i < containerLines.Length; i++)
            {
                outputLines[i] = containerLines[i];
            }
        }

        File.WriteAllLines(outputFile, outputLines);
        Console.WriteLine($"Сообщение успешно встроено в {outputFile}");
    }

    static void Extract(string containerFile, string mode, int bitsPerLine)
    {
        string[] lines = File.ReadAllLines(containerFile);
        string bitString = "";

        if (mode.ToLower() == "basic")
        {
            // Извлечение для метода 1
            foreach (string line in lines)
            {
                int spaceCount = line.Length - line.TrimEnd().Length;
                if (spaceCount == 1) bitString += "1";
                else if (spaceCount == 2) bitString += "0";
            }
        }
        else if (mode.ToLower() == "advanced")
        {
            // Извлечение для метода 2
            foreach (string line in lines)
            {
                string trimmed = line.TrimEnd();
                string spaces = line.Substring(trimmed.Length);
                
                foreach (char c in spaces)
                {
                    bitString += (c == '\u00A0') ? "1" : "0";
                }
            }
        }
        
        string result = "";
        for (int i = 0; i < bitString.Length - 7; i += 8)
        {
            string byteStr = bitString.Substring(i, 8);
            result += (char)Convert.ToInt32(byteStr, 2);
        }

        Console.WriteLine($"Извлеченное сообщение: {result}");
    }
}