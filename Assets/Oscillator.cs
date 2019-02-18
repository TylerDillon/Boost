using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]

public class Oscillator : MonoBehaviour
{
    [SerializeField] Vector3 movementVector;
    [SerializeField] float period = 2f;

    Vector3 startingPos;

    //todo remove from inspector later
    float movementFactor; //0 for not moved, 1 for fully moved

    // Start is called before the first frame update
    void Start()
    {
        startingPos = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        float cycles = Time.time / period; //grows continually from 0

        if (period != 0) {
            const float tau = Mathf.PI * 2f;
            float rawSinWave = Mathf.Sin(cycles * tau);

            movementFactor = rawSinWave / 2f + 0.5f;
        }
        
        

        Vector3 offset = movementVector * movementFactor;
        transform.position = startingPos + offset;
    }
}
