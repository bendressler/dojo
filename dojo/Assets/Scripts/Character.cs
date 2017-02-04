using UnityEngine;
using System.Collections;

public class Character : MonoBehaviour {

	private class School {

		public Vector3 location;
		public int currentLevel;
		public float attraction;

		public School (GameObject building){
			location = new Vector3(building.transform.position.x,building.transform.position.y,(building.transform.position.z-1));
			currentLevel = building.GetComponentInChildren<Structure> ().level;
			attraction = 0;
		}

		public float Attraction (GameObject character, int charLevel){
			float distance = Mathf.Abs(Vector3.Distance (location, character.transform.position));
			int levelDelta = currentLevel - charLevel;
			float attract = levelDelta / distance;
			return attract;
		}

	}

	public int birthYear;
	public int age;
	public int fame;
	public bool active;
	public GameObject[] village;
	public GameManager gameManager;

	School[] preference;
	School activeSchool;

	int actionThreshold;
	int determination;
	float readiness;
	int yearning;
	int health;
	int stamina;
	int altriusm;
	float activeTimer;
	float passiveTimer;


	// Use this for initialization
	void Start () {
		gameManager = GameObject.Find ("GameManager").GetComponent<GameManager> ();
		birthYear = gameManager.year - Mathf.RoundToInt(Random.Range(10f,25f));
		active = true;
		actionThreshold = 100;
		determination = 30;
		preference = new School[village.Length];
		for (int i = 0; i < village.Length; i++) {
			preference [i] = new School(village [i]);
		}
	}
	
	// Update is called once per frame
	void Update () {
		age = gameManager.year - birthYear;
		//Debug.Log ("It's the year " + gameManager.year + " and I'm " + age + " years old.");


		//passive behaviour
		if (!active) {
			readiness += 0.5f;

			if (passiveTimer <= gameManager.gameTime) {

				Vector3 strollDest;
				strollDest = new Vector3(transform.position.x + (Random.Range (-2f, 2f)), 0.15f, transform.position.z + (Random.Range (-2f, 2f)));

				GetComponent<NavMeshAgent> ().destination = strollDest;
				GetComponent<NavMeshAgent> ().speed = 1.5f;

				Debug.Log ("I'm resting");
				passiveTimer += 10f;

				float priority = 0;
				School nextSchool = preference [0];
				foreach (School i in preference) {
					i.attraction = i.Attraction (gameObject, 0);
					if (i.attraction > priority) {
						priority = i.attraction;
						nextSchool = i;
					}
				}
				if (priority * readiness > actionThreshold) {
					active = true;
					Debug.Log ("I'm getting active");
					activeTimer = gameManager.gameTime + determination;
					activeSchool = nextSchool;
					GetComponent<NavMeshAgent> ().destination = activeSchool.location;
					GetComponent<NavMeshAgent> ().speed = 3.5f;
				}
			}
		}

		//active behaviour
		if (active) {

			float fameGain = 0;

			if (activeSchool != null && (Vector3.Distance (transform.position, activeSchool.location) < 1)) {
				fameGain += 0.1f;
				readiness -= 0.5f;
			}

			if (activeTimer <= gameManager.gameTime) {
				Debug.Log ("I need to rest");
				Debug.Log ("I've gained " + fameGain + " experience");
				fame += Mathf.RoundToInt(fameGain);
				fameGain = 0;
				active = false;
				passiveTimer = gameManager.gameTime + 1;
			}

		}



	}

	void LateUpdate(){
		transform.eulerAngles = new Vector3(0,0,0);
	}

}
