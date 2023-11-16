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
    public Rigidbody rb;
    public float speed = 110f;
    public float rayCastLength = 60f;
    public float rotationSpeed = 180f;
    public Animator animator;
    private bool startWalking;
    // Environment parameters
    public float environmentSizeX = 700f;
    public float environmentSizeZ = 600f;

    // Agent parameters
    public GameObject marketPrefab;
    public GameObject marketPrefab1;
    public GameObject enemyPrefab;
    public GameObject enemy2Prefab;
    public GameObject MoneyPrefab;
    public GameObject CoinPrefab;
    public GameObject DealPrefab;

    // Agent states
    private bool dealState = false;
    private bool enemy2spawned = false;


    // positions
    private Vector3 marketPosition;
    private Vector3 enemyPosition;



    public override void OnEpisodeBegin()
    {
        // Clear spawned elements from the previous episode
        ClearSpawnedElements();

        // Randomly place agent and market
        transform.position = GetRandomPosition();
        spawnMarket();

        // Initialize states
        dealState = false;
        enemy2spawned = false;

        //animations
        startWalking = false;
        animator.SetBool("WalkNow", startWalking);

    }



    public override void CollectObservations(VectorSensor sensor)
    {
        // 1. Observation of the direction of the market
        Vector3 directionToMarket = (marketPrefab.transform.position - transform.position).normalized;
        sensor.AddObservation(directionToMarket.x);
        sensor.AddObservation(directionToMarket.z);

        // 3. Observation of the current state of the enemy
        if (dealState)
        {
            // Return the position if the enemy has been spawned but not collected
            Vector3 directionToEnemy = (enemyPrefab.transform.position - transform.position).normalized;
            sensor.AddObservation(directionToEnemy.x);
            sensor.AddObservation(directionToEnemy.z);
        }
        else
        {
            // Return zeros if the enemy has been collected
            sensor.AddObservation(0f);
            sensor.AddObservation(0f);
        }

        if (enemy2spawned)
        {
            // Return the position if the enemy has been spawned but not collected
            Vector3 directionToEnemy2 = (enemy2Prefab.transform.position - transform.position).normalized;
            sensor.AddObservation(directionToEnemy2.x);
            sensor.AddObservation(directionToEnemy2.z);
        }
        else
        {
            // Return zeros if the enemy has been collected
            sensor.AddObservation(0f);
            sensor.AddObservation(0f);
        }

        // 2. Observation of the direction the agent is facing //3 VALUES //Total =9
        sensor.AddObservation(transform.forward);

    }


    public void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.tag == "marketplace")
        {
            dealState = true;
            AddReward(1f);
            SpawnEnemy();
            spawnDeal();
            Destroy(collision.gameObject);

        }

        if (collision.collider.tag == "marketplace2")
        {
            AddReward(1f);
            Destroy(collision.gameObject);
            EndEpisode();

        }
        if (collision.collider.tag == "Enemy")
        {
            AddReward(1f);
            SpawnEnemy2();
            spawnDeal();
            MoveEnemy(new Vector3(48.0f, 10.9f, -293.2f));
            SpawnThings();
            dealState = false;
            enemy2spawned = true;

        }

        if (collision.collider.tag == "Enemy2")
        {
            AddReward(1f);
            MoveEnemy2(new Vector3(-48.0f, 10.9f, -293.2f));
            SpawnThings();
            spawnMarket2();

        }
        if (collision.collider.tag == "Wall")
        {
            AddReward(-1.1f);
            EndEpisode();
            Debug.Log("COLLIDED WITH WALL");

        }

    }


    public override void OnActionReceived(ActionBuffers actions)
    {
        // Normal movement based on actions
        var actionTaken = actions.ContinuousActions;

        // Use index 0 for forward movement and index 1 for turning
        float moveDirection = actionTaken[0];
        float turnDirection = actionTaken[1];

        Vector3 moveVector = transform.forward * moveDirection * speed;
        rb.MovePosition(rb.position + moveVector * Time.fixedDeltaTime);

        float rotation = turnDirection * rotationSpeed * Time.fixedDeltaTime;
        rb.MoveRotation(rb.rotation * Quaternion.Euler(Vector3.up * rotation));


        startWalking = (moveDirection != 0) || (turnDirection != 0);
        animator.SetBool("WalkNow", startWalking);

        startWalking = (moveDirection != 0) || (turnDirection != 0);
        animator.SetBool("WalkNow", startWalking);

        AddReward(0.01f);
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

    private void SpawnThings()
    {
        GameObject money = Instantiate(MoneyPrefab, new Vector3(-69.5f, 4.9f, -84.3f), Quaternion.identity);

        Destroy(money, 6f);
        GameObject coin = Instantiate(CoinPrefab, new Vector3(25.5f, -18.0f, -82.2f), Quaternion.identity);

        Destroy(coin, 10f);
    }

    private void spawnDeal()
    {
        Vector3 spawnPosition = transform.position + new Vector3(0, 10.2f, 5f);

        GameObject deal = Instantiate(DealPrefab, spawnPosition, Quaternion.identity);
        Destroy(deal, 6f);
    }


    // Spawn enemy in the environment
    private void SpawnEnemy()
    {
        // Calculate the position to spawn the deal prefab
        Vector3 spawnPosition = GetRandomPosition();

        // Modify the y-coordinate to be 2.2
        spawnPosition.y = 1.9f;

        // Spawn deal prefab at the modified position
        GameObject enemy = Instantiate(enemyPrefab, spawnPosition, Quaternion.identity);
    }

    private void SpawnEnemy2()
    {
        // Calculate the position to spawn the deal prefab
        Vector3 spawnPosition = GetRandomPosition();

        // Modify the y-coordinate to be 2.2
        spawnPosition.y = 1.9f;

        // Spawn deal prefab at the modified position
        GameObject s_enemy = Instantiate(enemy2Prefab, spawnPosition, Quaternion.identity);
    }
    private void spawnMarket()
    {
        // Calculate the position to spawn the market prefab
        Vector3 spawnPosition = GetRandomPosition();

        // Modify the y value to 2.2
        spawnPosition.y = 2.2f;

        // Spawn market prefab at the modified position with a rotation of -90 degrees around the x-axis
        Quaternion rotation = Quaternion.Euler(-90f, 0f, 0f);
        GameObject market = Instantiate(marketPrefab, spawnPosition, rotation);
    }
    private void spawnMarket2()
    {
        // Calculate the position to spawn the market prefab
        Vector3 spawnPosition = GetRandomPosition();

        // Modify the y value to 2.2
        spawnPosition.y = 2.2f;

        // Spawn market prefab at the modified position with a rotation of -90 degrees around the x-axis
        Quaternion rotation = Quaternion.Euler(-90f, 0f, 0f);
        GameObject mmarket = Instantiate(marketPrefab1, spawnPosition, rotation);

    }


    // Move enemy to a specific position
    private void MoveEnemy(Vector3 position)
    {
        GameObject enemy = GameObject.FindGameObjectWithTag("Enemy");
        if (enemy != null)
        {
            enemy.transform.position = position;
        }
    }

    private void MoveEnemy2(Vector3 position)
    {
        GameObject s_enemy = GameObject.FindGameObjectWithTag("Enemy2");
        if (s_enemy != null)
        {
            s_enemy.transform.position = position;
        }
    }
    private Vector3 GetRandomPosition()
    {
        float x = Random.Range(-environmentSizeX / 2, environmentSizeX / 2);
        float z = Random.Range(-environmentSizeZ / 2, environmentSizeZ / 2);
        return new Vector3(x, 4.0f, z);
    }

    private void ClearSpawnedElements()
    {
        // Destroy all GameObjects with the "Toy", "Enemy", "Deal", "Money", and "Coin" tags

        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        foreach (GameObject enemy in enemies)
        {
            Destroy(enemy);
        }

        GameObject[] markets = GameObject.FindGameObjectsWithTag("marketplace");
        foreach (GameObject market in markets)
        {
            Destroy(market);
        }

        GameObject[] mmarkets = GameObject.FindGameObjectsWithTag("marketplace2");
        foreach (GameObject mmarket in mmarkets)
        {
            Destroy(mmarket);
        }

        GameObject[] golds = GameObject.FindGameObjectsWithTag("Money");
        foreach (GameObject gold in golds)
        {
            Destroy(gold);
        }

        GameObject[] s_enemies = GameObject.FindGameObjectsWithTag("Enemy2");
        foreach (GameObject s_enemy in s_enemies)
        {
            Destroy(s_enemy);
        }

    }
    //mlagents-learn config/SolderAgent.yaml --run-id="Run 1"
    //mlagents-learn config/SolderAgent.yaml --run-id="TRAINING" --no-graphics
    //>mlagents-learn config/SolderAgent.yaml --run-id="TRIAL2"  --resume --no-graphics
}
