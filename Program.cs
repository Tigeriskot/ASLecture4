using System;
using System.Collections.Generic;
using System.IO;


namespace ASLecture4
{
    class Program
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("************* Лабораторные работы по 4 лекции *************\n" +
                "************* Реализация алгоритмов Кнута-Морриса-Пратта и Бойера-Мура *************\n");

            // считываем исходный текст из файла в переменную TextInFile

            Console.WriteLine("Введите название файла в котором будет происходить поиск: ");


            try
            {   // чтение данных из файла
                using (StreamReader sr = new StreamReader(Console.ReadLine()))
                {
                    string TextInFile = sr.ReadToEnd();
                    Console.WriteLine("Текст файла: ");
                    Console.WriteLine(TextInFile);
                    // метод, который демонстрирует работу алгоритмов
                    Realization(TextInFile);

                }


            }
            catch (Exception ex)
            {

                Console.WriteLine("Файл не найден");
                Console.WriteLine(ex.Message);
                Console.WriteLine("Если хотите ввести текст вручную, напишите 1");

                if (Console.ReadLine() == "1")
                {
                    Console.WriteLine("Введите текст для работы с алгоритмами");
                    string Text = Console.ReadLine();
                    Realization(Text);
                }


            }





            Console.Read();
        }




        public static void Realization(string Text)
        {
            string SearchingText;

            Console.WriteLine("Полученный текст: " + Text);
            Console.WriteLine("Введите текст, которые хотите искать в " + Text);
            SearchingText = Console.ReadLine();

            Console.WriteLine("Демонстрация работы алгоритма Кнута-Морриса-Пратта");
            KMP(SearchingText, Text);

            Console.WriteLine("Демонстрация работы алгоритма Бойера-Мура");
            BM(SearchingText, Text);

            Console.WriteLine();
        }


        // Метод(вспомогательный) для вывода на экран содержимого массива массива граней и т.п.
        public static void Show(int[] Array)
        {
            for (int i = 0; i < Array.Length; ++i)
            {
                Console.Write(Array[i] + " ");
            }
            Console.WriteLine();
        }

        // Метод, который реализует алгоритм Кнута-Морриса-Пратта
        // OriginalText - входной текст 
        // SearchingTexth - искомая подстрока
        public static void KMP(string SearchingText, string OriginalText)
        {
            int count = 0;
            int[] bp = new int[SearchingText.Length];
            int[] bpm = new int[SearchingText.Length];
            // Построение массива граней
            PrefixBorderArray(SearchingText, bp); 
            
            int SearchingTextLength = SearchingText.Length;
            int OriginalTextLength = OriginalText.Length;
            // Построение модифицированного массива граней
            BPToBPM(bp, bpm, SearchingTextLength);
            // Текущий индекс в образце
            int IndexInSearchingText = 0;
            // Цикл по символам текста OriginalText
            for (int i = 0; i < OriginalTextLength; ++i)
            { // Быстрые продвижения при фиксированном i
                while (IndexInSearchingText != 0 && SearchingText[IndexInSearchingText] != OriginalText[i]) 
                    IndexInSearchingText = bpm[IndexInSearchingText - 1];
                // «Честное» сравнение очередной пары символов
                if (SearchingText[IndexInSearchingText] == OriginalText[i]) 
                    ++IndexInSearchingText;
                if (IndexInSearchingText == SearchingTextLength)
                {
                    count++;
                    Console.WriteLine("Вхождение с позиции " + Convert.ToString(i - IndexInSearchingText + 1));
                    IndexInSearchingText = bpm[IndexInSearchingText - 1];
                }
            }
            if (count == 0)
            {
                Console.WriteLine("Вхождений искомой подстроки нет");
            }
        }

        // Формирование словаря списков позиций
        // SearchingTextLength - длина входного образца (SearchingText)
        // DictionaryOfCharEntered - параметр, содержащий для каждого символа алфавита список позиций его вхождений в образец
        public static Dictionary<char, List<int>> PositionList(string SearchingText)
        {
            Dictionary<char, List<int>> DictionaryOfCharEntered = new Dictionary<char, List<int>>();

            int SearchingTextLength = SearchingText.Length;
            for (int k = SearchingTextLength - 1; k >= 0; --k)
            {
                try
                {
                    DictionaryOfCharEntered[SearchingText[k]].Add(k);
                }
                catch
                {
                    DictionaryOfCharEntered[SearchingText[k]] = new List<int>();
                    DictionaryOfCharEntered[SearchingText[k]].Add(k);
                }

            }
            return DictionaryOfCharEntered;
        }


        // Вычисление сдвига по плохому символу
        // DictionaryOfCharEntered - словарь, содержащий для каждого символа алфавита список позиций его вхождений в образец
        // CharBad - символ, предшествующий не совпавшему
        // PosBad - индекс, на котором начало образца
        // ListOfSpecificCharEntered - список позиций данного символа CharBad
        // nPos - искомая позиция слева от плохого символа
        public static int BadCharShift(Dictionary<char, List<int>> DictionaryOfCharEntered, char CharBad, int PosBad)
        {
            if (PosBad < 0)
                return 1; // Образец совпал – сдвиг на 1
            int nPos = -1;
           
            if (DictionaryOfCharEntered.ContainsKey(CharBad))
            {   // Список не пуст
                List<int> ListOfSpecificCharEntered = DictionaryOfCharEntered[CharBad];
                // Ищем элемент, меньший чем плохая позиция
                foreach (int objList in ListOfSpecificCharEntered)
                {
                    if (objList < PosBad)
                    {
                        nPos = objList;
                        break;
                    }
                }
            }
            return PosBad - nPos;
        }

        // Метод, который реализует алгоритм Бойера-Мура
        // OriginalText - входной текст 
        // SearchingTexth - искомая подстрока
        // RightBorder - правая граница «прикладывания» образца
        // DictionaryOfCharEntered - словарь, содержащий для каждого символа алфавита список позиций его вхождений в образец
        public static void BM(string SearchingText, string OriginalText)
        {
            int count = 0;
            Dictionary<char, List<int>> pl = PositionList(SearchingText); ;
            // Длина искомой строки
            int SearchingTextLength = SearchingText.Length;
            // Длина входного текста 
            int OriginalTextLength = OriginalText.Length;
            // Поиск вхождений
            int RightBorder = SearchingTextLength;
            while (RightBorder <= OriginalTextLength)
            { // Сравнение образца с текстом справа налево
                // Итератор по искомой строке
                int IterOfSearchingText = SearchingTextLength - 1;
                // Итератор по подстроке входного текста
                int IterOfSubOriginalText = RightBorder - 1;
                for (; IterOfSearchingText >= 0; --IterOfSearchingText, --IterOfSubOriginalText)
                    if (SearchingText[IterOfSearchingText] != OriginalText[IterOfSubOriginalText])
                        break;// OriginalText[i] – плохой символ
                              // Результаты сравнения


                if (IterOfSearchingText < 0)
                {
                    Console.WriteLine("Точка вхождения " + (IterOfSubOriginalText + 1));
                    count++;
                }


                RightBorder += BadCharShift(pl, OriginalText[Math.Max(0, IterOfSubOriginalText)], IterOfSearchingText);
            }
            if (count == 0)
            {
                Console.WriteLine("Вхождений искомой подстроки нет");
            }
        }



    // Алгоритм создания массива граней префиксов
    public static void PrefixBorderArray(string TextInFile, int[] bp)
    {
        int n = TextInFile.Length;
        int bpRight;
        bp[0] = 0;
        for (int i = 1; i < n; ++i)
        { // i – длина рассматриваемого префикса
            bpRight = bp[i - 1]; // Позиция справа от предыдущей грани
            while (bpRight != 0 && TextInFile[i] != TextInFile[bpRight])
                bpRight = bp[bpRight - 1];
            // Длина на 1 больше, чем позиция
            if (TextInFile[i] == TextInFile[bpRight])
                bp[i] = bpRight + 1;
            else bp[i] = 0;
        }
    }

        // Алгоритм построения модифицированного массива граней префиксов
        // При построение не используется исходный текст
        static void BPToBPM(int[] bp, int[] bpm, int n)
        {
            bpm[0] = 0; bpm[n - 1] = bp[n - 1];
            for (int i = 1; i < n - 1; ++i)
            {
                // Проверка совпадения следующих символов
                if (bp[i] != 0 && (bp[i] + 1 == bp[i + 1]))
                    bpm[i] = bpm[bp[i] - 1];
                else bpm[i] = bp[i];
            }
        }
    }
}
