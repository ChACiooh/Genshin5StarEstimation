using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace src
{
    class CalProb
    {
        private static double p = 0.006;    // probability of win 5 star.
        private static double q = 0.3235613866818617;   // probability of win 5 star from AMPLIFY-th.
        private static int AMPLIFY = 74;
        //private double p_, q_;

        private ProbNStack[][] want, nowant;
        private double[] prob;
        private static int DOL_SIZE = 14;
        private static int GACHA_SIZE = 90;
        private static int WANT = 1;
        private static int NOWANT = 0;
        public static int MAX_GACHA = 1260;

        private ProbNStack[][] dp;  // DOL_SIZE * MAX_GACHA

        public CalProb()
        {
            initializeTable();
            gen_dp(0, true);
        }

        public CalProb(int stack_, bool getPic)
        {
            initializeTable();
            InitializeBasicProb(stack_, getPic);
            gen_dp(stack_, getPic);
        }

        ~CalProb()
        {
            // ?
        }

        public static double GetBasicP()    { return p; }
        public static double GetBasicQ()    { return q; }

        private void initializeTable()
        {
            want = new ProbNStack[3][];
            nowant = new ProbNStack[3][];
            prob = new double[GACHA_SIZE+1];
            dp = new ProbNStack[DOL_SIZE+1][];
            for(int i = 0; i <= 2; ++i)
            {
                want[i] = new ProbNStack[GACHA_SIZE + 1];
                nowant[i] = new ProbNStack[GACHA_SIZE + 1];
                for(int j = 0; j <= GACHA_SIZE; ++j)
                {
                    want[i][j] = new ProbNStack();
                    nowant[i][j] = new ProbNStack();
                }
            }
            for(int i = 0; i <= DOL_SIZE ; ++i)
            {
                dp[i] = new ProbNStack[MAX_GACHA+1];
                for(int j = 0; j <= MAX_GACHA; ++j) {
                    dp[i][j] = new ProbNStack();
                }
            }
            for(int j = 1; j <= GACHA_SIZE; ++j)
            {
                prob[j] = pic5Nth(p, q, j);
            }
        }

        public void InitializeBasicProb(int stack_, bool getPic)
        {
            int j = 1;            

            // getPic에 따른 초기 돌파 조건 초기화
            // whatIwant~ 사용
            want[1][0].SetStack(stack_);
            nowant[1][0].SetStack(stack_);
            while(j <= stack_)
            {
                want[1][j].SetStack(j);
                nowant[1][j].SetStack(j);
                ++j;
            }
            while(j <= GACHA_SIZE)
            {
                want[1][j].SetProb(picWhatIwantInN(stack_, j - stack_, getPic));
                nowant[1][j].SetProb(picWhatIdonWantInN(stack_, j - stack_, getPic));
                
                //want[1][j].SetProb(pic5Nth(p_, q_, j));
                ++j;
            }

            // 직전에 픽업을 했을 때에 대한 일반 확률식
            for(j = 1; j <= GACHA_SIZE; ++j)
            {
                want[2][j].SetProb(picWhatIwantInN(0, j, true));   
                nowant[2][j].SetProb(picWhatIdonWantInN(0,j,true));   
            }

            // 직전에 픽뚫을 했을 때에 대한 일반 확률식
            for(j = 1; j <= GACHA_SIZE; ++j)
            {
                want[0][j].SetProb(picWhatIwantInN(0, j, false));
            }
        }

        private double calCondP(double p_, double q_, int stack_)
        {
            double condp = pic5Nth(p_, q_, stack_);
            if(stack_ >= AMPLIFY && stack_ < GACHA_SIZE)
            {
                condp /= q_;
            }
            else if(stack_ < AMPLIFY)
            {
                condp /= p_;
            }
            return condp;
        }

        /*
         *  wn : true = want, false = nowant
         */
        private double first_pic(int gacha, int cond)
        {
            if(gacha >= GACHA_SIZE)  gacha = GACHA_SIZE;
            else if(gacha <= 0) return 0.0;
            /*
            *   w_i = 2/3*prob^T * w_(i-1) + prob^T * n_(i-1)
            *   n_i = 1/3*prob^T * w_(i-1)
            *   i번째 stage에서 j개의 가챠를 돌려서 5성을 획득할 확률
            *   그리고 그것은 내가 원하는 것(want)과 원하지 않는 것(nowant)로 따로 분류
            *   i-1번째에서 만약 k개의 가챠만에 나온 확률에 j개의 가챠를 돌려서 5성을 얻으면 w_ijk
            *   3차원을 하지 말고, 데이터 구조를 활용
            */

            if(cond == WANT)  return want[1][gacha].GetProb();
            else if(cond == NOWANT)    return nowant[1][gacha].GetProb();
            
            Console.WriteLine("condition error:{0}", cond);
            return 0.0;
        }

        private double prob_on_pic(int gacha, int cond)
        {
            if(cond == WANT)    return want[2][gacha].GetProb();    // 픽업베이스에 픽업 확률
            else if(cond == NOWANT) return nowant[2][gacha].GetProb();  // 픽업베이스에 픽뚫 확률

            return 0.0;
        }

        private double prob_on_nopic(int gacha)
        {
            return want[0][gacha].GetProb();
        }

        private void gen_dp(int stack_, bool getPic)
        {
            for(int j = 1; j <= MAX_GACHA;++j)
            {   
                dp[1][j].SetProb(first_pic(j, WANT));    // 기본 첫 스테이지
                dp[1][j].SetStack(j);
                
                if(j > GACHA_SIZE && getPic)
                {
                    for(int k = 1; k < j && k <= GACHA_SIZE; ++k)
                    {
                        double dp1j = dp[1][j].GetProb();
                        double np = picWhatIdonWantInN(stack_, j-k, getPic) * picWhatIwantInN(0, k, false);
                        double dn = dp[1][j-k].GetProb() + np;
                        
                        if(dp1j < dn)
                        {
                            dp[1][j].SetProb(dn);
                            dp[1][j].SetStack(k);
                        }
                    }
                    if(dp[1][j].GetProb() >= 1.0) dp[1][j].SetProb(1.0);
                }
            }
            for(int n = 2; n <= DOL_SIZE; ++n)
            {
                for(int j = 1; j <= MAX_GACHA; ++j) // 전체 가챠 스테이지에서 사용하는 가차권 j개
                {
                    for(int k = 1; k < j && k <= GACHA_SIZE; ++k)   // 이번 가챠 스테이지에서 사용하는 가차권 k개
                    {
                        double dpnj, pp;
                        if(n % 2 == 0)  // 픽뚫 확률 (짝수는 못 뽑는 거)
                        {
                            dpnj = dp[n][j].GetProb();
                            pp = dp[n-1][j - k].GetProb() * prob_on_pic(k, NOWANT);
                            if(dpnj < pp)
                            {
                                dp[n][j].SetProb(pp);
                                dp[n][j].SetStack(k);
                            }
                        }
                        else    // 픽업-픽업 또는 픽뚫-픽업 (홀수는 뽑는 거)
                        {
                            pp = dp[n-2][j-k].GetProb() * prob_on_pic(k, WANT);
                            double np = dp[n-1][j-k].GetProb() * prob_on_nopic(k);
                            dpnj = dp[n][j].GetProb();
                            if(dpnj < pp + np)
                            {
                                dp[n][j].SetProb(pp + np);
                                dp[n][j].SetStack(k);
                            }
                        }
                        if(dp[n][j].GetProb() >= 1.0) dp[n][j].SetProb(1.0);
                        dp[n][j].SetStack(k);
                    }
                }
            }
        }

        public double DoubleMax(double a, double b)
        {
            return a > b ? a : b;
        }

        public double GetDP(int ndol, int gacha)
        {
            return dp[ndol * 2 - 1][gacha].GetProb();
        }

        public double picWhatIdonWantInN(int stack_, int nyeoncha, bool getPic)
        {
            if(nyeoncha <= 0)   return 0.0;
            if(!getPic)
            {
                return 0.0;
            }
            else if(stack_ + nyeoncha >= GACHA_SIZE)    return 0.5;

            double prob = 0.0, p_, q_;

            p_ = p / 2;
            q_ = q / 2;

            for(int i = stack_ + 1; i <= stack_ + nyeoncha && i <= GACHA_SIZE; ++i)
            {
                prob += pic5Nth(p_, q_, i);
            }

            return prob;
        }

        
        public double picWhatIwantInN(int stack_, int nyeoncha, bool getPic)
        {
            if(nyeoncha <= 0)   return 0.0;
            double prob = 0.0;
            double p_, q_;
            if(getPic)
            {
                p_ = p / 2;
                q_ = q / 2;
            }
            else
            {
                p_ = p;
                q_ = q;
            }

            double condp = calCondP(p_, q_, stack_);
            
            for(int i = stack_ + 1; i <= stack_ + nyeoncha && i <= GACHA_SIZE; ++i) {
                prob += pic5Nth(p_, q_, i);
            }

            return prob / condp;
        }

        public double pic5Nth(double p_, double q_, int nth)
        {
            double pk = 0.0;
            if(nth < AMPLIFY)
            {
                pk = pow(1-p, nth - 1) * p_;
            }
            else if(nth < GACHA_SIZE)
            {
                pk = pow(1-p, AMPLIFY - 1) * pow(1-q, nth - AMPLIFY) * q_;
            }
            else if(nth >= GACHA_SIZE)
            {
                pk = pow(1-p, AMPLIFY - 1) * pow(1-q, GACHA_SIZE - AMPLIFY);
            }

            return pk;
        }

        public double pow(double under, int exp)
        {
            double res = 1.0;
            for(int i = 0 ; i < exp; ++i) {
                res *= under;
            }
            return res;
        }

        public void PrintDP(int stack, int ndol, bool getPic)
        {
            for(int n = 1; n <= 7; ++n)
            {   
                string file_name = ""+n+(getPic ? "_basic_" : "_pick_")+stack+".txt";
                //string fn_likelihood = "ll"+n+(getPic ? "_basic" : "_pick")+".txt";
                StreamWriter sw = new StreamWriter(file_name);
                //StreamWriter swll = new StreamWriter(fn_likelihood);
                for(int i = 1; i <= CalProb.MAX_GACHA; ++i)
                {
                    double probability = GetDP(n, i) * 100;
                    sw.WriteLine("{0}:{1}%", i, probability);
                    if(probability >= 100.0)    break;
                    //swll.WriteLine("{0}:{1}%", i, cp.likelihood(n, i, getPic)*100);
                }
                sw.Close();
                //swll.Close();
            }
        }

        public double likelihood(int n, int gacha, bool getPic)
        {
            //double next_prob = pic5Nth(p_, q_, gacha);
            //double a = (1.0 - dp[n * 2 - 1][gacha-1].GetProb()) * next_prob;
            //double b = (1.0 - dp[(n-1) * 2 - 1][gacha-1].GetProb()) * next_prob;
            double a = dp[n*2-1][gacha].GetProb() - dp[n*2-1][gacha-1].GetProb();
            double p_ = p, q_ = q;

            if(getPic)
            {
                p_ /= 2;
                q_ /= 2;
            }

            double b = 0.0;

            if(n > 1)
            {
                b = dp[(n-1)*2-1][gacha-1].GetProb();
            }
            else if (n == 1)
            {
                for(int i = 1; i < gacha; ++i)
                {
                    b += (1 - pic5Nth(p_, q_, i));
                }
            }
            return (a+b) * pic5Nth(p_, q_, gacha);
        }
    }
}