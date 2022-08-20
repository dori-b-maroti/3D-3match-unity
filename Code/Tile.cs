/*Property of Dorothea "Dori" B-Maroti
----All rights reserved----*/

using System;
using UnityEngine;

public class Tile : MonoBehaviour
{
    public uint type;
    public int index;
    public event Action<int> movedDown;

    private const float minimalDistance = 0.001f;
    private const float animationSpeed = 1.0f;
    private const float accelerationIncrease = 0.1f;
    private float acceleration = 0.0f;
    private bool isAnimationOngoing = false;
    private float moveToYPosition;
    private bool isHidden = false;

    void Update()
    {
        if (!isAnimationOngoing)
        {
            return;
        }

        float step = animationSpeed * acceleration * Time.deltaTime;
        transform.position = Vector3.MoveTowards(transform.position, new Vector3(transform.position.x, moveToYPosition, 0), step);
        acceleration += accelerationIncrease;
        if (Vector3.Distance(transform.position, new Vector3(transform.position.x, moveToYPosition, 0)) < minimalDistance)
        {
            isAnimationOngoing = false;
            if (movedDown != null && !isHidden)
            {
                movedDown(index);
            }
        }
    }

    public void SetupForDrop(float moveToY)
    {
        isAnimationOngoing = true;
        moveToYPosition = moveToY;
        acceleration = 0.0f;
    }

    public void Hide()
    {
        gameObject.SetActive(false);
        isHidden = true;
    }

    public void CopyTile(Tile tileToCopy)
    {
        type = tileToCopy.type;
        Transform transform = gameObject.transform;
        Transform tileToCopyTransform = tileToCopy.transform;
        transform.position = tileToCopyTransform.position;
        transform.localScale = tileToCopyTransform.localScale;
        GetComponent<MeshFilter>().mesh = tileToCopy.GetComponent<MeshFilter>().mesh;
        GetComponent<MeshRenderer>().material = tileToCopy.GetComponent<MeshRenderer>().material;
    }
}
