using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DelaunayTriangulation
{
    public class Edge
    {
        private float m_Length;
        public float length
        {
            get
            {
                return m_Length;
            }
        }

        private Vertex m_Point0;
        public Vertex point0
        {
            get
            {
                return m_Point0;
            }
        }

        private Vertex m_Point1;
        public Vertex point1
        {
            get
            {
                return m_Point1;
            }
        }

        public Edge(Vertex point0, Vertex point1)
        {
            m_Point0 = point0;
            m_Point1 = point1;

            Vector2 edgeVector = m_Point1.position - m_Point0.position;
            m_Length = edgeVector.magnitude;
        }

        public float Length()
        {
            return m_Length;
        }

        public override bool Equals(object obj)
        {
            Edge other = obj as Edge;

            if (other != null)
            {
                // Check if the two first points overlap
                bool isSame = other.point0.Equals(point0) && other.point1.Equals(point1);

                // Check if the points overlap in cross
                isSame |= other.point1.Equals(point0) && other.point0.Equals(point1);

                return isSame;
            }
            
            return false;
        }

        public override int GetHashCode()
        {
            return m_Point0.GetHashCode() + 31 * m_Point1.GetHashCode();
        }
    }
}
