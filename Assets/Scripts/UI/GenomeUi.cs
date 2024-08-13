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

        public void SetGenomeUiData(int place, float Fitness)
        {
            if(place <= 3)
            {
                transform.localScale = new Vector3(1.06f, 1.06f, 1.06f);
            }

            PlaceT.text = place.ToString();
            FitnessScoreT.text = ((int)Fitness).ToString();
        }
    }
}
