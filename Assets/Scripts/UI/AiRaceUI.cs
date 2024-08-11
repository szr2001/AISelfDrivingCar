using AISelfDrivingCar.Handlers.UI;
using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

namespace AISelfDrivingCar.Handlers.U
{
    public class AiRaceUI : MonoBehaviour
    {
        public TextMeshProUGUI CurrentGenerationT;
        public TextMeshProUGUI CurrentGenomeT;
        public TextMeshProUGUI PopulationT;
        public TextMeshProUGUI SimSpeedT;
        public TextMeshProUGUI FpsT;

        public Transform ScoreBoardHolder;
        public GenomeUi UiPrefab;
        public int SimulationSpeed = 5;

        private List<GenomeUi> SpawnedGemonesUi = new();
        private int BestCarSelection;

        private void Awake()
        {
            SetSimulationSpeed(SimulationSpeed);
        }

        private void Start()
        {
            StartCoroutine(CalculateFps());
        }

        private IEnumerator CalculateFps()
        {
            WaitForSeconds wait1s = new(1);
            float fps = 0;
            while (true)
            {
                yield return wait1s;

                fps = (int)(1f / Time.unscaledDeltaTime);
                FpsT.text = $"FPS: {fps}";
            }
        }

        public void InitializeAiRaceUi(int population, int bestCarSelection)
        {
            BestCarSelection = bestCarSelection;
            PopulationT.text = population.ToString();
            GenerateScoreboard(new float[bestCarSelection]);
        }

        public void GenerateScoreboard(float[] Scores)
        {
            foreach(GenomeUi genome in SpawnedGemonesUi)
            {
                Destroy(genome.gameObject);
            }
            SpawnedGemonesUi.Clear();

            for (int i = 0; i < BestCarSelection; i++)
            {
                GameObject GenomeUiObj = Instantiate(UiPrefab.gameObject, ScoreBoardHolder);
                SpawnedGemonesUi.Add(GenomeUiObj.GetComponent<GenomeUi>());
                SpawnedGemonesUi[i].SetGenomeUiData(i + 1, Scores[i]);
            }
        }

        public void SetSimulationSpeed(int speed)
        {
            SimulationSpeed += speed;
            if (SimulationSpeed < 1)
            {
                SimulationSpeed = 1;
            }
            SimSpeedT.text = SimulationSpeed.ToString();
            Time.timeScale = SimulationSpeed;
        }

        public void UpdateGenerationText(int CurrentGeneration)
        {
            CurrentGenerationT.text = CurrentGeneration.ToString();
        }

        public void UpdateGenomText(int CurrentGenome)
        {
            CurrentGenomeT.text = CurrentGenome.ToString();
        }
    }
}