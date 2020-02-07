using System.Collections.Generic;
using SDD.Events;
using UnityEngine;
using UnityStandardAssets.Vehicles.Car;

namespace DefaultNamespace
{
    public class Racer : MonoBehaviour
    {
        public int Laps { get; set; }
        public int Position { get; set; }
        
        public float CurrentDistance { get; set; }
        public Vector3 StartPosition { get; set; }
        public float EndRaceTime { get; set; }

        private Circuit currentCircuit;
        private CarController currentCar;
        private List<Racer> racers;

        private void Start()
        {
            currentCircuit = GameManager.Instance.CurrentCircuit;
            currentCar = GetComponent<CarController>();
            racers = GameManager.Instance.Racers;
            CurrentDistance = 0;
            StartPosition = transform.position;
        }

        private float getCurrentPercProgression()
        {
            return (CurrentDistance / 
                    currentCircuit.LevelBaseSpline.GetTotalLength()) * 100;
        }

        private void Update()
        {
            racers.Sort((r1, r2) => r1.CurrentDistance.CompareTo(r2.CurrentDistance));
            Position = racers.IndexOf(this);
            if (currentCar.CurrentSpeed > 0)
            {
                CurrentDistance += currentCar.CurrentSpeed * Time.deltaTime;
            }
        }

        private void OnCollisionStay(Collision other)
        {
            if (other.collider.CompareTag("Road") && currentCar.CurrentSpeed > 0)
            {
                CurrentDistance += currentCar.CurrentSpeed * Time.deltaTime;
            }
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (collision.collider.CompareTag("Finish"))
            {
                if (Laps < currentCircuit.MaxLaps)
                {
                    if ((Laps == 0) || getCurrentPercProgression() % 100 > 90)
                    {
                        Laps++;
                    }
                }
                else
                {
                    EndRaceTime = GameManager.Instance.Timer;
                    if (GetComponent<Player>())
                    {
                        EventManager.Instance.Raise(new GameVictoryEvent()
                        {
                            ePlayer = GetComponent<Player>()
                        });
                    }
                    else
                    {
                        EventManager.Instance.Raise(new GameOverEvent());
                    }
                }
            }
        }
    }
}