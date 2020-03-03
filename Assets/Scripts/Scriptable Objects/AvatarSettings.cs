using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AvatarSettings : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}


//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//[CreateAssetMenu(fileName = "Scale", menuName = "Music/MusicalScale", order = 1)]
//public class MusicalScale : ScriptableObject
//{
//    public int[] Notes;

//    public void Play(AudioSource Audio, NoteSet SoundSet, int ScaleIndex, int Offset, float Volume)
//    {
//        ScaleIndex = ScaleIndex % Notes.Length;
//        int clipIndex = Notes[ScaleIndex] + Offset;
//        var clip = SoundSet.Sounds[clipIndex];
//        Audio.PlayOneShot(clip, Volume);
//    }
//}
