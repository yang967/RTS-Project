using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class test : MonoBehaviour
{
    int integrationValue;

    // Start is called before the first frame update
    void Start()
    {
        integrationValue = 0;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void setIntegrationValue(int i)
    {
        integrationValue = i;
    }
}
