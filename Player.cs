using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;

public class Player : MonoBehaviour
{
    public AudioSource AS;
    public AudioClip[] AC;
    bool ispaused, muted,next,isloop;
    int selectedSong,current = 0;
    public GameObject Mute, unmute, play, pause,loop, noLoop;
    public Text text;//,list;
    public GameObject image;
    public Slider Volume, MusicController;
    float originalVol;
    DirectoryInfo path;
    public static Player Instance;
    List<string> Files = new List<string>();
    string myPath;
    public GameObject[] listings;
    WWW myClip;
    public GameObject listingPrefab;
    public Transform listingsParent;
    private int index;
    bool deleted;
    int playbackProgress ;
    public Text minutes,second;
   

    void Start()
    {
        

        Mute.SetActive(false);
        pause.SetActive(false);
        loop.SetActive(false);
        ispaused = false;
        muted = false;
        next = false;
        isloop = false;
        deleted = false;
        playbackProgress = 0;
        //        text.text = AC[current].name;

        Volume.value = Volume.maxValue;
        StartCoroutine(UpdateMusicLibrary());
        
        AS.clip = AC[current];
  

    }



    void Awake()
    {
        Instance = this;
        AS = GetComponent<AudioSource>();
#if UNITY_ANDROID
		myPath = "/storage/emulated/0/Music";
#endif
#if UNITY_STANDALONE
        myPath = "C:/Users/Blue Falcon/Music/Hoja";
#endif
#if Unity_Editor
		myPath = "Builds/Music";
#endif
        path = new DirectoryInfo(myPath);

    }

    
    public void Play()
    {
        MusicController.value = 0;
        if (next)
        {
            current++;
            if (current == AC.Length)
            {
                current = 0;
            }
           
            next = false;
            
        }
        if (ispaused)
        {
            AS.timeSamples = playbackProgress;
            AS.Play();
            
        }
        if (ispaused == false)
        {
            AS.clip = AC[current];
            AS.Play();
        }

//        text.text = AC[current].name;
        ispaused = false;
        play.SetActive(false);
        pause.SetActive(true);
           
    }
    public void LoopBtn()
    {

        AS.loop = false;

        loop.SetActive(false);
        noLoop.SetActive(true);
        isloop = false;
        
    }

    public void NoloopBtn()
    {

        AS.loop = true;
        loop.SetActive(true);
        noLoop.SetActive(false);
        isloop = true;
        if (isloop)
        {

            AS.clip = AC[current];
            AS.Play();

        }
    }
    public void Pause()
    {

        if (ispaused == false)
        {
            AS.Pause();
            ispaused = true;
            pause.SetActive(false);
            play.SetActive(true);
            playbackProgress = AS.timeSamples;
        }

    }

    public void playnext()
    {
        AS.Stop();
        MusicController.value = 0;
        current++;
        if (current == AC.Length)
        {
            current = 0;

        }

        AS.clip = AC[current];
        AS.Play();

    }

    public void playprev()
    {
        AS.Stop();
        MusicController.value = 0;
        current--;
        if (current == -1)
        {
            current = AC.Length - 1;

        }

        AS.clip = AC[current] ;
        AS.Play();
        //text.text = AC[current].name;

    }

    public void muteUnmute()
    {

        if (muted == false)
        {
            AS.mute = !AS.mute;
            muted = true;
            unmute.gameObject.SetActive(false);
            Mute.gameObject.SetActive(true);
            AS.volume = 0;
            Volume.value = 0;
            if (Volume.value > 0)
            {
                AS.volume = Volume.value;
                AS.mute = !AS.mute;
            }

        }
        else
        {
            
            Volume.value = Volume.maxValue;
            AS.volume = Volume.value;

            muted = false;
            unmute.gameObject.SetActive(true);
            Mute.gameObject.SetActive(false);
            
        }

    }


    public void moveMusicSlider()
    {
        AS.time = MusicController.value;
    }

    public void deleteMusic()
    {
        List<AudioClip> delete = new List<AudioClip>();
        List<GameObject> Prefebdelete = new List<GameObject>();

        for (int i = selectedSong ; i < AC.Length; i++)
        {

            delete.Add(AC[i]);
           Prefebdelete.Add(listings[i]);
        }
        delete.RemoveAt(selectedSong);
        Prefebdelete.RemoveAt(selectedSong);

         AC = new AudioClip[AC.Length - 1];
         listings = new GameObject[listings.Length - 1];
       
        for (int i = selectedSong; i < AC.Length; i++)
        {

            AC[i] = delete[i];
            listings[i] = Prefebdelete[i];
        }
    
    }
    public void addMusic()
    {
        
    }

