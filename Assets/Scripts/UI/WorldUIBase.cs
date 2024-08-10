using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AISelfDrivingCar.Handlers.UI
{
    public abstract class WorldUIBase : MonoBehaviour
    {
        public bool IsUiVisible = false;
        public virtual void ShowUI(object[] data = null)
        {
            IsUiVisible = true;
        }
        public virtual void HideUI()
        {
            IsUiVisible = false;
        }
    }
}