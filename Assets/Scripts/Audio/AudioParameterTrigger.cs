using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioParameterTrigger : MonoBehaviour
{
    [SerializeField]
    private AK.Wwise.RTPC parameter;

    [SerializeField]
    private int parameterValue; // I only have 1 use case that requires an int. Maybe expand this to accept multiple types?

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (parameter != null)  parameter.SetGlobalValue(parameterValue);
        }
    }
}
