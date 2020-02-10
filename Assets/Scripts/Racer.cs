using System;
using System.Collections.Generic;
using System.Linq;
using SDD.Events;
using UnityEngine;
using UnityStandardAssets.Utility;

namespace DefaultNamespace
{
    public class Racer : MonoBehaviour
    {
        public int Laps { get; set; }
        public int Position { get; set; }
        public float EndRaceTime { get; set; }

        private List<Racer> sortedRacers;
        private Circuit currentCircuit;
        private WaypointCircuit wpc;

        public int WaypointCount  { get; set; }
        public Transform LastWaypoint { get; set; }

        private void Start()
        {
            currentCircuit = GameManager.Instance.CurrentCircuit;
            wpc = currentCircuit.GetComponentInChildren<WaypointCircuit>();
            sortedRacers = GameManager.Instance.Racers;
            LastWaypoint = wpc.Waypoints[0];
        }

        private void UpdatePosition()
        {
            List<Transform> waypoints = wpc.Waypoints.ToList();
            Transform nearestWaypoint = waypoints.Find(wp =>
                Vector3.Distance(transform.position, wp.position) < 10f);
            if ((nearestWaypoint != null) && (nearestWaypoint != LastWaypoint))
            {
                if (GetComponent<Player>() != null)
                {
                    sortedRacers.Sort(((r1, r2) => r2.Laps.CompareTo(r1.Laps)));
                    sortedRacers.Sort(((r1, r2) => 
                        wpc.Waypoints.ToList().IndexOf(r2.LastWaypoint)
                            .CompareTo(wpc.Waypoints.ToList().IndexOf(r1.LastWaypoint))));
                    Position = sortedRacers.IndexOf(this) + 1;
                }
                LastWaypoint = nearestWaypoint;
                WaypointCount++;
            }
        }

        private void Update()
        {
            UpdatePosition();
        }
        
        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Finish"))
            {
                if (Laps <= currentCircuit.MaxLaps)
                {
                    if ((Laps == 0) || 
                        (WaypointCount > (wpc.Waypoints.Length*.5f) * Laps))
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
                    else if (GameManager.Instance.
                        Racers.FindAll(r => r.EndRaceTime > 0).Count == 
                             GameManager.Instance.NumberOfCars - 1)
                    {
                        // If everybody except player has finished the race
                        EventManager.Instance.Raise(new GameOverEvent());
                    }
                }
            }
        }
        
        private void OnCollisionEnter(Collision other)
        {
            if (other.transform.parent.CompareTag("Limits"))
            {
                var position = LastWaypoint.position;
                transform.position = position;
                transform.rotation = 
                    Quaternion.LookRotation(position, Vector3.up);
            }
        }
    }
}