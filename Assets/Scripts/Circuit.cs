using System;
using System.Collections.Generic;
using DefaultNamespace;
using UnityEngine;
using SDD.Events;
using Spline;
using UnityEngine.Experimental.GlobalIllumination;
using UnityStandardAssets.Utility;
using Random = UnityEngine.Random;
using RenderSettings = UnityEngine.RenderSettings;

public class Circuit : MonoBehaviour, IEventHandler
{
    #region Settings & Prefabs

    [field: Header(("LevelDesign"))] 
    [SerializeField] private BezierSpline levelBaseSpline;
    [SerializeField] private int roadWidth;
    [SerializeField] private int groundWidth;
    [SerializeField] private Material roadMaterial;
    [SerializeField] private Material groundMaterial;
    [SerializeField] private Material skyboxMaterial;

    [Header(("LevelSettings"))] 
    [SerializeField] public int MaxLaps = 1;

    [Header(("ScenePrefabs"))]
    [SerializeField] private GameObject roadLinePrefab;
    [SerializeField] private GameObject finishingLinePrefab;
    
    #endregion

    #region Events' subscription
    public void SubscribeEvents()
    {
    }

    public void UnsubscribeEvents()
    {
    }
    #endregion

    #region MonoBehaviour lifecycle
    private void Awake()
    {
        SubscribeEvents();
    }

    private void Start()
    {
        GenerateCircuit();
    }
    
    private void OnDestroy()
    {
        UnsubscribeEvents();
    }
    #endregion

    #region Level generation
    private GameObject levelElements, wallsGO, bonusesGO,
        playersGO, carsGO, groundGO, roadGO, finishingLineGO;

    private void GenerateCircuit()
    {
        //Setting Skybox
        RenderSettings.skybox = skyboxMaterial;
        
        //Level static scene elements
        levelElements = new GameObject("Level Scene Elements");
        levelElements.transform.SetParent(transform);
        
        //Moveable elements
        carsGO = new GameObject("Cars");
        carsGO.transform.SetParent(transform);

        //Generate Ground
        groundGO = GenerateElementFromSpline("Ground", groundWidth, groundMaterial);
        groundGO.transform.position = new Vector3(0,-0.05f,0);
        groundGO.tag = "Ground";
        
        //Generate Road
        roadGO = GenerateElementFromSpline("Road", roadWidth, roadMaterial);
        RetrieveWaypointsFromSpline(roadGO, levelBaseSpline, 50);
        roadGO.tag = "Road";
        GenerateRoadLines();

        //Generating limits objects for each sides
        GenerateLimits(10);

        Vector3 splineFirstPoint = levelBaseSpline.GetPoint(0);
        
        //Adding Finishing Line and Finish Box
        GameObject finishBoxGO = new GameObject("FinishBox");
        finishBoxGO.transform.parent = levelElements.transform;
        finishBoxGO.transform.tag = "Finish";
        finishBoxGO.transform.position = 
            new Vector3(splineFirstPoint.x,splineFirstPoint.y + 20,0);
        BoxCollider b = finishBoxGO.AddComponent<BoxCollider>();
        b.size = new Vector3(groundWidth,45,roadWidth/10f);
        b.isTrigger = true;

        finishingLineGO = Instantiate(finishingLinePrefab, levelElements.transform);
        finishingLineGO.transform.position = 
            new Vector3(splineFirstPoint.x,splineFirstPoint.y + 0.01f,0);
        finishingLineGO.transform.localScale += new Vector3(0,0,roadWidth/10f);
        finishingLineGO.transform.Rotate(0,-90,0);
        
        //Generate start positions
        List<Vector3> startPositions = GenerateStartPositions(splineFirstPoint);
        Vector3 playerStartPosition = 
            startPositions[Random.Range(0, startPositions.Count)];
        startPositions.Remove(playerStartPosition);

        //Add player
        GameObject playerGO = Instantiate(
            GameManager.Instance.GetSelectedCar(), playerStartPosition, 
            Quaternion.identity, carsGO.transform);
        playerGO.tag = "Player";
        playerGO.AddComponent<Player>();
        playerGO.AddComponent<Racer>();

        //Add AI opponents
        for (int i = 0; i < GameManager.Instance.NumberOfCars - 1; i++)
        {
            GameObject carGO = Instantiate(
                GameManager.Instance.AiCarPrefabs[
                    Random.Range(0, GameManager.Instance.AiCarPrefabs.Length)
                ], startPositions[i], 
                Quaternion.identity, carsGO.transform);
            carGO.AddComponent<Racer>();
        }

        EventManager.Instance.Raise(new CircuitHasBeenInstantiatedEvent());
    }

