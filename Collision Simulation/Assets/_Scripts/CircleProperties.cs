using UnityEngine;
public class CircleProperties
{
    public Vector2 Position;
    public Vector2 Velocity;
    public Vector2 Acceleration;
    public float Radius;
    public float Mass;
    public CircleProperties(Vector2 Position, Vector2 Velocity, Vector2 Acceleration, float Radius, float Mass)
    {
        this.Position = Position;
        this.Velocity= Velocity;
        this.Acceleration = Acceleration;
        this.Radius = Radius;
        this.Mass = Mass;
    }
}