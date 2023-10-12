using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Throwable : MonoBehaviour
{
    GameObject heldBy;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Holder hold = collision.gameObject.GetComponent<Holder>();
        if (hold!=null && hold.holding == false)
        {
            hold.holding = this;
            heldBy = collision.gameObject;
            transform.parent = hold.holdPosition;
            transform.localPosition = Vector3.zero;
        }
    }
}
