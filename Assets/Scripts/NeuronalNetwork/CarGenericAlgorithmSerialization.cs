using System.Collections.Generic;
using System.Collections;
using Newtonsoft.Json;
using UnityEngine;
using System.IO;
using System;
using System.Text;


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

            List<NeuronalData> neuronalDatas = new();

            foreach(NeuronalNetwork nn in carAlgorithmManager.BestNeuronalNetworks)
            {
                neuronalDatas.Add(nn.GetNeuronalData());
            }

            CarGenerationData carGeneration = new(neuronalDatas.ToArray(), carAlgorithmManager.currentGeneration);

            string json = JsonConvert.SerializeObject(carGeneration, Formatting.Indented);
            File.WriteAllText($"{FileName}_{ID}.json", json);

            Debug.Log($"SAVED best of generation {carGeneration.CurrentGeneration}");
        }

        public void LoadGeneration()
        {
            Debug.Log($"LOADED generation");
        }
    }
}