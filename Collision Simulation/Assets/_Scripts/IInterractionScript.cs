using System.Collections.Generic;
using UnityEngine;

public class IInterractionScript : MonoBehaviour
{
    Vector3 ClickedPosition;
    bool isPressingSpawnButton;
    Transform GhostCircle;
    private void Start()
    {
        GhostCircle = Instantiate(CollisionSimulationHandler.Instance.CirlcePrefab, Vector3.zero, Quaternion.identity).transform;
        GhostCircle.localScale *= 2;
        GhostCircle.gameObject.SetActive(false);
    }
    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            isPressingSpawnButton = true;
            ClickedPosition = CursorScript.worldPosition;

            GhostCircle.gameObject.SetActive(true);
            GhostCircle.position = ClickedPosition;
            GhostCircle.GetComponent<SpriteRenderer>().color = Color.green;
        }
        if (Input.GetButtonUp("Fire1"))
        {
            List<CircleProperties> newCirclesArray = new List<CircleProperties>(CollisionSimulationHandler.Instance.Circles);
            newCirclesArray.Add(new CircleProperties(ClickedPosition, CursorScript.worldPosition - ClickedPosition, CollisionSimulationHandler.Instance.StartAcceleration, 1, 1));
            CollisionSimulationHandler.Instance.UpdateParticlesArray(newCirclesArray.ToArray());

            GhostCircle.gameObject.SetActive(false);

            isPressingSpawnButton = false;
        }
    }
    private void OnGUI()
    {
        if (!isPressingSpawnButton) return;
        Debug.DrawLine(CursorScript.worldPosition, ClickedPosition);
    }
}