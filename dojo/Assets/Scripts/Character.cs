using UnityEngine;
using System.Collections;

public class Character : MonoBehaviour {



	public int birthYear;
	public int age;
	public int fame;
	public bool active;
	public GameManager gameManager;
	public GameObject grave;
	public GameObject graveHolder;

	Structure[] preference;
	Structure activeSchool;

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
		graveHolder = GameObject.Find ("Graveholder");
		birthYear = gameManager.year - Mathf.RoundToInt(Random.Range(10f,25f));
		active = false;
		actionThreshold = 100;
		determination = 30;
		preference = new Structure[gameManager.village.Length];
		for (int i = 0; i < gameManager.village.Length; i++) {
			preference [i] = gameManager.village [i];
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
				GetComponent<NavMeshAgent> ().speed = (1.5f * gameManager.timeScale);
				GetComponent<NavMeshAgent> ().acceleration = (10 * gameManager.timeScale);

				Debug.Log ("I'm resting");
				passiveTimer += 10f;

				float priority = 0;
				Structure nextSchool = preference [0];
				foreach (Structure i in preference) {
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
					GetComponent<NavMeshAgent> ().speed = (3.5f * gameManager.timeScale);
					GetComponent<NavMeshAgent> ().acceleration = (10 * gameManager.timeScale);
					activeSchool.tenants.Add (this);
				}
			}
		}

		//active behaviour
		if (active) {

			float xpGain = 0;

			if (activeSchool != null && (Vector3.Distance (transform.position, activeSchool.location) < 1)) {
				xpGain += 0.1f * gameManager.timeScale * activeSchool.level;
				readiness -= 0.5f * gameManager.timeScale;
			}

			if (activeTimer <= gameManager.gameTime) {
				Debug.Log ("I need to rest");
				AttributeGain (xpGain, activeSchool);
				xpGain = 0;
				active = false;
				passiveTimer = gameManager.gameTime + 1;
				activeSchool.tenants.Remove (this);

			}

		}



	}


	void AttributeGain(float xp, Structure school){
		string attribute = "I wasted my life";

		if (school.name == "dojo") {
			attribute = "Stamina";
			stamina += Mathf.RoundToInt(xp);
		}
		else if (school.name == "bath") {
			health += Mathf.RoundToInt(xp);
			attribute = "Health";
		}
		else if (school.name == "shrine") {
			determination += Mathf.RoundToInt(xp);
			attribute = "Determination";
		}
		else if (school.name == "library") {
			fame += Mathf.RoundToInt(xp);
			attribute = "Fame";
		}
		Debug.Log ("I've gained " + xp + " " + attribute);
	}


	void LateUpdate(){
		transform.eulerAngles = new Vector3(0,0,0);
		if (age > 65) {
			Debug.Log ("My death contributes " + fame + " fame to the village!");
			gameManager.villageFame += fame;
			GameObject instance = Instantiate (grave, gameObject.transform.position, Quaternion.identity) as GameObject;
			instance.transform.SetParent (graveHolder.transform);
			Destroy (gameObject);
		}
	}

}
