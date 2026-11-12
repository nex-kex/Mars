using System;
using System.Drawing;
using System.Windows.Forms;
using System.IO;
using Mars.NeuroNet; // пространство имён для работы с файлами

namespace Mars
{
    // Архитектура - Логистическая функция
    // Функция активации - 15-70-38-10
    // Функция выходного слоя - логистическая
    public partial class FormMain : Form
    {
        private double[] inputPixels; // массив входных данных
        private Network network;      // объявление нейросети


        public FormMain()
        {
            InitializeComponent();
            inputPixels = new double[15]; // инициализация массива
            network = new Network();
        }

        private void pixel_Click(object sender, EventArgs e)
        {
            if (((Button)sender).BackColor == Color.White)
            {
                ((Button)sender).BackColor = Color.Black;
                inputPixels[((Button)sender).TabIndex] = 1d;
            }
            else
            {
                ((Button)sender).BackColor = Color.White;
                inputPixels[((Button)sender).TabIndex] = 0d;

            }
        }

        private void buttonSaveTest_Click(object sender, EventArgs e)
        {
            string path = AppDomain.CurrentDomain.BaseDirectory + "test.txt";
            string tmpStr = numericUpDownAnswer.Value.ToString();

            for (int i = 0; i < inputPixels.Length; i++)
                tmpStr += " " + inputPixels[i].ToString();
            tmpStr += "\n";

            File.AppendAllText(path, tmpStr);
        }

        private void buttonSaveTraining_Click(object sender, EventArgs e)
        {
            string path = AppDomain.CurrentDomain.BaseDirectory + "train.txt";
            string tmpStr = numericUpDownAnswer.Value.ToString();

            for (int i = 0; i < inputPixels.Length; i++)
                tmpStr += " " + inputPixels[i].ToString();
            tmpStr += "\n";

            File.AppendAllText(path, tmpStr);
        }

        private void buttonRecognize_Click(object sender, EventArgs e)
        {
            network.ForwardPass(inputPixels); // считаем выходы всех 10 нейронов

            double[] fact = network.Fact; // массив вероятностей

            int[] order = new int[fact.Length]; // массив порядка цифр (в порядке убывания вероятностей)
            for (int i = 0; i < order.Length; i++) order[i] = i; // цифры от 0 до 9

            // сортировка пузырьком индексов по убыванию вероятностей
            for (int i = 0; i < order.Length - 1; i++)
            {
                for (int j = 0; j < order.Length - 1 - i; j++)
                {
                    if (fact[order[j]] < fact[order[j + 1]])
                    {
                        int t = order[j];
                        order[j] = order[j + 1];
                        order[j + 1] = t;
                    }
                }
            }

            // Очищаем таблицу и задаём количество строк
            dataGridViewResults.Rows.Clear();
            dataGridViewResults.Rows.Add(order.Length); // 10 строк

            // Заполняем ячейки
            for (int k = 0; k < order.Length; k++)
            {
                dataGridViewResults.Rows[k].Cells[0].Value = order[k].ToString();
                dataGridViewResults.Rows[k].Cells[1].Value =
                    (100 * fact[order[k]]).ToString("0.00") + " %";
            }


            //labelOutput.Text = network.Fact.ToList().IndexOf(network.Fact.Max()).ToString();
            //labelProbability.Text = (100 * network.Fact.Max()).ToString("0.00") + " %";
        }

        // обработчик события клика кнопки "обучить"
        private void buttonTraining_Click(object sender, EventArgs e)
        {
            network.Train(network);

            MessageBox.Show("Обучение успешно завершено.", "Информация",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}
