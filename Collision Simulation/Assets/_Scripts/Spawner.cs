using UnityEngine;
public class Spawner : MonoBehaviour
{
    public static Spawner Instance;
    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(this);
    }
    public CircleProperties[] CreateCircles(int NCircles,Vector2 StartVelocity, Vector2 StartAcceleration, float Radius) //this garbage spawning code gonna change
    {
        CircleProperties[] Circles = new CircleProperties[NCircles];
        float _Radius = 0;
        for (int i = 0; i < Circles.Length; i++)
        {
            if (Radius == -1) _Radius = 1; // this is just for test gonna delete it
            else _Radius = UnityEngine.Random.Range(.5f, Radius);
            Circles[i] = new CircleProperties(new Vector2(2f * i, 2f * i), new Vector2 (UnityEngine.Random.Range(-StartVelocity.x, StartVelocity.x), UnityEngine.Random.Range(-StartVelocity.y, StartVelocity.y)), StartAcceleration, _Radius, UnityEngine.Random.Range(.1f, 2.5f));
        }

        return Circles;
    }
}