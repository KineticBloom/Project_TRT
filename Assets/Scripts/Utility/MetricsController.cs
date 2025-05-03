using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MetricsController : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Metrics.StartSession();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnApplicationQuit()
    {
        Metrics.EndSession();
    }
}
