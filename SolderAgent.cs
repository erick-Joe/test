using System.Collections;
using System.Collections.Generic;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEditor.Timeline.Actions;
using UnityEngine;

public class SolderAgent : Agent
{
    public List<Transform> MainPathChecks = new List<Transform>();
    public List<Transform> EnemyPathChecks = new List<Transform>();
    public Vector3 startingPosition = new Vector3(-2923.3f, 0f, 840.9f);
    public CharacterController characterController;
    public float speed = 10f;
    public float rayCastLength = 60f;
    public float rotationSpeed = 180f;
    public Animator animator;
    //All enemies in the environment
    public GameObject Enemy1;
    public GameObject Enemy2;
    public GameObject Enemy3;
    public GameObject Enemy4;



    private bool isWalking;
    private Transform currentWaypoint;
    private int currentWaypointIndex = 0;
    private object vectorAction;
    private int mainPathCheckpointIndex;
    private int branchPathCheckpointIndex;
    //Variables to help me track the checkpoints
    private int previousMainPathCheckpointIndex = -1;
    private int previousBranchPathCheckpointIndex = -1;
    //Positions of the enemies
    private Vector3 initialPosition1;
    private Vector3 initialPosition2;
    private Vector3 initialPosition3;
    private Vector3 initialPosition4;
    private int currentTargetIndex;
    private bool episodeEnded;


    [SerializeField] private MainPathCheckManager mainPathCheckpoints;
    [SerializeField] private EnemyPathCheckManager branchPathCheckpoints;
    [SerializeField] private string CheckpointTag;

    public override void Initialize()
    {
        // Initialize the list of main path checkpoints
        foreach (GameObject checkpointObject in GameObject.FindGameObjectsWithTag("MainPathCheck"))
        {
            MainPathChecks.Add(checkpointObject.transform); // Access the Transform component directly
        }

        // Initialize the list of enemy branch checkpoints
        foreach (GameObject checkpointObject in GameObject.FindGameObjectsWithTag("EnemyPathCheck"))
        {
            EnemyPathChecks.Add(checkpointObject.transform); // Access the Transform component directly
        }

        // Initialize the first checkpoint
        if (MainPathChecks.Count > 0)
        {
            currentWaypoint = MainPathChecks[currentWaypointIndex];
        }



    }


    public override void OnEpisodeBegin()
    {
        // Reposition the agent
        transform.position = startingPosition;
        transform.rotation = Quaternion.Euler(0f, 180f, 0f);


        // Reseting used variables
        currentWaypointIndex = 0;
        mainPathCheckpointIndex = 0;
        branchPathCheckpointIndex = 0;
        previousMainPathCheckpointIndex = -1;
        previousBranchPathCheckpointIndex = -1;


        //animations
        isWalking = false;
        animator.SetBool("IsWalking", isWalking);

        // Reset checkpoints
        if (MainPathChecks.Count > 0)
        {
            currentWaypoint = MainPathChecks[currentWaypointIndex];
        }
        else
        {
            currentWaypoint = null;
        }

        //Call the function to reset the episode and environment
        ResetEpisode();


    }

