using System.Collections.Generic;
using UnityEngine;

public class CollisionSimulationHandler : MonoBehaviour
{
    public static CollisionSimulationHandler Instance;
    public CircleProperties[] Circles;

    bool startDrawing; // to draw guizmos (only the border)

    [Tooltip("particles stuff")]
    public int NCircles; // numver of circles / particles
    public Vector2 StartVelocity;// set random velocity to all particles
    public Vector2 StartAcceleration; // set random acceleration to all particles
    public float RadiusInterval; // an intervale between 0.5 and RadiusInterval , if <= .5 || == 1 ? all the particles gonna have radius = 1 : RadiusInterval

    [Tooltip("border stuff")]
    public float BorderSize;
    public float BorderResistance = 1 / 2; //apon collision with a wall decrease the velocity mag by BorderResistance amount 
    public float MinVelocityToFreez = 2; // if the velocity magnitude of a particle goes below MinVelocityToFreez after collision with a wall set the velocity to 0

    [Tooltip("scene stuff")]
    public GameObject CirlcePrefab;
    public List<Transform> circlesTr;
    public Camera MainCamer;

    [Tooltip("test stuff")]
    //those r only to play with the simulation nothing else gonna change in the future
    public bool UpdateChanges;
    public bool updateAcceleration, updateVelocities, updateCameraSize, UpdateColors;
    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(this);
    }
    private void Start()
    {
        MainCamer = Camera.main;
        circlesTr = new List<Transform>();
        MainCamer.orthographicSize = BorderSize;
        startDrawing = true;

        if (RadiusInterval <= .5f) RadiusInterval = -1;
        Circles = Spawner.Instance.CreateCircles(NCircles, StartVelocity, StartAcceleration, RadiusInterval);

        for (int i = 0; i < Circles.Length; i++)
        {
            circlesTr.Add(Instantiate(CirlcePrefab, Circles[i].Position, Quaternion.identity, transform).transform);
        }
    }
    private void Update()
    {
        SimulateCollision();
        #region extra
        //these are only for testing and playing with the simulation gonna change in the future
        if (UpdateChanges)
        {
            UpdateChanges = false;

            if (updateAcceleration)
            {
                UpdateAcceleration();
            }
            if (updateVelocities)
            {
                UpdateVelocities();
            }
            if (updateCameraSize)
            {
                MainCamer.orthographicSize = BorderSize;
            }
        }
        if (UpdateColors) // just to add some color to the particles
        {
            UpdateColors = false;
            for (int i = 0; i < Circles.Length; i++)
            {
                if (i < (Circles.Length) * 1/3)
                {
                    circlesTr[i].GetComponent<SpriteRenderer>().color = Color.white;
                }
                else if (i > (Circles.Length) * 1 / 3 && i < (Circles.Length) * 2/3)
                {
                    circlesTr[i].GetComponent<SpriteRenderer>().color = Color.red;
                }
                else
                {
                    circlesTr[i].GetComponent<SpriteRenderer>().color = Color.blue;
                }
            }
        }
        #endregion
    }
    #region Simulation Code
    void SimulateCollision()
    {
        Bounding_Volume_Hierarchy.CalculateCollision(Circles, true);
        for (int i = 0; i < Circles.Length; i++)
        {
            ContinousWallsCollisionDetection(i); //checks if the current circle passed through walls or not if so calculate the new position
            Circles[i].Position += Circles[i].Velocity * Time.deltaTime; //calculates the position of the current cirlce
            Circles[i].Velocity += Circles[i].Acceleration * Time.deltaTime; //calculates the velocity of the current circle
            circlesTr[i].position = Circles[i].Position; //updates the current circle gameobject in the scene
        }
    }
    public void CalculateCollisionInAnArray(CircleProperties[] Circles)
    {
        for (int i = 0; i < Circles.Length; i++)
        {
            for (int j = i + 1; j < Circles.Length; j++)
            {
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
                        CalculateNewVelocitiesAfterCollision(Circles, i, j);
                    }
                }
            }
        }
    }
    void CalculateNewVelocitiesAfterCollision(CircleProperties[] Circles ,int i, int j)
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
        //calculate circle position in the next frame
        float NextXPos = Circles[i].Position.x + Circles[i].Velocity.x * Time.deltaTime;
        float NextYPos = Circles[i].Position.y + Circles[i].Velocity.y * Time.deltaTime;
        //checks if the next position is outside the bounding box or inside
        //check Up and Bottm
        if (NextYPos > BorderSize - Circles[i].Radius)
        {
            Circles[i].Position.y = (BorderSize - Circles[i].Radius) - ((NextYPos) - (BorderSize - Circles[i].Radius));
            if (Circles[i].Velocity.magnitude < MinVelocityToFreez) Circles[i].Velocity = Vector2.zero;
            else Circles[i].Velocity.y *= -1 * BorderResistance;
        }
        //check Bottom
        if (NextYPos < -BorderSize + Circles[i].Radius)
        {
            Circles[i].Position.y = (-BorderSize + Circles[i].Radius) - ((NextYPos) - (-BorderSize + Circles[i].Radius));
            if (Circles[i].Velocity.magnitude < MinVelocityToFreez) Circles[i].Velocity = Vector2.zero;
            else Circles[i].Velocity.y *= -1 * BorderResistance;
        }
        //check Right and Left
        if (NextXPos > BorderSize - Circles[i].Radius)
        {
            Circles[i].Position.x = (BorderSize - Circles[i].Radius) - ((NextXPos) - (BorderSize - Circles[i].Radius));
            if (Circles[i].Velocity.magnitude < MinVelocityToFreez) Circles[i].Velocity = Vector2.zero;
            else Circles[i].Velocity.x *= -1 * BorderResistance;
        }
        //check Left
        if (NextXPos < -BorderSize + Circles[i].Radius)
        {
            Circles[i].Position.x = (-BorderSize + Circles[i].Radius) - ((NextXPos) - (-BorderSize + Circles[i].Radius));
            if (Circles[i].Velocity.magnitude < MinVelocityToFreez) Circles[i].Velocity = Vector2.zero;
            else Circles[i].Velocity.x *= -1 * BorderResistance;
        }
    }
    #endregion
    #region extra
    private void OnDrawGizmos()
    {
        if (!startDrawing) return;
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
    #endregion
}