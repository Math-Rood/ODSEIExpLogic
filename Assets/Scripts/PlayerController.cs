using UnityEngine;
using System.Collections; 

public class PlayerController : MonoBehaviour
{
    public int currentX = 0;
    public int currentY = 0;
    public float moveSpeed = 0.2f;
    public float turnSpeed = 0.1f;
    public SpriteRenderer playerSpriteRenderer;

    public enum Direction { North, East, South, West }
    public Direction currentDirection = Direction.East;

    private GridManager gridManager;
    private Coroutine currentMovementCoroutine; 


    void Awake()
    {
        gridManager = FindObjectOfType<GridManager>();
        if (gridManager == null) Debug.LogError("GridManager not found!");
    }

    public void InitializePlayer(int startX, int startY, Direction startDirection)
    {
        currentX = startX;
        currentY = startY;
        currentDirection = startDirection;
        UpdateVisualPositionAndRotation();
    }

    void UpdateVisualPositionAndRotation()
    {
        transform.position = new Vector3(currentX * gridManager.cellSize, currentY * gridManager.cellSize, transform.position.z);

        float zRotation = 0;
        switch (currentDirection)
        {
            case Direction.North: zRotation = 90; break;
            case Direction.East: zRotation = 0; break;
            case Direction.South: zRotation = -90; break;
            case Direction.West: zRotation = 180; break;
        }
        transform.rotation = Quaternion.Euler(0, 0, zRotation);
    }

    public IEnumerator MoveForward()
    {
        int nextX = currentX;
        int nextY = currentY;

        switch (currentDirection)
        {
            case Direction.North: nextY++; break;
            case Direction.East: nextX++; break;
            case Direction.South: nextY--; break;
            case Direction.West: nextX--; break;
        }

        
        if (nextX >= 0 && nextX < gridManager.gridSize &&
            nextY >= 0 && nextY < gridManager.gridSize)
        {
            currentX = nextX;
            currentY = nextY;

            
            yield return StartCoroutine(MoveSmoothly());
        }
        else
        {
            Debug.Log("Move para frente não encontrado...");
            
        }
    }

    IEnumerator MoveSmoothly()
    {
        Vector3 targetPosition = new Vector3(currentX * gridManager.cellSize, currentY * gridManager.cellSize, transform.position.z);
        Vector3 startPosition = transform.position;
        float elapsedTime = 0;

        while (elapsedTime < moveSpeed)
        {
            transform.position = Vector3.Lerp(startPosition, targetPosition, (elapsedTime / moveSpeed));
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        transform.position = targetPosition;

        
        gridManager.CheckPlayerPosition(currentX, currentY);
    }

    public IEnumerator TurnRight()
    {
        switch (currentDirection)
        {
            case Direction.North: currentDirection = Direction.East; break;
            case Direction.East: currentDirection = Direction.South; break;
            case Direction.South: currentDirection = Direction.West; break;
            case Direction.West: currentDirection = Direction.North; break;
        }

        Quaternion startRotation = transform.rotation;
        Quaternion targetRotation = Quaternion.Euler(0, 0, GetZRotationForDirection(currentDirection));
        float elapsedTime = 0;

        while (elapsedTime < turnSpeed)
        {
            transform.rotation = Quaternion.Slerp(startRotation, targetRotation, (elapsedTime / turnSpeed));
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        transform.rotation = targetRotation;
    }

    public IEnumerator TurnLeft()
    {
        switch (currentDirection)
        {
            case Direction.North: currentDirection = Direction.West; break;
            case Direction.East: currentDirection = Direction.North; break;
            case Direction.South: currentDirection = Direction.East; break;
            case Direction.West: currentDirection = Direction.South; break;
        }

        Quaternion startRotation = transform.rotation;
        Quaternion targetRotation = Quaternion.Euler(0, 0, GetZRotationForDirection(currentDirection));
        float elapsedTime = 0;

        while (elapsedTime < turnSpeed)
        {
            transform.rotation = Quaternion.Slerp(startRotation, targetRotation, (elapsedTime / turnSpeed));
            elapsedTime += Time.deltaTime; 
            yield return null;
        }
        transform.rotation = targetRotation;
    }

    private float GetZRotationForDirection(Direction dir)
    {
        switch (dir)
        {
            case Direction.North: return 90;
            case Direction.East: return 0;
            case Direction.South: return -90;
            case Direction.West: return 180;
            default: return 0;
        }
    }
}