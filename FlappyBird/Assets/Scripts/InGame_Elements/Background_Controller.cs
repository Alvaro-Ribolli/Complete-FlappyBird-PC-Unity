using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Background_Controller : MonoBehaviour
{
	[SerializeField] private float speed;
    [SerializeField] private List<SpriteRenderer> backgroungsPrefabs;

    private List<Rigidbody2D> backgroundsInstances = new List<Rigidbody2D>();
	private int bgIndex = -1;

	private float bgXsize;
	private float bgYPos;
	private GameController gameController;

	#region Unity Functions
	private void Start()
	{
		RandomBG();
		gameController = GameController.Instance;
	}

	private void OnEnable()
	{
		if(gameController == null)
			gameController = GameController.Instance;

		gameController.GameReset_Event += RandomBG;
	}

	private void OnDisable()
	{
		gameController.GameReset_Event -= RandomBG;
	}

	private void FixedUpdate()
	{
		//Only rolls the bg if the game is not in the score state
		if (gameController.gameState != gameStates.score)
		{
			Vector2 nextPos;

			for (int i = 0; i < backgroundsInstances.Count; i++)
			{
				//calculate the next background positions according the speed
				nextPos = backgroundsInstances[i].position - Vector2.right * (speed * Time.fixedDeltaTime);
				backgroundsInstances[i].MovePosition(nextPos);

				//If background is out of the screen, than reposition it to behing the next bg.
				if (nextPos.x <= -bgXsize)
				{
					int nextBG = (int)Mathf.Repeat(i + 1, backgroundsInstances.Count);
					backgroundsInstances[i].MovePosition(new Vector2(backgroundsInstances[nextBG].position.x + bgXsize, bgYPos) - Vector2.right * (speed * Time.fixedDeltaTime));
				}
			}
		}
	}
	#endregion

	//Random which background will be used in this run.
	private void RandomBG()
	{
		int newIndex = Random.Range(0, backgroungsPrefabs.Count);

		if (newIndex != bgIndex)
		{
			bgIndex = newIndex;
			bgXsize = backgroungsPrefabs[bgIndex].bounds.size.x;
			bgYPos = backgroungsPrefabs[bgIndex].transform.position.y;

			for (int i = 0; i < backgroundsInstances.Count; i++)
			{
				Destroy(backgroundsInstances[i].gameObject);
			}

			backgroundsInstances.Clear();

			for (int i = 0; i < 2; i++)
			{
				Rigidbody2D newBG = Instantiate(backgroungsPrefabs[bgIndex], transform).GetComponent<Rigidbody2D>();
				newBG.transform.position = new Vector2(bgXsize * i - (speed * Time.fixedDeltaTime), bgYPos);
				backgroundsInstances.Add(newBG);
			}
		}
	}
}
