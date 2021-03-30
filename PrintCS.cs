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

        public PrintCS() {}

        public void PrintTxtAll()
        {
            string path = "./result_txt/";
            string file_name = path + "result.txt";
            StreamWriter sw = new StreamWriter(file_name);
            bool getPic = false;
            for(int stack = 0; stack < CalProb.GACHA_SIZE; ++stack)
            {
                cp = new CalProb(stack, getPic);
                for(int n = 1; n <= CalProb.DOL_SIZE / 2; ++n)
                {   
                    // getpic_stack_gacha_ndol
                    string data = (getPic ? "Y_" : "N_")+stack;
                    
                    for(int gacha = 1; gacha + stack <= CalProb.MAX_GACHA; ++gacha)
                    {
                        double probability = cp.GetDP(n, stack + gacha) * 100;
                        bool flag = probability >= 99.99999;
                        if(flag)    probability = 100.0;
                        sw.WriteLine("{0}_{1}_{2} {3}%", data, gacha, n, probability);
                        if(flag)    break;
                    }
                    
                }
                if(!getPic && stack == CalProb.GACHA_SIZE - 1)
                {
                    getPic = !getPic;
                    stack = -1;
                }
            }
                
            sw.Close();
        }
        public void PrintTxt(int stack, int ndol, bool getPic)
        {
            for(int n = 1; n <= ndol; ++n)
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
                try
                {
                    xdoc.Save(@file_name);
                } catch (System.IO.DirectoryNotFoundException) {
                    System.IO.Directory.CreateDirectory(path);
                    xdoc.Save(@file_name);
                }
            }
        }

        public void PrintXmlAll()
        {
            //
            String path = "./result_xml/";
            String file_name = path + "result.xml";
            XmlDocument xdoc = new XmlDocument();
            XmlNode root = xdoc.CreateElement("Tbl_ChrGachaProb");

            bool getPic = false;
            for(int stack = 0; stack < CalProb.GACHA_SIZE; ++stack)
            {
                for(int ndol = 1; ndol <= CalProb.DOL_SIZE / 2; ++ndol)    
                {
                    for(int gacha = 1; gacha + stack <= CalProb.MAX_GACHA; ++gacha)
                    {
                        // TODO
                    }
                }
            }
        }
    }
}