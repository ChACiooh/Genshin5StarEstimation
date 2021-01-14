namespace src
{
    class ProbNStack
    {
        private double probability_;
        private int stack_;

        public ProbNStack()
        {
            probability_ = 0.0;
            stack_ = 0;
        }

        public ProbNStack(double _p_, int _s_)
        {
            probability_ = _p_;
            stack_ = _s_;
        }

        public double GetProb()
        {
            return probability_;
        }

        public int GetStack()
        {
            return stack_;
        }

        public bool SetProb(double prob)
        {
            bool res = probability_ != prob;
            probability_ = prob;
            return res;
        }

        public bool SetStack(int stack)
        {
            bool res = stack_ != stack;
            stack_ = stack;
            return res;
        }
    }
}