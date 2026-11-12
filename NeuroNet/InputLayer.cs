using System;
using System.IO;

namespace Mars.NeuroNet
{
    class InputLayer
    {
        // поля
        private double[,] trainSet; // 100 изображений в обучающей выборке
        private double[,] testSet; // 10 изображений в тестовой выборке
        private static readonly Random random = new Random(); // генератор случайных чисел

        // свойства 
        public double[,] TrainSet { get => trainSet; }
        public double[,] TestSet { get => testSet; }

        // конструктор
        public InputLayer(NetworkMode nm)
        {
            string path = AppDomain.CurrentDomain.BaseDirectory; // директория, где находится приложение
            string[] tmpArrStr; // временный массив строк
            string[] tmpStr;    // временный массив элементов в строке

            switch (nm)
            {
                case NetworkMode.Train:
                    tmpArrStr = File.ReadAllLines(path + "train.txt"); // считывание из файла обучающей выборки
                    trainSet = new double[tmpArrStr.Length, 16];       // определение массива обучающей выборки

                    for (int i = 0; i < tmpArrStr.Length; i++) // цикл перебора строк обучающей выборки
                    {
                        tmpStr = tmpArrStr[i].Split(' '); // разбиение i-ой строки на массив отдельных символов

                        for (int j = 0; j < 16; j++) // цикл заполнения i-ой строки обучающей выборки
                            trainSet[i, j] = double.Parse(tmpStr[j]); // строковое значение числа преобразуется в двоичное число
                    }
                    break;

                case NetworkMode.Test:
                    tmpArrStr = File.ReadAllLines(path + "test.txt"); // считывание из файла тестовой выборки
                    testSet = new double[tmpArrStr.Length, 16];       // определение массива тестовой выборки

                    for (int i = 0; i < tmpArrStr.Length; i++) // цикл перебора строк тестовой выборки
                    {
                        tmpStr = tmpArrStr[i].Split(' '); // разбиение i-ой строки на массив отдельных символов

                        for (int j = 0; j < 16; j++) // цикл заполнения i-ой строки тестовой выборки
                            testSet[i, j] = double.Parse(tmpStr[j]); // строковое значение числа преобразуется в двоичное число
                    }
                    break;
            }
        }

        // перетасовка строк массива методом Фишера-Йетса (Fisher-Yates)
        public void ShufflingArrayRows(double[,] arr)
        {
            int rows = arr.GetLength(0); // количество строк
            int cols = arr.GetLength(1); // количество столбцов

            double[] temp = new double[cols]; // временная строка
            int j; // номер случайно выбранной строки

            // перебор строк в обратном порядке
            for (int i = rows - 1; i > 0; i--)
            {
                j = random.Next(i + 1); // случайный индекс от 0 до i

                // обмен строк i и j
                for (int k = 0; k < cols; k++)
                {
                    temp[k] = arr[i, k];
                    arr[i, k] = arr[j, k];
                    arr[j, k] = temp[k];
                }
            }
        }
    }
}
