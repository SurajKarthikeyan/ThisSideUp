using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movable : MonoBehaviour
{
    public static Movable selected;

    Rigidbody2D rigid;
    TargetJoint2D joint;

    [Tooltip("The distance away from this object's origin that the joint can attach")]
    [SerializeField]
    float attachDistance;
    [Tooltip("The factor of torque applied to align the package with gravity")]
    [SerializeField]
    float torqueFactor = 0.1f;
    [Tooltip("The transform this object can be dragged near")]
    [SerializeField]
    Transform dragOrigin;
    [Tooltip("The distance from the drag origin (if assigned) this object can be dragged")]
    [SerializeField]
    float dragDistance;

    float gravityScale;
    static Plane plane;
    static LayerMask movableLayers;

    private void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        joint = GetComponent<TargetJoint2D>();

        gravityScale = rigid.gravityScale;
        plane = new Plane(Vector3.forward, Vector3.zero);
        movableLayers = LayerMask.GetMask("Package");
    }

    private void FixedUpdate()
    {
        if (selected == this)
        {
            Vector2 target = CameraController.main.MouseToWorldPoint();
            
            //Keep within drag area
            if (dragOrigin != null)
            {
                Vector2 targetOffset = target - (Vector2)dragOrigin.position;
                if (targetOffset.magnitude > dragDistance)
                    targetOffset = targetOffset.normalized * dragDistance;
                target = (Vector2)dragOrigin.position + targetOffset;
            }

            joint.target = target;

            //Hang properly
            float targetAngle = Vector2.Angle(Vector2.up, joint.anchor);
            float angleDifference = -Mathf.DeltaAngle(targetAngle, rigid.rotation);
            rigid.AddTorque(angleDifference * torqueFactor * Time.fixedDeltaTime);
        }

        if (!Input.GetMouseButton(0))
        {
            rigid.gravityScale = gravityScale;
            joint.enabled = false;
            selected = null;
        }
    }

    private void OnMouseDown()
    {
        if (selected == null)
        {
            if (dragOrigin == null || ((Vector2)transform.position - (Vector2)dragOrigin.position).magnitude <= dragDistance )
            {
                selected = this;
                joint.enabled = true;
                Vector2 jointTarget = CameraController.main.MouseToWorldPoint() - rigid.position;
                if (jointTarget.magnitude > attachDistance)
                    jointTarget = jointTarget.normalized * attachDistance;
                joint.anchor = Quaternion.AngleAxis(-rigid.rotation, Vector3.forward) * jointTarget;
                rigid.gravityScale = 0;
            }
        }
    }

    private void OnMouseUp()
    {
        ReleasePackage();
        
    }

    public void SetDragOrigin(Transform t)
    {
        dragOrigin = t;
    }

    public void ReleasePackage()
    {
        if (selected == this)
        {
            selected = null;
            joint.enabled = false;
        }
    }
}
