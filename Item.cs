using System;

namespace si_1
{
    public class Item
    {
        public int _id { get; }
        public int _profit { get; }
        public int _weight { get; }
        public int _nodeId { get; }

        public Item(int id, int profit, int weight, int nodeId)
        {
            _id = id;
            _profit = profit;
            _weight = weight;
            _nodeId = nodeId;
        }

    }
}
