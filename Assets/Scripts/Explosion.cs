using UnityEngine;
using System.Collections;

public class Explosion : MonoBehaviour {

    public bool ismustMove;
    [HideInInspector]
    public Transform source;
    float speed = 10.0f;
    Vector3 movingVec;

    void Start()
    {
        Destroy(gameObject, 3.5f);
    }

    void Update()
    {
        if (ismustMove)
        {
            try
            {
                if (speed > 2.7f) speed *= 0.99f;
                else speed *= 0.8f;

                transform.position = Vector3.Lerp(transform.position, source.position, Time.deltaTime * speed);
            }
            catch
            {
                Destroy(gameObject);
            }
        }
    }
}
