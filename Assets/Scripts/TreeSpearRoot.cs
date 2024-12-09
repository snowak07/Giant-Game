using UnityEngine;

public class TreeSpearRoot : MonoBehaviour
{
    public bool broken { get; private set; }
    //protected Vector3 groundOffset = new Vector3(0, 2.5f, 0);
    protected Vector3 groundOffset = new Vector3(0, 0, 2.5f);
    protected float uprightForce = 100.0f;
    protected float uprightDampening = 2.0f;

    void Start()
    {
        broken = false;
    }

    //public void AddRoot()
    //{
    //    Debug.Log("AddRoot");
    //    //SpringJoint joint = gameObject.AddComponent<SpringJoint>();
    //    FixedJoint joint = gameObject.AddComponent<FixedJoint>();
    //    joint.anchor = -groundOffset;
    //    //joint.spring = 100.0f;
    //    //joint.damper = 1000.0f;
    //    //joint.minDistance = 0;
    //    //joint.maxDistance = 0.1f;
    //    joint.breakForce = 5000.0f;
    //    joint.breakTorque = 5000.0f;
    //}

    public void AddRoot()
    {
        Debug.Log("AddRoot");
        //SpringJoint joint = gameObject.AddComponent<SpringJoint>();
        HingeJoint joint = gameObject.AddComponent<HingeJoint>();
        joint.anchor = -groundOffset;
        //joint.axis = 
        //joint.spring = 100.0f;
        //joint.damper = 1000.0f;
        //joint.minDistance = 0;
        //joint.maxDistance = 0.1f;
        joint.breakForce = 5000.0f;
        joint.breakTorque = 5000.0f;
    }

    protected void FixedUpdate()
    {
        //if (TryGetComponent(out SpringJoint joint))
        //if (TryGetComponent(out FixedJoint joint))
        //{
        //    Debug.Log("Found FixedJoint");
        //    Rigidbody treeSpearBody = GetComponent<Rigidbody>();
        //    Vector3 uprightTorque = Vector3.Cross(transform.forward, Vector3.up) * uprightForce;
        //    treeSpearBody.AddTorque(uprightTorque - treeSpearBody.angularVelocity * uprightDampening);
        //}
    }

    protected void OnJointBreak(float breakForce)
    {
        Debug.Log("Joint Broken");
        broken = true;
    }

    public void SetAxis(Vector3 newAxis)
    {
        if (!broken)
        {
            Debug.Log("Setting new axis: " + newAxis);
            GetComponent<HingeJoint>().axis = newAxis;
        }
    }
}
