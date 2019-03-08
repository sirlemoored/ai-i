using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace si_1
{
    class Individual
    {
        public int _routeLength { get; }
        public List<int> _order { get; }

        public Individual(int routeLength)
        {
            _routeLength = routeLength;
            _order = Enumerable.Range(0, _routeLength).OrderBy(x => Guid.NewGuid()).ToList();
        }

        // DEBUG
        public string PrintRoute()
        {
            StringBuilder sb = new StringBuilder();
            for(int i = 0; i < _routeLength; i++)
            {
                sb.Append(_order[i] + "-");
            }
            sb.Append(_order[0]);
            return sb.ToString();
        }
    }
}
