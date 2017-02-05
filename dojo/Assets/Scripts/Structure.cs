using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Structure : MonoBehaviour {

	public int level;
	public string type;
	public Vector3 location;
	public float attraction;
	public List <Character> tenants = new List<Character>();


	// Use this for initialization
	void Start () {
		location = new Vector3(transform.position.x,transform.position.y,(transform.position.z-1));
		attraction = 0;
	}
	
	// Update is called once per frame
	void Update () {
		GetLevel();
	
	}

	public float Attraction (GameObject character, int charLevel){
		float distance = Mathf.Abs(Vector3.Distance (location, character.transform.position));
		float attract = level / distance;
		return attract;
	}

	int GetLevel (){
		float maxFame = 0;
		foreach (Character c in tenants) {
			if (c.fame > maxFame) {
				maxFame = c.fame;
			}
		}
		return Mathf.RoundToInt (maxFame);
	}
}
