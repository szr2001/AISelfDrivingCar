using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace AISelfDrivingCar.Handlers.UI
{
    public class GenomeUi : MonoBehaviour
    {
        public TextMeshProUGUI PlaceT;
        public TextMeshProUGUI FitnessScoreT;

        public void SetGenomeUiData(int place, int Fitness)
        {
            PlaceT.text = place.ToString();
            FitnessScoreT.text = Fitness.ToString();
        }
    }
}
