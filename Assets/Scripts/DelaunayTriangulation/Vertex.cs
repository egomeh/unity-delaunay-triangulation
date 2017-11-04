using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DelaunayTriangulation
{
    public class Vertex
    {
        private Vector2 m_Position;
        public Vector2 position
        {
            get
            {
                return m_Position;
            }
        }

        private int m_Index;
        public int index
        {
            get
            {
                return m_Index;
            }
        }

        public Vertex(Vector2 position, int index)
        {
            m_Position = position;
            m_Index = index;
        }

        public override bool Equals(object obj)
        {
            Vertex other = obj as Vertex;
            
            if (other == null)
            {
                return false;
            }

            if (other.index != m_Index)
            {
                return false;
            }

            return true;
        }

        public override int GetHashCode()
        {
            return m_Index.GetHashCode();
        }

        public Vertex Clone()
        {
            return new Vertex(m_Position, index);
        }
    }
}