using System.Collections;
using System.Collections.Generic;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEditor.Timeline.Actions;
using UnityEngine;
using UnityEngine.EventSystems;

public class SolderAgent : Agent
{
    public Vector3 startingPosition = new Vector3(-188.7f, 3f, 0f);
    public Vector3 bossPosition = new Vector3(0f, 23.2f, -132.3f);
    public Rigidbody rb;
    public float speed = 110f;
    public float rayCastLength = 60f;
    public float rotationSpeed = 180f;
    public float maxDistanceToTarget = 50.2f;
    private Vector3 currentTargetPosition;
    public Animator animator;
    //All enemies in the environment
    public GameObject Enemy1;
    public GameObject Enemy2;
    public GameObject Enemy3;
    public GameObject Enemy4;
    public GameObject Enemy5;
    public GameObject Enemy6;
    public GameObject Enemy7;
    public GameObject Enemy8;
    public GameObject Enemy9;
    public GameObject Enemy10;
    public GameObject Enemy11;
    public GameObject Enemy12;
    public GameObject Enemy13;
    //Boss implementation
    public GameObject Boss;
    public GameObject MoneyPrefab;
    public GameObject CoinPrefab;
    private bool collected = false;

    private bool startWalking;
    private object vectorAction;

    private int currentTargetIndex;
    // Positions of the enemies (now exposed in the Unity Editor)
    public Vector3 initialPosition1 = new Vector3(-188.0f, 10.9f, 197.0f);
    public Vector3 initialPosition2 = new Vector3(-69.0f, 10.9f, 197.0f);
    public Vector3 initialPosition3 = new Vector3(57.0f, 10.9f, 203.0f);
    public Vector3 initialPosition4 = new Vector3(173.0f, 10.9f, 201.0f);
    public Vector3 initialPosition5 = new Vector3(177.0f, 10.9f, 42.0f);
    public Vector3 initialPosition6 = new Vector3(179.0f, 10.9f, -143.0f);
    public Vector3 initialPosition7 = new Vector3(-184.0f, 10.9f, -149.0f);
    public Vector3 initialPosition8 = new Vector3(-182.0f, 10.9f, 42.0f);
    public Vector3 initialPosition9 = new Vector3(-117.0f, 10.9f, 90.0f);
    public Vector3 initialPosition10 = new Vector3(-3.0f, 10.9f, 90.0f);
    public Vector3 initialPosition11 = new Vector3(112.0f, 10.9f, 92.0f);
    public Vector3 initialPosition12 = new Vector3(61.0f, 10.9f, -12.0f);
    public Vector3 initialPosition13 = new Vector3(-102.0f, 10.9f, -7.0f);


    public override void Initialize()
    {

        if (rb == null)
        {
            rb = GetComponent<Rigidbody>();
        }
    }


    public override void OnEpisodeBegin()
    {
        // Reposition the agent
        transform.position = startingPosition;
        transform.rotation = Quaternion.Euler(0f, 180f, 0f);

        // Set the initial target position
        currentTargetPosition = Enemy1.transform.position;
        collected = false;


        //animations
        startWalking = false;
        animator.SetBool("WalkNow", startWalking);

        //Call the function to reset the episode and environment
        ResetEpisode();


    }

    public override void CollectObservations(VectorSensor sensor)
    {
        //1 Value
        sensor.AddObservation(collected);

        // Direction Agent is facing (1 Vector3 = 3 values)
        sensor.AddObservation(transform.forward);

        // Current target position (3 values for x, y, and z)
        sensor.AddObservation(currentTargetPosition.x);
        sensor.AddObservation(currentTargetPosition.y);
        sensor.AddObservation(currentTargetPosition.z);

        // Distance to current target position (1 float = 1 value)
        sensor.AddObservation(Vector3.Distance(currentTargetPosition, transform.position));

    }



    public void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            SetCurrentTargetPosition();
            GameObject collidedTarget = collision.gameObject;
            int collidedTargetIndex = GetTargetByIndex(collidedTarget);

