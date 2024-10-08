using AISelfDrivingCar.Handlers.Cars;
using MathNet.Numerics.LinearAlgebra;
using Random = UnityEngine.Random;
using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine;
using System.Linq;
using System;
using Unity.Properties;

//Creates the cars and saves the ones that have the best fitness
//and uses them to create the next cars
namespace RT.NeuronalNetwork
{
    public class CarGenericAlgorithmManager : MonoBehaviour
    {
        public static CarGenericAlgorithmManager Instance;

        public CarController carController;

        [Header("Controls")]
        public int InitialPopulation = 85;
        [Range(0f, 1f)]
        public float MutationRate = 0.055f;

        [HideInInspector]
        public UnityEvent<int> OnNextGeneration = new();
        [HideInInspector]
        public UnityEvent<int> OnNextGenome = new();

        //controlls how the parents are combined
        [Header("Crossover Controls")]
        //The amount of the best cars we get for the next generation
        public int BestCarSelection = 8;

        //the amount of cars to create from the best ones, the rest we randomize
        public int NumberToCrossover;

        [HideInInspector]
        public List<NeuronalNetwork> BestNeuronalNetworks = new();

        //all the selected networks
        private List<int> genePool = new();
        //how many are selected vs how many are random generated
        private int naturallySelected;

        private NeuronalNetwork[] population;

        [Header("Debug")]
        //in each generation there is a population
        //when we create a new set of populations we increment the generation
        //Represents how fast they learn, the least ammount of generation to
        //acomplish the target the better!
        private int _currentGeneration;
        public int currentGeneration
        {
            get
            {
                return _currentGeneration;
            }
            set
            {
                _currentGeneration = value;
                OnNextGeneration?.Invoke(_currentGeneration);
            }
        }
        //each individual car in the population
        private int _currentGenome = 0;
        public int currentGenome
        {
            get
            {
                return _currentGenome;
            }
            set
            {
                _currentGenome = value;
                OnNextGenome?.Invoke(_currentGenome);
            }
        }

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
            }

