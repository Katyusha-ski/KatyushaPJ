using UnityEngine;
using System.Collections.Generic;

public class AssisterController : MonoBehaviour
{
    public Transform player;
    public float followDelay = 0.5f;
    public PlayerController playerController;

    private Queue<Vector3> positionQueue = new Queue<Vector3>();
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // Store the player's position every frame
        positionQueue.Enqueue(player.position);

        // Calculate the number of frames needed for the desired delay
        int delayFrames = Mathf.RoundToInt(followDelay / Time.deltaTime);

        // When the queue is large enough, move the assister to the player's old position
        if (positionQueue.Count > delayFrames)
        {
            Vector3 targetPosition = positionQueue.Dequeue();
            float assisterSpeed = playerController != null ? playerController.CurrentSpeed : 2.0f;

            // Move the assister towards the target position at the player's current speed
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, assisterSpeed * Time.deltaTime);
        }
    }
}
