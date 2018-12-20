using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleCollisionController : MonoBehaviour
{

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void OnParticleCollision(GameObject other)
    {
        if (other.tag == "Character")
        {
            var character = other.GetComponent(typeof(Character)) as Character;
            character.Stan();
        }
    }
}
