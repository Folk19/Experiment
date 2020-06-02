using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CRFSharp
{
    public class EncoderArgs
    {
        public int max_iter = 1000; //maximum iteration, when encoding iteration reaches this value, the process will be ended.
        public int min_feature_freq = 2; //minimum feature frequency, if one feature's frequency is less than this value, the feature will be dropped.
        public double min_diff = 0.0001; //minimum diff value, when diff less than the value consecutive 3 times, the process will be ended.
        public double slot_usage_rate_threshold = 0.95; //the maximum slot usage rate threshold when building feature set.
        public int threads_num = 8; //the amount of threads used to train model.
        public Encoder.REG_TYPE regType = Encoder.REG_TYPE.L2; //regularization type
        public string strTemplateFileName = null; //template file name
        public string strTrainingCorpus = null; //training corpus file name
        public string strEncodedModelFileName = null; //encoded model file name
        public string strRetrainModelFileName = null; //the model file name for re-training
        public int debugLevel = 0; //Debug level
        public uint hugeLexMemLoad = 0;
        public double C = 1.0; //cost factor, too big or small value may lead encoded model over tune or under tune

        public EncoderArgs(string trainingFileName, string templateFileName, string outputModelFileName)
        {
            strTrainingCorpus = trainingFileName;
            strTemplateFileName = templateFileName;
            strEncodedModelFileName = outputModelFileName;
        }

    }

    public class DecoderArgs
    {
        //public string strModelFileName;
        //public string strInputFileName;
        //public string strOutputFileName;
        //public string strOutputSegFileName;

        public string predictstring;
        public int outputstyle;
        public int nbest;
        public int thread;
        public int probLevel;

        public DecoderArgs(string predictString, int outputStyle, int nBest = 1, int Thread = 1, int prob = 0)
        {
            predictstring = predictString;
            outputstyle = outputStyle;
            nbest = nBest;
            thread = Thread;
            probLevel = prob;
        }
    }
}
