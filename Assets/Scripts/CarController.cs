using RT.NeuronalNetwork;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Timeline;
using UnityEngine;

namespace AISelfDrivingCar.Handlers.Cars
{
    [RequireComponent(typeof(NeuronalNetwork))]
    public class CarController : MonoBehaviour
    {
        private Vector3 startPosition;
        private Vector3 startRotation;
        private NeuronalNetwork NNet;

        [Range(-1, 1)]
        public float Acceleration;
        public float VerticalSpeed = 11.4f;
        [Range(-1, 1)]
        public float Turning;
        public float HorizontalSpeed = 0.02f;

        public float TimeSinceStart = 0f;
        public float sensorSensitity = 20;
        public float CarMaxSimulationTime = 20;
        public float LowFitnessValue = 40;
        public float MaxFitnessValue = 1000;

        [Header("Neuronal Network")]
        public int Layers = 1;
        public int Neurons = 10;

        //Genetic algorithm
        [Header("Fitness")]
        //How well this instance did
        public float OverallFitness;
        //we value how far it goes and not how fast it goes
        //How important is the distance traveled
        public float DistanceMultiplier = 1.4f;
        //how important is the average speed
        public float AverageSpeedMultiplier = 0.2f;
        //How important is to stay away from the walls by favoring
        //a bigger value to the sensor distance
        public float SensorMultiplier = 0.1f;

        //Values used to calculate the fitness (how well this car did)
        private Vector3 lastPosition;
        private float totalDistanceTraveled;
        private float averageSpeed;

        private float leftSensor;
        private float RightSensor;
        private float ForwardSensor;

        private void Awake()
        {
            startPosition = transform.position;
            startRotation = transform.eulerAngles;
            NNet = GetComponent<NeuronalNetwork>();
            
            //test
            NNet.InitialiseNetwork(Layers,Neurons);
        }

        //when the car simulation stops, reset the values back to normal so we can run it again
        public void Reset()
        {
            averageSpeed = 0f;
            TimeSinceStart = 0f;
            OverallFitness = 0f;
            totalDistanceTraveled = 0f;
            lastPosition = startPosition;
            transform.position = startPosition;
            transform.eulerAngles = startRotation;
        }

        private void FixedUpdate()
        {
            ReadSensors();
            
            lastPosition = transform.position;

            (Acceleration, Turning) = NNet.RunNetwork(leftSensor, ForwardSensor, RightSensor);
            //asign from neuronalnet
            DriveCar(Acceleration, Turning);

            TimeSinceStart += Time.deltaTime;

            CalculateFitness();

            //Acceleration = 0;
            //Turning = 0;  
        }

        private void OnTriggerEnter(Collider other)
        {
            NNet.InitialiseNetwork(Layers,Neurons);
            Reset();
        }

        private void CalculateFitness()
        {
            totalDistanceTraveled += Vector3.Distance(transform.position, lastPosition);
            averageSpeed = totalDistanceTraveled / TimeSinceStart;
            //calculate the fitness (how well the car did) based on distance traveled
            //and average speed and multiplied by the Avgspeedmultiplier and totaldistance
            //multiplier which specify the importance of avgspeed over distance
            OverallFitness =
                (totalDistanceTraveled * DistanceMultiplier) +
                (averageSpeed * AverageSpeedMultiplier) +
                (((leftSensor * ForwardSensor * RightSensor) / 3) * SensorMultiplier);

            if(TimeSinceStart > CarMaxSimulationTime && OverallFitness < LowFitnessValue)
            {
                Reset();
            }
            if(OverallFitness >= MaxFitnessValue)
            {
                //save
                Reset();
            }
        }

        private void ReadSensors()
        {
            Vector3 right = (transform.forward + transform.right);
            Vector3 forrward = transform.forward;
            Vector3 left = (transform.forward - transform.right);

            Ray ray = new(transform.position, right);

            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                RightSensor = hit.distance / sensorSensitity;
                Debug.DrawLine(ray.origin, hit.point, Color.red);
                Debug.Log($"Right {RightSensor}");
            }
            
            ray.direction = forrward;

            if (Physics.Raycast(ray, out hit))
            {
                ForwardSensor = hit.distance / sensorSensitity;
                Debug.DrawLine(ray.origin, hit.point, Color.red);
                Debug.Log($"Forward {ForwardSensor}");
            }

            ray.direction = left ;
            
            if (Physics.Raycast(ray, out hit))
            {
                leftSensor = hit.distance / sensorSensitity;
                Debug.DrawLine(ray.origin, hit.point, Color.red);
                Debug.Log($"Left {leftSensor}");
            }
        }

        Vector3 input;
        public void DriveCar(float verticalMove, float horizontalMove)
        {
            input = Vector3.Lerp
                (
                    Vector3.zero, 
                    new Vector3(0,0,verticalMove * VerticalSpeed), 
                    0.02f
                );
            input = transform.TransformDirection(input);
            transform.position += input;

            transform.eulerAngles += new Vector3 (0, horizontalMove * 90 * HorizontalSpeed, 0);
        }
    }
}