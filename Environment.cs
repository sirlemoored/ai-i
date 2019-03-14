using System;
using System.Collections;
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
        private int _tournamentSize = 5;
        private float _probabilityMutation = 0.05f;
        private float _probabilityCrossover = 0.7f;
        private float _percentageMutation = 0.1f;
        private float _fitnessExpModifier = 1.0f;
        private List<Node> _nodes;
        public List<Individual> _individuals;
        private List<Individual> _toCrossover;

        public Environment(int populationSize, string fileName, float probabilityMutation, float probabilityCrossover, float percentageMutation, float fitnessExpModifier)
        {
            _fileName = fileName;
            _populationSize = populationSize;
            _nodes = new List<Node>();
            _individuals = new List<Individual>(_populationSize);
            _probabilityCrossover = probabilityCrossover;
            _probabilityMutation = probabilityMutation;
            _percentageMutation = percentageMutation;
            _fitnessExpModifier = fitnessExpModifier;
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

            if (matrixSize <= 0)
                return;

            _distanceMatrix = new float[matrixSize, matrixSize];

            for (int i = 0; i < matrixSize; i++)
            {
                for (int j = 0; j < matrixSize; j++)
                {
                    float distance = (float)Math.Ceiling((float)Math.Sqrt(Math.Pow(_nodes[i]._posX - _nodes[j]._posX, 2) + (float)(Math.Pow(_nodes[i]._posY - _nodes[j]._posY, 2))));
                    _distanceMatrix[i, j] = distance;
                }
            }
        }

        public void InitializePopulation()
        {
            for (int i = 0; i < _populationSize; i++)
                _individuals.Add(new Individual(_dimension));
        }

        public void AssessPopulationFitness()
        {
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

        public void SelectRoulette()
        {
            _individuals.Sort(new IndividualComparer().Compare);
            float[] fitnesses = new float[_individuals.Count];

            // NORMALIZING NEGATIVE AND POSITIVE FITNESSES

            float fitnessSumPos = 0; float fitnessAvgPos = 1; int fitnessCountPos = 0; float fitnessMax = 0;
            float fitnessSumNeg = 0; float fitnessAvgNeg = 1; int fitnessCountNeg = 0; float fitnessMin = 0;
            float normalizedSum = 0;

            for (int i = 0; i < _individuals.Count; i++)
            {
                float fitness = _individuals[i].GetTotalCost();
                if (fitness < 0)
                {
                    fitnessSumNeg += fitness;
                    fitnessCountNeg++;
                    if (fitness < fitnessMin)
                        fitnessMin = fitness;
                }
                else
                {
                    fitnessSumPos += fitness;
                    fitnessCountPos++;
                    if (fitness > fitnessMax)
                        fitnessMax = fitness;
                }

            }

            if (fitnessCountNeg > 0)
                fitnessAvgNeg = fitnessSumNeg / (float)fitnessCountNeg;
            if (fitnessCountPos > 0)
                fitnessAvgPos = fitnessSumPos / (float)fitnessCountPos;

            if (fitnessMax == 0 || fitnessMin == 0)
            {
                fitnessMax = 1;
                fitnessMin = 1;
            }

            for (int i = 0; i < _individuals.Count; i++)
            {
                float fitness = _individuals[i].GetTotalCost();
                if (fitness < 0)
                    fitnesses[i] = (float)Math.Exp(_fitnessExpModifier * fitness / fitnessAvgNeg * -1);
                else
                    fitnesses[i] = (float)Math.Exp(_fitnessExpModifier * fitness / fitnessAvgPos * (fitnessMax / Math.Abs(fitnessMin)));
                normalizedSum += fitnesses[i];
            }
            fitnesses[0] /= normalizedSum;
            for (int i = 1; i < _individuals.Count; i++)
                fitnesses[i] = (fitnesses[i] / normalizedSum) + fitnesses[i - 1];
            fitnesses[_individuals.Count - 1] = 1;

            Random rnd = new Random();
            _toCrossover = new List<Individual>(_populationSize);
            for (int j = 0; j < _populationSize; j++)
            {
                float prob = (float)rnd.NextDouble();
                int indx = 0;
                if (prob < fitnesses[0])
                    indx = 0;
                else
                {
                    for (int i = 0; i < _individuals.Count - 1; i++)
                        if (prob > fitnesses[i] && prob <= fitnesses[i + 1])
                        {
                            indx = i + 1;
                            break;
                        }
                }
                _toCrossover.Add(_individuals[indx]);
            }

            _toCrossover = _toCrossover.OrderBy(x => Guid.NewGuid()).ToList();

            for (int i = 0; i < (int)(_populationSize * 0.05); i++)
            {
                _toCrossover[i] = _individuals[i];
            }
        }

        public void SelectTournament()
        {
            _toCrossover = new List<Individual>(_populationSize);
            for (int i = 0; i < _populationSize; i++)
            {
                List<int> tempOrder = Enumerable.Range(0, _individuals.Count).OrderBy(x => Guid.NewGuid()).ToList();
                tempOrder = tempOrder.GetRange(0, _tournamentSize);
                var z = tempOrder.First(el => _individuals[el].GetTotalCost() == tempOrder.Max(x => _individuals[x].GetTotalCost()));
                _toCrossover.Add(_individuals[z]);
            }

            _toCrossover = _toCrossover.OrderBy(x => Guid.NewGuid()).ToList();

        }

        public void CrossoverPopulation()
        {
            Random rnd = new Random();

            for (int i = 0; i < _toCrossover.Count / 2; i++)
            {
                float prob = (float)rnd.NextDouble();
                if (prob <= _probabilityCrossover)
                {
                    _individuals[2 * i] = CrossoverER(_toCrossover[2 * i], _toCrossover[2 * i + 1]);
                    _individuals[2 * i + 1] = CrossoverER(_toCrossover[2 * i + 1], _toCrossover[2 * i]);
                }
                else
                {
                    _individuals[2 * i] = _toCrossover[2 * i];
                    _individuals[2 * i + 1] = _toCrossover[2 * i + 1];
                }
            }
        }

        public void MutatePopulation()
        {
            Random rnd = new Random();
            for (int i = 0; i < _individuals.Count; i++)
                if ((float)rnd.NextDouble() < _probabilityMutation)
                {
                    _individuals[i] = MutateSwap(_individuals[i]);
                }
        }

        public Individual CrossoverER(Individual ind1, Individual ind2)
        {
            if (ind1._routeLength != ind2._routeLength || ind2._routeLength != _dimension || ind1._routeLength != _dimension)
                return null;

            List<SortedSet<int>> neighbors = new List<SortedSet<int>>();
            for (int i = 0; i < _dimension; i++)
            {
                neighbors.Add(new SortedSet<int>());
                int indx = ind1._order.IndexOf(i);
                neighbors[i].Add(ind1._order[(indx + 1) % _dimension]);
                if (indx - 1 < 0)
                    neighbors[i].Add(ind1._order[_dimension - 1]);
                else
                    neighbors[i].Add(ind1._order[indx - 1]);

                indx = ind2._order.IndexOf(i);
                neighbors[i].Add(ind2._order[(indx + 1) % _dimension]);
                if (indx - 1 < 0)
                    neighbors[i].Add(ind2._order[_dimension - 1]);
                else
                    neighbors[i].Add(ind2._order[indx - 1]);
            }

            List<int> childOrder = new List<int>();
            int currentNode = ind1._order[0];
            childOrder.Add(currentNode);
            while (childOrder.Count < _dimension)
            {
                for (int i = 0; i < _dimension; i++)
                    neighbors[i].Remove(currentNode);
                int leastConnections = _dimension;
                int leastConnected = 0;
                if (neighbors[currentNode].Count == 0)
                {
                    for (int i = 0; i < _dimension; i++)
                        if (neighbors[i].Count != 0)
                        {
                            leastConnected = i;
                            break;
                        }
                }
                else
                {

                    foreach (var node in neighbors[currentNode])
                    {
                        if (neighbors[node].Count < leastConnections)
                        {
                            leastConnected = node;
                            leastConnections = neighbors[node].Count;
                        }
                    }
                }
                currentNode = leastConnected;
                childOrder.Add(currentNode);
            }
            return new Individual(childOrder);
        }

        public Individual MutateSwap(Individual ind)
        {
            Random rnd = new Random();

            int numMutations = (int)(_dimension * _percentageMutation) / 2;

            for (int i = 0; i < numMutations; i++)
            {
                int indx1 = rnd.Next(0, ind._routeLength);
                int indx2 = rnd.Next(0, ind._routeLength);
                
                int buffer = ind._order[indx2];
                ind._order[indx2] = ind._order[indx1];
                ind._order[indx1] = buffer;
            }

            return new Individual(ind._order);
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

        public void ExportIndividual(Individual ind)
        {
            StreamWriter sr = new StreamWriter("D:\\Informatyka\\Semestr 6\\si_1\\si_1\\results\\res-" +_fileName.Remove(_fileName.Length - 4) + "-" + (int)ind.GetTotalCost() + ".csv");
            foreach (var nodeIdx in ind._order)
            {
                sr.WriteLine(_nodes[nodeIdx]._posX + ";" +
                    "" + _nodes[nodeIdx]._posY);
            }
            sr.Close();
        }

        public Individual getBest()
        {
            Individual best = _individuals[0];
            AssessPopulationFitness();
            foreach (var item in _individuals)
            {
                if (item.GetTotalCost() > best.GetTotalCost())
                    best = item;
            }

            return best;
        }
    }
}
