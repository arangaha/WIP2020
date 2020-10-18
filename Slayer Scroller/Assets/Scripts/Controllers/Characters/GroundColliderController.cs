using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundColliderController : MonoBehaviour
{
    public bool grounded = false;

    void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.tag.Equals("Terrain")&&collision.gameObject.layer.Equals(LayerMask.NameToLayer("Ground")))
        {
            grounded = true;
        }
        
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.tag.Equals("Terrain") && collision.gameObject.layer.Equals(LayerMask.NameToLayer("Ground")))
        {
            grounded = false;
        }
    }
}
