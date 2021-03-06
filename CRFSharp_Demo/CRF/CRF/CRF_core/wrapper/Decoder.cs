﻿/**********************************************/
/*Project: CRF#                               */
/*Author: Zhongkai Fu                         */
/*Email: fuzhongkai@gmail.com                 */
/**********************************************/

using System;
using System.Collections.Generic;
using System.Text;
using CRFSharp;

namespace CRFSharp
{
    public class Decoder
    {
        ModelReader modelReader;

        //Load encoded model form file
        public bool LoadModel(string strModelFileName)
        {
            modelReader = new ModelReader();
            return modelReader.LoadModel(strModelFileName);
        }

        public SegDecoderTagger CreateTagger()
        {
            if (modelReader == null)
            {
                return null;
            }
            SegDecoderTagger tagger = new SegDecoderTagger();
            tagger.init_by_model(modelReader);

            return tagger;
        }

        //Segment given text
        public int Segment(crf_seg_out[] pout, //segment result
            SegDecoderTagger tagger, //Tagger per thread
            List<List<string>> inbuf //feature set for segment
            )
        {
            int ret = 0;
            if (inbuf.Count == 0)
            {
                //Empty input string
                return Utils.ERROR_SUCCESS;
            }

            ret = tagger.reset();
            if (ret < 0)
            {
                return ret;
            }

            ret = tagger.add(inbuf);
            if (ret < 0)
            {
                return ret;
            }

            //parse
            ret = tagger.parse();
            if (ret < 0)
            {
                return ret;
            }

            //wrap result
            ret = tagger.output(pout);
            if (ret < 0)
            {
                return ret;
            }

            return Utils.ERROR_SUCCESS;
        }



        //Segment given text
        public int Segment(crf_term_out[] pout, //segment result
            DecoderTagger tagger, //Tagger per thread
            List<List<string>> inbuf //feature set for segment
            )
        {
            int ret = 0;
            if (inbuf.Count == 0)
            {
                //Empty input string
                return Utils.ERROR_SUCCESS;
            }

            ret = tagger.reset();
            if (ret < 0)
            {
                return ret;
            }

            ret = tagger.add(inbuf);
            if (ret < 0)
            {
                return ret;
            }

            //parse
            ret = tagger.parse();
            if (ret < 0)
            {
                return ret;
            }

            //wrap result
            ret = tagger.output(pout);
            if (ret < 0)
            {
                return ret;
            }

            return Utils.ERROR_SUCCESS;
        }
    }
}
