using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class Grid : MonoBehaviour
{
    [SerializeField] private int _sizeX, _sizeY;

    private Vector3[] vertices;

    private Mesh mesh;

    private void Awake()
    {
        StartCoroutine(Generate());
    }

    private IEnumerator Generate()
    {
        var wait = new WaitForSeconds(0.05f);

        mesh = GetComponent<MeshFilter>().mesh = new Mesh();

        mesh.name = "Procedural generated Grid";



        vertices = new Vector3[(_sizeX + 1) * (_sizeY + 1)];
        for (int i = 0, y = 0; y <= _sizeY; y++)
            for (int x = 0; x <= _sizeX; x++, i++)
            {
                //vertices[i] = transform.TransformPoint(new Vector3(x, y));
                vertices[i] = new Vector3(x, y);
                yield return wait;
            }

        mesh.vertices = vertices;

        int[] triangles = new int[6];
        triangles[0] = 0;
        triangles[1] = _sizeX + 1;
        triangles[3] = triangles[2] = 1;
        triangles[4] = triangles[1] = _sizeX + 1;
        triangles[5] = _sizeX + 2;
        mesh.triangles = triangles;
    }
    private void OnDrawGizmos()
    {
        if (vertices == null)
            return;
        Gizmos.color = Color.black;
        for (int i = 0; i < vertices.Length; i++)
        {
            Gizmos.DrawSphere(vertices[i], 0.1f);
        }
    }
}
