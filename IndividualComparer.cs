using System.Collections;

namespace si_1
{
    class IndividualComparer : IComparer
    {
        public int Compare(object x, object y)
        {
            if (((Individual)x).GetTotalCost() < ((Individual)y).GetTotalCost())
                return 1;
            else if (((Individual)x).GetTotalCost() == ((Individual)y).GetTotalCost())
                return 0;
            else
                return -1;
        }
    }
}
