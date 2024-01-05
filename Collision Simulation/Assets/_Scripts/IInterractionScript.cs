using System.Collections.Generic;
using UnityEngine;

public class IInterractionScript : MonoBehaviour
{
    Vector3 ClickedPosition;
    bool isPressingSpawnButton;
    Transform GhostCircle;
    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            isPressingSpawnButton = true;
            ClickedPosition = CursorScript.worldPosition;
            GhostCircle = Instantiate(CollisionSimulationHandler.Instance.CirlcePrefab, ClickedPosition, Quaternion.identity).transform;
            GhostCircle.GetComponent<SpriteRenderer>().color = Color.green;
            GhostCircle.localScale *= 2;
        }
        if (Input.GetButtonUp("Fire1"))
        {
            List<CircleProperties> newCirclesArray = new List<CircleProperties>(CollisionSimulationHandler.Instance.Circles);
            newCirclesArray.Add(new CircleProperties(ClickedPosition, CursorScript.worldPosition - ClickedPosition, CollisionSimulationHandler.Instance.StartAcceleration, 1, 1));
            CollisionSimulationHandler.Instance.UpdateParticlesArray(newCirclesArray.ToArray());

            Destroy(GhostCircle.gameObject);
            GhostCircle = null;

            isPressingSpawnButton = false;
        }
    }
    private void OnDrawGizmos()
    {
        if (!isPressingSpawnButton) return;
        Debug.DrawLine(CursorScript.worldPosition, ClickedPosition);
    }
}