using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using PUtil.GenericMath;

public class SwarmOptimTest : MonoBehaviour
{
    SwarmOptimisation swarm;
    Vector<double> lowerBound;
    Vector<double> upperBound;

    //Vector<double>[] points;
    int n;

    public Func<Vector<double>, double> function;
    int stepNum = 0;


    // Start is called before the first frame update
    void Start()
    {
        function = Something;
        lowerBound = new Vector<double>(2);
        lowerBound[0] = -1;
        lowerBound[1] = -1;

        upperBound = new Vector<double>(2);
        upperBound[0] = 6;
        upperBound[1] = 8.8;


        swarm = new SwarmOptimisation(lowerBound, upperBound, 10, 0.1, 1.0, 1.0);
        swarm.Initialise(function);
        //points = swarm.points;
        n = swarm.points.Length;

        for(int i=0; i<n; i++)
        {
            Debug.Log("The " + i + "th point = " + swarm.points[i].ToString());
        }

        stepNum = 0;
    }

    // Update is called once per frame
    void Update()
    {
        swarm.Step(function);
        stepNum++;
        Debug.Log("Step number = " + stepNum);
        /*for (int i = 0; i < n; i++)
        {
            Debug.Log("The " + i + "th Velocity = " + swarm.velocity[i].ToString());
        }*/
        Debug.Log("Best value = " + function(swarm.gMin));
        Debug.Log("Best position found = " + swarm.gMin.ToString());
    }

    double Something(Vector<double> v)
    {
        double sum = 0;
        for(int i=0; i<v.rows; i++)
        {
            sum += GenericMath.Abs(v[i]);
        }
        return sum;
    }
}
