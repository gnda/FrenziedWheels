using SDD.Events;
using UnityEngine;

namespace DefaultNamespace
{
    public class Racer : MonoBehaviour
    {
        public int Laps { get; set; }
        public int Position { get; set; }
        public float EndRaceTime { get; set; }

        private Circuit currentCircuit;

        private void Start()
        {
            currentCircuit = GameManager.Instance.CurrentCircuit;
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
                }
            }
        }
    }
}