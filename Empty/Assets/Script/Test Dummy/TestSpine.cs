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
        // Match 성공
        if (Input.GetKeyDown(KeyCode.F))
        {
            isMatch = true;
            count++;
            // Animation 진행
        }
        
        // Match 실패
        if (Input.GetKeyDown(KeyCode.G))
        {
            isMatch = false;
            count = 0;
            // Animation 진행
        }
    }
}
