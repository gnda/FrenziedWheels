using System.Collections;
using SDD.Events;
using Spline;
using UnityEngine;

namespace DefaultNamespace
{
    public class AIController : SimpleGameStateObserver
    {
        private Car currentCar;
        private Circuit currentCircuit;
        private BezierSpline circuitSpline;
        private int steps = 2000; 
        private float nextStep = 1;
	
        #region MonoBehaviour lifecycle
        // Use this for initialization
        void Start ()
        {
            currentCar = GetComponent<Car>();
            currentCircuit = GameManager.Instance.CurrentCircuit;
            circuitSpline = currentCircuit.LevelBaseSpline;
            currentCar.CurrentSpeed = 100;
        }

        // Update is called once per frame
        void Update () {
            if (GameManager.Instance && !GameManager.Instance.IsPlaying) return;

            //Vector3 nextPosition = circuitSpline.GetPoint((Time.deltaTime % steps) / steps);

            Vector3 nextPoint = circuitSpline.GetPoint(nextStep % steps / steps);
            if (Vector3.Distance(transform.position,nextPoint) > 10f){
                currentCar.CurrentSpeed += (currentCar.AccelerationRate * Time.deltaTime);
                //transform.Translate(nextPoint);
                transform.position = nextPoint;
            }
            else
            {
                currentCar.CurrentSpeed -= (currentCar.DecelerationRate * Time.deltaTime);
                nextStep += 1;
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if(GameManager.Instance.IsPlaying
               && other.gameObject.CompareTag("FinishLine"))
            {
                EventManager.Instance.Raise(new ElementMustBeDestroyedEvent()
                    { eElement = gameObject });
            }
        }
        #endregion
    }
}