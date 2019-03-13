using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

namespace si_1
{
    class Environment
    {

        private int _populationSize;
        private string _fileName;
        private float _minSpeed;
        private float _maxSpeed;
        private int _capacity;
        private float[,] _distanceMatrix;
        private int _dimension;
        private int _numberItems;
        private List<Node> _nodes;
        private List<Individual> _individuals;

        public Environment(int populationSize, string fileName)
        {
            _fileName = fileName;
            _populationSize = populationSize;
            _nodes = new List<Node>();
            _individuals = new List<Individual>(_populationSize);
        }

        public void LoadData()
        {
            StreamReader str = new StreamReader("D:\\Informatyka\\Semestr 6\\si_1\\si_1\\data\\" + _fileName);
            List<string> parameters = new List<string>();
            
            for (int i = 0; i < 9; i++)
            {
                string line = str.ReadLine();
                string[] words = line.Split(new char[] { ':' });
                parameters.Add(words[1].Replace('\t', ' ').Trim());
            }

            _dimension = int.Parse(parameters[2], CultureInfo.InvariantCulture);
            _numberItems = int.Parse(parameters[3], CultureInfo.InvariantCulture);
            _capacity = int.Parse(parameters[4], CultureInfo.InvariantCulture);
            _minSpeed = float.Parse(parameters[5], CultureInfo.InvariantCulture);
            _maxSpeed = float.Parse(parameters[6], CultureInfo.InvariantCulture);
            
            str.ReadLine();

            for (int i = 0; i < _dimension; i++)
            {
                string line = str.ReadLine();
                string[] data = line.Split(new char[] { '\t' });
                Node n = new Node(int.Parse(data[0], CultureInfo.InvariantCulture) - 1, float.Parse(data[1], CultureInfo.InvariantCulture), float.Parse(data[2], CultureInfo.InvariantCulture));
                InsertNode(n);
            }

            str.ReadLine();

            for (int i = 0; i < _numberItems; i++)
            {
                string line = str.ReadLine();
                string[] data = line.Split(new char[] { '\t' });
                Item it = new Item(int.Parse(data[0], CultureInfo.InvariantCulture) - 1, int.Parse(data[1], CultureInfo.InvariantCulture), int.Parse(data[2], CultureInfo.InvariantCulture), int.Parse(data[3], CultureInfo.InvariantCulture) - 1);
                InsertItem(it);
            }


        }

        public void InsertNode(Node node)
        {
            _nodes.Add(node);
        }

        public void InsertItem(Item item)
        {
            int newId = item._id;
            bool nodeFound = false;
            for (int i = 0; i < _nodes.Count && !nodeFound; i++)
            {
                if (_nodes[i]._id == item._nodeId)
                {
                    nodeFound = true;
                    _nodes[i].AssignItem(item);
                }
            }
        }

        public void CreateDistanceMatrix()
        {
            int matrixSize = _nodes.Count;

            if (matrixSize <= 0)                                                                                // check whether matrix size is smaller than 1
                return;

            _distanceMatrix = new float[matrixSize, matrixSize];

            for (int i = 0; i < matrixSize; i++)                                                                // calculate euclidean distance between two points
            {
                for (int j = 0; j < matrixSize; j++)
                {   
                    float distance = (float)Math.Ceiling((float)Math.Sqrt(Math.Pow(_nodes[i]._posX - _nodes[j]._posX, 2) + (float)(Math.Pow(_nodes[i]._posY - _nodes[j]._posY, 2))));
                    _distanceMatrix[i,j] = distance;
                }
            }
        }

        public void InitializePopulation()
        {
            for (int i = 0; i < _populationSize; i++)
                _individuals.Add(new Individual(_dimension));

            for (int i = 0; i < _populationSize; i++)
                CalculateCosts(_individuals[i]);
        }

        public void CalculateCosts(Individual ind)
        {
            int totalWeight = 0;
            float time = 0;
            float distance = 0;
            float velocity = 0;
            float value = 0;
            for (int i = 0; i < ind._routeLength - 1; i++)
            {
                int weight = 0;
                if (_nodes[ind._order[i]]._items.Count != 0)
                {
                    weight = _nodes[ind._order[i]]._items.First()._weight;
                    if (totalWeight + weight <= _capacity)
                    {
                        totalWeight += weight;
                        value += _nodes[ind._order[i]]._items.First()._profit;
                    }
                }                                                                                              

                distance = _distanceMatrix[_nodes[ind._order[i]]._id, _nodes[ind._order[i + 1]]._id];
                velocity = _maxSpeed - (float)totalWeight * (_maxSpeed - _minSpeed) / (float)_capacity;
                time += distance / velocity;
            }

            distance = _distanceMatrix[_nodes[ind._order[ind._routeLength - 1]]._id, _nodes[ind._order[0]]._id];
            velocity = _maxSpeed - (float)totalWeight * (_maxSpeed - _minSpeed) / (float)_capacity;
            time += distance / velocity;

            ind._costF = time;
            ind._costG = value;

        }

        public int getNumberOfNodes() => _dimension;
        
        // DEBUG
        public void PrintDistanceMatrix()
        {
            for (int i = 0; i < _nodes.Count; i++)
            {
                for (int j = 0; j < _nodes.Count; j++)
                {
                    Console.Write(String.Format("{0, 10}", _distanceMatrix[i, j]));
                }
                Console.WriteLine('\n');
            }
        }

        // DEBUG
        public void PrintNodes()
        {
            foreach (var node in _nodes)
            {
                Console.Write("Node id: " + node._id + " Items: ");
                foreach (var item in node._items)
                {
                    Console.Write(item.GetProfit() + " ");
                }
                Console.WriteLine();
            }
        }

        // DEBUG
        public void PrintPopulation()
        {
            for (int i = 0; i < _populationSize; i++)
                Console.WriteLine("OSOBNIK " + i + " | ZYSK " + _individuals[i].GetTotalCost() + " | DROGA " + _individuals[i].PrintRoute());
        }
    }
}
