using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace DelaunayTriangulation
{
    public class Triangulation
    {
        private List<Triangle> m_Trinagles;
        public List<Triangle> triangles
        {
            get
            {
                return m_Trinagles;
            }
        }

        private Rect m_BoundingBox;
        private Rect boundingBox
        {
            get
            {
                if (m_BoundingBox == null)
                {
                    m_BoundingBox = new Rect(0, 0, 0, 0);
                }
                return m_BoundingBox;
            }
            set
            {
                m_BoundingBox = value;
            }
        }

        public Triangulation()
        {
            m_Trinagles = new List<Triangle>();
        }

        // Make triangulation from set of triangled, will not actually
        // triangulate, but only store tringles and allow insertion of points
        public Triangulation(List<Triangle> triangles)
        {

            m_Trinagles = triangles;

            // If there are triangles, create a bounding box
            if (m_Trinagles.Count > 0)
            {
                Vertex v = m_Trinagles[0].vertex0;

                float firstX = v.position.x;
                float firstY = v.position.y;

                Rect bBox = new Rect(firstX, firstY, firstX, firstY);

                foreach (Triangle triangle in m_Trinagles)
                {
                    List<Vertex> triangleVertecies = new List<Vertex>();

                    triangleVertecies.Add(triangle.vertex0);
                    triangleVertecies.Add(triangle.vertex1);
                    triangleVertecies.Add(triangle.vertex2);

                    foreach (Vertex vertex in triangleVertecies)
                    {
                        bBox.x = Mathf.Min(boundingBox.x, vertex.position.x);
                        bBox.y = Mathf.Min(boundingBox.y, vertex.position.y);
                        bBox.width = Mathf.Max(boundingBox.width, vertex.position.x);
                        bBox.height = Mathf.Max(boundingBox.height, vertex.position.y);
                    }
                }

                boundingBox = bBox;
            }
        }

        // Make triangulation from set of points
        public Triangulation(List<Vertex> points) : this()
        {
            if (points.Count < 3)
            {
                return;
            }

            // Start with empty triangulation
            List<Triangle> triangulation = new List<Triangle>();

            float firstX = points[0].position.x;
            float firstY = points[0].position.y;

            Rect bBox = new Rect(firstX, firstY, firstX, firstY);

            foreach (Vertex vertex in points)
            {
                bBox.x = Mathf.Min(boundingBox.x, vertex.position.x);
                bBox.y = Mathf.Min(boundingBox.y, vertex.position.y);
                bBox.width = Mathf.Max(boundingBox.width, vertex.position.x);
                bBox.height = Mathf.Max(boundingBox.height, vertex.position.y);
            }

            boundingBox = bBox;

            float superWidth = (boundingBox.size.x - boundingBox.position.x) * 3f + 1f;
            float superHeight = (boundingBox.size.y - boundingBox.position.y) * 3f + 1f;

            // Make super triangle
            Vertex super0 = new Vertex(boundingBox.position - new Vector2(10f, 12f) * 100f, -1);
            Vertex super1 = new Vertex(new Vector2(boundingBox.position.x + superWidth * 100f, boundingBox.position.y * 100f - .95f), -2);
            Vertex super2 = new Vertex(new Vector2(boundingBox.position.x - .95f, boundingBox.position.y + superHeight * 100f), -3);

            Triangle super = new Triangle(super0, super1, super2);

            triangulation.Add(super);

            // Iterate over each point, insert and triangulate
            foreach (Vertex vertex in points)
            {
                // Triangles invalidated at vertex insertion
                List<Triangle> badTriangles = new List<Triangle>();

                // List of edges that should be removed
                List<Edge> polygon = new List<Edge>();

                // List of edges that are incorrectly marked for rmoval
                List<Edge> badEdges = new List<Edge>();

                // Find all invalid triangles
                foreach (Triangle triangle in triangulation)
                {
                    // If the point is in the circumcircle in
                    // a trinagle, add it to the bad set
                    if (triangle.PointInCurcumcircle(vertex))
                    {
                        // Add the triangle and the edges
                        badTriangles.Add(triangle);
                        polygon.Add(triangle.edge0);
                        polygon.Add(triangle.edge1);
                        polygon.Add(triangle.edge2);
                    }
                }

                // Remove bad trinagles from current triangulation
                triangulation.RemoveAll(x => badTriangles.Contains(x));

                // Identify edges in the polygon that are shared
                // among multiple triangles
                for (int i = 0; i < polygon.Count; ++i)
                {
                    for (int j = 0; j < polygon.Count; ++j)
                    {
                        if (i == j)
                        {
                            continue;
                        }

                        // If an edge is shared with another triangle
                        // it is removed from the list and will not be
                        // retriangulated.
                        if (polygon[i].Equals(polygon[j]))
                        {
                            badEdges.Add(polygon[i]);
                            badEdges.Add(polygon[j]);
                        }
                    }
                }

                // Remove shared edges from the polygon
                polygon.RemoveAll(x => badEdges.Contains(x));

                // Retriangulate the empty polygonal space
                foreach (Edge edge in polygon)
                {
                    triangulation.Add(new Triangle(edge.point0, edge.point1, vertex));
                }
            }

            // Cleanup from the super trinagle
            for (int i = triangulation.Count - 1; i >= 0; --i)
            {
                // Check if any point is part of the super triangle
                bool isSuper = triangulation[i].Contains(super0);
                isSuper |= triangulation[i].Contains(super1);
                isSuper |= triangulation[i].Contains(super2);

                // If so, remove triangle from triangulation
                if (isSuper)
                {
                    triangulation.RemoveAt(i);
                }
            }

            m_Trinagles = triangulation;
        }

        // Used for adding an internal vertex
        // used only when the vertex falls within the triangulation
        public void AddInternal(Vertex vertex)
        {
            // Triangles invalidated at vertex insertion
            List<Triangle> badTriangles = new List<Triangle>();

            // List of edges that should be removed
            List<Edge> polygon = new List<Edge>();

            // List of edges that are incorrectly marked for rmoval
            List<Edge> badEdges = new List<Edge>();

            // Find all invalid triangles
            foreach (Triangle triangle in m_Trinagles)
            {
                // If the point is in the circumcircle in
                // a trinagle, add it to the bad set
                if (triangle.PointInCurcumcircle(vertex))
                {
                    // Add the triangle and the edges
                    badTriangles.Add(triangle);
                    polygon.Add(triangle.edge0);
                    polygon.Add(triangle.edge1);
                    polygon.Add(triangle.edge2);
                }
            }

            // Remove bad trinagles from current triangulation
            m_Trinagles.RemoveAll(x => badTriangles.Contains(x));

            // Identify edges in the polygon that are shared
            // among multiple triangles
            for (int i = 0; i < polygon.Count; ++i)
            {
                for (int j = 0; j < polygon.Count; ++j)
                {
                    if (i == j)
                    {
                        continue;
                    }

                    // If an edge is shared with another triangle
                    // it is removed from the list and will not be
                    // retriangulated.
                    if (polygon[i].Equals(polygon[j]))
                    {
                        badEdges.Add(polygon[i]);
                        badEdges.Add(polygon[j]);
                    }
                }
            }

            // Remove shared edges from the polygon
            polygon.RemoveAll(x => badEdges.Contains(x));

            // Retriangulate the empty polygonal space
            foreach (Edge edge in polygon)
            {
                m_Trinagles.Add(new Triangle(edge.point0, edge.point1, vertex));
            }
        }
    }
}

