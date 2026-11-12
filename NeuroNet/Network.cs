namespace Mars.NeuroNet
{
    class Network
    {
        // все слои сети
        private InputLayer input_layer = null;
        private HiddenLayer hidden_layer1 = new HiddenLayer(70, 15, NeuronType.Hidden, nameof(hidden_layer1));
        private HiddenLayer hidden_layer2 = new HiddenLayer(38, 70, NeuronType.Hidden, nameof(hidden_layer2));
        private OutputLayer output_layer  = new OutputLayer(10, 38, NeuronType.Output, nameof(output_layer));

        private double[] fact = new double[10]; // массив фактического выхода сети
        private double[] e_error_avr; // среднее значение энергии ошибки эпохи обучения

        // свойства
        public double[] Fact { get => fact; }   // массив фактического выхода сети

        // среднее значение энергии ошибки эпохи обучения
        public double[] E_error_avr { get => e_error_avr; set => e_error_avr = value; }

        // конструктор
        public Network() { }

        // прямой проход сети
        public void ForwardPass(double[] netInput)
        {
            hidden_layer1.Data = netInput;
            hidden_layer1.Recognize(null, hidden_layer2);
            hidden_layer2.Recognize(null, output_layer);
            output_layer.Recognize(this, null);
        }

        // непосредственное обучение
        public void Train(Network net) // backpropagation method
        {
            input_layer = new InputLayer(NetworkMode.Train); // инициализация входного слоя для формирования множества обучающих образов

            int epoches = 10;     // количество эпох обучения
            double tmpSumError;   // временная переменная сумма ошибки
            double[] errors;      // вектор (массив) сигнала ошибки выходного слоя
            double[] temp_gsums1; // вектор градиента 1-го скрытого слоя
            double[] temp_gsums2; // вектор градиента 2-го скрытого слоя

            e_error_avr = new double[epoches];

            for (int k = 0;  k < epoches; k++) // перебор эпох обучения
            {
                e_error_avr[k] = 0; // в начале каждой эпохи обучения значение средней энергии ошибки эпохи обнуляется
                input_layer.ShufflingArrayRows(input_layer.TrainSet); // перетасовка обучающей выборки
                for (int i = 0; i < input_layer.TrainSet.GetLength(0); i++) // цикл перебора обучающих примеров (образов)
                {
                    double[] tmpTrain = new double[15]; // обучающий образ
                    for (int j = 0; j < tmpTrain.Length; j++)
                        tmpTrain[j] = input_layer.TrainSet[i, j + 1];

                    // прямой проход обучающего образа
                    ForwardPass(tmpTrain); 

                    // вычисление ошибки по итерации
                    tmpSumError = 0; // для каждого обучающего образа среднее значение ошибки этого образа обнуляется
                    errors = new double[fact.Length]; // переопределение массива сигнала ошибки выходного слоя
                    for (int x = 0; x < errors.Length; x++)
                    {
                        if (x == input_layer.TrainSet[i, 0]) // если номер выходного нейрона совпадает с желаемым откликом
                            errors[x] = 1.0 - fact[x];
                        else
                            errors[x] = -fact[x]; // errors[x] = 0.0 - net.fact[x];

                        tmpSumError += errors[x] * errors[x] / 2;
                    }
                    e_error_avr[k] += tmpSumError / errors.Length; // суммарное значение энергии ошибки k-ой эпохи

                    // обратный проход и коррекция весов
                    temp_gsums2 = output_layer.BackwardsPass(errors);
                    temp_gsums1 = hidden_layer2.BackwardsPass(temp_gsums2);
                    hidden_layer1.BackwardsPass(temp_gsums1);
                }
                e_error_avr[k] /= input_layer.TrainSet.GetLength(0); // среднее значение энергии ошибки одной эпохи
            }

            input_layer = null; // обнуление (уборка) входного слоя

            // запись скорректированных весов в "память" 
            hidden_layer1.WeightInitialize(MemoryMode.SET, nameof(hidden_layer1) + "_memory.csv");
            hidden_layer2.WeightInitialize(MemoryMode.SET, nameof(hidden_layer2) + "_memory.csv");
            output_layer.WeightInitialize(MemoryMode.SET, nameof(output_layer) + "_memory.csv");
        }
    }
}
