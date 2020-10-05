using UnityEngine;
using System.Collections;

public class soundManager : MonoBehaviour
{
    public AudioSource efxSource;                   //Drag a reference to the audio source which will play the sound effects.
    public AudioSource natureSound; //настоящие звуки ДЕНЬ
    //public AudioSource musicSource;                 //Drag a reference to the audio source which will play the music.
    public AudioSource seaChanel; //повторяющиеся звуки типа воды огня итпы
    public AudioSource fireChanel; //повторяющиеся звуки типа воды огня итпы
    public AudioSource forestChanel; //повторяющиеся звуки типа воды огня итпы
    public AudioSource rainChanel; //повторяющиеся звуки типа воды огня итпы
    public AudioSource nightChanel; //настоящие звуки НОЧЬ

    public static soundManager instance = null;     //Allows other scripts to call functions from SoundManager.             
    public float lowPitchRange = .95f;              //The lowest a sound effect will be randomly pitched.
    public float highPitchRange = 1.05f;            //The highest a sound effect will be randomly pitched.

    public AudioClip getFilm;
    public AudioClip makePhoto;
    public AudioClip emptyPhoto;
    public AudioClip noMoney;
    public AudioClip policeMoney;
    public AudioClip smallWin;
    public AudioClip bigWin;
    public AudioClip door;
    public AudioClip sea;
    public AudioClip step;
    public AudioClip fire;
    public AudioClip ufoin;
    public AudioClip ufocatch;
    public AudioClip delfin;
    public AudioClip shipin;
    public AudioClip flash;
    public AudioClip newlevel;
    public AudioClip night;

    void Awake()
    {
        //Check if there is already an instance of SoundManager
        if (instance == null)
            //if not, set it to this.
            instance = this;
        //If instance already exists:
        else if (instance != this)
            //Destroy this, this enforces our singleton pattern so there can only be one instance of SoundManager.
            Destroy(gameObject);

        //Set SoundManager to DontDestroyOnLoad so that it won't be destroyed when reloading our scene.
        //DontDestroyOnLoad(gameObject);
    }


    //Used to play single sound clips.
    public void PlaySingle(string clip, string channelType = "fx", float volume = 100)
    {
        AudioClip tmp=null;
        switch (clip)
        {
            case "getFilm": tmp = getFilm; break;
            case "makePhoto": tmp = makePhoto; break;
            case "emptyPhoto": tmp = emptyPhoto; break;
            case "noMoney": tmp = noMoney; break;
            case "policeMoney": tmp = policeMoney; break;
            case "smallWin": tmp = smallWin; break;
            case "bigWin": tmp = bigWin; break;
            case "door": tmp = door; break;
            case "step": tmp = step; break;
            case "ufoin": tmp = ufoin; break;
            case "ufocatch": tmp = ufocatch; break;
            case "delfin": tmp = delfin; break;
            case "shipin": tmp = shipin; break;
            case "flash": tmp = flash; break;
            case "newlevel": tmp = newlevel; break;
        }

        
        switch (channelType)
        {
            case "fx": efxSource.clip = tmp; efxSource.Play(); efxSource.volume=volume / 100;  break;
         //   case "music": musicSource.Play(); musicSource.volume=volume / 100; print("music"); break;
            case "nature": natureSound.clip = tmp; natureSound.Play(); natureSound.volume=volume / 100; break;
            case "seaChanel": seaChanel.volume = volume / 100; break;
            case "fireChanel": fireChanel.volume = volume / 100; break;
            case "forestChanel": forestChanel.volume = volume / 100; break;
            case "rainChanel": rainChanel.volume = volume / 100; break;
            case "nightChanel": nightChanel.volume = volume / 100; break;
        }

       /* //Set the clip of our efxSource audio source to the clip passed in as a parameter.
        efxSource.clip = tmp;
        //Play the clip.
        efxSource.Play();*/
    }

    public void mute(bool trigger=true,bool todo=false)
    {
       // print("mute");
        if(panels.instance.isSound)
        if (!todo)
        {
            AudioListener.pause = false;
            AudioListener.volume = 0;
        }
        else
        {
            AudioListener.volume = 1;
        }
        /*
        if (trigger)
        {
            if (AudioListener.pause == false)
            {
               // AudioListener.pause = true;
                AudioListener.volume = 0;
            }
            else
            {
                //AudioListener.pause = false;
                AudioListener.volume = 1;
            }
        }
        else
        {
            if (todo == false)
            {
                //AudioListener.pause = true;
                AudioListener.volume = 0;
            }
            else
            {
                //AudioListener.pause = false;
                AudioListener.volume = 1;
            }
        }*/
    }

    //RandomizeSfx chooses randomly between various audio clips and slightly changes their pitch.
    public void RandomizeSfx(params AudioClip[] clips)
    {
        //Generate a random number between 0 and the length of our array of clips passed in.
        int randomIndex = Random.Range(0, clips.Length);

        //Choose a random pitch to play back our clip at between our high and low pitch ranges.
        float randomPitch = Random.Range(lowPitchRange, highPitchRange);

        //Set the pitch of the audio source to the randomly chosen pitch.
        efxSource.pitch = randomPitch;

        //Set the clip to the clip at our randomly chosen index.
        efxSource.clip = clips[randomIndex];

        //Play the clip.
        efxSource.Play();
    }
}