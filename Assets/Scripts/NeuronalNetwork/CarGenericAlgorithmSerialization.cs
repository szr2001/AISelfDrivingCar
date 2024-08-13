using System.Collections.Generic;
using System.Collections;
using Newtonsoft.Json;
using UnityEngine;
using System.IO;
using System;


namespace RT.NeuronalNetwork
{
    [RequireComponent(typeof(CarGenericAlgorithmManager))]
    public class CarGenericAlgorithmSerialization : MonoBehaviour
    {
        public string FileName;

        private CarGenericAlgorithmManager carAlgorithmManager;
        private string SavePath;

        private void Awake()
        {
            SavePath = Application.dataPath;
            carAlgorithmManager = GetComponent<CarGenericAlgorithmManager>();
        }

        public void SaveGeneration()
        {
            if (Directory.Exists(SavePath + $"\\{FileName}"))
            {
                Directory.CreateDirectory(SavePath + $"\\{FileName}");
            }
            SavePath = SavePath + $"\\{FileName}";
            string ID = DateTime.Now.Ticks.ToString();

            CarGenerationData carGeneration = new();



            //string json = JsonConvert.SerializeObject(inputLayer.ToArray());
            //File.WriteAllText(FileName + "\\inputLayer_" + ID + ".txt", json);
            //json = JsonConvert.SerializeObject(hiddenLayers.ToArray());
            //File.WriteAllText(FileName + "\\hiddenLayers_" + ID + ".txt", json);
            //json = JsonConvert.SerializeObject(outputLayer.ToArray());
            //File.WriteAllText(FileName + "\\outputLayers_" + ID + ".txt", json);
            //json = JsonConvert.SerializeObject(weights.ToArray());
            //File.WriteAllText(FileName + "\\weights_" + ID + ".txt", json);
            //json = JsonConvert.SerializeObject(biases.ToArray());
            //File.WriteAllText(FileName + "\\biases_" + ID + ".txt", json);
        }
        //create a NeuronalNetworkData class with serializable to hold all the data
        //and serializa an array of NeuronalNetowkrData as json
        public void LoadGeneration()
        {

        }
    }
}