namespace GridLib.Square
{
    public class SquareRotation
    {
        private uint _CWcount;

        public int CWcount
        {
            get { return (int)_CWcount; }
            set
            {
                _CWcount = NumUtil.WrapPos(value, 4);
            }
        }

        public SquareRotation(int CWcount)
        {
            this.CWcount = CWcount;
        }

        public static readonly SquareRotation CW = new SquareRotation(1);
        public static readonly SquareRotation Neutral = new SquareRotation(0);
        public static readonly SquareRotation CCW = new SquareRotation(-1);

        public static SquareRotation operator+(SquareRotation left, SquareRotation right)
        {
            return new SquareRotation(left.CWcount + right.CWcount);
        }

        public static SquareRotation operator *(SquareRotation left, int right)
        {
            return new SquareRotation(left.CWcount * right);
        }

        public static SquareRotation operator *(int left, SquareRotation right)
        {
            return right * left;
        }
    }
}
