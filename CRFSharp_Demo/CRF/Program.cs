using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CRFSharp;
using System.IO;

namespace CRF
{
    class Program
    {
        static void Main(string[] args)
        {
            /*training*/
            EncoderConsole encoder = new EncoderConsole();
            EncoderArgs encoderArg = new EncoderArgs("ExampleData\\training.txt", "ExampleData\\template", "ExampleData\\model");
            encoder.Train(encoderArg);

            /*predixt*/
            DecoderConsole decoder = new DecoderConsole("ExampleData\\model");//執行這一行就會先把model load入記憶體


            string predict = File.ReadAllText("ExampleData\\test.txt");
            DecoderArgs decoderArg = new DecoderArgs(predict, 0);
            string result = decoder.Predict(decoderArg);

            Console.WriteLine(result);
            Console.ReadKey();
        }
    }
}
