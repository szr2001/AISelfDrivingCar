using AISelfDrivingCar.Handlers.Cars;
using AISelfDrivingCar.Handlers.UI;
using AISelfDrivingCar.Interfaces;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AISelfDrivingCar.Handlers
{
    [RequireComponent(typeof(CarController))]
    public class CarUiHandler : MonoBehaviour, IInteract
    {
        public WorldCarUI WorldCarUI;

        private CarController carController;
        private void Awake()
        {
            carController = GetComponent<CarController>();
        }

        public void Interact()
        {
            if (WorldCarUI == null) return;
            WorldCarUI.ShowUI();
        }
    }
}