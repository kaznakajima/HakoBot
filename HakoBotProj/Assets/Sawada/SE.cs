using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SE : MonoBehaviour {
 
	// Use this for initialization
	void Start () {
        Sound.LoadSe("takuru", "attak");
        Sound.LoadSe("arm", "arm");
        Sound.LoadSe("Damag", "Damag");
        Sound.LoadSe("Ranking", "zyuni");
        Sound.LoadSe("GameStart", "Start");
        Sound.LoadSe("noizu", "noizu");
        Sound.LoadSe("shutter", "shutter");
        
        
        
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
