using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AISelfDrivingCar.Handlers.UI
{
    public abstract class WorldUIBase : MonoBehaviour
    {
        public abstract void ShowUI(object[] data = null);
        public abstract void HideUI();
    }
}