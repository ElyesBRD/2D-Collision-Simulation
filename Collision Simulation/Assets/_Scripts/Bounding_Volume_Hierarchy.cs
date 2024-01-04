using System.Collections.Generic;
using UnityEngine;
public class Bounding_Volume_Hierarchy 
{
    public static void CalculateCollision(CircleProperties[] circlesArray,bool isXAxis)
    {
        int circleArrayLengh = circlesArray.Length;

        if (circleArrayLengh <= 1) return;

        List<CircleProperties> RightBoxVolume = new List<CircleProperties>();
        List<CircleProperties> LeftBoxVolume = new List<CircleProperties>();

        if (isXAxis)
        {
            float SxPositions = 0;

            for (int i = 0; i < circleArrayLengh; i++)
            {
                SxPositions += circlesArray[i].Position.x;
            }
            float medxAxis = SxPositions / circleArrayLengh;

            for (int i = 0; i < circleArrayLengh; i++)
            {
                CircleProperties currentCircle = circlesArray[i];
                if (Mathf.Abs(currentCircle.Position.x - medxAxis) < currentCircle.Radius)
                {
                    RightBoxVolume.Add(currentCircle);
                    LeftBoxVolume.Add(currentCircle);
                }
                else if (currentCircle.Position.x > medxAxis) RightBoxVolume.Add(currentCircle);
                else if (currentCircle.Position.x < medxAxis) LeftBoxVolume.Add(currentCircle);
            }
        }
        else
        {
            float SyPositions = 0;

            for (int i = 0; i < circleArrayLengh; i++)
            {
                SyPositions += circlesArray[i].Position.y;
            }
            float medyAxis = SyPositions / circleArrayLengh;

            for (int i = 0; i < circleArrayLengh; i++)
            {
                CircleProperties currentCircle = circlesArray[i];
                if (Mathf.Abs(currentCircle.Position.y - medyAxis) < currentCircle.Radius)
                {
                    RightBoxVolume.Add(currentCircle);
                    LeftBoxVolume.Add(currentCircle);
                }
                else if (currentCircle.Position.y > medyAxis) RightBoxVolume.Add(currentCircle);
                else if (currentCircle.Position.y < medyAxis) LeftBoxVolume.Add(currentCircle);
            }
        }

        if (RightBoxVolume.Count == circleArrayLengh || LeftBoxVolume.Count == circleArrayLengh)
        {
            if (RightBoxVolume.Count == circleArrayLengh) CollisionSimulationHandler.Instance.CalculateCollisionInAnArray(RightBoxVolume.ToArray());
            else CollisionSimulationHandler.Instance.CalculateCollisionInAnArray(LeftBoxVolume.ToArray());
            return;
        }
        CalculateCollision(RightBoxVolume.ToArray(), !isXAxis);
        CalculateCollision(LeftBoxVolume.ToArray(), !isXAxis);
    }
}