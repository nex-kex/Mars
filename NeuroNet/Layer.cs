using System;
using System.IO;
using System.Windows.Forms;

namespace Mars.NeuroNet
{
    abstract class Layer
    {
        // Поля
        protected string nameLayer;     // наименование слоя
        string pathDirWeigths;          // путь к каталогу файла весов
        string pathFileWeigths;         // путь к файлу весов
        protected int numOfNeurons;     // число нейронов слоя
        protected int numOfPrevNeurons; // число нейронов прошлого слоя
        protected Neuron[] neurons;     // массив нейронов
        protected const double learningRate = 0.05; // скорость обучения
        protected const double momentum = 0.5;      // момент
        protected double[,] lastDeltaWeights;       // веса

        // Свойства
        public double[] Data // передача входных данных на нейроны слоя и активация нейрона
        {
            set
            {
                for (int i = 0; i < numOfNeurons; i++)
                {
                    neurons[i].Activator(value);
                }
            }
        }

        // Конструктор
        protected Layer(int non, int nopn, NeuronType nt, string nmLayer)
        {
            numOfNeurons = non;        // количество нейронов текущего слоя
            numOfPrevNeurons = nopn;   // количество нейронов предыдущего слоя
            neurons = new Neuron[non]; // определение массива нейронов
            nameLayer = nmLayer;       // наименование слоя, которое используется для связи с файлом
            pathDirWeigths = AppDomain.CurrentDomain.BaseDirectory + "memory\\"; // путь к каталогу
            pathFileWeigths = pathDirWeigths + nameLayer + "_memory.csv";        // путь к файлу синаптических весов

            lastDeltaWeights = new double[non, nopn + 1];
            double[,] Weights; // временный массив синаптических весов текущего слоя

            if (File.Exists(pathFileWeigths))
                Weights = WeightInitialize(MemoryMode.GET, pathFileWeigths); // считываем файл весов
            else
            {
                Directory.CreateDirectory(pathDirWeigths);                    // создание директории
                Weights = WeightInitialize(MemoryMode.INIT, pathFileWeigths); // если нет файла весов, инициализация
            }

            for (int i = 0; i < non;  i++) // цикл формирования нейронов слоя и заполнение ими массива нейронов
            {
                double[] tmpWeights = new double[nopn + 1];
                for (int j = 0; j < nopn + 1; j++)
                    tmpWeights[j] = Weights[i, j];
                neurons[i] = new Neuron(tmpWeights, nt); // заполнение массива нейронами
            }
        }

        // Метод работы с массивом синаптических весов слоя
        public double[,] WeightInitialize(MemoryMode mm, string path)
        {
            char[] delim = new char[] { ';', ' ' }; // разделитель слов
            string tmpStr;                          // временная строка для чтения
            string[] tmpStrWeights;                 // временный массив строк
            double[,] weights = new double[numOfNeurons, numOfPrevNeurons + 1];

            switch (mm)
            {
                case MemoryMode.GET:
                    tmpStrWeights = File.ReadAllLines(path); // считывание строк текстового массива весов
                    string[] memory_element;
                    for (int i = 0; i < numOfNeurons; i++)
                    {
                        memory_element = tmpStrWeights[i].Split(delim); // разбить строку на элементы
                        for (int j = 0; j < numOfPrevNeurons; j++)
                        {
                            weights[i, j] = double.Parse(memory_element[j].Replace(',', '.'),
                                System.Globalization.CultureInfo.InvariantCulture); // строковое преобразиование
                        }
                    }
                    break;
                case MemoryMode.SET:
                    tmpStrWeights = new string[numOfNeurons]; // построчный массив синаптических весов

                    if (!File.Exists(path)) // если файла синаптических весов нет
                    {
                        MessageBox.Show("!!! В Н И М А Н И Е !!!\nФайл " + nameLayer + "_memory.csv синаптических весов НЕ НАЙДЕН!!!" +
                            "\nПосле нажатия кнопки ОК автоматически создастся отсутствующий файл, и все значения весов будут сохранены в нём.",
                            "Внимание", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    }

                    for (int i = 0; i < numOfNeurons; i++)
                    {
                        tmpStr = neurons[i].Weights[0].ToString(); // порог - первое значение строки
                        for (int j = 0; j< numOfPrevNeurons; j++)
                        {
                            tmpStr += delim[0] + neurons[i].Weights[j].ToString();
                        }
                        tmpStrWeights[i] = tmpStr;
                    }
                    File.WriteAllLines(path, tmpStrWeights);

                    break;

                case MemoryMode.INIT:
                    MessageBox.Show("!!! В Н И М А Н И Е !!!\nФайл " + nameLayer + "_memory.csv синаптических весов НЕ НАЙДЕН!!!" +
                            "\nПосле нажатия кнопки ОК автоматически проинициализируются веса и создастся отсутствующий файл, " +
                            "нейросеть вернётся к исходному состоянию <<новорождённости>>, и потребуется обучать её ЗАНОВО.",
                            "Внимание", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);

                    Random random = new Random(); // генератор случайных чисел

                    // инициализация исходных значений синаптических весов нейронного слоя
                    double tmpRatio; // коэффициент - стандартное отклонение распределения
                    double tmpShift; // смещение - среднее распределения
                    double[] tmpArr = new double[numOfPrevNeurons + 1]; // массив весов одного нейрона
                    tmpStrWeights = new string[numOfNeurons];           // массив строк синаптических весов

                    for (int i = 0; i < numOfNeurons; i++) // цикл перебора нейронов в слое
                    {
                        tmpRatio = Math.Sqrt(Math.Sqrt(1.0 / numOfPrevNeurons)); // n^(-1/4)
                        tmpShift = 0.0;                                          // среднее = 0

                        for (int j = 0; j < numOfPrevNeurons + 1; j++)
                        {
                            double u1 = 1.0 - random.NextDouble(); // рандомное число из (0, 1]
                            double u2 = 1.0 - random.NextDouble();
                            double z = Math.Sqrt(-2.0 * Math.Log(u1)) * Math.Cos(2.0 * Math.PI * u2);

                            tmpArr[j] = tmpShift + tmpRatio * z;
                            weights[i, j] = tmpArr[j]; // сохранение в массив весов
                        }

                        tmpStr = tmpArr[0].ToString(System.Globalization.CultureInfo.InvariantCulture); // порог
                        for (int j = 1; j < numOfPrevNeurons + 1; j++)
                        {
                            tmpStr += delim[0] +
                                tmpArr[j].ToString(System.Globalization.CultureInfo.InvariantCulture);
                        }
                        tmpStrWeights[i] = tmpStr; // строка синаптических весов одного нейрона
                    }

                    File.WriteAllLines(path, tmpStrWeights); // записываем файл весов
                    break;
                default:
                    break;
            }

            return weights;
        }

        abstract public void Recognize(Network net, Layer nextLayer); // для прямых проходов
        abstract public double[] BackwardsPass(double[] stuff); // для обратных проходов
    }
}
