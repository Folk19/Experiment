using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using CRFSharp;
using System.Threading.Tasks;
using System.Collections.Concurrent;

namespace CRFSharp
{
    class DecoderConsole
    {
        private Decoder _crfWrapper;

        public DecoderConsole(string ModelFileName)
        {
            _crfWrapper = new Decoder();

            //Load model from file
            if (_crfWrapper.LoadModel(ModelFileName) == false)
            {
                throw new Exception("model format error");
            }
        }

        public void Usage()
        {
            Console.WriteLine("CRFSharpConsole -decode <options>");
            Console.WriteLine("-modelfile <string>  : The model file used for decoding");
            Console.WriteLine("-inputfile <string>  : The input file to predict its content tags");
            Console.WriteLine("-outputfile <string> : The output file to save raw tagged result");
            Console.WriteLine("-outputsegfile <string> : The output file to save segmented tagged result");
            Console.WriteLine("-nbest <int>         : Output n-best result, default value is 1");
            Console.WriteLine("-thread <int>        : <int> threads used for decoding");
            Console.WriteLine("-prob                : output probability, default is not output");
            Console.WriteLine("                       0 - not output probability");
            Console.WriteLine("                       1 - only output the sequence label probability");
            Console.WriteLine("                       2 - output both sequence label and individual entity probability");
            Console.WriteLine("Example: ");
            Console.WriteLine("         CRFSharp_Console -decode -modelfile ner.model -inputfile ner_test.txt -outputfile ner_test_result.txt -outputsegfile ner_test_result_seg.txt -thread 4 -nbest 3 -prob 2");
        }

        object rdLocker = new object();

        public string Predict(DecoderArgs _property)
        {

            ParallelOptions parallelOption = new ParallelOptions();
            StringWriter sw = null, swSeg = null;

            if (_property.outputstyle == 0)
                sw = new StringWriter();
            else
                swSeg = new StringWriter();


            ConcurrentQueue<List<List<string>>> queueRecords = new ConcurrentQueue<List<List<string>>>();
            ConcurrentQueue<List<List<string>>> queueSegRecords = new ConcurrentQueue<List<List<string>>>();

            StringReader sr = new StringReader(_property.predictstring);
            parallelOption.MaxDegreeOfParallelism = _property.thread;
            Parallel.For(0, parallelOption.MaxDegreeOfParallelism, t =>
            {
                //Create decoder tagger instance. If the running environment is multi-threads, each thread needs a separated instance
                SegDecoderTagger tagger = _crfWrapper.CreateTagger();
                tagger.set_nbest(_property.nbest);
                tagger.set_vlevel(_property.probLevel);

                //Initialize result
                crf_seg_out[] crf_out = new crf_seg_out[_property.nbest];
                for (int i = 0; i < _property.nbest; i++)
                {
                    crf_out[i] = new crf_seg_out();
                }

                List<List<string>> inbuf = new List<List<string>>();
                while (true)
                {
                    lock (rdLocker)
                    {
                        if (ReadRecord(inbuf, sr) == false)
                        {
                            break;
                        }

                        queueRecords.Enqueue(inbuf);
                        queueSegRecords.Enqueue(inbuf);
                    }

                    //Call CRFSharp wrapper to predict given string's tags
                    if (swSeg != null)
                    {
                        _crfWrapper.Segment(crf_out, tagger, inbuf);
                    }
                    else
                    {
                        _crfWrapper.Segment((crf_term_out[])crf_out, (DecoderTagger)tagger, inbuf);
                    }

                    List<List<string>> peek = null;

                    //Save segmented tagged result into file
                    if (swSeg != null)
                    {
                        List<string> rstList = ConvertCRFTermOutToStringList(inbuf, crf_out);
                        while (peek != inbuf)
                        {
                            queueSegRecords.TryPeek(out peek);
                        }
                        foreach (string item in rstList)
                        {
                            swSeg.WriteLine(item);
                        }
                        queueSegRecords.TryDequeue(out peek);
                        peek = null;
                    }

                    //Save raw tagged result (with probability) into file
                    if (sw != null)
                    {
                        while (peek != inbuf)
                        {
                            queueRecords.TryPeek(out peek);
                        }
                        OutputRawResult(inbuf, crf_out, tagger, sw);
                        queueRecords.TryDequeue(out peek);

                    }
                }
            });
        


            if (sw != null)
            {
                sw.Close();
                return sw.ToString();
            }
            if (swSeg != null)
            {
                swSeg.Close();
                return swSeg.ToString();
            }

            return "";
        }

