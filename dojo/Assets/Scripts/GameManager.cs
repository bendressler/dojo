using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour {

	public float gameTime;
	public float timeScale;
	public float villageFame;
	public int maxStamina;
	public int maxWisdom;
	public int year;
	public int noviceTimer;

	public GameObject samurai;
	public GameObject samuraiHolder;
	public Structure[] village;



		// Use this for initialization
	void Start () {
		gameTime = Time.time;
		timeScale = 1f;
		year = 0;
		noviceTimer = Mathf.RoundToInt(gameTime + 2);
	}
	
	// Update is called once per frame
	void Update () {
		gameTime += Time.deltaTime * timeScale;
		year = Mathf.RoundToInt (gameTime / 10f);

		if (noviceTimer <= gameTime) {
			GameObject noviceInstance = Instantiate (samurai, new Vector3 (0, 0, -12), Quaternion.identity) as GameObject;
			noviceInstance.transform.SetParent (samuraiHolder.transform);
			noviceTimer = Mathf.RoundToInt(gameTime + Random.Range (5, 15));
		}

	}
}
