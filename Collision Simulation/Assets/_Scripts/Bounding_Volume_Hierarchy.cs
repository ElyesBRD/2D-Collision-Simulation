using System.Collections.Generic;
using UnityEngine;
public class Bounding_Volume_Hierarchy : MonoBehaviour
{
    public static Bounding_Volume_Hierarchy Instance;
    public BoxVolume boxVolumeTree;
    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(this);
    }
    public void CreateBoxVolumeTree(CircleProperties[] circlesArray)
    {
        boxVolumeTree = null;
        boxVolumeTree = CreateBoxVolumeTree(new BoxVolume(circlesArray, null, null, true));
        if (circlesArray.Length > 0) { }
        //yes
        Debug.Log(boxVolumeTree);
    }
    BoxVolume CreateBoxVolumeTree(BoxVolume currentBoxVolume)
    {
        int circleArrayLengh = currentBoxVolume.CirclesArray.Length;

        if (circleArrayLengh <= 1) return currentBoxVolume;

        List<CircleProperties> RightBoxVolume = new List<CircleProperties>();
        List<CircleProperties> LeftBoxVolume = new List<CircleProperties>();
        int repetition = 0;
        if (currentBoxVolume.isXAxis)
        {
            float SxPositions = 0;

            for (int i = 0; i < circleArrayLengh; i++)
            {
                SxPositions += currentBoxVolume.CirclesArray[i].Position.x;
            }
            float medxAxis = SxPositions / circleArrayLengh;

            for (int i = 0; i < circleArrayLengh; i++)
            {
                CircleProperties currentCircle = currentBoxVolume.CirclesArray[i];
                //for x Axis
                if (Mathf.Abs(currentCircle.Position.x - medxAxis) < currentCircle.Radius)
                {
                    RightBoxVolume.Add(currentCircle);
                    LeftBoxVolume.Add(currentCircle);
                    repetition++;
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
                SyPositions += currentBoxVolume.CirclesArray[i].Position.y;
            }
            float medyAxis = SyPositions / circleArrayLengh;

            for (int i = 0; i < circleArrayLengh; i++)
            {
                CircleProperties currentCircle = currentBoxVolume.CirclesArray[i];
                //for y Axis
                if (Mathf.Abs(currentCircle.Position.y - medyAxis) < currentCircle.Radius)
                {
                    RightBoxVolume.Add(currentCircle);
                    LeftBoxVolume.Add(currentCircle);
                }
                else if (currentCircle.Position.y > medyAxis) RightBoxVolume.Add(currentCircle);
                else if (currentCircle.Position.y < medyAxis) LeftBoxVolume.Add(currentCircle);
            }
        }

        //if (RightBoxVolume.Count == circleArrayLengh && LeftBoxVolume.Count == circleArrayLengh) return currentBoxVolume;
        if (RightBoxVolume.Count == circleArrayLengh || LeftBoxVolume.Count == circleArrayLengh)
        {
            return currentBoxVolume;
        }
        currentBoxVolume.CirclesArray = new CircleProperties[0];
        currentBoxVolume.Right = CreateBoxVolumeTree(new BoxVolume(RightBoxVolume.ToArray(), null, null, !currentBoxVolume.isXAxis));
        currentBoxVolume.Left = CreateBoxVolumeTree(new BoxVolume(LeftBoxVolume.ToArray(), null, null, !currentBoxVolume.isXAxis));

        return currentBoxVolume;
    }
}
public class BoxVolume
{
    public CircleProperties[] CirclesArray;
    public BoxVolume Right;
    public BoxVolume Left;
    public bool isXAxis;
    public BoxVolume(CircleProperties[] circlesArray, BoxVolume right, BoxVolume left, bool isXAxis)
    {
        CirclesArray = circlesArray;
        Right = right;
        Left = left;
        this.isXAxis = isXAxis;
    }
}