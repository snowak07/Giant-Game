using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class InteractableRoot : MonoBehaviour
{
    public bool broken { get; private set; }
    protected float uprightForce = 5000.0f;
    public float uprightDampening = 3.0f;
    protected bool applyReturnForce;

    void Start()
    {
        broken = false;
    }

    public void AddRoot(Vector3 groundOffset)
    {
        applyReturnForce = true;

        GetComponent<Rigidbody>().angularDamping = 27.0f;

        ConfigurableJoint joint = gameObject.AddComponent<ConfigurableJoint>();

        joint.xMotion = ConfigurableJointMotion.Locked;
        joint.yMotion = ConfigurableJointMotion.Locked;
        joint.zMotion = ConfigurableJointMotion.Locked;

        joint.angularXMotion = ConfigurableJointMotion.Free;
        joint.angularYMotion = ConfigurableJointMotion.Free;
        joint.angularZMotion = ConfigurableJointMotion.Locked;

        joint.anchor = groundOffset;
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
        GetComponent<Rigidbody>().angularDamping = 0.05f;
    }

    /**
     * Disabled on Select so that it doesn't interfere with PhysicsHand joint update
     */
    public void DisableReturnForce(SelectEnterEventArgs args)
    {
        applyReturnForce = false;
    }

    public void EnableReturnForce(SelectExitEventArgs args)
    {
        applyReturnForce = true;
    }
}
