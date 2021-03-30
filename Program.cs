using System;

namespace src
{
    class Program
    {
        private static PrintCS pcs;
        static void Main(string[] args)
        {
            //input_sequence();
            file_sequence();
        }

        static void file_sequence()
        {
            print_txt_all();
            //print_xml_all();
        }

        static void input_sequence()
        {
            int stack, gacha, ndol;
            bool getPic;
            Console.WriteLine("전에 픽업캐 뽑았나?(Y/N)");
            var getPic_ = Console.ReadLine();
            Console.WriteLine("현재 쌓인 스택값 입력:");
            stack = int.Parse(Console.ReadLine());

            getPic = (getPic_[0] == 'y' || getPic_[0] == 'Y');
            if(!getPic)
            {
                Console.WriteLine("다음번 5성은 무조건 픽업 캐릭터");
            }
            else
            {
                Console.WriteLine("기본 확률 업 적용");
            }
            
            Console.Write("지를 수 있는 가챠 수 > ");
            gacha = int.Parse(Console.ReadLine());
            
            Console.Write("뽑고자 하는 픽업캐 돌파 수(명함 = 1) > ");
            ndol = int.Parse(Console.ReadLine());

            CalProb cp = new CalProb(stack, getPic);
            double result = cp.GetDP(ndol, stack + gacha);

            Console.WriteLine("{0}연차 내로 뽑을 확률 = {1}%", gacha, result * 100);
        }

        static void print_txt(int ndol, bool getPic)
        {
            pcs = new PrintCS();
            pcs.PrintTxt(ndol, getPic);
        }

        static void print_xml(int ndol, bool getPic)
        {
            pcs = new PrintCS();
            pcs.PrintXml(ndol, getPic);
        }

        static void print_txt_all()
        {
            pcs = new PrintCS();
            pcs.PrintTxtAll();
        }

        static void print_xml_all()
        {
            pcs = new PrintCS();
            pcs.PrintXmlAll();
        }
    }
}
