using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomMove : MonoBehaviour
{
    const float kArea = 25f;

    private Vector2 m_Direction;

    private void RandomizeDirection()
    {
        float angle = Random.Range(-.5f, .5f);
        Vector2 toCenter = -(new Vector2(transform.position.x, transform.position.z).normalized);
        float cosAngle = Mathf.Cos(angle);
        float sinAngle = Mathf.Sin(angle);
        m_Direction = new Vector2(toCenter.x * cosAngle - toCenter.y * sinAngle, toCenter.x * sinAngle + toCenter.y * cosAngle);
    }

    public void Start()
    {
        RandomizeDirection();
    }


    public void Update()
    {
        transform.position += new Vector3(m_Direction.x, 0f, m_Direction.y) * Time.deltaTime * 4f;

        if (transform.position.magnitude > kArea)
        {
            transform.position.Normalize();
            //transform.position *= kArea;
            RandomizeDirection();
        }
    }
}
