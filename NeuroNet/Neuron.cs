using System;
using static System.Math;

namespace Mars.NeuroNet
{
    class Neuron
    {
        private double[] inputs;   // входы
        private double[] weights;  // веса
        private double output;     // выход
        private double derivative; // производная
        private NeuronType type;   // тип нейрона

        // свойства
        public double[] Weights { get => weights; set => weights = value; }
        public double[] Inputs { get => inputs; set => inputs = value; }
        public double Output { get => output; }
        public double Derivative { get => derivative; }

        // конструктор
        public Neuron(double[] memoryWeights, NeuronType typeNeuron)
        {
            type = typeNeuron;
            weights = memoryWeights;
        }

        // метод активации нейрона
        public void Activator(double[] i)
        {
            inputs = i;

            double sum = weights[0];

            for (int j = 0; j < inputs.Length; j++)
            {
                sum += inputs[j] * weights[j + 1];
            }

            switch (type)
            {
                case NeuronType.Hidden:
                    output = Logistic(sum);
                    derivative = LogisticDerivative(output);
                    break;

                case NeuronType.Output:
                    output = Logistic(sum);
                    //derivative = LogisticDerivative(output);
                    break;
            }
        }

        // логистическая функция активации нейрона
        private double Logistic(double x)
        {
            return 1.0 / (1.0 + Exp(-x));
        }

        // производная логистической функции
        private double LogisticDerivative(double output)
        {
            return output * (1.0 - output);
        }
    }
}