    public override void CollectObservations(VectorSensor sensor)
    {

        // Observations for the nearest checkpoint
        if (currentWaypoint != null)
        {
            Vector3 toWaypoint = currentWaypoint.position - transform.position;
            sensor.AddObservation(toWaypoint.normalized); // Direction to the nearest checkpoint
            sensor.AddObservation(toWaypoint.magnitude);  // Distance to the nearest checkpoint
        }

        // The position of the agent
        sensor.AddObservation(transform.localPosition.x);
        sensor.AddObservation(transform.localPosition.y);

        sensor.AddObservation(transform.position);

        // Raycast to detect enemies, walls, and checkpoints
        Ray ray = new Ray(transform.position, transform.forward);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, rayCastLength))
        {
            GameObject hitObject = hit.collider.gameObject;

            if (hitObject.CompareTag("Enemy"))
            {
                sensor.AddObservation(hitObject.transform.position);
                float distanceToEnemy = Vector3.Distance(transform.position, hitObject.transform.position);
                sensor.AddObservation(distanceToEnemy);
            }
            if (hitObject.CompareTag("Wall"))
            {
                sensor.AddObservation(hitObject.transform.position);
                float distanceToWall = Vector3.Distance(transform.position, hitObject.transform.position);
                sensor.AddObservation(distanceToWall);
            }
            if (hitObject.CompareTag("Poison"))
            {
                sensor.AddObservation(hitObject.transform.position);
                float distanceToPoison = Vector3.Distance(transform.position, hitObject.transform.position);
                sensor.AddObservation(distanceToPoison);
            }
            if (hitObject.CompareTag("EnemyPathCheck"))
            {
                sensor.AddObservation(hitObject.transform.position);
                float distanceToEnemyPathCheck = Vector3.Distance(transform.position, hitObject.transform.position);
                sensor.AddObservation(distanceToEnemyPathCheck);
            }
            if (hitObject.CompareTag("MainPathCheck"))
            {
                sensor.AddObservation(hitObject.transform.position);
                float distanceToMainPathCheck = Vector3.Distance(transform.position, hitObject.transform.position);
                sensor.AddObservation(distanceToMainPathCheck);
            }
        }

    }

    public void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            GameObject collidedTarget = collision.gameObject;
            int collidedTargetIndex = GetTargetByIndex(collidedTarget);

            if (collidedTargetIndex == currentTargetIndex)
            {
                // The agent collided with the correct target
                DisableTarget(collidedTarget);
                currentTargetIndex++;

                if (currentTargetIndex <= 4)
                {
                    // Provide a reward to the agent
                    float reward = 20.0f;
                    // Provide the reward to your reinforcement learning agent here
                    Debug.Log("Agent hit target " + (currentTargetIndex - 1) + " and received a reward of " + reward);
                }
            }
            else
            {
                // The agent collided with the wrong target, end the episode
                EndEpisode();
            }
        }
        if (collision.collider.tag == "Wall")
        {
            AddReward(-3f);
            EndEpisode();
            //EndEpisode();// I have not decided on this
        }
        if (collision.collider.tag == "Poison")
        {
            AddReward(-6f);
            EndEpisode();
        }



        // The checkpoint integration using the collision method
        if (collision.gameObject.CompareTag("MainPathCheck"))
        {
            int checkpointIndex = mainPathCheckpoints.GetCheckpointIndex(collision.gameObject);
            if (checkpointIndex > previousMainPathCheckpointIndex)
            {
                MainPathCheckpointTriggered(true);
                previousMainPathCheckpointIndex = checkpointIndex;

                // Add a reward for passing the correct checkpoint on the main path
                AddReward(1.0f);
                Debug.Log("Passed the correct MAIN checkpoint with index: " + previousMainPathCheckpointIndex);

            }
            else
            {
                MainPathCheckpointTriggered(false);

                // Apply a penalty for passing the wrong checkpoint on the main path
                AddReward(-1.0f);
                Debug.Log("Passed the Wrong MAIN checkpoint with index: " + previousMainPathCheckpointIndex);

            }
        }
        else if (collision.gameObject.CompareTag("EnemyPathCheck"))
        {
            int checkpointIndex = branchPathCheckpoints.GetCheckpointIndex(collision.gameObject);
            if (checkpointIndex > previousBranchPathCheckpointIndex)
            {
                BranchPathCheckpointTriggered(true);
                previousBranchPathCheckpointIndex = checkpointIndex;

                // Add a reward for passing the correct checkpoint on the branch path
                AddReward(2.0f);
                Debug.Log("Passed the correct ENEMY PATH checkpoint with index: " + previousBranchPathCheckpointIndex);

            }
            else
            {
                BranchPathCheckpointTriggered(false);

                // Apply a smaller penalty for passing the wrong checkpoint on the branch path
                AddReward(-0.5f);
                Debug.Log("Passed the wrong ENEMY PATH checkpoint with index: " + previousBranchPathCheckpointIndex);

            }
        }
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        var actionTaken = actions.ContinuousActions;

        float moveDirection = actionTaken[0];
        float turnDirection = actionTaken[1];

        // Calculate forward movement
        float moveSpeed = moveDirection * speed;
        transform.Translate(Vector3.forward * moveSpeed * Time.fixedDeltaTime);

        // Calculate rotation based on turnDirection
        float rotation = turnDirection * rotationSpeed;

        // Rotate the agent
        transform.Rotate(Vector3.up, rotation * Time.fixedDeltaTime);

        // Check if the agent is moving
        isWalking = (moveSpeed != 0) || (rotation != 0);
        animator.SetBool("IsWalking", isWalking);

        // Apply a penalty for every step to encourage the agent to reach the goal
        AddReward(-0.01f);
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
        mainPathCheckpoints.OnCheckpointTriggered += MainPathCheckpointTriggered;
        branchPathCheckpoints.OnCheckpointTriggered += BranchPathCheckpointTriggered;

        //I am testing this codes
        // Store the initial positions of each target
        initialPosition1 = Enemy1.transform.position;
        initialPosition2 = Enemy2.transform.position;
        initialPosition3 = Enemy3.transform.position;
        initialPosition4 = Enemy4.transform.position;
        // Initialize variables
        currentTargetIndex = 1;
        episodeEnded = false;
    }
    private void MainPathCheckpointTriggered(bool isCorrectCheckpoint)
    {
       
    }

    private void BranchPathCheckpointTriggered(bool isCorrectCheckpoint)
    {
        
    }

    //I am testing this codes
    private void ResetEpisode()
    {
        // Reset the episode by restoring all targets to their initial positions
        Enemy1.transform.position = initialPosition1;
        Enemy2.transform.position = initialPosition2;
        Enemy3.transform.position = initialPosition3;
        Enemy4.transform.position = initialPosition4;
        // Reset episode-related variables
        currentTargetIndex = 1;
        episodeEnded = false;

        // Re-enable the renderer and collider components to make the objects visible and interactable
        Renderer renderer1 = Enemy1.GetComponent<Renderer>();
        Renderer renderer2 = Enemy2.GetComponent<Renderer>();
        Renderer renderer3 = Enemy3.GetComponent<Renderer>();
        Renderer renderer4 = Enemy4.GetComponent<Renderer>();

        Collider collider1 = Enemy1.GetComponent<Collider>();
        Collider collider2 = Enemy2.GetComponent<Collider>();
        Collider collider3 = Enemy3.GetComponent<Collider>();
        Collider collider4 = Enemy4.GetComponent<Collider>();

        if (renderer1 != null)
        {
            renderer1.enabled = true;
        }
        if (renderer2 != null)
        {
            renderer2.enabled = true;
        }
        if (renderer3 != null)
        {
            renderer3.enabled = true;
        }
        if (renderer4 != null)
        {
            renderer4.enabled = true;
        }

        if (collider1 != null)
        {
            collider1.enabled = true;
        }
        if (collider2 != null)
        {
            collider2.enabled = true;
        }
        if (collider3 != null)
        {
            collider3.enabled = true;
        }
        if (collider4 != null)
        {
            collider4.enabled = true;
        }
    }

    //Testing still
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

        return 0;
    }
    //Codes under test
    void DisableTarget(GameObject target)
    {
        // Disable the renderer and collider of the target object
        Renderer renderer = target.GetComponent<Renderer>();
        Collider collider = target.GetComponent<Collider>();

        if (renderer != null)
        {
            renderer.enabled = false; // Make the object not visible
        }

        if (collider != null)
        {
            collider.enabled = false; // Make the object not interactable
        }
    }

    }
