using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class zBGM : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame

    void Update()
    {
        //bool jDown = Input.GetButtonDown("Jump");
        //Debug.Log(jDown);
        //if (jDown) play_stop_music();
    }

    public AudioSource audioSource;



    public void play_stop_music()
    {
        if (!audioSource.isPlaying)
        {
            audioSource.Play();
        }
        else
        {
            audioSource.Stop();
        }
    }

    
    public void pause_music()
    {
        if (audioSource.isPlaying)
        {
            audioSource.Pause();
        }
    }
    //∏ƒ±‰“Ù¡ø
    public void change_volume(float volume)
    {
        audioSource.volume = volume;
    }

}
