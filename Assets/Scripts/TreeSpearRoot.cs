using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class TreeSpearRoot : MonoBehaviour
{
    public bool broken { get; private set; }
    protected Vector3 groundOffset = new Vector3(0, 0, 2.5f);
    protected float uprightForce = 1000.0f;
    public float uprightDampening = 2.0f; // FIXME: Adjust so it doesn't vibrate when stationary
    protected bool applyReturnForce;

    void Start()
    {
        broken = false;
    }

    public void AddRoot()
    {
        applyReturnForce = true;

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
        if (applyReturnForce && !broken && TryGetComponent(out ConfigurableJoint joint) && TryGetComponent(out Rigidbody treeSpearBody))
        {
            Vector3 uprightTorque = Vector3.Cross(transform.forward, Vector3.up) * uprightForce;
            treeSpearBody.AddTorque(uprightTorque - treeSpearBody.angularVelocity * uprightDampening);
        }
    }

    protected void OnJointBreak(float breakForce)
    {
        broken = true;
    }

    public void DisableReturnForce(SelectEnterEventArgs args)
    {
        // Disabled on Select so that it doesn't interfere with PhysicsHand joint update
        applyReturnForce = false;
    }

    public void EnableReturnForce(SelectExitEventArgs args)
    {
        applyReturnForce = true;
    }
}
