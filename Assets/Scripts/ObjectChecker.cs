using System.Collections;
using System.Collections.Generic;
using UnityEngine;

static public class ObjectChecker
{
    static public bool isGround;                  // проверка земли
    static public bool isWallLeft;                // проверка сетны слева
    static public bool isWallRight;               // проверка стены справа
    static public float rayDistanceGround = 1.5f; // расстояние до земли
    static public float rayDistanceWall = 4f;     // расстояние до стены
    static public void Checker()
    {
        RaycastHit2D hit_ground = Physics2D.Raycast(CharacterMovement.rb.position, Vector2.down, rayDistanceGround, LayerMask.GetMask("Ground"));
        RaycastHit2D hit_wall_left = Physics2D.Raycast(CharacterMovement.rb.position, Vector2.left, rayDistanceWall, LayerMask.GetMask("Wall"));
        RaycastHit2D hit_wall_right = Physics2D.Raycast(CharacterMovement.rb.position, Vector2.right, rayDistanceWall, LayerMask.GetMask("Wall"));
       
        //Проверка наличия земли
        if (hit_ground.collider != null)
        {
            isGround = true;
        }
        else
        {
            isGround = false;
        }
        //Проверка наличия стены слева
        if (hit_wall_left.collider != null)
        {
            isWallLeft = true;
        }
        else
        {
            isWallLeft = false;
        }
        //Проверка наличия стены справа
        if (hit_wall_right.collider != null)
        {
            isWallRight = true;
        }
        else
        {
            isWallRight = false;
        }
    }
}
