using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace src
{
    class Program
    {
        static void Main(string[] args)
        {
            //Console.WriteLine("Hello World!");
            
            Console.WriteLine("현재 쌓인 스택값 입력:");
            int stack = int.Parse(Console.ReadLine());
            Console.WriteLine("전에 픽업캐 뽑았나?(Y/N)");
            var getPic_ = Console.ReadLine();

            bool getPic = (getPic_[0] == 'y' || getPic_[0] == 'Y');
            if(!getPic)
            {
                Console.WriteLine("다음번 5성은 무조건 픽업 캐릭터");
            }
            else
            {
                Console.WriteLine("기본 확률 업 적용");
            }

            CalProb cp = new CalProb(stack, getPic);
            
            Console.Write("지를 수 있는 가챠 수 > ");
            int gacha = int.Parse(Console.ReadLine());
            
            Console.Write("뽑고자 하는 픽업캐 돌파 수(명함 = 1) > ");
            int ndol = int.Parse(Console.ReadLine());

            double result = cp.GetDP(ndol, stack + gacha);

            Console.WriteLine("{0}연차 내로 뽑을 확률 = {1}%", gacha, result * 100);

            for(int n = 1; n <= 7; ++n)
            {   
                string file_name = ""+n+(getPic ? "_basic" : "_pick")+".txt";
                StreamWriter sw = new StreamWriter(file_name);
                for(int i = 1; i <= 1260; ++i)
                {
                    sw.WriteLine("{0}:{1}%", i, cp.GetDP(n, i)*100);
                }
                sw.Close();
            }
           
        }
    }
}
