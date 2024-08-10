using AISelfDrivingCar.Handlers.Camera;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace AISelfDrivingCar.Handlers.UI
{
    public class WorldCarUI : WorldUIBase
    {
        public TextMeshProUGUI FitnessT;
        public TextMeshProUGUI SpeedT;
        public TextMeshProUGUI ElapsedTimeT;

        public int MaxSpeed = 130;

        private Canvas canvas;
        private void Awake()
        {
            canvas = GetComponent<Canvas>();
        }

        private void Start()
        {
            canvas.worldCamera = CameraControler.PlayerCamera;
        }

        private void Update()
        {
            if (CameraControler.PlayerCamera == null) return;

            transform.rotation = Quaternion.LookRotation(transform.position - CameraControler.PlayerCamera.gameObject.transform.position);
        }

        public void UpdateUiData(float speed, float elapsedTime, float fitness)
        {
            SpeedT.text = ((int)(MaxSpeed * speed)).ToString();
            FitnessT.text = ((int)fitness).ToString();
            ElapsedTimeT.text = ((int)elapsedTime).ToString();
        }

        public override void HideUI()
        {
            base.HideUI();
        
        }

        public override void ShowUI(object[] data = null)
        {
            base.ShowUI(data);
        }
    }
}