            DontDestroyOnLoad(gameObject);
        }

        private void Start()
        {
            carController.OnDeath.AddListener(Death);
            CreatePopulation();
        }

        private void CreatePopulation()
        {
            population = new NeuronalNetwork[InitialPopulation];
            FillPopulationWithRandomValues(population, 0);
            ResetToCurrentGenome();
        }

        private void ResetToCurrentGenome()
        {
            carController.ResetWithNetwork(population[currentGenome]);
        }

        private void FillPopulationWithRandomValues(NeuronalNetwork[] newPopulation, int startIndex)
        {
            while (startIndex < InitialPopulation)
            {
                newPopulation[startIndex] = new(carController.Layers, carController.Neurons, carController.InputLayerCount, carController.OutputLayerCount);
                startIndex++;
            }
        }

        public void Death(float fitness, NeuronalNetwork network)
        {
            if (currentGenome < population.Length - 1)
            {
                //save the fitness of the car so we can filter them in the future
                population[currentGenome].Fitness = fitness;
                currentGenome++;
                ResetToCurrentGenome();
            }
            else
            {
                RePopulate();
            }
        }

        public void RePopulate(CarGenerationData genData = null)
        {
            genePool.Clear();
            naturallySelected = 0;
            BestNeuronalNetworks.Clear();

            NeuronalNetwork[] newPopulation;
            if (genData == null)
            {
                ShortPopulation();
                newPopulation = PickBestPopulation();
            }
            else
            {
                newPopulation = LoadBestNetsFromData(genData);
            }

            PopulationCrossover(newPopulation);
            PopulationMutate(newPopulation);

            FillPopulationWithRandomValues(newPopulation, naturallySelected);

            population = newPopulation;
            currentGenome = 0;

            ResetToCurrentGenome();

            currentGeneration++;
        }

        private NeuronalNetwork[] LoadBestNetsFromData(CarGenerationData genData)
        {
            NeuronalNetwork[] newPopulation = new NeuronalNetwork[InitialPopulation];

            //create the 8 networks
            for (int i = 0; i < genData.GenerationData.Length; i++)
            {
                NeuronalData NetData = genData.GenerationData[0];
                NeuronalNetwork newNet = new(NetData.HiddenLayers.Length, NetData.HiddenLayers[0].Length, NetData.InputLayers.Length, NetData.OutputLayers.Length);
                newNet.OverrideNeuronalData(NetData);
                newNet.Fitness = NetData.Fitness;

                BestNeuronalNetworks.Add(newNet.CopyNeuronalNetwork(carController.Layers, carController.Neurons, carController.InputLayerCount, carController.OutputLayerCount));

                newPopulation[naturallySelected] = newNet.CopyNeuronalNetwork(carController.Layers, carController.Neurons, carController.InputLayerCount, carController.OutputLayerCount);
                newPopulation[naturallySelected].Fitness = 0;
                naturallySelected++;

                int ChancesOfSelection = Mathf.RoundToInt(population[i].Fitness * 10);

                for (int c = 0; c < ChancesOfSelection; c++)
                {
                    genePool.Add(i);
                }
            }
            return newPopulation;
        }

        private NeuronalNetwork[] PickBestPopulation()
        {
            NeuronalNetwork[] newPopulation = new NeuronalNetwork[InitialPopulation];
            //pick best
            for (int i = 0; i < BestCarSelection; i++)
            {
                //clone the best ones
                BestNeuronalNetworks.Add(population[i].CopyNeuronalNetwork(carController.Layers, carController.Neurons, carController.InputLayerCount, carController.OutputLayerCount));

                //avoid changing the original array
                newPopulation[naturallySelected] = population[i].CopyNeuronalNetwork(carController.Layers, carController.Neurons, carController.InputLayerCount, carController.OutputLayerCount);
                newPopulation[naturallySelected].Fitness = 0;
                naturallySelected++;

                int ChancesOfSelection = Mathf.RoundToInt(population[i].Fitness * 10);

                for (int c = 0; c < ChancesOfSelection; c++)
                {
                    genePool.Add(i);
                }
            }
            return newPopulation;
        }

        private void PopulationCrossover(NeuronalNetwork[] newPopulation)
        {
            //loop 2 at the time
            for (int i = 0; i < NumberToCrossover; i += 2)
            {
                int ParentAIndex = i;
                int ParentBIndex = i + 1;
                //we selected the best and the worst and added it in the genepool
                //and we get 2 parents which are not the same for virtual sex
                if (genePool.Count >= 1)
                {
                    for (int j = 0; j < 100; j++)
                    {
                        ParentAIndex = genePool[Random.Range(0, genePool.Count)];
                        ParentBIndex = genePool[Random.Range(0, genePool.Count)];
                        if (ParentAIndex != ParentBIndex) break;
                    }
                }

                NeuronalNetwork Child1 = new(carController.Layers, carController.Neurons, carController.InputLayerCount, carController.OutputLayerCount);
                NeuronalNetwork Child2 = new(carController.Layers, carController.Neurons, carController.InputLayerCount, carController.OutputLayerCount);

                Child1.Fitness = 0;
                Child2.Fitness = 0;

                //not really correct way of doing crossover
                for (int w = 0; w < Child1.Weights.Count; w++)
                {
                    if (Random.Range(0f, 1f) < 0.5f)
                    {
                        Child1.Weights[w] = population[ParentAIndex].Weights[w];
                        Child2.Weights[w] = population[ParentBIndex].Weights[w];
                    }
                    else
                    {
                        Child2.Weights[w] = population[ParentAIndex].Weights[w];
                        Child1.Weights[w] = population[ParentBIndex].Weights[w];
                    }
                }

                for (int w = 0; w < Child1.Biases.Count; w++)
                {
                    if (Random.Range(0f, 1f) < 0.5f)
                    {
                        Child1.Biases[w] = population[ParentAIndex].Biases[w];
                        Child2.Biases[w] = population[ParentBIndex].Biases[w];
                    }
                    else
                    {
                        Child2.Biases[w] = population[ParentAIndex].Biases[w];
                        Child1.Biases[w] = population[ParentBIndex].Biases[w];
                    }
                }

                newPopulation[naturallySelected] = Child1;
                naturallySelected++;

                newPopulation[naturallySelected] = Child2;
                naturallySelected++;
            }
        }

        private void PopulationMutate(NeuronalNetwork[] newPopulation)
        {
            //get the children and add a chance for mutation
            for (int i = 0; i < naturallySelected; i++)
            {
                for (int j = 0; j < newPopulation[i].Weights.Count; j++)
                {
                    if (Random.Range(0f, 1f) < MutationRate)
                    {
                        newPopulation[i].Weights[j] = MutateWeights(newPopulation[i].Weights[j]);
                    }
                }
            }
        }

        private Matrix<float> MutateWeights(Matrix<float> weight)
        {
            //get a random amount of values dividing by 7 to not pick to manu or to little
            int randomPoints = Random.Range(1, (weight.RowCount * weight.ColumnCount) / 7);

            Matrix<float> mutateWeights = weight;

            for (int i = 0; i < randomPoints; i++)
            {
                int randomCollumn = Random.Range(0, mutateWeights.ColumnCount);
                int randomRow = Random.Range(0, mutateWeights.RowCount);
                mutateWeights[randomRow, randomCollumn] = Mathf.Clamp(mutateWeights[randomRow, randomCollumn] + Random.Range(-1f, 1f), -1f, 1f);
            }

            return mutateWeights;
        }

        private void ShortPopulation()
        {
            population = population.OrderByDescending((i) => i.Fitness).ToArray();
        }
    }

    [Serializable]
    public class CarGenerationData
    {
        public NeuronalData[] GenerationData;
        public int GenerationNumber;

        public CarGenerationData(NeuronalData[] generationData, int currentGeneration)
        {
            GenerationData = generationData;
            GenerationNumber = currentGeneration;
        }
    }
}