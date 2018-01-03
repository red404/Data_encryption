using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace data_encryption
{
    public class ArtNeuronNetwork
    {
        /*
         * TPM — это особый вид многоуровневой нейронной сети прямого распространения.
         * Она состоит из одного выходного нейрона, K скрытых нейронов и K*N входных нейронов. 
         * Входные нейроны принимают двоичные значения
         */

        private ArtNeuron output;
        private ArtNeuron[] hidden;
        private ArtNeuron[] input;
        public int K;
        public int N;
        public int L;

        public ArtNeuronNetwork(int _N, int _K, int _L)
        {
            N = _N;
            K = _K;
            L = _L;
            /* MAKE TPM */
            output = new ArtNeuron(_K, 1, 2); // выходной нейрон (тип 2) имеет по _K входов, амплитуду весов [-1;+1]
            hidden = new ArtNeuron[_K];
            input = new ArtNeuron[_K * _N];
            for (int i = 0; i < _K; i++)
            {
                hidden[i] = new ArtNeuron(_N, 1, 1); // скрытые нейроны (тип 1) имеют по _N входов и амплитуду весов [-1;+1]
                for (int j = 0; j < _N; j++)
                {
                    input[(i * _N) + j] = new ArtNeuron(1, _L, 0); // входные нейроны (тип 0) имеют по одному входу и амплитуду весов [-_L;+_L]
                    Random r = new Random();
                    input[(i * _N) + j].weight_reset(1, r);
                }
            }
        }

        public void use_hebbs_rule(object[] res1, object[] res2)
        {
            //int A = Convert.ToInt32(res1[0]), B = Convert.ToInt32(res2[0]);
            for (int i = 0; i < input.GetLength(0); i++)
            {   
                // для каждого входного нейрона применяем правило Хебба
                // в качестве параметром передаём обе нейросети
                // однако вторую можно заменить (см. комментарий к correct_weight() в Form1.cs)
                input[i].hebbs_rule(res1, res2, i, N, L);
            }
        }


        public object[] getOutp(int[][] inP) // ПОДАЁМ ВХОДНОЙ ВЕКТОР, ПОЛУЧАЕМ ВЫХОД СЕТИ
        {
            int[][] h_inP = new int[K][];
            int[] o_inP = new int[K];
            if (inP.GetLength(0) != getInpSize()) { return new object[] { -999 };/*ошибка*/}
            else
                for (int i = 0; i < K; i++)
                {
                    h_inP[i] = new int[N];/*создаём временный массив для скрытых нейронов*/
                    for (int j = 0; j < N; j++)
                    {// строкой ниже была ошибка в индексации, поэтому не все элементы входных данных были проинициализированы. Исправлено.
                        input[(i * N) + j].setInputs(inP[(i * N) + j]);/*присваиваем входной массив для входных нейронов*/
                        /*присваиваем входной массив для скрытых нейронов*/
                        h_inP[i][j] = input[(i * N) + j].calc_out_for_inputs();// рассчитывая выходы входных нейронов
                    }
                    hidden[i].setInputs(h_inP[i]);//подаём временный массив на вход скрытым нейронам
                    /*присваиваем входной массив для выходного нейрона*/
                    o_inP[i] = hidden[i].calc_out_for_hidden(h_inP[i]);// рассчитывая выходы скрытых нейронов
                }
            output.setInputs(o_inP);
            output.set_output(output.calc_out_for_outputs(o_inP));
            /*output.get*/
            return new object[] { output.get_output(), h_inP, o_inP };
        }
        
        public int getInpSize()
        {   // общее кол-во входных нейронов в сети
            return K * N;
        }

        public int[][] getWeights()
        {   // возвращаем массив массивов весов для каждого нейрона
            int[][] s = new int[N][];
            for (int i = 0; i < N; i++)
            {
                s[i] = (int[])(input[i].getWeights());
            }
            return s;
        }

    }
}
