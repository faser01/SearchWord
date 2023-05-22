using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using File = System.IO.File;

namespace SearchWord
{
    internal class Program
    {
        public class SearchResult
        {
            public string FileName { get; set; }
            public string Word { get; set; }
            public int Count { get; set; }
        }

        [STAThread]
        static void Main(string[] args)
        {
            string[] meaningfulInfo = { "Съешь ещё этих мягких французских булок, да выпей чаю ", " Зять съел щи, чан брюквы, шип. Эх, фигу ждём!",
                "Аэрофотосъёмка ландшафта уже выявила земли богачей и процветающих крестьян." };
            Random rand = new Random();

            string fileName1 = "file1.txt";
            string fileName2 = "file2.txt";
            string fileName3 = "file3.txt";
            string fileName4 = "file4.txt";

            using (StreamWriter sw1 = new StreamWriter(fileName1))
            using (StreamWriter sw2 = new StreamWriter(fileName2))
            using (StreamWriter sw3 = new StreamWriter(fileName3))
            using (StreamWriter sw4 = new StreamWriter(fileName4))
            {
                for (int i = 0; i < 10000; i++)
                {
                    if (i % 4 == 0)
                    {
                        string randomString = new string(Enumerable.Repeat("АБВГДЕЖЗИКЛМНОПРСТУФХЦЧШЩЬЫЪЭЮЯ0123456789", 10)
                        .Select(s => s[rand.Next(s.Length)]).ToArray());
                        sw1.WriteLine(randomString);
                        sw2.WriteLine(randomString);
                        sw3.WriteLine(randomString);
                        sw4.WriteLine(randomString);
                    }
                    else
                    {
                        string meaningfulSentence = meaningfulInfo[rand.Next(meaningfulInfo.Length)];
                        sw1.WriteLine(meaningfulSentence);
                        sw2.WriteLine(meaningfulSentence);
                        sw3.WriteLine(meaningfulSentence);
                        sw4.WriteLine(meaningfulSentence);
                    }
                }
            }




            FolderBrowserDialog folderDialog = new FolderBrowserDialog();
            folderDialog.Description = "Выберите папку для поиска слов";
            folderDialog.SelectedPath = Directory.GetCurrentDirectory(); // установил текущую директорию


            // Если пользователь выбрал папку и нажал ОК
            if (folderDialog.ShowDialog() == DialogResult.OK)
            {
                var words = ReadWordsFromFile("words.txt");
                var files = Directory.GetFiles(folderDialog.SelectedPath, "*.txt");
                var results = new List<SearchResult>();
                Console.WriteLine($"Поиск в файле: {files}");


                foreach (var file in files)
                {
                    var fileName = Path.GetFileName(file);
                    foreach (var word in words)
                    {
                        var count = CountWordccurrences(File.ReadAllText(file), word);
                        if (count > 0)
                        {
                            results.Add(new SearchResult
                            {
                                FileName = fileName,
                                Word = word,
                                Count = count
                            });
                        }
                    }
                }

                PrintResults(results);

                // Визуализация с задержкой
                foreach (var result in results)
                {
                    Console.WriteLine($"{result.FileName}: {result.Word} - {result.Count}");
                    Task.Delay(500).Wait();
                }
                SaveResultsToFile(results, "report.txt");
                Console.WriteLine($"Результат записан в файл: report.txt");
            }
        }

    

        public static void SaveResultsToFile(List<SearchResult> results, string fileName)
        {
            using (var outputFile = new StreamWriter(fileName))
            {
                foreach (var fileNameGroup in results.GroupBy(r => r.FileName).OrderBy(g => g.Key))
                {
                    outputFile.WriteLine(fileNameGroup.Key);
                    foreach (var result in fileNameGroup.OrderBy(r => r.Word))
                    {
                        outputFile.WriteLine($"\t{result.Word} - {result.Count}");
                    }
                }
            }
        }

        public static int CountWordccurrences(string source, string word)
        {
            var count = 0;
            var index = source.IndexOf(word, StringComparison.OrdinalIgnoreCase);
            while (index != -1)
            {
                count++;
                index = source.IndexOf(word, index + 1, StringComparison.OrdinalIgnoreCase);
            }
            return count;
        }

        public static void PrintResults(List<SearchResult> results)
        {
            foreach (var fileName in results.Select(r => r.FileName).Distinct().OrderBy(f => f))
            {
                Console.WriteLine(fileName);
                foreach (var result in results.Where(r => r.FileName == fileName).OrderBy(r => r.Word))
                {
                    Console.WriteLine($"\t{result.Word} - {result.Count}");
                }
            }
        }

        public static List<string> ReadWordsFromFile(string fileName)
        {
            var words = new List<string>();
            using (StreamReader inputFile = new StreamReader(fileName))
            {
                while (!inputFile.EndOfStream)
                {
                    var word = inputFile.ReadLine().Trim();
                    if (!string.IsNullOrWhiteSpace(word))
                    {
                        words.Add(word);
                    }
                }
            }
            return words;
        }
    }
}
