using System.Collections;
using UnityEngine;

namespace AISelfDrivingCar.Handlers.Extra
{
    public class RandomMeshActivator : MonoBehaviour
    {
        private void Awake()
        {
            int childCount = transform.childCount;

            GameObject SelectedMesh = transform.GetChild(Random.Range(0, childCount)).gameObject;
            SelectedMesh.SetActive(true);  
        }
    }
}