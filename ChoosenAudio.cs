using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChoosenAudio : MonoBehaviour {

    
    public void choose()
    {
        
        Player.Instance.ChoosenAudioPlay(transform.parent.name);
        
    }
	
	
}
