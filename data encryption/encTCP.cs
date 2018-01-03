using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace data_encryption
{
    class encTCP
    {
        public encTCP()
        {
            /* create connection */
            /* set server, find client */
        }


        /*
        public void sendInputs(int[][] inP)
        {
            //sending Inputs to B
        }
        */

        public int getOutput(double[][] inP /*client_ID or smthing*/)
        {
            // сначала sendInputs выполняется здесь, затем ожидаются вычисления и
            // здесь же выполняется приём выходного значения
            int outp = 0;
            /* connect and receive output from B */
            return outp;
        }
    }
}
