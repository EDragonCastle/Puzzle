using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestSpine : MonoBehaviour
{
    public bool isMatch;
    public int count;

    // Update is called once per frame
    void Update()
    {
        // Match ����
        if (Input.GetKeyDown(KeyCode.F))
        {
            isMatch = true;
            count++;
            // Animation ����
        }
        
        // Match ����
        if (Input.GetKeyDown(KeyCode.G))
        {
            isMatch = false;
            count = 0;
            // Animation ����
        }
    }
}
