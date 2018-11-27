using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BGM : MonoBehaviour {
    private AudioSource audiosource;
    public AudioClip batrusound;
    // Use this for initialization
    void Start () {
        Sound.LoadBgm("bgm", "BGM");
        Sound.PlayBGM("bgm");
        Sound.LoadSe("SE","SE1");
        Sound.LoadSe("Dmg1", "SE2");
        Sound.LoadSe("Dmg2", "SE3");
        Sound.LoadSe("Dmg3", "SE4");
        audiosource = GameObject.Find("Sound").GetComponent<AudioSource>();

    }
	
	// Update is called once per frame
	void Update () {

        if (Input.GetKey(KeyCode.A))
        {
            audiosource.clip = batrusound;
            audiosource.Play();
            SceneManager.LoadScene("Test2");
          

        }
        if (Input.GetKey(KeyCode.C))
        {
            Sound.PlaySe("SE",0);
        }
        if (Input.GetKey(KeyCode.X))
        {
            Sound.PlaySe("Dmg1",1);
            
        }
        }
}
