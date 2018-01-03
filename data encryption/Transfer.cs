using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Threading;

namespace data_encryption
{
    public class Transfer
    {
        public int lol = 0; // флаг контроля процесса
        public event EventHandler<MyEventArgs> eventFromMyClass;
        public event EventHandler<ProgrEventArgs> progressEventFromMyClass;
        public string SomeString = "";

        public void setProgress(int c_val, int val)
        {
            Thread.Sleep(1);
            // метод, возвращающий в основной поток значение текущего прогресса синхронизации
            Task.Factory.StartNew((Action)delegate
            {
                if (progressEventFromMyClass != null)
                {
                    progressEventFromMyClass(this, new ProgrEventArgs(c_val, val));
                }
            });
        }

        public void showM(string s)
        {
            Thread.Sleep(1);
            set_s(s);
            // логирование
            Task.Factory.StartNew((Action)delegate
            {
                if (eventFromMyClass != null)
                {
                    eventFromMyClass(this, new MyEventArgs(SomeString));
                }
            });
        }

        public void Run(string s1, string s2, string s3, string s4, CheckBox c1, ProgressBar p1, string s5)
        {
            long count = 0; // сброс счётчика
            /* инициализация сетей*/
            ArtNeuronNetwork tpm1 = new ArtNeuronNetwork(Convert.ToInt32(s1), Convert.ToInt32(s2), Convert.ToInt32(s3));
            ArtNeuronNetwork tpm2 = new ArtNeuronNetwork(Convert.ToInt32(s1), Convert.ToInt32(s2), Convert.ToInt32(s3));
            /* синхронизация сетей*/
            synchronization(tpm1, tpm2, out count, s4, c1, p1, s5);
            MessageBox.Show("Синхронизация прекращена/закончена!");
            // вывод весов
            ShowWeights(tpm1, tpm2, count);
        }


        public void synchronization(ArtNeuronNetwork t1, ArtNeuronNetwork t2, out long count, string s1, CheckBox c1, ProgressBar p1, string s5) // меняем/не меняем весы
        {
            count = 0;
            int A = 0, B = 0; // реализовать синхронизацию
            int k = 0;
            int s = t1.getInpSize();
            encTCP cB = new encTCP();   // обмен по сети
            Random r = new Random();

            int[][] inP = new int[s][];
            //double[][] inP1 = new double[s][];
            for (int i = 0; i < s; i++)
            {
                inP[i] = new int[1];
                //inP1[i] = new double[1];
            }
            //cB.sendInputs(); // нужны ли мне лишние методы

            // пока не достигнуто условие синхронизации
            // или пока флаг контроля процесса не больше 1
            double c_synch = t1.K * t1.N * Math.Log(2 * t1.L + 1);
            int progress = 0, calcProgress = 0;

            while ((k < Convert.ToInt32(s1)) && (lol < 2) && (calcProgress < 100))
            //while ((k < Convert.ToInt32(s1)) && (lol < 2))
            //for (int k = 0; k < synch; k++)
            {
                //if (count < 1) { progress = 0; calcProgress = 0; }
                //else
                //{
                    calcProgress = Convert.ToInt32(count / c_synch * 100);
                    progress = Convert.ToInt32(Math.Round(k / Convert.ToDouble(s1) * 10000 / 100));
                //}
                setProgress(calcProgress, progress);

                /* синхронизация */

                //Thread.Sleep(50);
                
                // приостанавливаем поток, если флаг = 1
                while (lol == 1) Thread.Sleep(100);

                if (((count++ % Convert.ToInt32(s5)) == 0) && (c1.Checked == true))
                {
                    // вывод весов
                    ShowWeights(t1, t2, count);
                }
                if (count == Math.Round(c_synch)) ShowWeights(t1, t2, count);

                for (int i = 0; i < s; i++)
                    for (int j = 0; j < 1; j++)
                    {
                        r = new Random();
                        //inP[i][j] = r.NextDouble() * Math.Pow(-1, r.Next());
                        inP[i][j] = (int)Math.Pow(-1, r.Next());
                    }
                object[] res1 = t1.getOutp(inP), res2 = t2.getOutp(inP);
                A = Convert.ToInt32(res1[0]); // первый возвращаемый параметр - выход сети.
                //B = cB.getOutput(inP); // для получения выхода с удалённого клиента
                B = Convert.ToInt32(res2[0]);
                //MessageBox.Show(A.ToString() + " - " + B.ToString() + "\n k = " + k.ToString());
                //textBox3.Text += "";
                //setProgress(Convert.ToInt32((Math.Round(Convert.ToDouble(k + 1) / Convert.ToDouble(s1) * 10000)/100)));
                
                if (A == B)
                {
                    k++;
                    correct_weight(res1, res2, t1, t2);
                }
                else
                {
                    k = 0;
                }

                string t = Environment.NewLine + "Шаг №: " + count.ToString() + Environment.NewLine + "Выход первой сети: " + A.ToString() + Environment.NewLine + "Выход второй сети: " + B.ToString() + Environment.NewLine + "\n k = " + k.ToString() + "\n" + Environment.NewLine;
                showM(t);
            }
        }

