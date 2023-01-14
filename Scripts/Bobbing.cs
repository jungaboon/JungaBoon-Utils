using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bobbing : MonoBehaviour
{
    [SerializeField] private float amplitude;
    [SerializeField] private float frequency;
    [SerializeField] private float offset;

    private void Start()
    {
        StartCoroutine(Bob());
    }

    private IEnumerator Bob()
    {
        while(true)
        {
            float y = Mathf.Sin(Time.time * frequency) * amplitude;
            transform.position = Vector3.Lerp(transform.position, new Vector3(transform.position.x, transform.position.y + y, transform.position.z), Time.deltaTime * 2f);
            yield return null;
        }
    }
}