    private IEnumerator UpdateMusicLibrary()
    {
        int length = path.GetFiles().Length;
        
        AC = new AudioClip[length];
        listings = new GameObject[length];
        

        for (int i = 0; i < length; i++)
        {
            
            if (!(path.GetFiles()[i].FullName.Contains("wav") || path.GetFiles()[i].FullName.Contains("ogg")))
                continue;

#if UNITY_STANDALONE
            myClip = new WWW("file:///" + path.GetFiles()[i].FullName);
#endif
#if Unity_Android
			myClip = cashWWWInstance.GetCachedWWW("file:///" + path.GetFiles()[i].FullName);
#endif
            GameObject obj = Instantiate(listingPrefab);
            obj.transform.parent = listingsParent;

            obj.GetComponentInChildren<TextMeshProUGUI>().SetText( path.GetFiles()[i].Name);
            obj.name = path.GetFiles()[i].Name;
            obj.GetComponent<RectTransform>().localScale = Vector3.one;

            listings[i] = obj;
            AC[i] = myClip.GetAudioClip(false, false);
            AC[i].name = path.GetFiles()[i].Name;
            while (AC[i].loadState != AudioDataLoadState.Loaded)
            {
                yield return null;
            }   
        }      
    }
    
    /*private void GetSongsFromFolder()
    {
        DirectoryInfo directoryInfo = new DirectoryInfo(Application.streamingAssetsPath);
        FileInfo[] songFiles = directoryInfo.GetFiles("*.*");

        foreach (FileInfo songFile in songFiles)
        {
            StartCoroutine(ConvertFilesToAudioClip(songFile));
        
        }

    }

    private IEnumerator ConvertFilesToAudioClip(FileInfo songFile)
    {
        if (songFile.Name.Contains("mp3") || songFile.Name.Contains("wav") || songFile.Name.Contains("ogg"))
        {

            string songName = songFile.FullName.ToString();
            string url = string.Format("file://{0}", songName);
            WWW www = new WWW(url);
            yield return www;
            AC.Add(www.GetAudioClip(false, false));

        }
        else
        {
            yield break;}
    }
    */
    /*void displayList()
    {
        list.text = "";

        for (int i = 0; i < c; i++)
        {
            list.text += i + 1 + "__ " + AC[i].name + "\n";
        }

    }*/

    void volumeControl()
    {
        
        if (Volume.value == 0)
        {
            Mute.SetActive(true);
            unmute.SetActive(false);
        }
        if (Volume.value > 0)
        {
            Mute.SetActive(false);
            unmute.SetActive(true);
        }
        AS.volume = Volume.value;

    }
    
    void musicControlSlider()
    {
        MusicController.value = AS.time;

    }
    void autoNext()
    {       
            next = true;
            Play();
    }

    public void ChoosenAudioPlay(string name)
    {
        string str;
        

        for (int i = 0; i < AC.Length; i++)
        {
            str = AC[i].name;
            if (str != null) { 
            if (name.Equals(str)) 
            {
                
                print(AC[i].name);
                current = selectedSong =  i;
                                
                Play();

            }
            }

        }
        
    }

    public void OnClickQuitButton()
    {
        Time.timeScale = 1;
        Application.Quit();
    }

    void calculateTime()
    {
        string min = Mathf.Floor((int)AS.time / 60).ToString("00");
        string sec = ((int)AS.time % 60).ToString("00");
        minutes.text = min;
        second.text = sec;

    }

    void Update()
    {
        // AS.clip = AC[current];
        //  Color newcolor = new Color(Random.value, Random.value, Random.value);
        //image.GetComponent<Image>().color = Color.Lerp(image.GetComponent<Image>().color, newcolor, Time.deltaTime );
       

        volumeControl();

        MusicController.maxValue = AC[current].length;
        if (AS.isPlaying)
        {
            text.text = AC[current].name;
            musicControlSlider();
            calculateTime();
        }

        if (!AS.isPlaying && AS.time == AC[current].length)
            autoNext();


       
    }

}