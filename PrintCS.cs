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
        private int id;

        public PrintCS(CalProb calProb)
        {
            cp = calProb;   // reference 참조인데 사실 상관이 없다. 어차피 이 클래스는 출력용
            id = 1;
        }

        public PrintCS() { id = 1; }

        public void PrintTxtAll()
        {
            for(int ndol = 1; ndol <= CalProb.DOL_SIZE / 2; ++ndol) {
                PrintTxt(ndol, false);
            }
            for(int ndol = 1; ndol <= CalProb.DOL_SIZE / 2; ++ndol) {
                PrintTxt(ndol, true);
            }
        }
        public void PrintTxt(int ndol, bool getPic)
        {
            CalProb[] cplist = new CalProb[CalProb.GACHA_SIZE];
            for(int stack = 0; stack < CalProb.GACHA_SIZE; ++stack) {
                CalProb tmp = new CalProb(stack, getPic);
                cplist[stack] = tmp;
            }
            
            string path = "./result_txt/"; 
            //string file_name = path+(getPic ? "Y_" : "N_")+n+".txt";
            string file_name = path + (getPic ? "basic_" : "pick_") + ndol + ".txt";
            StreamWriter sw = new StreamWriter(file_name);
            for(int stack = 0; stack < CalProb.GACHA_SIZE; ++stack)
            {
                cp = cplist[stack];
                
                for(int gacha = 1; gacha + stack <= CalProb.MAX_GACHA; ++gacha)
                {
                    double probability = cp.GetDP(ndol, stack + gacha) * 100;
                    bool flag = probability >= 99.99999;
                    if(flag)    probability = 100.0;
                    sw.WriteLine("{0}_{1} {2}%", stack, gacha, probability);
                    if(flag)    break;
                }
            }
            sw.Close();
        }

        public void PrintXml(int ndol, bool getPic)
        {
            String path = "./result_xml/";
            String file_name = path + (getPic ? "basic_" : "pick_") + ndol + ".xml";

            XmlDocument xdoc = new XmlDocument();
            XmlNode root = xdoc.CreateElement("Tbl_ChrGachaProb");
            xdoc.AppendChild(root);
            
            for(int stack = 0; stack < CalProb.GACHA_SIZE; ++stack)
            {
                cp = new CalProb(stack, getPic);
                for(int gacha = 1; gacha + stack <= CalProb.MAX_GACHA; ++gacha)
                {
                    double probability = cp.GetDP(ndol, stack + gacha) * 100;
                    bool flag = probability >= 99.99999;
                    if(flag)    probability = 100.0;
                    XmlNode n_gacha = xdoc.CreateElement("Try");
                    XmlAttribute n_gacha_attr_id = xdoc.CreateAttribute("Id");
                    n_gacha_attr_id.Value += id++;
                    
                    XmlAttribute n_gacha_attr_stack = xdoc.CreateAttribute("STACK");
                    n_gacha_attr_stack.Value += stack;

                    XmlAttribute n_gacha_attr_ndol = xdoc.CreateAttribute("NDOL");
                    n_gacha_attr_ndol.Value += ndol;
                    
                    XmlAttribute n_gacha_attr_gacha = xdoc.CreateAttribute("GACHA");
                    n_gacha_attr_gacha.Value += gacha;
                    XmlAttribute n_gacha_attr_prob = xdoc.CreateAttribute("PROB");
                    n_gacha_attr_prob.Value += probability + "%";
                    n_gacha.Attributes.Append(n_gacha_attr_id);
                    n_gacha.Attributes.Append(n_gacha_attr_stack);
                    n_gacha.Attributes.Append(n_gacha_attr_ndol);
                    n_gacha.Attributes.Append(n_gacha_attr_gacha);
                    n_gacha.Attributes.Append(n_gacha_attr_prob);
                    root.AppendChild(n_gacha);
                    if(flag)    break;
                }
            }
                
            // XML 파일 저장
            try {
                xdoc.Save(@file_name);
            } catch (System.IO.DirectoryNotFoundException) {
                System.IO.Directory.CreateDirectory(path);
                xdoc.Save(@file_name);
            }
        }

        public void PrintXmlAll()
        {
            for(int ndol = 1; ndol <= CalProb.DOL_SIZE / 2; ++ndol) {
                PrintXml(ndol, false);
            }
            for(int ndol = 1; ndol <= CalProb.DOL_SIZE / 2; ++ndol) {
                PrintXml(ndol, true);
            }
        }
    }
}