        private bool ReadRecord(List<List<string>> inbuf, StringReader sr)
        {
            inbuf.Clear();

            while (true)
            {
                string strLine = sr.ReadLine();
                if (strLine == null)
                {
                    //At the end of current file
                    if (inbuf.Count == 0)
                    {
                        return false;
                    }
                    else
                    {
                        return true;
                    }
                }
                strLine = strLine.Trim();
                if (strLine.Length == 0)
                {
                    return true;
                }

                //Read feature set for each record
                string[] items = strLine.Split(new char[] { '\t' });
                inbuf.Add(new List<string>());
                foreach (string item in items)
                {
                    inbuf[inbuf.Count - 1].Add(item);
                }
            }
        }

        //Output raw result with probability
        private string OutputRawResult(List<List<string>> inbuf, crf_term_out[] crf_out, SegDecoderTagger tagger, StringWriter sw)
        {

            //best N-result
            for (int k = 0; k < crf_out.Length; k++)
            {
                if (crf_out[k] == null)
                {
                    //No more result
                    break;
                }

                StringBuilder sb = new StringBuilder();

                crf_term_out crf_seg_out = crf_out[k];
                //Show the entire sequence probability
                //For each token
                for (int i = 0; i < inbuf.Count; i++)
                {
                    //Show all features
                    for (int j = 0; j < inbuf[i].Count; j++)
                    {
                        sb.Append(inbuf[i][j]);
                        sb.Append("\t");
                    }

                    //Show the best result and its probability
                    sb.Append(crf_seg_out.result_[i]);

                    if (tagger.vlevel_ > 1)
                    {
                        sb.Append("\t");
                        sb.Append(crf_seg_out.weight_[i]);

                        //Show the probability of all tags
                        sb.Append("\t");
                        for (int j = 0; j < tagger.ysize_; j++)
                        {
                            sb.Append(tagger.yname(j));
                            sb.Append("/");
                            sb.Append(tagger.prob(i, j));

                            if (j < tagger.ysize_ - 1)
                            {
                                sb.Append("\t");
                            }
                        }
                    }
                    sb.AppendLine();
                }
                if (tagger.vlevel_ > 0)
                {
                    sw.WriteLine("#{0}", crf_seg_out.prob);
                }
                sw.WriteLine(sb.ToString().Trim());
                sw.WriteLine();
            }


            return sw.ToString();
        }

        //Convert CRFSharp output format to string list
        private List<string> ConvertCRFTermOutToStringList(List<List<string>> inbuf, crf_seg_out[] crf_out)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < inbuf.Count; i++)
            {
                sb.Append(inbuf[i][0]);
            }

            string strText = sb.ToString();
            List<string> rstList = new List<string>();
            for (int i = 0; i < crf_out.Length; i++)
            {
                if (crf_out[i] == null)
                {
                    //No more result
                    break;
                }

                sb.Clear();
                crf_seg_out crf_term_out = crf_out[i];
                for (int j = 0; j < crf_term_out.Count; j++)
                {
                    string str = strText.Substring(crf_term_out.tokenList[j].offset, crf_term_out.tokenList[j].length);
                    string strNE = crf_term_out.tokenList[j].strTag;

                    sb.Append(str);
                    if (strNE.Length > 0)
                    {
                        sb.Append("[" + strNE + "]");
                    }
                    sb.Append(" ");
                }
                rstList.Add(sb.ToString().Trim());
            }

            return rstList;
        }
    }
}
