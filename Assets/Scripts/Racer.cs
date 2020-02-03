using System.Collections.Generic;
using SDD.Events;
using UnityEngine;

namespace DefaultNamespace
{
    public class Racer : MonoBehaviour
    {
        public int Laps { get; set; }
        public int Position { get; set; }
        
        public float CurrentDistance { get; set; }
        public float EndRaceTime { get; set; }

        private Circuit currentCircuit;
        private Car currentCar;
        private List<Racer> racers;

        private void Start()
        {
            currentCircuit = GameManager.Instance.CurrentCircuit;
            currentCar = GetComponent<Car>();
            racers = GameManager.Instance.Racers;
            CurrentDistance = 0;
        }

        private void Update()
        {
            Position = racers.IndexOf(this);
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
                    Laps++;
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