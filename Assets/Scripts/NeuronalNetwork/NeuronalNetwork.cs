using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using MathNet.Numerics.LinearAlgebra;
using Random = UnityEngine.Random;
//using MathNet.Numerics.dll
namespace RT.NeuronalNetwork
{
    public class NeuronalNetwork
    {
        public Matrix<float> InputLayer = Matrix<float>.Build.Dense(1, 3);

        public List<Matrix<float>> HiddenLayers = new();

        public Matrix<float> OutputLayer = Matrix<float>.Build.Dense(1, 2);

        public List<Matrix<float>> Weights = new();

        public List<float> Biases = new();

        public float Fitness;
        //add constructor

        public void InitialiseNetwork(int hiddenLayersCount, int hiddenNeuronalCount, int inputLayerCount = 3, int outputLayerCount = 2)
        {
            InputLayer.Clear();
            HiddenLayers.Clear();
            OutputLayer.Clear();
            Weights.Clear();
            Biases.Clear();

            InputLayer = Matrix<float>.Build.Dense(1, inputLayerCount);
            OutputLayer = Matrix<float>.Build.Dense(1, outputLayerCount);

            for (int layer = 0; layer < hiddenLayersCount + 1; layer++)
            {
                Matrix<float> newMat = Matrix<float>.Build.Dense(1, hiddenNeuronalCount);
                HiddenLayers.Add(newMat);

                Biases.Add(Random.Range(-1f, 1f));
            
                //connect the weights to neurons

                if(layer == 0)
                {
                    Matrix<float> inputToFirstLayer = Matrix<float>.Build.Dense(inputLayerCount, hiddenNeuronalCount);
                    Weights.Add(inputToFirstLayer);
                }

                Matrix<float> hiddenToHidden = Matrix<float>.Build.Dense(hiddenNeuronalCount, hiddenNeuronalCount);
                Weights.Add(hiddenToHidden);
            }

            Matrix<float> outputWeight = Matrix<float>.Build.Dense(hiddenNeuronalCount, outputLayerCount);
            Weights.Add(outputWeight);
            Biases.Add(Random.Range(-1f, 1f));
            
            RandomizeWeights();
        }

        public NeuronalNetwork CopyNeuronalNetwork(int hiddenLayerCount, int hiddenNeuronCount)
        {
            NeuronalNetwork newNet = new();

            List<Matrix<float>> newWeights = new();

            //bad performance
            for (int i = 0; i < Weights.Count; i++)
            {
                Matrix<float> currentWeight = Matrix<float>.Build.Dense(Weights[i].RowCount, Weights[i].ColumnCount);

                //copy the values from the original to the new one
                for (int x = 0; x < currentWeight.RowCount; x++)
                {
                    for (int y = 0; y < currentWeight.ColumnCount; y++)
                    {
                        currentWeight[x, y] = Weights[i][x, y];
                    }
                }
                newWeights.Add(currentWeight);
            }

            List<float> newBiases = new();
            newBiases.AddRange(Biases);

            newNet.Weights = newWeights;
            newNet.Biases = newBiases;

            newNet.InitializeHidden(hiddenLayerCount, hiddenNeuronCount);

            return newNet;
        }

        private void InitializeHidden(int hiddenLayerCount, int hiddenNeuronCount)
        {
            InputLayer.Clear();
            HiddenLayers.Clear();
            OutputLayer.Clear();

            for (int i = 0; i < hiddenLayerCount + 1; i++)
            {
                Matrix<float> newHiddenLayer = Matrix<float>.Build.Dense(1,hiddenNeuronCount );
                HiddenLayers.Add(newHiddenLayer);
            }
        }

        private void RandomizeWeights()
        {
            for (int i = 0; i < Weights.Count; i++)
            {
                for (int x = 0; x < Weights[i].RowCount; x++)
                {
                    for (int y = 0; y < Weights[i].ColumnCount; y++)
                    {
                        Weights[i][x, y] = Random.Range(-1f,1f);
                    }
                }
            }
        }

        //returns the output layer, takes the input layer
        //write so it returns an array of floats and takes an array of floats
        //because the input and output layer is set at initializeNetwork
        public (float, float) RunNetwork(float a, float b, float c)
        {
            InputLayer[0, 0] = a;
            InputLayer[0, 1] = b;
            InputLayer[0, 2] = c;

            //pointWisetan for activation because we want -1 and 1
            //so we don't lose any data
            //if we want a 0-1 value, we can use the sigmoid activation function at the end
            InputLayer = InputLayer.PointwiseTanh();

            //we calculate the first layer
            HiddenLayers[0] = ((InputLayer * Weights[0]) + Biases[0]).PointwiseTanh();

            //we calculate the rest of the layers starting from 1 because 0 was calculated above
            for (int i = 1; i < HiddenLayers.Count; i++)
            {
                //we multiplied the previous layer with the weights connecting the previous
                //layer to the current layer and add the biases and then activation
                HiddenLayers[i] = ((HiddenLayers[i-1] * Weights[i]) + Biases[i]).PointwiseTanh();
            }

            //asign the output layers
            //take the last layer multiplie by the last weights and adding the last biases

            OutputLayer = ((HiddenLayers[HiddenLayers.Count - 1] * Weights[Weights.Count - 1]) + Biases[Biases.Count - 1]).PointwiseTanh();
            //first is acceleration second is steering, TanH returns -1 or 1
            return (Sigmoid(OutputLayer[0,0]), (float)Math.Tanh(OutputLayer[0,1]));
        }

        //returns a value betweeen 0 and 1
        private float Sigmoid(float s)
        {
            return (1 / (1 + Mathf.Exp(-s)));
        }
    }
}
