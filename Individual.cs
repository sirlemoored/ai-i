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
        public float _costF { get; set; }
        public float _costG { get; set; }

        public Individual(int routeLength)
        {
            _routeLength = routeLength;
            _order = Enumerable.Range(0, _routeLength).OrderBy(x => Guid.NewGuid()).ToList();
        }

        public Individual(List<int> order)
        {
            _routeLength = order.Count;
            _order = new List<int>(order);
        }

        public float GetTotalCost() => _costG - _costF;

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

        // DEBUG
        public string PrintCosts()
        {
            return "Czas: " + _costF + ", łup:" + _costG;
        }
    }
}
