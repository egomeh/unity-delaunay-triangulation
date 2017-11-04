using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace DelaunayTriangulation
{
    public class Triangle
    {
        private Vertex m_Vertex0, m_Vertex1, m_Vertex2;
        private Edge m_Edge0, m_Edge1, m_Edge2;

        private Vector2 m_CircumcircleCenter;
        private Vector2 circumcircleCenter
        {
            get
            {
                if (m_CircumcircleCenter == null)
                {
                    m_CircumcircleCenter = new Vector2();
                }
                return m_CircumcircleCenter;
            }
            set
            {
                m_CircumcircleCenter = value;
            }
        }

        private float m_CircumcircleRadius;
        private float circumcircleRadius
        {
            get
            {
                return m_CircumcircleRadius;
            }

            set
            {
                m_CircumcircleRadius = value;
            }
        }

        public Vertex vertex0
        {
            get
            {
                return m_Vertex0;
            }
        }

        public Vertex vertex1
        {
            get
            {
                return m_Vertex1;
            }
        }

        public Vertex vertex2
        {
            get
            {
                return m_Vertex2;
            }
        }

        public Edge edge0
        {
            get
            {
                return m_Edge0;
            }
        }

        public Edge edge1
        {
            get
            {
                return m_Edge1;
            }
        }

        public Edge edge2
        {
            get
            {
                return m_Edge2;
            }
        }

        public Triangle(Vertex p0, Vertex p1, Vertex p2, bool clockwise)
        {
            List<Vertex> inputPoints = new List<Vertex>();

            inputPoints.Add(p0);
            inputPoints.Add(p1);
            inputPoints.Add(p2);

            inputPoints = inputPoints.OrderBy(x => x.position.x).ToList();

            m_Vertex0 = inputPoints[0].Clone();

            Vector2 up = inputPoints[2].position - inputPoints[0].position;
            up = new Vector2(-up.y, up.x);

            float distanceToPlane = Vector2.Dot(up, (inputPoints[1].position - inputPoints[0].position));

            int clockWiseShift = clockwise ? 0 : 1;

            if (distanceToPlane > 0f)
            {
                m_Vertex1 = inputPoints[1 + clockWiseShift].Clone();
                m_Vertex2 = inputPoints[2 - clockWiseShift].Clone();
            }
            else
            {
                m_Vertex1 = inputPoints[2 - clockWiseShift].Clone();
                m_Vertex2 = inputPoints[1 + clockWiseShift].Clone();
            }

            m_Edge0 = new Edge(m_Vertex0, m_Vertex1);
            m_Edge1 = new Edge(m_Vertex1, m_Vertex2);
            m_Edge2 = new Edge(m_Vertex2, m_Vertex0);

            float len0Square = (vertex0.position.x * vertex0.position.x) + (vertex0.position.y * vertex0.position.y);
            float len1Square = (vertex1.position.x * vertex1.position.x) + (vertex1.position.y * vertex1.position.y);
            float len2Square = (vertex2.position.x * vertex2.position.x) + (vertex2.position.y * vertex2.position.y);

            // Compute the circumcircle of the triangle.
            // TODO: Find better solution for this.
            Vector2 circleCenter = new Vector2();

            circleCenter.x = (len0Square * (vertex2.position.y - vertex1.position.y) + len1Square * (vertex0.position.y - vertex2.position.y) + len2Square * (vertex1.position.y - vertex0.position.y)) / (vertex0.position.x * (vertex2.position.y - vertex1.position.y) + vertex1.position.x * (vertex0.position.y - vertex2.position.y) + vertex2.position.x * (vertex1.position.y - vertex0.position.y)) / 2f;
            circleCenter.y = (len0Square * (vertex2.position.x - vertex1.position.x) + len1Square * (vertex0.position.x - vertex2.position.x) + len2Square * (vertex1.position.x - vertex0.position.x)) / (vertex0.position.y * (vertex2.position.x - vertex1.position.x) + vertex1.position.y * (vertex0.position.x - vertex2.position.x) + vertex2.position.y * (vertex1.position.x - vertex0.position.x)) / 2f;

            m_CircumcircleCenter = circleCenter;

            circumcircleRadius = Mathf.Sqrt(((vertex1.position.x - circumcircleCenter.x) * (vertex1.position.x - circumcircleCenter.x)) + ((vertex1.position.y - circumcircleCenter.y) * (vertex1.position.y - circumcircleCenter.y)));
        }

        public Triangle(Vertex p0, Vertex p1, Vertex p2) : this(p0, p1, p2, true)
        {
            // Do nothing special for defualt case
        }

        public bool PointInCurcumcircle(Vector2 vertex)
        {
            float xDiff = (vertex.x - circumcircleCenter.x);
            float yDiff = (vertex.y - circumcircleCenter.y);
            float distance = Mathf.Sqrt(xDiff * xDiff + yDiff * yDiff);

            return distance <= circumcircleRadius;
        }

        public bool PointInCurcumcircle(Vertex vertex)
        {
            bool isInCircumcircle = PointInCurcumcircle(vertex.position);

            return isInCircumcircle;
        }

        public bool Contains(Vertex vertex)
        {
            bool contains = vertex.Equals(vertex0);
            contains |= vertex.Equals(vertex1);
            contains |= vertex.Equals(vertex2);

            return contains;
        }

        public bool Contains(Edge edge)
        {
            bool contains = edge.Equals(edge0);
            contains |= edge.Equals(edge1);
            contains |= edge.Equals(edge2);

            return contains;
        }

        public List<Vertex> GetOverlappingSet(Triangle other)
        {
            List<Vertex> overlap = new List<Vertex>();

            if (other.Contains(vertex0))
            {
                overlap.Add(vertex0);
            }

            if (other.Contains(vertex1))
            {
                overlap.Add(vertex1);
            }

            if (other.Contains(vertex2))
            {
                overlap.Add(vertex2);
            }

            return overlap;
        }

        public override bool Equals(object obj)
        {
            Triangle other = obj as Triangle;

            if (other == null)
            {
                return false;
            }

            bool isSame = m_Vertex0.Equals(other.vertex0) ||
                m_Vertex0.Equals(other.vertex1) || m_Vertex0.Equals(other.vertex2);

            isSame &= m_Vertex1.Equals(other.vertex0) ||
                m_Vertex1.Equals(other.vertex1) || m_Vertex1.Equals(other.vertex2);

            isSame &= m_Vertex2.Equals(other.vertex0) ||
                m_Vertex2.Equals(other.vertex1) || m_Vertex2.Equals(other.vertex2);

            return isSame;
        }

        public override int GetHashCode()
        {
            int hash0 = vertex0.index.GetHashCode();
            int hash1 = vertex1.index.GetHashCode();
            int hash2 = vertex2.index.GetHashCode();
            int combined = ((hash0 << 3) + hash0 ^ hash1) << 5;
            combined += combined ^ hash2;

            return combined;
        }

    }
}
