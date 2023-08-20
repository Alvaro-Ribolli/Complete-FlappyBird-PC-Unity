using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PipesController : MonoBehaviour
{
    [SerializeField] private float pipesInterval = 2f;
    [SerializeField] private float pipesSpeed = 200;

    [SerializeField] private Rigidbody2D[] pipesPrefabs;

    private int currentIndex;
    private List<Rigidbody2D> pipes = new List<Rigidbody2D>();

    private GameController gameController;
    private float spawnCountdown;


	#region Unity Functions
	private void Start()
	{
        RandomPipes();
	}

	private void OnEnable()
	{
    	if(gameController == null)
        gameController = GameController.Instance;

        gameController.GameReset_Event += ResetPipes;
    }

    private void OnDisable()
	{
        gameController.GameReset_Event -= ResetPipes;
    }

	void Update()
    {
        spawnCountdown -= Time.deltaTime;

        //Only spawn the pipe if in game state and if the countdown is 0.
        if(gameController.gameState == gameStates.game && spawnCountdown <= 0)
		{
            //instantiate new pipe if there are no pipes
            if(pipes.Count <= 0)
                InstantiateNewPipe();
			else
			{
                //Reposition an existing pipe that was not on screen to the beggining of the line
				for (int i = 0; i < pipes.Count; i++)
				{
                    if (pipes[i].position.x < -3.5f)
                    {
                        pipes[i].position = new Vector2(3.5f, Random.Range(2f, -2f));
                        
                        if (!pipes[i].gameObject.activeInHierarchy)
                            pipes[i].gameObject.SetActive(true);
                        
                        break;
                    }

                    //Spawn a new pipe if necessary
					if (i == pipes.Count - 1)
					{
						InstantiateNewPipe();
						break;
					}
				}
			}

            spawnCountdown = pipesInterval;
		}
    }

	private void FixedUpdate()
	{
        //Move the pipes
		if(gameController.gameState == gameStates.game && pipes.Count > 0)
		{
			for (int i = 0; i < pipes.Count; i++)
			{
                pipes[i].MovePosition(pipes[i].position - Vector2.right * (pipesSpeed * Time.fixedDeltaTime));
			}
		}
	}
	#endregion

    //Random which pipe will be used in this run.
	private void RandomPipes()
	{
        int newIndex = Random.Range(0, pipesPrefabs.Length);

        if(newIndex != currentIndex)
		{
            currentIndex = newIndex;

			for (int i = 0; i < pipes.Count; i++)
			{
                Destroy(pipes[i].gameObject);
			}

            pipes.Clear();
		}
	}

    //Instantiate and position a new pipe
	private void InstantiateNewPipe()
	{
        Rigidbody2D newPipe = Instantiate(pipesPrefabs[currentIndex], transform);
        newPipe.position = new Vector2(3.5f, Random.Range(2f, -2f));
        pipes.Add(newPipe);
    }

    //Reset all pipes when the game returns to the menu.
    private void ResetPipes()
	{
        RandomPipes();

		for (int i = 0; i < pipes.Count; i++)
		{
            pipes[i].gameObject.SetActive(false);
		}
	}
}
