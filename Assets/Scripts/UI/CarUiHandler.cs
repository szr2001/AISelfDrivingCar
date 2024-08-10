using AISelfDrivingCar.Handlers.Cars;
using AISelfDrivingCar.Handlers.UI;
using AISelfDrivingCar.Interfaces;
using UnityEngine;

namespace AISelfDrivingCar.Handlers
{
    [RequireComponent(typeof(CarController))]
    public class CarUiHandler : MonoBehaviour
    {
        public WorldCarUI WorldCarUI;

        private CarController carController;
        private void Awake()
        {
            carController = GetComponent<CarController>();
        }

        private void Update()
        {
            if (carController == null) return;

            WorldCarUI.UpdateUiData(carController.Acceleration, carController.TimeSinceStart, carController.OverallFitness);
        }
    }
}