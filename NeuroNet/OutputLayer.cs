using System.Runtime.Remoting;

namespace Mars.NeuroNet
{
    class OutputLayer : Layer
    {
        public OutputLayer(int non, int nopn, NeuronType nt, string type) : base(non, nopn, nt, type) { }

        // прямой проход
        public override void Recognize(Network net, Layer nextLayer)
        {
            double e_sum = 0;
            for (int i = 0; i < neurons.Length; i++)
                e_sum += neurons[i].Output;
            for (int i = 0; i < neurons.Length; i++)
                net.Fact[i] = neurons[i].Output / e_sum;
        }

        // обратный проход
        public override double[] BackwardsPass(double[] errors)
        {
            double[] gr_sum = new double[numOfPrevNeurons];

            // вычисление градиентных сумм выходного слоя 
            for (int j = 0; j < numOfPrevNeurons; j++)
            {
                double sum = 0;
                for (int k = 0; k < numOfNeurons; k++)
                    sum += neurons[k].Weights[j + 1] * errors[k];

                gr_sum[j] = sum;
            }

            // цикл коррекции синаптических весов
            for (int i = 0; i < numOfNeurons; i++)
                for (int n = 0; n < numOfPrevNeurons + 1; n++)
                {
                    double delta_w;
                    if (n == 0) // если порог
                        delta_w = momentum * lastDeltaWeights[i, 0] + learningRate * errors[i];
                    else
                        delta_w = momentum * lastDeltaWeights[i, n] + learningRate * neurons[i].Inputs[n - 1] * errors[i];
                    lastDeltaWeights[i, n] = delta_w;
                    neurons[i].Weights[n] += delta_w; // коррекция весов
                }
            return gr_sum;
        }
    }
}
