#encoding=utf-8
import sys, codecs, jieba

reload(sys)
sys.setdefaultencoding('utf-8')

jeiba_output_file = codecs.open('jeiba_title_output1.txt', 'w','utf-8')
with codecs.open('embBaiduIndexName.txt', 'r','utf-8') as content:
    for line in content:
        #print line
        words = jieba.cut(line, cut_all=False)
        str = ''
        for word in words:
            str = str + word + ' '
        #print str
        jeiba_output_file.write(str.strip()+'\n')
content.close()
jeiba_output_file.close()
print "done"