using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

/****************** Artificial Neuron ******************/

namespace data_encryption
{
    public class ArtNeuron
    {
        private double[] mul; // Тут будем хранить отмасштабированные сигналы
        private int[] weight; // Массив для хранения весов
        private int[] input; // Входная информация
        //public int limit = 9 ; // Порог - выбран экспериментально, для быстрого обучения
        //private double sum; // Тут сохраним сумму масштабированных сигналов
        private int w_range;
        public int neuron_type;
        private int output_out;

        public ArtNeuron(int size, int L, int T, int[] inP = null) // Задаем свойства при создании объекта
        {
            Random r = new Random();
            neuron_type = T;
            w_range = L;    // амплитуда весов
            weight = new int[size]; // Определяемся с размером массива (число входов)
            mul = new double[size];
            input = new int[size];
            weight_reset(size, r);
            input = inP ; // Получаем входные данные
            if (T == 2) output_out = 0;
        }


        public void weight_reset(int size, Random r)
        {
            for (int i = 0; i < size; i++)
            {
                weight[i] = (int)(r.Next() % w_range * Math.Pow(-1.0, r.Next()));
            }
        }

        public void setInputs(int[] inP)
        {
            input = inP;
        }

        public int sgn(double x)
        {
            if (x <= 0) return -1;
            else
                return 1;
        }

        public int calc_out_for_inputs()
        {
            /* расчёт*/
            //input = inP;
            int _t = 0;
            for (int i = 0; i < input.GetLength(0); i++)
            {
                _t += weight[i] * input[i];
            }
            return _t;/* разобраться, что возвращает входной нейрон */
        }


        public int calc_out_for_hidden(int[] inP)
        //public double calc_out_for_hidden(double[] inP)
        {
            /* расчёт*/
            //input = inP;
            int _t = 0;
            for (int i = 0; i < inP.GetLength(0); i++)
            {
                _t += inP[i];
            }
            return sgn(_t);/* разобраться, что возвращает скрытый нейрон */
            //return _t;
        }

        public int calc_out_for_outputs(int[] inP)
        {
            /* расчёт*/
            //input = inP;
            int _t = inP[0];
            for (int i = 1; i < inP.GetLength(0); i++)
            {
                _t *= inP[i];
            }
            return sgn(_t);
        }

        public void set_output(int s)
        {
            output_out = s;
        }

        public int get_output()
        {
            return output_out;
        }



        public void hebbs_rule(object[] res1, object[] res2, int s, int N, int L)
        {
            int k = weight.GetLength(0);
            if (neuron_type == 0)
            {
                /* Применить правило хебба для входных нейронов, если выходы сетей совпали */
                /*Правило Хебба:
                w_i^+=w_i+\sigma_ix_i\Theta(\sigma_i\tau)\Theta(\tau^A\tau^B)
                Анти-правило Хебба:
                w_i^+=w_i-\sigma_ix_i\Theta(\sigma_i\tau)\Theta(\tau^A\tau^B)
                Случайное блуждание:
                w_i^+=w_i+x_i\Theta(\sigma_i\tau)\Theta(\tau^A\tau^B)*/
                var h1 = res1[1] as int[][]; // второй параметр - двумерный массив вещ.чисел;
                var o1 = res1[2] as int[];   // третий параметр - выход сети - вещественное число;
                var o2 = res2[2] as int[];
                var out1 = res1[0];
                var out2 = res2[0];
                

                for (int i = 0; i < k; i++)
                {
                    // original
                    //weight[i] = ampl(weight[i] + input[i] * h1[s / N][s % N] * sgn(h1[s / N][s % N] * o1[s / N]) * sgn(o1[s / N] * o2[s / N]), L);
                    
                    //weight[i] = ampl(weight[i] + input[i] * h1[s / N][s % N] * sgn(h1[s / N][s % N] * Convert.ToInt32(out1)) * sgn(Convert.ToInt32(out1) * Convert.ToInt32(out2)), L);

                    weight[i] = ampl(weight[i] + input[i] * h1[s / N][s % N] * theta(h1[s / N][s % N], Convert.ToInt32(out1)) * theta(Convert.ToInt32(out1), Convert.ToInt32(out2)), L);
                    
                    // http://arxiv.org/pdf/cs/0408046v2.pdf    
                    //weight[i] = ampl(weight[i] + input[i] * o1[s / N], L);
                    
                    //  + o1[s/n]
                    //weight[i] = ampl(weight[i] + input[i] * o1[s / N] * sgn(h1[s / N][s % N] * o1[s / N]) * sgn(o1[s / N] * o2[s / N]), L);

                    //  + h1[s / N][s % N]
                    //weight[i] = ampl(weight[i] + h1[s / N][s % N] * input[i] * sgn(h1[s / N][s % N] * o1[s / N]) * sgn(o1[s / N] * o2[s / N]), L);
                    
                    // theta
                    //weight[i] = ampl(weight[i] + input[i] * o1[s / N] * theta(h1[s / N][s % N], o1[s / N]) * theta(o1[s / N], o2[s / N]), L);
                }
            }
        }

        public int theta(double x, double y)
        {
            int s = sgn(x - y);
            //MessageBox.Show(s.ToString());
            return s;
        }
        /* реализовать характеристисческую функцию для амплитуды */
        private int ampl(int x, int L)
        {
            if (Math.Abs(x) > L) return sgn(x) * L;
            else
                return x;
        }
        public int[] getWeights()
        {   // возвращаем массив весов
            return weight;
        }
    }
}
