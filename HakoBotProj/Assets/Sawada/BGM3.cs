using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class BGM3 : MonoBehaviour {
    private AudioSource audiosource;
    public AudioClip TaitolSound;
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
            SceneManager.LoadScene("Test");
            audiosource.clip =TaitolSound;
            audiosource.Play();

        }
    }
}
