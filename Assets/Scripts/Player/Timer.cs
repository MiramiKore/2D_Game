using System.Collections;
using System.Collections.Generic;
using UnityEngine;

static public class Timer
{
    static public float dashTimer = 0f; //время таймера
    static public float jumpTimer = 0f;
    static public float glideTimer = 0f;


    static public void Update()
    {
        TimeRefresh();
    }
    static public void TimeRefresh()
    {
        if (dashTimer > 0f)
        {
            dashTimer -= Time.deltaTime;
        }

        if (jumpTimer > 0f)
        {
            jumpTimer -= Time.deltaTime;
        }

        if (glideTimer > 0f)
        {
            glideTimer -= Time.deltaTime;
        }
    }
}
