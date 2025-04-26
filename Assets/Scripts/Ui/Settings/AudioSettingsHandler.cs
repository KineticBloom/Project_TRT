using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioSettingsHandler : MonoBehaviour
{
    public List<AK.Wwise.RTPC> busses;

    public  void InitFromPrefs()
    {
        foreach(var bus in busses)
        {
            string busName = bus.ToString();

            if (PlayerPrefs.HasKey(busName))
            {
                float volume = PlayerPrefs.GetFloat(busName);
                bus.SetGlobalValue(volume);
            }
        }
    }
}