    private GameObject GenerateElementFromSpline(String name, 
        int width, Material mat = null, float xOffset = 0)
    {
        GameObject elementGO = new GameObject(name);
        elementGO.transform.SetParent(levelElements.transform);
        MeshFilter elementMF = elementGO.AddComponent<MeshFilter>();
        MeshRenderer elementMR = elementGO.AddComponent<MeshRenderer>();
        elementMF.sharedMesh = MeshGenerator.
            ExtrudeMeshAlongSpline(levelBaseSpline, width, xOffset);
        if (mat != null)
        {
            elementMR.material = mat;
        }
        elementGO.AddComponent<MeshCollider>();

        return elementGO;
    }
    
    private void RetrieveWaypointsFromSpline(GameObject go, 
        BezierSpline spline, int steps = 100)
    {
        WaypointCircuit wpc = go.AddComponent<WaypointCircuit>();
        wpc.waypointList.items = new Transform[1];

        for (int i = 0; i <= steps; i++)
        {
            GameObject newChild = new GameObject("Waypoint " + (i).ToString("000"));
            newChild.transform.parent = wpc.transform;
            newChild.transform.position = spline.GetPoint((float) i / steps);
            newChild.transform.rotation = spline.GetOrientation((float) i / steps);
            if (i > 0)
            {
                Array.Resize(ref wpc.waypointList.items, wpc.waypointList.items.Length + 1);
            }
            wpc.waypointList.items[i] = newChild.transform;
        }
        
        wpc.numPoints = wpc.Waypoints.Length;
        wpc.CachePositionsAndDistances();
    }

    private List<Vector3> GenerateStartPositions(Vector3 splineFirstPoint)
    {
        List<Vector3> startPositions = new List<Vector3>();
        int numberOfCars = GameManager.Instance.NumberOfCars;

        for (int i = 1; i <= (int)Math.Ceiling(numberOfCars / 3f); i++)
        {
            for (int j = -1; j < 2; j++)
            {
                if (startPositions.Count == numberOfCars)
                    return startPositions;
                Vector3 newPosition = 
                    new Vector3( splineFirstPoint.x + j * 4, splineFirstPoint.y, -i * 8);
                startPositions.Add(newPosition);
            }
        }

        return startPositions;
    }

    private void GenerateRoadLines(int spacing = 160)
    {
        GameObject roadLinesGO = new GameObject("Road Lines");
        roadLinesGO.transform.parent = levelElements.transform;
        for(int i = 0; i < spacing; i++)
        {
            GameObject roadLineGO = 
                Instantiate(roadLinePrefab, roadLinesGO.transform);
            Vector3 position = roadLineGO.transform.position;
            
            position = levelBaseSpline.GetPoint((float) i/spacing);
            Vector3 relativePos =
                levelBaseSpline.GetPoint(((float) i + 1) / spacing) - position;
            position += new Vector3(0,0.15f,0);
            
            roadLineGO.transform.position = position;
            roadLineGO.transform.rotation = 
                Quaternion.LookRotation(relativePos, Vector3.up);
        }
    }
    
    //Number of limits increases the duplication of limits on y axis
    private void GenerateLimits(int numberOfLimits, int width = 5)
    {
        //Generate Terrain Limits
        GameObject allLimitsGO = new GameObject("AllLimits");
        allLimitsGO.transform.parent = levelElements.transform;

        for (int i = 0; i < numberOfLimits; i++)
        {
            GameObject limitsGO = new GameObject("Limits");
            limitsGO.transform.parent = allLimitsGO.transform;
            limitsGO.transform.tag = "Limits";
            
            GameObject leftLimitsGO = GenerateElementFromSpline("LeftLimit", width, 
                null, .5f * groundWidth/2f);
            leftLimitsGO.transform.parent = limitsGO.transform;
            Destroy(leftLimitsGO.GetComponent<MeshFilter>());
            Destroy(leftLimitsGO.GetComponent<MeshRenderer>());
            
            GameObject rightLimitsGO = GenerateElementFromSpline("RightLimit", width, 
                null, -.5f * groundWidth/2f);
            rightLimitsGO.transform.parent = limitsGO.transform;
            Destroy(rightLimitsGO.GetComponent<MeshFilter>());
            Destroy(rightLimitsGO.GetComponent<MeshRenderer>());
        
            limitsGO.transform.position += new Vector3(0,1.5f * i,0);
        }
        
        allLimitsGO.transform.position += new Vector3(0,0.5f,0);
    }
    #endregion
}