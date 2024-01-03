using System.Collections.Generic;
using UnityEngine;

public class CollisionSimulationHandler : MonoBehaviour
{
    bool startDrawing;
    public static CollisionSimulationHandler Instance;
    public CircleProperties[] Circles;
    public int NCircles;
    public Vector2 StartVelocity;
    public Vector2 StartAcceleration;
    public float BorderSize;
    public float BorderResistance = 1 / 2;
    public float MinVelocityToFreez = 2;

    public GameObject CirlcePrefab;
    public List<Transform> circlesTr;
    public Camera MainCamer;

    public bool UpdateChanges;
    public bool updateAcceleration, updateVelocities, updateCameraSize;
    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(this);
    }
    private void Start()
    {
        MainCamer = Camera.main;
        MainCamer.orthographicSize = BorderSize;
        circlesTr = new List<Transform>();
        Circles = Spawner.Instance.CreateCircles(NCircles, StartVelocity, StartAcceleration);
        startDrawing = true;
        for (int i = 0; i < Circles.Length; i++)
        {
            circlesTr.Add(Instantiate(CirlcePrefab, Circles[i].Position, Quaternion.identity, transform).transform);
            circlesTr[i].localScale = Vector3.one * Circles[i].Radius * 2;
        }
    }
    private void Update()
    {
        MoveCircles();
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Bounding_Volume_Hierarchy.Instance.CreateBoxVolumeTree(Circles);
        }
        if (UpdateChanges)
        {
            UpdateChanges = false;

            if (updateAcceleration)
            {
                updateAcceleration= false;
                UpdateAcceleration();
            }
            if (updateVelocities)
            {
                updateVelocities = false;
                UpdateVelocities();
            }
            if (updateCameraSize)
            {
                updateCameraSize = false;
                MainCamer.orthographicSize = BorderSize;
            }
        }
    }
    void MoveCircles()
    {
        for (int i = 0; i < Circles.Length; i++)
        {
            ContinousWallsCollisionDetection(i);
            CollisionWithOtherSpheres(i);
            Circles[i].Velocity += Circles[i].Acceleration * Time.deltaTime;
            circlesTr[i].position = Circles[i].Position;
            //CheckWallsCollision(i);
        }
    }
    void CollisionWithOtherSpheres(int i)
    {
        for (int j = i; j < Circles.Length; j++)
        {
            if (i == j) continue;
            float distanceBetweenTwoPoints = (Circles[i].Position - Circles[j].Position).magnitude;
            float someOfTwoRadius = Circles[i].Radius + Circles[j].Radius;
            if (distanceBetweenTwoPoints <= someOfTwoRadius)
            {
                if (distanceBetweenTwoPoints < someOfTwoRadius - .1f)
                {
                    float xDistance = ((Circles[i].Radius + Circles[j].Radius) - Mathf.Abs((Circles[i].Position.x - Circles[j].Position.x))) / 4;
                    float yDistance = ((Circles[i].Radius + Circles[j].Radius) - Mathf.Abs((Circles[i].Position.y - Circles[j].Position.y))) / 4;

                    if (Circles[i].Position.x > Circles[j].Position.x)
                    {
                        Circles[i].Position.x += xDistance;
                        Circles[j].Position.x -= xDistance;
                    }
                    else
                    {
                        Circles[i].Position.x -= xDistance;
                        Circles[j].Position.x += xDistance;
                    }
                    if (Circles[i].Position.y > Circles[j].Position.y)
                    {
                        Circles[i].Position.y += yDistance;
                        Circles[j].Position.y -= yDistance;
                    }
                    else
                    {
                        Circles[i].Position.y -= yDistance;
                        Circles[j].Position.y += yDistance;
                    }
                }
                CalculateNewVelocitiesAfterCollision(i, j);
            }
        }
    }
    void CalculateNewVelocitiesAfterCollision(int i, int j)
    {
        //steps:
        //1
        Vector2 un = new Vector2(Circles[i].Position.x - Circles[j].Position.x, Circles[i].Position.y - Circles[j].Position.y).normalized;
        Vector2 ut = new Vector2(-un.y, un.x);

        //2
        Vector2 V1 = Circles[i].Velocity;
        Vector2 V2 = Circles[j].Velocity;

        //3
        float V1n = Vector2.Dot(un, V1);
        float V2n = Vector2.Dot(un, V2);

        float V1t = Vector2.Dot(ut, V1);
        float V2t = Vector2.Dot(ut, V2);

        //4
        float nV1t = V1t;
        float nV2t = V2t;

        //5
        float nV1n = (V1n * (Circles[i].Mass - Circles[j].Mass) + 2 * Circles[j].Mass * V2n) / (Circles[i].Mass + Circles[j].Mass);
        float nV2n = (V2n * (Circles[j].Mass - Circles[i].Mass) + 2 * Circles[i].Mass * V1n) / (Circles[i].Mass + Circles[j].Mass);

        //6
        Vector2 nV1nVector = nV1n * un;
        Vector2 nV2nVector = nV2n * un;

        Vector2 nV1tVector = nV1t * ut;
        Vector2 nV2tVector = nV2t * ut;

        //7
        Circles[i].Velocity = nV1nVector + nV1tVector;
        Circles[j].Velocity = nV2nVector + nV2tVector;
    }
    void ContinousWallsCollisionDetection(int i)
    {
        //check up
        //time = (BorderSize - Circles[i].Radius - (Circles[i].Position.y + Circles[i].Velocity.y * Time.deltaTime)) / (Circles[i].Position.y - (Circles[i].Position.y + Circles[i].Velocity.y * Time.deltaTime));
        if (Circles[i].Position.y + Circles[i].Velocity.y * Time.deltaTime > BorderSize - Circles[i].Radius)
        {
            //Circles[i].Position.y = LinearCordsDependingOnTimeIntervale(time, Circles[i].Position.y, Circles[i].Position.y + Circles[i].Velocity.y * Time.deltaTime);
            Circles[i].Position.y = (BorderSize - Circles[i].Radius) - ((Circles[i].Position.y + Circles[i].Velocity.y * Time.deltaTime) - (BorderSize - Circles[i].Radius));
            if (Circles[i].Velocity.magnitude < MinVelocityToFreez) Circles[i].Velocity = Vector2.zero;
            else Circles[i].Velocity.y *= -1 * BorderResistance;
        }
        if (Circles[i].Position.y + Circles[i].Velocity.y * Time.deltaTime < -BorderSize + Circles[i].Radius)
        {
            Circles[i].Position.y = (-BorderSize + Circles[i].Radius) - ((Circles[i].Position.y + Circles[i].Velocity.y * Time.deltaTime) - (-BorderSize + Circles[i].Radius));
            if (Circles[i].Velocity.magnitude < MinVelocityToFreez) Circles[i].Velocity = Vector2.zero;
            else Circles[i].Velocity.y *= -1 * BorderResistance;
        }
        if (Circles[i].Position.x + Circles[i].Velocity.x * Time.deltaTime > BorderSize - Circles[i].Radius)
        {
            Circles[i].Position.x = (BorderSize - Circles[i].Radius) - ((Circles[i].Position.x + Circles[i].Velocity.x * Time.deltaTime) - (BorderSize - Circles[i].Radius));
            if (Circles[i].Velocity.magnitude < MinVelocityToFreez) Circles[i].Velocity = Vector2.zero;
            else Circles[i].Velocity.x *= -1 * BorderResistance;
        }
        if (Circles[i].Position.x + Circles[i].Velocity.x * Time.deltaTime < -BorderSize + Circles[i].Radius)
        {
            Circles[i].Position.x = (-BorderSize + Circles[i].Radius) - ((Circles[i].Position.x + Circles[i].Velocity.x * Time.deltaTime) - (-BorderSize + Circles[i].Radius));
            if (Circles[i].Velocity.magnitude < MinVelocityToFreez) Circles[i].Velocity = Vector2.zero;
            else Circles[i].Velocity.x *= -1 * BorderResistance;
        }

        Circles[i].Position += Circles[i].Velocity * Time.deltaTime;
    }
    #region Old Trash Code
    float LinearCordsDependingOnTimeIntervale(float t, float Cord1, float Cords2) => t * Cords2 + (1 - t) * Cord1;
    void CheckWallsCollision(int i)
    {
        if (Circles[i].Position.y <= -BorderSize + Circles[i].Radius || Circles[i].Position.y >= BorderSize - Circles[i].Radius)
        {
            Circles[i].Velocity.y = -Circles[i].Velocity.y * BorderResistance;
        }
        if (Circles[i].Position.x <= -BorderSize + Circles[i].Radius || Circles[i].Position.x >= BorderSize - Circles[i].Radius)
        {
            Circles[i].Velocity.x = -Circles[i].Velocity.x * BorderResistance;
        }
    }
    #endregion
    private void OnDrawGizmos()
    {
        if (!startDrawing) return;
        //for (int i = 0; i < Circles.Length; i++)
        //{
        //    Gizmos.color = Color.white;
        //    Gizmos.DrawSphere(Circles[i].Position, Circles[i].Radius);
        //    //Gizmos.color = Color.black;
        //    //Gizmos.DrawWireCube(Vector2.zero, Vector2.one * ((BorderSize - Circles[i].Radius) * 2));
        //}
        Gizmos.color = Color.white;
        Gizmos.DrawWireCube(Vector2.zero, Vector2.one * BorderSize * 2);
    }
    
    public void UpdateAcceleration()
    {
        for (int i = 0; i < Circles.Length; i++)
        {
            Circles[i].Acceleration = StartAcceleration;
        }
    }
    public void UpdateVelocities()
    {
        for (int i = 0; i < Circles.Length; i++)
        {
            Circles[i].Velocity = new Vector2(UnityEngine.Random.Range(-StartVelocity.x, StartVelocity.x), UnityEngine.Random.Range(-StartVelocity.y, StartVelocity.y));
        }
    }
}