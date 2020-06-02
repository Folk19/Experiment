using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CRFSharp
{
    class ShrinkConsole
    {
        public void Run(string[] args)
        {
            if (args.Length != 4)
            {
                Console.WriteLine("CRFSharpConsole -shrink [Encoded CRF model file name] [Shrinked CRF model file name] [thread num]");
                return;
            }

            Shrink shrink = new Shrink();
            shrink.Process(args[1], args[2], int.Parse(args[3]));
        }
    }
}
