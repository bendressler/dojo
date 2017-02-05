using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Character : MonoBehaviour {

	public class Attribute {
		public string name;
		public int value;
		public int preference;
		public Structure school;

		public Attribute(string name, int value, int preference, GameObject school){
			this.name = name;
			this.value = value;
			this.preference = preference;
			this.school = school.GetComponent<Structure>();
		}
	}

	public int birthYear;
	public int age;
	public int fame;
	public bool active;
	public GameManager gameManager;
	public GameObject grave;
	public GameObject graveHolder;
	public List <Attribute> skills = new List<Attribute>();

	public Structure[] preference;
	public Structure activeSchool;

	public Sprite[] sprites;

	int actionThreshold;
	float readiness;
	float activeTimer;
	float passiveTimer;
	public Attribute stamina;
	public Attribute health;
	public Attribute determination;


	// Use this for initialization
	void Start () {

		gameManager = GameObject.Find ("GameManager").GetComponent<GameManager> ();
		graveHolder = GameObject.Find ("Graveholder");
		birthYear = gameManager.year - Mathf.RoundToInt(Random.Range(10f,25f));
		active = false;
		actionThreshold = 100;

		SetAttributes ();

		preference = new Structure[gameManager.village.Length];
		for (int i = 0; i < gameManager.village.Length; i++) {
			preference [i] = gameManager.village [i];
		}
	}

	public void SetAttributes(){
		health = new Attribute ("health",Mathf.RoundToInt(Random.Range(0,10)),Mathf.RoundToInt(Random.Range(0,10)),GameObject.Find("bath"));
		stamina = new Attribute ("stamina",Mathf.RoundToInt(Random.Range(0,10)),Mathf.RoundToInt(Random.Range(0,10)),GameObject.Find("dojo"));
		determination = new Attribute ("determination",Mathf.RoundToInt(Random.Range(0,10)),Mathf.RoundToInt(Random.Range(0,10)),GameObject.Find("shrine"));

		skills.Add (health);
		skills.Add (determination);
		skills.Add (stamina);

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

				Structure nextSchool = SetPriority();

				if (readiness > actionThreshold) {
					active = true;
					Debug.Log ("I'm getting active");
					activeTimer = gameManager.gameTime + determination.value;
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
				SetSprite ();

			}

		}



	}

	public Structure SetPriority(){
		//for each attribute, get value and current structure level
		//get attribute with highest delta
		//return Structure for that delta
		Debug.Log("I'm deciding on a priority");
		float maxScore = 100;
		Structure selection;
		selection = gameManager.village[Random.Range(0,gameManager.village.Length)];
		foreach (Attribute i in skills) {
			float score;
			score = (i.school.level - i.value) + i.preference;
			if (score > maxScore) {
				maxScore = score;
				selection = i.school;
			}
		}
		Debug.Log ("My priority is " + selection);
		return selection;
	}

	void AttributeGain(float xp, Structure school){
		string attribute = "I wasted my life";

		if (school.name == "dojo") {
			attribute = "Stamina";
			stamina.value += Mathf.RoundToInt(xp);
		}
		else if (school.name == "bath") {
			health.value += Mathf.RoundToInt(xp);
			attribute = "Health";
		}
		else if (school.name == "shrine") {
			determination.value += Mathf.RoundToInt(xp);
			attribute = "Determination";
		}
		else if (school.name == "library") {
			fame = stamina.value + health.value + determination.value;
			attribute = "Fame";
		}
		Debug.Log ("I've gained " + xp + " " + attribute);
	}

	void SetSprite(){
		if (fame > 100) {
			GetComponentInChildren<SpriteRenderer> ().sprite = sprites [1];
			if (fame > 200) {
				GetComponentInChildren<SpriteRenderer> ().sprite = sprites [2];
			}
		}
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
