using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class ShatterableInteractable : MonoBehaviour
{
    public GrabDirectInteractor grabDirectInteractor = null;

    public void StartSplitMesh(bool destroy)
    {
        StartCoroutine(SplitMesh(destroy));
    }

    public IEnumerator SplitMesh(bool destroy)
    {

        if (GetComponent<MeshFilter>() == null || GetComponent<SkinnedMeshRenderer>() == null)
        {
            yield return null;
        }

        if (GetComponent<Collider>())
        {
            GetComponent<Collider>().enabled = false;
        }

        Mesh M = new Mesh();
        if (GetComponent<MeshFilter>())
        {
            M = GetComponent<MeshFilter>().mesh;
        }
        else if (GetComponent<SkinnedMeshRenderer>())
        {
            M = GetComponent<SkinnedMeshRenderer>().sharedMesh;
        }

        Material[] materials = new Material[0];
        if (GetComponent<MeshRenderer>())
        {
            materials = GetComponent<MeshRenderer>().materials;
        }
        else if (GetComponent<SkinnedMeshRenderer>())
        {
            materials = GetComponent<SkinnedMeshRenderer>().materials;
        }

        Vector3[] verts = M.vertices;
        Vector3[] normals = M.normals;
        Vector2[] uvs = M.uv;

        // Choose vertice in the center of the object. Average all points to get center point. Will not work properly with objects that are "concave".
        Vector3 average_vertice = new Vector3(0, 0, 0);
        foreach (Vector3 vert in verts)
        {
            average_vertice = average_vertice + vert;
        }
        average_vertice = average_vertice / verts.Length;

        for (int submesh = 0; submesh < M.subMeshCount; submesh++)
        {

            int[] indices = M.GetTriangles(submesh);

            for (int i = 0; i < indices.Length; i += 3)
            {
                Vector3[] newVerts = new Vector3[4];
                Vector3[] newNormals = new Vector3[4];
                Vector2[] newUvs = new Vector2[4];
                for (int n = 0; n < 3; n++)
                {
                    // Get the next 3 values for the current triangle from the list of all the original objects indices.
                    int index = indices[i + n];
                    newVerts[n] = verts[index];
                    newUvs[n] = uvs[index];
                    newNormals[n] = normals[index];
                }

                Mesh mesh = new Mesh();
                newVerts[3] = average_vertice; // Add center vertice
                mesh.vertices = newVerts;
                Vector3 normal1 = CalculateTriangleNormal(newVerts[1], newVerts[2], newVerts[3]);
                Vector3 normal2 = CalculateTriangleNormal(newVerts[2], newVerts[3], newVerts[0]);
                Vector3 normal3 = CalculateTriangleNormal(newVerts[3], newVerts[0], newVerts[1]);
                Vector3 centerVertexNormal = normal1 + normal2 + normal3;
                centerVertexNormal = centerVertexNormal / centerVertexNormal.magnitude;
                newNormals[3] = centerVertexNormal; // Add normals for each of the 3 new triangles
                mesh.normals = newNormals;
                newUvs[3] = new Vector2(0, 1); // FIXME: This probably doesn't work.
                mesh.uv = newUvs;

                //mesh.triangles = new int[] { 0, 1, 2, 2, 1, 0 }; // Creates mesh on front and back side?
                mesh.triangles = new int[] { 0, 1, 2, 1, 2, 3, 2, 3, 0, 3, 0, 1, 1, 0, 3, 0, 3, 2, 3, 2, 1, 2, 1, 0};

                GameObject GO = new GameObject("Triangle " + (i / 3));
                GO.layer = LayerMask.NameToLayer("Particle");
                GO.transform.position = transform.position; // TODO: Might need to calculate new position that will make it fit together instead of spawning all of them in the same place.
                GO.transform.rotation = transform.rotation;
                GO.transform.localScale = transform.localScale;
                GO.AddComponent<MeshRenderer>().material = materials[submesh];
                GO.AddComponent<MeshFilter>().mesh = mesh;
                GO.AddComponent<BoxCollider>();
                //Vector3 explosionPos = new Vector3(transform.position.x + Random.Range(-0.5f, 0.5f), transform.position.y + Random.Range(0f, 0.5f), transform.position.z + Random.Range(-0.5f, 0.5f));
                Rigidbody newBody = GO.AddComponent<Rigidbody>(); //.AddExplosionForce(Random.Range(300, 500), explosionPos, 5);
                grabDirectInteractor.AttachJoint(newBody);
                GO.AddComponent<ForceGrabPullInteractable>();
                //Destroy(GO, 5 + Random.Range(0.0f, 5.0f));
            }
        }

        GetComponent<Renderer>().enabled = false;

        yield return new WaitForSeconds(1.0f);
        if (destroy == true)
        {
            Destroy(gameObject);
        }

    }

    private Vector3 CalculateTriangleNormal(Vector3 vert1, Vector3 vert2, Vector3 vert3)
    {
        Vector3 normal = Vector3.Cross((vert2 - vert1), (vert3 - vert1));
        return normal / normal.magnitude;
    }
}