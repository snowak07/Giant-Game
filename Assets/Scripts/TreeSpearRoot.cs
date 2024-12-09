using UnityEngine;

public class TreeSpearRoot : MonoBehaviour
{
    public bool broken { get; private set; }
    protected Vector3 groundOffset = new Vector3(0, 0, 2.5f);
    protected float uprightForce = 100.0f;
    protected float uprightDampening = 2.0f;

    void Start()
    {
        broken = false;
    }

    public void AddRoot()
    {
        ConfigurableJoint joint = gameObject.AddComponent<ConfigurableJoint>();

        joint.xMotion = ConfigurableJointMotion.Locked;
        joint.yMotion = ConfigurableJointMotion.Locked;
        joint.zMotion = ConfigurableJointMotion.Locked;

        joint.angularXMotion = ConfigurableJointMotion.Free;
        joint.angularYMotion = ConfigurableJointMotion.Free;
        joint.angularZMotion = ConfigurableJointMotion.Free;

        joint.anchor = -groundOffset;
        joint.breakForce = 5000.0f;
        joint.breakTorque = 5000.0f;
    }

    protected void FixedUpdate()
    {
        //if (TryGetComponent(out ConfigurableJoint joint))
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
}
