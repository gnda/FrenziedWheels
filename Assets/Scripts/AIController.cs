using System.Collections;
using SDD.Events;
using Spline;
using UnityEngine;

namespace DefaultNamespace
{
    public class AIController : SimpleGameStateObserver
    {
        private Car currentCar;
        private Racer currentRacer;
        private Circuit currentCircuit;
        private BezierSpline circuitSpline;
        private float nextStep;
        private Vector3 nextPosition;
        private bool IsMoving;
	
        #region MonoBehaviour lifecycle
        // Use this for initialization
        void Start ()
        {
            currentCar = GetComponent<Car>();
            currentRacer = GetComponent<Racer>();
            currentCircuit = GameManager.Instance.CurrentCircuit;
            circuitSpline = currentCircuit.LevelBaseSpline;
            currentCar.CurrentSpeed = 10;
            nextPosition = circuitSpline.GetPoint(nextStep / circuitSpline.GetTotalLength());
            nextPosition.y = 0.5f;
        }

        IEnumerator MoveCoroutine()
        {
            Transform transf = transform;
            var startingPos = transf.position;
            Quaternion startRot = transf.rotation;
            
            float timeElapsed = 0;
            IsMoving = true;

            Vector3 relativePos = nextPosition - startingPos;
            
            while (timeElapsed < 1f)
            {
                transform.rotation = Quaternion.Slerp(
                    startRot, 
                    Quaternion.LookRotation(relativePos, Vector3.up), timeElapsed / 1f);
                transform.position =  Vector3.Lerp(startingPos, nextPosition, timeElapsed / 1f);
                timeElapsed += Time.deltaTime;
                yield return null;
            }
            
            transform.position = nextPosition;
            transform.rotation = Quaternion.LookRotation(relativePos, Vector3.up);

            yield return IsMoving = false;
        }

        // Update is called once per frame
        private void Update()
        {
            if (GameManager.Instance && !GameManager.Instance.IsPlaying) return;

            if (IsMoving) return;
            
            nextStep += 10;
            nextPosition = circuitSpline.GetPoint((nextStep % 
                circuitSpline.GetTotalLength()) / circuitSpline.GetTotalLength());
            //nextPosition.y = transform.position.y;

            StartCoroutine(MoveCoroutine());
        }
        #endregion
    }
}