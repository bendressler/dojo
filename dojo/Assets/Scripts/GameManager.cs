using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour {

	public float gameTime;
	public float timeScale;
	public int villageFame;
	public int maxStamina;
	public int maxWisdom;
	public int year;



		// Use this for initialization
	void Start () {
		gameTime = Time.time;
		timeScale = 1f;
		year = 0;
	}
	
	// Update is called once per frame
	void Update () {
		gameTime += Time.deltaTime * timeScale;
		year = Mathf.RoundToInt (gameTime / 20f);
	}
}