            if (collidedTargetIndex == currentTargetIndex && collidedTargetIndex == 1)
            {
                if (!collected)
                {
                    // The agent collided with the correct target, and Collect is false
                    AddReward(1f);
                    DisableTarget(collidedTarget);
                    currentTargetIndex++;
                    collected = true;
                    currentTargetPosition = Boss.transform.position;
                }
                else
                {
                    // The agent collided with the correct target, but Collect is true
                    currentTargetPosition = Boss.transform.position;
                }
            }
            else if (collidedTargetIndex == currentTargetIndex && collidedTargetIndex == 2)
            {
                if (!collected)
                {
                    // The agent collided with the correct target, and Collect is false
                    AddReward(1f);
                    DisableTarget(collidedTarget);
                    currentTargetIndex++;
                    collected = true;
                    currentTargetPosition = Boss.transform.position;
                }
                else
                {
                    // The agent collided with the correct target, but Collect is true
                    currentTargetPosition = Boss.transform.position;
                }
            }
            else if (collidedTargetIndex == currentTargetIndex && collidedTargetIndex == 3)
            {
                if (!collected)
                {
                    // The agent collided with the correct target, and Collect is false
                    AddReward(1f);
                    DisableTarget(collidedTarget);
                    currentTargetIndex++;
                    collected = true;
                    currentTargetPosition = Boss.transform.position;
                }
                else
                {
                    // The agent collided with the correct target, but Collect is true
                    currentTargetPosition = Boss.transform.position;
                }
            }
            else if (collidedTargetIndex == currentTargetIndex && collidedTargetIndex == 4)
            {
                if (!collected)
                {
                    // The agent collided with the correct target, and Collect is false
                    AddReward(1f);
                    DisableTarget(collidedTarget);
                    currentTargetIndex++;
                    collected = true;
                    currentTargetPosition = Boss.transform.position;
                }
                else
                {
                    // The agent collided with the correct target, but Collect is true
                    currentTargetPosition = Boss.transform.position;
                }
            }
            else if (collidedTargetIndex == currentTargetIndex && collidedTargetIndex == 5)
            {
                if (!collected)
                {
                    // The agent collided with the correct target, and Collect is false
                    AddReward(1f);
                    DisableTarget(collidedTarget);
                    currentTargetIndex++;
                    collected = true;
                    currentTargetPosition = Boss.transform.position;
                }
                else
                {
                    // The agent collided with the correct target, but Collect is true
                    currentTargetPosition = Boss.transform.position;
                }
            }
            else if (collidedTargetIndex == currentTargetIndex && collidedTargetIndex == 6)
            {
                if (!collected)
                {
                    // The agent collided with the correct target, and Collect is false
                    AddReward(1f);
                    DisableTarget(collidedTarget);
                    currentTargetIndex++;
                    collected = true;
                    currentTargetPosition = Boss.transform.position;
                }
                else
                {
                    // The agent collided with the correct target, but Collect is true
                    currentTargetPosition = Boss.transform.position;
                }
            }
            else if (collidedTargetIndex == currentTargetIndex && collidedTargetIndex == 7)
            {
                if (!collected)
                {
                    // The agent collided with the correct target, and Collect is false
                    AddReward(1f);
                    DisableTarget(collidedTarget);
                    currentTargetIndex++;
                    collected = true;
                    currentTargetPosition = Boss.transform.position;
                }
                else
                {
                    // The agent collided with the correct target, but Collect is true
                    currentTargetPosition = Boss.transform.position;
                }
            }
            else if (collidedTargetIndex == currentTargetIndex && collidedTargetIndex == 8)
            {
                if (!collected)
                {
                    // The agent collided with the correct target, and Collect is false
                    AddReward(1f);
                    DisableTarget(collidedTarget);
                    currentTargetIndex++;
                    collected = true;
                    currentTargetPosition = Boss.transform.position;
                }
                else
                {
                    // The agent collided with the correct target, but Collect is true
                    currentTargetPosition = Boss.transform.position;
                }
            }
            else if (collidedTargetIndex == currentTargetIndex && collidedTargetIndex == 9)
            {
                if (!collected)
                {
                    // The agent collided with the correct target, and Collect is false
                    AddReward(1f);
                    DisableTarget(collidedTarget);
                    currentTargetIndex++;
                    collected = true;
                    currentTargetPosition = Boss.transform.position;
                }
                else
                {
                    // The agent collided with the correct target, but Collect is true
                    currentTargetPosition = Boss.transform.position;
                }
            }
            else if (collidedTargetIndex == currentTargetIndex && collidedTargetIndex == 10)
            {
                if (!collected)
                {
                    // The agent collided with the correct target, and Collect is false
                    AddReward(1f);
                    DisableTarget(collidedTarget);
                    currentTargetIndex++;
                    collected = true;
                    currentTargetPosition = Boss.transform.position;
                }
                else
                {
                    currentTargetPosition = Boss.transform.position;
                }
            }
            else if (collidedTargetIndex == currentTargetIndex && collidedTargetIndex == 11)
            {
                if (!collected)
                {
                    // The agent collided with the correct target, and Collect is false
                    AddReward(1f);
                    DisableTarget(collidedTarget);
                    currentTargetIndex++;
                    collected = true;
                    currentTargetPosition = Boss.transform.position;
                }
                else
                {
                    currentTargetPosition = Boss.transform.position;
                }
            }
            else if (collidedTargetIndex == currentTargetIndex && collidedTargetIndex == 12)
            {
                if (!collected)
                {
                    // The agent collided with the correct target, and Collect is false
                    AddReward(1f);
                    DisableTarget(collidedTarget);
                    currentTargetIndex++;
                    collected = true;
                    currentTargetPosition = Boss.transform.position;
                }
                else
                {
                    currentTargetPosition = Boss.transform.position;
                }
            }
            else if (collidedTargetIndex == currentTargetIndex && collidedTargetIndex == 13)
            {
                if (!collected)
                {
                    // The agent collided with the correct target, and Collect is false
                    AddReward(1f);
                    DisableTarget(collidedTarget);
                    currentTargetIndex++;
                    collected = true;
                    currentTargetPosition = Boss.transform.position;
                }
                else
                {
                    currentTargetPosition = Boss.transform.position;
                }
            }
        }
        if (collision.collider.tag == "Wall")
        {
            AddReward(-1.1f);
            EndEpisode();
            Debug.Log("COLLIDED WITH WALL");
        }

        if (collision.collider.tag == "Boss")
        {
            if (currentTargetIndex == 1)
            {
                if (!collected)
                {
                    currentTargetPosition = Enemy1.transform.position;
                }
                else
                {
                    SpawnThings();
                    currentTargetPosition = Enemy2.transform.position;
                    AddReward(0.1f);
                    collected = false;
                }
            }

            if (currentTargetIndex == 2)
            {
                if (!collected)
                {
                    currentTargetPosition = Enemy2.transform.position;

                }
                else
                {
                    SpawnThings();
                    currentTargetPosition = Enemy3.transform.position;
                    AddReward(0.1f);
                    collected = false;

                }
            }
            if (currentTargetIndex == 3)
            {
                if (!collected)
                {
                    currentTargetPosition = Enemy3.transform.position;

                }
                else
                {
                    SpawnThings();
                    currentTargetPosition = Enemy4.transform.position;
                    AddReward(0.1f);
                    collected = false;

                }
            }
            if (currentTargetIndex == 4)
            {
                if (!collected)
                {
                    currentTargetPosition = Enemy4.transform.position;

                }
                else
                {
                    SpawnThings();
                    currentTargetPosition = Enemy5.transform.position;
                    AddReward(0.1f);
                    collected = false;

                }
            }
            if (currentTargetIndex == 5)
            {
                if (!collected)
                {
                    currentTargetPosition = Enemy5.transform.position;

                }
                else
                {
                    SpawnThings();
                    currentTargetPosition = Enemy6.transform.position;
                    AddReward(0.1f);
                    collected = false;

                }
            }
            if (currentTargetIndex == 6)
            {
                if (!collected)
                {
                    currentTargetPosition = Enemy6.transform.position;


                }
                else
                {
                    SpawnThings();
                    currentTargetPosition = Enemy7.transform.position;
                    AddReward(0.1f);
                    collected = false;

                }
            }
            if (currentTargetIndex == 7)
            {
                if (!collected)
                {
                    currentTargetPosition = Enemy7.transform.position;

                }
                else
                {
                    SpawnThings();
                    currentTargetPosition = Enemy8.transform.position;
                    AddReward(0.1f);
                    collected = false;

                }
            }
            if (currentTargetIndex == 8)
            {
                if (!collected)
                {
                    currentTargetPosition = Enemy8.transform.position;

                }
                else
                {
                    SpawnThings();
                    currentTargetPosition = Enemy9.transform.position;
                    AddReward(0.1f);
                    collected = false;

                }
            }
            if (currentTargetIndex == 9)
            {
                if (!collected)
                {
                    currentTargetPosition = Enemy9.transform.position;

                }
                else
                {
                    SpawnThings();
                    currentTargetPosition = Enemy10.transform.position;
                    AddReward(0.1f);
                    collected = false;

                }
            }
            if (currentTargetIndex == 10)
            {
                if (!collected)
                {
                    currentTargetPosition = Enemy10.transform.position;

                }
                else
                {
                    SpawnThings();
                    currentTargetPosition = Enemy11.transform.position;
                    AddReward(0.1f);
                    collected = false;

                }
            }
            if (currentTargetIndex == 11)
            {
                if (!collected)
                {
                    currentTargetPosition = Enemy11.transform.position;

                }
                else
                {
                    SpawnThings();
                    currentTargetPosition = Enemy12.transform.position;
                    AddReward(0.1f);
                    collected = false;

                }
            }
            if (currentTargetIndex == 12)
            {
                if (!collected)
                {
                    currentTargetPosition = Enemy12.transform.position;

                }
                else
                {
                    SpawnThings();
                    currentTargetPosition = Enemy13.transform.position;
                    AddReward(0.1f);
                    collected = false;

                }
            }
            if (currentTargetIndex == 13)
            {
                if (!collected)
                {
                    currentTargetPosition = Enemy13.transform.position;

                }
                else
                {
                    SpawnThings();
                    currentTargetPosition = Enemy13.transform.position;
                    AddReward(0.1f);
                    collected = false;

                }
            }

            if (currentTargetIndex == 14)
            {
                if (!collected)
                {
                    currentTargetPosition = Enemy13.transform.position;

                }
                else
                {
                    AddReward(0.1f);
                    EndEpisode();

                }
            }

        }


    }

    private void SetCurrentTargetPosition()
    {
        if (!collected)
        {
            // The agent collided with the correct target, and Collect is false

            switch (currentTargetIndex)
            {
                case 1:
                    currentTargetPosition = Enemy1.transform.position;
                    break;
                case 2:
                    currentTargetPosition = Enemy2.transform.position;
                    break;
                case 3:
                    currentTargetPosition = Enemy3.transform.position;
                    break;
                case 4:
                    currentTargetPosition = Enemy4.transform.position;
                    break;
                case 5:
                    currentTargetPosition = Enemy5.transform.position;
                    break;
                case 6:
                    currentTargetPosition = Enemy6.transform.position;
                    break;
                case 7:
                    currentTargetPosition = Enemy7.transform.position;
                    break;
                case 8:
                    currentTargetPosition = Enemy8.transform.position;
                    break;
                case 9:
                    currentTargetPosition = Enemy9.transform.position;
                    break;
                case 10:
                    currentTargetPosition = Enemy10.transform.position;
                    break;
                case 11:
                    currentTargetPosition = Enemy11.transform.position;
                    break;
                case 12:
                    currentTargetPosition = Enemy12.transform.position;
                    break;
                case 13:
                    currentTargetPosition = Enemy13.transform.position;
                    break;
            }
        }
        else
        {
            // The agent collided with the correct target, but Collect is true
            currentTargetPosition = Boss.transform.position;
        }

    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        // Normal movement based on actions
        var actionTaken = actions.ContinuousActions;

        // Use index 0 for forward movement and index 1 for turning
        float moveDirection = actionTaken[0];
        float turnDirection = actionTaken[1];

        // Calculate forward movement
        Vector3 moveVector = transform.forward * moveDirection * speed;

        // Check if the agent is close to the target position
        float distanceToTarget = Vector3.Distance(transform.position, currentTargetPosition);

        // Adjust movement based on the distance to the target
        float movementMultiplier = Mathf.Clamp01(distanceToTarget / maxDistanceToTarget);
        moveVector *= movementMultiplier;

        // Apply movement using Rigidbody
        rb.MovePosition(rb.position + moveVector * Time.fixedDeltaTime);

        // Rotate the agent
        float rotation = turnDirection * rotationSpeed * Time.fixedDeltaTime;
        rb.MoveRotation(rb.rotation * Quaternion.Euler(Vector3.up * rotation));

        // Check if the agent is moving
        startWalking = (moveDirection != 0) || (turnDirection != 0);
        animator.SetBool("WalkNow", startWalking);

        // Apply a penalty for every step to encourage the agent to reach the goal
        //if (MaxStep > 0) AddReward(-1f / MaxStep);
    }


    public override void Heuristic(in ActionBuffers actionsOut)
    {
        ActionSegment<float> actions = actionsOut.ContinuousActions;

        // Control forward and backward movement
        float moveInput = 0;
        if (Input.GetKey("w")) moveInput = 1;
        else if (Input.GetKey("s")) moveInput = -1;

        // Control turning
        float turnInput = 0;
        if (Input.GetKey("d")) turnInput = 1;
        else if (Input.GetKey("a")) turnInput = -1;

        actions[0] = moveInput;
        actions[1] = turnInput;
    }

    private void Start()
    {
        currentTargetIndex = 1;
    }
   

    //I am testing this codes
    private void ResetEpisode()
    {
        // Reset the episode by restoring all targets to their initial positions
        Enemy1.transform.position = initialPosition1;
        Enemy2.transform.position = initialPosition2;
        Enemy3.transform.position = initialPosition3;
        Enemy4.transform.position = initialPosition4;
        Enemy5.transform.position = initialPosition5;
        Enemy6.transform.position = initialPosition6;
        Enemy7.transform.position = initialPosition7;
        Enemy8.transform.position = initialPosition8;
        Enemy9.transform.position = initialPosition9;
        Enemy10.transform.position = initialPosition10;
        Enemy11.transform.position = initialPosition11;
        Enemy12.transform.position = initialPosition12;
        Enemy13.transform.position = initialPosition13;
        Boss.transform.position = bossPosition;


        // Reset episode-related variables
        currentTargetIndex = 1;

    }

    //Getting the index of each enemy
    int GetTargetByIndex(GameObject target)
    {
        if (target == Enemy1)
        {
            return 1;
        }
        else if (target == Enemy2)
        {
            return 2;
        }
        else if (target == Enemy3)
        {
            return 3;
        }
        else if (target == Enemy4)
        {
            return 4;
        }

        else if (target == Enemy5)
        {
            return 5;
        }
        else if (target == Enemy6)
        {
            return 6;
        }
        else if (target == Enemy7)
        {
            return 7;
        }
        else if (target == Enemy8)
        {
            return 8;
        }
        else if (target == Enemy9)
        {
            return 9;
        }
        else if (target == Enemy10)
        {
            return 10;
        }
        else if (target == Enemy11)
        {
            return 11;
        }
        else if (target == Enemy12)
        {
            return 12;
        }
        else if (target == Enemy13)
        {
            return 13;
        }

        return 0;
    }
    //Codes under test
    void DisableTarget(GameObject target)
    {
        // Get the index of the target
        int targetIndex = GetTargetByIndex(target);

        // Change the position of the enemy based on its index
        Vector3 newPosition = GetNewEnemyPosition(targetIndex);
        target.transform.position = newPosition;
    }


    // Function to get the new position based on the target index
    Vector3 GetNewEnemyPosition(int targetIndex)
    {
        switch (targetIndex)
        {
            case 1:
                return new Vector3(199.0f, 10.9f, -293.2f);
            case 2:
                return new Vector3(153.0f, 10.9f, -293.2f);
            case 3:
                return new Vector3(99.0f, 10.9f, -293.2f);
            case 4:
                return new Vector3(153.0f, 10.9f, - 293.2f);
            case 5:
                return new Vector3(48.0f, 10.9f, -293.2f);
            case 6:
                return new Vector3(-2.0f, 10.9f, -293.2f);
            case 7:
                return new Vector3(-48.0f, 10.9f, -293.2f);
            case 8:
                return new Vector3(-97.0f, 10.9f, -293.2f);
            case 9:
                return new Vector3(-153.0f, 10.9f, -293.2f);
            case 10:
                return new Vector3(-198.0f, 10.9f, -293.2f);
            case 11:
                return new Vector3(-20.0f, 10.9f, -304.2f);
            case 12:
                return new Vector3(-182.0f, 10.9f, -304.2f);
            case 13:
                return new Vector3(121.0f, 10.9f, -304.2f);
            default:
                // If the index is not recognized, return a default position
                return Vector3.zero;
        }
    }
    
    private void SpawnThings()
    {
        // Spawn money prefab at the specified position
        GameObject money = Instantiate(MoneyPrefab, new Vector3(-69.5f, 4.9f, -84.3f), Quaternion.identity);

        // Destroy the money prefab after 4 seconds
        Destroy(money, 6f);

        // Spawn CoinPrefab just above the agent's position with adjusted height
        GameObject coin = Instantiate(CoinPrefab, new Vector3(25.5f, -18.0f, -82.2f), Quaternion.identity);

        // Destroy the coin prefab after 4 seconds
        Destroy(coin, 10f);
    }

}
//mlagents-learn config/SolderAgent.yaml --run-id="Run 1"
//mlagents-learn config/SolderAgent.yaml --run-id="TRAINING" --no-graphics

