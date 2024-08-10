using System.Collections;
using UnityEngine;

namespace AISelfDrivingCar.Handlers.Extra
{
    public class RandomMeshActivator : MonoBehaviour
    {
        private int selectedMesh = -1;
        private int childCount;

        private void Awake()
        {
            childCount = transform.childCount;
            ChooseRandomMesh();
        }

        public void ChooseRandomMesh()
        {
            if(selectedMesh !=  -1)
            {
                transform.GetChild(selectedMesh).gameObject.SetActive(false);
            }

            selectedMesh = Random.Range(0, childCount);

            transform.GetChild(selectedMesh).gameObject.SetActive(true);
        }
    }
}