        /*
        private void ShowWeights(ArtNeuronNetwork tpm1, ArtNeuronNetwork tpm2, int count)
        {
            string s = "\n На " + count.ToString() + "-й итерации " + "веса сетей: \n" + "Первая сеть:\n";
            for (int i = 0; i < tpm1.getInpSize() / tpm1.K; i++)
            {
                for (int j = 0; j < 1; j++)
                {
                    s += "\t" + i.ToString() + "-й эл.: " + tpm1.getWeights()[i][j].ToString();
                }
            }
            s += "\n\n\n";
            s += "\n Вторая сеть: \n";
            for (int i = 0; i < tpm2.getInpSize() / tpm1.K; i++)
            {
                for (int j = 0; j < 1; j++)
                {
                    s += "\t" + i.ToString() + "-й эл.: " + tpm2.getWeights()[i][j].ToString();
                }
            }
            s += "\n\n\n";
            MessageBox.Show(s);
        }*/

        private void ShowWeights(ArtNeuronNetwork tpm1, ArtNeuronNetwork tpm2, long count)
        {
            string s = "\n На " + count.ToString() + "-й итерации " + "веса сетей: \n";

            s += Environment.NewLine + "Первая сеть:\n" + Environment.NewLine;
            for (int i = 0; i < tpm1.getInpSize() / tpm1.K; i++)
            {
                for (int j = 0; j < 1; j++)
                {
                    s += "\t" + i.ToString() + "-й эл.: " + tpm1.getWeights()[i][j].ToString();
                }
            }

            s += Environment.NewLine + Environment.NewLine + "\n Вторая сеть: \n" + Environment.NewLine;
            for (int i = 0; i < tpm2.getInpSize() / tpm1.K; i++)
            {
                for (int j = 0; j < 1; j++)
                {
                    s += "\t" + i.ToString() + "-й эл.: " + tpm2.getWeights()[i][j].ToString();
                }
            }
            s += "\n\n\n" + Environment.NewLine + Environment.NewLine;
            showM(s);
        }


        public void correct_weight(object[] res1, object[] res2, ArtNeuronNetwork t1, ArtNeuronNetwork t2)
        {
            // для каждой сети используем правило хебба, вторая сеть передаётся как параметр, чтобы получить её выход
            // в дальнейшем, когда целиком сеть получить по сети будет нельзя, вторым параметром будет передаваться
            // только выход сети, который получен от синхронизируемого абонента.
            t1.use_hebbs_rule(res1, res2);
            t2.use_hebbs_rule(res2, res1);
        }


        public void set_s(string ns)
        {
            SomeString = ns;
        }
    }
}
