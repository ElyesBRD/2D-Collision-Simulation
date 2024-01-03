using UnityEngine;
public class Spawner : MonoBehaviour
{
    public static Spawner Instance;
    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(this);
    }
    public CircleProperties[] CreateCircles(int NCircles,Vector2 StartVelocity, Vector2 StartAcceleration)
    {
        CircleProperties[] Circles = new CircleProperties[NCircles];

        for (int i = 0; i < Circles.Length; i++)
        {
            Circles[i] = new CircleProperties(new Vector2(2f * i, 2f * i), new Vector2 (UnityEngine.Random.Range(-StartVelocity.x, StartVelocity.x), UnityEngine.Random.Range(-StartVelocity.y, StartVelocity.y)), StartAcceleration, UnityEngine.Random.Range(.5f,2.5f), UnityEngine.Random.Range(.1f, 2.5f));
        }

        return Circles;
    }
}