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
    public float speed = 110f;
    public float rayCastLength = 60f;
    public float rotationSpeed = 180f;
    float thresholdDistance = 5.5f;
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


    private bool startWalking;
    private Transform currentWaypoint;
    private int currentWaypointIndex = 0;
    private object vectorAction;
    private int mainPathCheckpointIndex;
    private int branchPathCheckpointIndex;
    //Variables to help me track the checkpoints
    private int expectedMainPathCheckpointIndex = 0;
    private int expectedBranchPathCheckpointIndex = 0;



    private int currentTargetIndex;
    // Positions of the enemies (now exposed in the Unity Editor)
    public Vector3 initialPosition1 = new Vector3(-2627.23f, 0f, 52.2f);
    public Vector3 initialPosition2 = new Vector3(-3418.9f, 0f, 37.9f);
    public Vector3 initialPosition3 = new Vector3(-3420.1f, 0f, -571.7f);
    public Vector3 initialPosition4 = new Vector3(-2912.0f, 0f, -1369f);
    public Vector3 initialPosition5 = new Vector3(-1911.5f, 0f, -1307f);
    public Vector3 initialPosition6 = new Vector3(-1902.5f, 0f, -577.7f);
    public Vector3 initialPosition7 = new Vector3(-2046f, 0f, 16.2f);
    public Vector3 initialPosition8 = new Vector3(-2517.8f, 0f, 39f);
    public Vector3 initialPosition9 = new Vector3(-1932.9f, 0f, 28.4f);
    public Vector3 initialPosition10 = new Vector3(-1164f, 0f, 28.4f);
    public Vector3 initialPosition11 = new Vector3(-1679.6f, 0f, -1320.1f);
    public Vector3 initialPosition12 = new Vector3(-1424f, 0f, -570f);
    public Vector3 initialPosition13 = new Vector3(-1164f, 0f, -570f);



    [SerializeField] private MainPathCheckManager mainPathCheckpoints;
    [SerializeField] private EnemyPathCheckManager branchPathCheckpoints;
    [SerializeField] private string CheckpointTag;

    public override void Initialize()
    {
        // Initialize the list of main path checkpoints
        foreach (GameObject checkpointObject in GameObject.FindGameObjectsWithTag("MainPathCheck"))
        {
            MainPathChecks.Add(checkpointObject.transform); 
        }

        // Initialize the list of enemy branch checkpoints
        foreach (GameObject checkpointObject in GameObject.FindGameObjectsWithTag("EnemyPathCheck"))
        {
            EnemyPathChecks.Add(checkpointObject.transform); 
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
        expectedMainPathCheckpointIndex = 0;
        expectedBranchPathCheckpointIndex = 0;



        //animations
        startWalking = false;
        animator.SetBool("WalkNow", startWalking);

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
            sensor.AddObservation(toWaypoint.normalized);
            sensor.AddObservation(toWaypoint.magnitude); 
        }
        else
        {
            sensor.AddObservation(Vector3.zero);
            sensor.AddObservation(0f); 
        }

        // The position of the agent
        sensor.AddObservation(transform.position.x);
        sensor.AddObservation(transform.position.y);
        sensor.AddObservation(transform.position);

        // Raycast to detect enemies, walls, and checkpoints
        Ray ray = new Ray(transform.position, transform.forward);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, rayCastLength))
        {
            GameObject hitObject = hit.collider.gameObject;

            if (hitObject.CompareTag("Enemy"))
            {
                // Get the enemy's position and calculate the distance to it
                Vector3 enemyPosition = hitObject.transform.position;
                float distanceToEnemy = Vector3.Distance(transform.position, enemyPosition);
                Debug.Log(distanceToEnemy);
                if (distanceToEnemy < thresholdDistance)
                {
                    // The agent is near an enemy, provide a reward of +5
                    sensor.AddObservation(enemyPosition);
                    sensor.AddObservation(distanceToEnemy);

                    AddReward(0.0625f);
                    Debug.Log("Near an enemy. Reward: 0.0625");
                }
                else
                {
                    // The agent is not within the threshold distance of the enemy
                    sensor.AddObservation(Vector3.zero);
                    sensor.AddObservation(0f);
                }


            }
            else
            {
                sensor.AddObservation(Vector3.zero);
                sensor.AddObservation(0f);
            }

            if (hitObject.CompareTag("Wall"))
            {
                sensor.AddObservation(hitObject.transform.position);
                float distanceToWall = Vector3.Distance(transform.position, hitObject.transform.position);
                sensor.AddObservation(distanceToWall);
            }
            else
            {
                sensor.AddObservation(Vector3.zero); 
                sensor.AddObservation(0f); 
            }

            if (hitObject.CompareTag("Poison"))
            {
                sensor.AddObservation(hitObject.transform.position);
                float distanceToPoison = Vector3.Distance(transform.position, hitObject.transform.position);
                sensor.AddObservation(distanceToPoison);
            }
            else
            {
                sensor.AddObservation(Vector3.zero); 
                sensor.AddObservation(0f); 
            }

            if (hitObject.CompareTag("EnemyPathCheck"))
            {
                sensor.AddObservation(hitObject.transform.position);
                float distanceToEnemyPathCheck = Vector3.Distance(transform.position, hitObject.transform.position);
                sensor.AddObservation(distanceToEnemyPathCheck);
            }
            else
            {
                sensor.AddObservation(Vector3.zero);
                sensor.AddObservation(0f); 
            }

            if (hitObject.CompareTag("MainPathCheck"))
            {
                sensor.AddObservation(hitObject.transform.position);
                float distanceToMainPathCheck = Vector3.Distance(transform.position, hitObject.transform.position);
                sensor.AddObservation(distanceToMainPathCheck);
            }
            else
            {
                sensor.AddObservation(Vector3.zero);
                sensor.AddObservation(0f); 
            }
        }
        else
        {
            // Filling with zeros if no ray hit
            sensor.AddObservation(Vector3.zero);
            sensor.AddObservation(0f);
            sensor.AddObservation(Vector3.zero);
            sensor.AddObservation(0f);
            sensor.AddObservation(Vector3.zero);
            sensor.AddObservation(0f);
            sensor.AddObservation(Vector3.zero);
            sensor.AddObservation(0f);
            sensor.AddObservation(Vector3.zero);
            sensor.AddObservation(0f);
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
                AddReward(+0.5f);
                DisableTarget(collidedTarget);
                currentTargetIndex++;

                //Messages to help me test
                Debug.Log("Killed enemy " + collidedTargetIndex + " for a reward of +0.5");
            }
            else
            {
                // The agent collided with the wrong target, end the episode
                AddReward(-0.5f);
                //EndEpisode();//TURN THIS BACK ON
                Debug.Log("Wrong enemy collision. Penalty:-0.5 ");
            }
        }
        if (collision.collider.tag == "Wall")
        {
            AddReward(-1f);
            EndEpisode() ;
            Debug.Log("Collision with a wall. Penalty: -1");
        }
        if (collision.collider.tag == "Poison")
        {
            AddReward(-1f);
            EndEpisode();
            Debug.Log("Collision with poison. Penalty: -1");
        }

        if (collision.collider.tag == "Finish")
        {
            AddReward(+1.5f);
            EndEpisode();
            Debug.Log("The game is over Boy, Reward: 1.5");
        }


        // The checkpoint integration using the collision method
        if (collision.gameObject.CompareTag("MainPathCheck"))
        {
            int checkpointIndex = mainPathCheckpoints.GetCheckpointIndex(collision.gameObject);
            if (checkpointIndex == expectedMainPathCheckpointIndex)
            {
                // The agent crossed the correct main path checkpoint
                AddReward(+0.5f);
                mainPathCheckpointIndex++;
                expectedMainPathCheckpointIndex++;
                Debug.Log("Crossed main path checkpoint " + checkpointIndex + ". Reward:0.5 ");
            }
            else if (checkpointIndex > expectedMainPathCheckpointIndex)
            {
                // The agent crossed a checkpoint out of order in the main path, apply a penalty
                AddReward(-0.1f);
                Debug.Log("Crossed main path checkpoint out of order. Penalty:-0.1 ");
            }
            else
            {
                // The agent crossed a checkpoint backward in the main path, apply a penalty
                AddReward(-0.25f);
                Debug.Log("Crossed main path checkpoint backward. Penalty:-0.25 ");
            }
        }
        else if (collision.gameObject.CompareTag("EnemyPathCheck"))
        {
            int checkpointIndex = branchPathCheckpoints.GetCheckpointIndex(collision.gameObject);
            if (checkpointIndex == expectedBranchPathCheckpointIndex)
            {
                // The agent crossed the correct enemy path checkpoint
                AddReward(+0.25f);
                branchPathCheckpointIndex++;
                expectedBranchPathCheckpointIndex++;
            }
            else if (checkpointIndex > expectedBranchPathCheckpointIndex)
            {
                // The agent crossed a checkpoint out of order in the enemy path, apply a penalty
                AddReward(-0.5f);
                Debug.Log("Crossed enemy path checkpoint out of order. Penalty:-0.5 ");
            }
            else
            {
                // The agent crossed a checkpoint backward in the enemy path, apply a penalty
                //AddReward(-0.05f);
                Debug.Log("Crossed enemy path checkpoint backward. Penalty:0.05 ");
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
        startWalking = (moveSpeed != 0) || (rotation != 0);
        animator.SetBool("WalkNow", startWalking);

        // Apply a penalty for every step to encourage the agent to reach the goal
        //AddReward(-0.01f);
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
        currentTargetIndex = 1;
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
        Enemy5.transform.position = initialPosition5;
        Enemy6.transform.position = initialPosition6;
        Enemy7.transform.position = initialPosition7;
        Enemy8.transform.position = initialPosition8;
        Enemy9.transform.position = initialPosition9;
        Enemy10.transform.position = initialPosition10;
        Enemy11.transform.position = initialPosition11;
        Enemy12.transform.position = initialPosition12;
        Enemy13.transform.position = initialPosition13;


        // Reset episode-related variables
        currentTargetIndex = 1;

        // Re-enable the renderer and collider components to make the objects visible and interactable
        Renderer[] renderers = new Renderer[]
        {
        Enemy1.GetComponent<Renderer>(),
        Enemy2.GetComponent<Renderer>(),
        Enemy3.GetComponent<Renderer>(),
        Enemy4.GetComponent<Renderer>(),
        Enemy5.GetComponent<Renderer>(),
        Enemy6.GetComponent<Renderer>(),
        Enemy7.GetComponent<Renderer>(),
        Enemy8.GetComponent<Renderer>(),
        Enemy9.GetComponent<Renderer>(),
        Enemy10.GetComponent<Renderer>(),
        Enemy11.GetComponent<Renderer>(),
        Enemy12.GetComponent<Renderer>(),
        Enemy13.GetComponent<Renderer>(),
        };

        Collider[] colliders = new Collider[]
    {
        Enemy1.GetComponent<Collider>(),
        Enemy2.GetComponent<Collider>(),
        Enemy3.GetComponent<Collider>(),
        Enemy4.GetComponent<Collider>(),
        Enemy5.GetComponent<Collider>(),
        Enemy6.GetComponent<Collider>(),
        Enemy7.GetComponent<Collider>(),
        Enemy8.GetComponent<Collider>(),
        Enemy9.GetComponent<Collider>(),
        Enemy10.GetComponent<Collider>(),
        Enemy11.GetComponent<Collider>(),
        Enemy12.GetComponent<Collider>(),
        Enemy13.GetComponent<Collider>(),

    };

        foreach (Renderer renderer in renderers)
        {
            if (renderer != null)
            {
                renderer.enabled = true; // Make the object visible
            }
        }

        foreach (Collider collider in colliders)
        {
            if (collider != null)
            {
                collider.enabled = true; // Make the object interactable
            }
        }

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
//mlagents-learn config/SolderAgent.yaml --run-id="Run 1"
