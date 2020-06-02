using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CRFSharp
{
    class EncoderConsole
    {
        public void Train(EncoderArgs arg)
        {
            Encoder encoder = new Encoder();

            bool bRet;
            bRet = encoder.Learn(arg);
        }

        private static void Usage()
        {
            Console.WriteLine("Linear-chain CRF encoder & decoder by Zhongkai Fu (fuzhongkai@gmail.com)");
            Console.WriteLine("CRFSharpConsole.exe -encode [parameters list]");
            Console.WriteLine("\t-template <string> : template file name");
            Console.WriteLine("\t-trainfile <string> : training corpus file name");
            Console.WriteLine("\t-modelfile <string> : encoded model file name");
            Console.WriteLine("\t-maxiter <int> : The maximum encoding iteration. Default value is 1000");
            Console.WriteLine("\t-minfeafreq <int> : Any feature's frequency is less than the value will be dropped. Default value is 2");
            Console.WriteLine("\t-mindiff <float> : If diff is less than the value consecutive 3 times, the encoding will be ended. Default value is 0.0001");
            Console.WriteLine("\t-thread <int> : the amount of threads for encoding. Default value is 1");
            Console.WriteLine("\t-slotrate <float> : the maximum slot usage rate threshold when building feature set. it is ranged in (0.0, 1.0). the higher value takes longer time to build feature set, but smaller feature set size.  Default value is 0.95");
            Console.WriteLine("\t-regtype <string> : regularization type (L1 and L2). L1 will generate a sparse model. Default is L2");
            Console.WriteLine("\t-hugelexmem <int> : build lexical dictionary in huge mode and shrinking start when used memory reaches this value. This mode can build more lexical items, but slowly. Value ranges [1,100] and default is disabled.");
            Console.WriteLine("\t-retrainmodel <string> : the existed model for re-training.");
            Console.WriteLine("\t-debug <int> : debug level, default value is 1");
            Console.WriteLine("\t               0 - no debug information output");
            Console.WriteLine("\t               1 - only output raw lexical dictionary for feature set");
            Console.WriteLine("\t               2 - full debug information output, both raw lexical dictionary and detailed encoded information for each iteration");
            Console.WriteLine();
            Console.WriteLine("Note: either -maxiter reaches setting value or -mindiff reaches setting value in consecutive three times, the training process will be finished and saved encoded model.");
            Console.WriteLine("Note: -hugelexmem is only used for special task, and it is not recommended for common task, since it costs lots of time for memory shrink in order to load more lexical features into memory");
            Console.WriteLine();
            Console.WriteLine("A command line example as follows:");
            Console.WriteLine("\tCRFSharpConsole.exe -encode -template template.1 -trainfile ner.train -modelfile ner.model -maxiter 100 -minfeafreq 1 -mindiff 0.0001 -thread 4 -debug 1 -slotrate 0.95");
        }
    }
}
