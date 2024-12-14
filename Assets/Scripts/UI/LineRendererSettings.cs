using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LineRendererSettings : MonoBehaviour
{
    [SerializeField] LineRenderer rend;

    public GameObject panel;
    public Image img;
    public Button btn;
    public LayerMask layerMask;

    public NewActionBasedXRController xrController;

    private Vector3[] points;

    // Start is called before the first frame update
    void Start()
    {
        rend = gameObject.GetComponent<LineRenderer>();
        img = panel.GetComponent<Image>();

        // Initialize the LineRenderer
        points = new Vector3[2];

        // Set the start point of the LineRenderer to the position of the gameObject
        points[0] = Vector3.zero;

        // Set the end point 20 units away from the gameObject on the Z axis (pointing forward)
        points[1] = transform.position + new Vector3(0, 0, 20);

        // Finally set the positions array on the LineRenderer to our new values
        rend.SetPositions(points);
        rend.enabled = true;
    }

    private void Update()
    {
        AlignLineRenderer(rend);

        if (AlignLineRenderer(rend) && xrController.PickupEnabled()) // TODO change to a more general var name
        {
            btn.onClick.Invoke();
        }
    }

    public bool AlignLineRenderer(LineRenderer rend)
    {
        bool hitBtn = false;

        Ray ray;
        ray = new Ray(transform.position, transform.forward);
        RaycastHit hit;

        points[0] = transform.parent.position; // TEST

        if (Physics.Raycast(ray, out hit, layerMask))
        {
            btn = hit.collider.gameObject.GetComponent<Button>();

            points[1] = transform.parent.forward + new Vector3(0, 0, hit.distance); // transform.forward
            rend.startColor = Color.red;
            rend.endColor = Color.red;

            hitBtn = true;
        }
        else
        {
            points[1] = transform.forward + new Vector3(0, 0, 20);
            rend.startColor = Color.red;
            rend.endColor = Color.red;

            hitBtn = false;
        }

        rend.SetPositions(points);
        rend.material.color = rend.startColor;
        return hitBtn;
    }

    public void ColorChangeOnClick()
    {
        if (btn != null)
        {
            if (btn.name == "red_btn")
            {
                img.color = Color.red;
            }
            else if (btn.name == "blue_btn")
            {
                img.color = Color.red;
            }
            else if (btn.name == "green_btn")
            {
                img.color = Color.red;
            }
        }
    }
}
