using System;
using System.IO;

namespace Mars.NeuroNet
{
    class InputLayer
    {
        // поля
        private double[,] trainSet; // 100 изображений в обучающей выборке
        private double[,] testSet; // 10 изображений в тестовой выборке

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
            int j; // номер случайно выбранной строки
        }
    }
}
