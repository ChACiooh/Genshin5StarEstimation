using System.Xml;
using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/***
*   reference : https://www.csharpstudy.com/Data/Xml-xmldoc.aspx
***/

namespace src
{
    class PrintCS
    {
        private CalProb cp;

        public PrintCS(CalProb calProb)
        {
            cp = calProb;   // reference 참조인데 사실 상관이 없다. 어차피 이 클래스는 출력용
        }

        PrintCS() {}

        public void PrintTxt(int stack, int ndol, bool getPic)
        {
            for(int n = 1; n <= 7; ++n)
            {   
                string path = "./result_txt/";
                string file_name = path+n+(getPic ? "_basic_" : "_pick_")+stack+".txt";
                StreamWriter sw = new StreamWriter(file_name);
                for(int i = 1; i <= CalProb.MAX_GACHA; ++i)
                {
                    double probability = cp.GetDP(n, stack + i) * 100;
                    bool flag = probability >= 99.99999;
                    if(flag)    probability = 100.0;
                    sw.WriteLine("{0}:{1}%", i, probability);
                    if(flag)    break;
                }
                sw.Close();
            }
        }

        public void PrintXml(int stack, int ndol, bool getPic)
        {
            String path = "./result_xml/";
            for(int k = 1; k <= ndol; ++k)
            {
                String file_name = path + k + "_" + (getPic ? "basic" : "pick") + "_" + stack + ".xml";
                XmlDocument xdoc = new XmlDocument();
                String table_name = "Character_Gacha";
                XmlNode root = xdoc.CreateElement(table_name);
                xdoc.AppendChild(root);
                    
                for(int j = 1; j <= CalProb.MAX_GACHA; ++j)
                {
                    double probability = cp.GetDP(k, stack + j) * 100;
                    bool flag = probability >= 99.99999;
                    if(flag)    probability = 100.0;
                    XmlNode n_gacha = xdoc.CreateElement("Gacha");
                    XmlAttribute n_gacha_attr = xdoc.CreateAttribute("Id");
                    n_gacha_attr.Value = "" + k + "_" + j;
                    n_gacha.Attributes.Append(n_gacha_attr);

                    XmlNode item = xdoc.CreateElement("Prob");
                    item.InnerText = ""+probability+"%";
                    n_gacha.AppendChild(item);

                    root.AppendChild(n_gacha);
                    if(flag)    break;
                }
                
                // XML 파일 저장
                xdoc.Save(@file_name);
            }
        }
    }
}