using System;
using System.Collections.Generic;

namespace si_1
{
    public class Node
    {
        public int _id { get; }
        public float _posX { get; }
        public float _posY { get; }
        public List<Item> _items { get; }

        public Node(int id, float posX, float posY)
        {
            _id = id;
            _posX = posX;
            _posY = posY;
            _items = new List<Item>();
        }

        public void AssignItem(Item item)
        {
            _items.Add(item);
        }
    }
}
