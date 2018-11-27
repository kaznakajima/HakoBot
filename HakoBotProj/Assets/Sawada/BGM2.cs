using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class BGM2 : MonoBehaviour {
    private AudioSource audiosource;
    public AudioClip endsound;
    // Use this for initialization
    void Start () {
      
        audiosource = GameObject.Find("Sound").GetComponent<AudioSource>();
    }
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKey(KeyCode.C))
        {
            Sound.PlaySe("SE");
        }
        if (Input.GetKey(KeyCode.A))
        {
           
            SceneManager.LoadScene("Test3");
            audiosource.clip = endsound;
            audiosource.Play();

        }
    }
}
