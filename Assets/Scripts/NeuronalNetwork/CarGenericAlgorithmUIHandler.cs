using AISelfDrivingCar.Handlers.U;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RT.NeuronalNetwork
{
    [RequireComponent(typeof(CarGenericAlgorithmManager))]
    public class CarGenericAlgorithmUIHandler : MonoBehaviour
    {
        public AiRaceUI raceUI;

        private CarGenericAlgorithmManager carGenericManager;

        private void Awake()
        {
            carGenericManager = GetComponent<CarGenericAlgorithmManager>();

            if (raceUI == null) return;
            if (carGenericManager == null) return;

            raceUI.InitializeAiRaceUi(carGenericManager.InitialPopulation, carGenericManager.BestCarSelection);
            carGenericManager.OnNextGenome.AddListener(GenomeChanged);
            carGenericManager.OnNextGeneration.AddListener(GenerationChanged);
        }

        private void GenomeChanged(int genomeIndex)
        {
            raceUI.UpdateGenomText(genomeIndex);
        }

        private void GenerationChanged(int generationIndex)
        {
            raceUI.UpdateGenerationText(generationIndex);
            raceUI.GenerateScoreboard(carGenericManager.BestNeuronalNetworks.ToArray());
        }
    }
}
