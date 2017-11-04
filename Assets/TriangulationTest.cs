using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class TriangulationTest : MonoBehaviour {
    public GameObject[] points;

    void Update ()
    {
        Mesh mesh = new Mesh();

        List<DelaunayTriangulation.Vertex> triangulationData = new List<DelaunayTriangulation.Vertex>();

        List<Vector3> meshData = new List<Vector3>();
        List<int> indecies = new List<int>();

        for (int i = 0; i < points.Length; ++i)
        {
            Vector3 position = points[i].transform.position;
            meshData.Add(position);
            triangulationData.Add(new DelaunayTriangulation.Vertex(new Vector2(position.x, position.z), i));
        }

        DelaunayTriangulation.Triangulation triangulation = new DelaunayTriangulation.Triangulation(triangulationData);

        foreach (DelaunayTriangulation.Triangle triangle in triangulation.triangles)
        {
            indecies.Add(triangle.vertex0.index);
            indecies.Add(triangle.vertex1.index);
            indecies.Add(triangle.vertex2.index);
        }

        mesh.SetVertices(meshData);
        mesh.SetIndices(indecies.ToArray(), MeshTopology.Triangles, 0);
        mesh.RecalculateBounds();
        mesh.RecalculateNormals();

        MeshFilter meshFilter = GetComponent<MeshFilter>();

        meshFilter.mesh = mesh;
    }
}
