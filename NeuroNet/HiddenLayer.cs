namespace Mars.NeuroNet
{
    class HiddenLayer : Layer
    {
        public HiddenLayer(int non, int nopn, NeuronType nt, string type) : base(non, nopn, nt, type) { }

        // прямой проход
        public override void Recognize(Network net, Layer nextLayer)
        {
            double[] hidden_out = new double[numOfNeurons]; // выходы нейрона
            for (int i = 0; i < numOfNeurons; i++)
                hidden_out[i] = neurons[i].Output;

            nextLayer.Data = hidden_out; // передача выходного сигнала на вход следующего слоя
        }

        // обратный проход
        public override double[] BackwardsPass(double[] gr_sums)
        {
            double[] gr_sum = new double[numOfPrevNeurons]; // локальный градиент или градиентные суммы

            // цикл вычисления градиентной суммы j-го нейрона
            for (int j = 0; j < numOfPrevNeurons; j++)
            {
                double sum = 0;
                for (int k = 0; k < numOfNeurons; k++)
                    sum += neurons[k].Weights[j + 1] * neurons[k].Derivative * gr_sums[k]; // через градиентные суммы и производную

                gr_sum[j] = sum;
            }

            // цикл коррекции синаптических весов
            for (int i = 0; i < numOfNeurons; i++) 
                for (int n = 0; n < numOfPrevNeurons + 1; n++)
                {
                    double delta_w;
                    if (n == 0) // если порог
                        delta_w = momentum * lastDeltaWeights[i, 0] + 
                            learningRate * neurons[i].Derivative * gr_sum[i];
                    else
                        delta_w = momentum * lastDeltaWeights[i, n] + 
                            learningRate * neurons[i].Inputs[n - 1] * neurons[i].Derivative * gr_sum[i];

                    lastDeltaWeights[i, n] = delta_w;
                    neurons[i].Weights[n] += delta_w; // коррекция весов
                }
            return gr_sum;
        }
    }
}
