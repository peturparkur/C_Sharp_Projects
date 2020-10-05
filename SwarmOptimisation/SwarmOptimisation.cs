//using System.Collections;
//using System.Collections.Generic;
using System;
using PUtil.GenericMath;

public class SwarmOptimisation
{
    public Vector<double>[] points; //the positions of each particle
    public Vector<double>[] velocity; //velocity of positions

    Vector<double> lowerBounds;
    Vector<double> upperBounds;
    int rowNum;

    //variables for optimsation
    public Vector<double>[] pMin; //particle minimum
    public Vector<double> gMin; //swarm's global minimum

    public double w; //factor of how much velocity each particle keeps
    public double p; //factor of how much particles go towards their best positon
    public double g; //factor of how much particles go towards the global best position


    public SwarmOptimisation(Vector<double> lBounds, Vector<double> uBounds, int rNum, double _w = 0.1, double _p=1.0, double _g=1.0)//rNum is the number of elements in a particular row
    {
        if (lBounds.rows != uBounds.rows) throw new Exception("Dimensions of bounds don't match");
        w = _w;
        p = _p;
        g = _g;

        lowerBounds = lBounds;
        upperBounds = uBounds;

        rowNum = rNum;
        int n = (int)Math.Pow(rNum, lBounds.rows);
        points = new Vector<double>[n];
        velocity = new Vector<double>[n];
        pMin = new Vector<double>[n];

        gMin = new Vector<double>(lBounds.rows);

        for(int i=0; i<points.Length; i++)
        {
            points[i] = new Vector<double>(lBounds.rows);
            velocity[i] = new Vector<double>(lBounds.rows);
            pMin[i] = new Vector<double>(lBounds.rows);
        }
    }

    public void Initialise(Func<Vector<double>, double> F) //initialise for this function
    {
        Init();
        gMin = BestPosition(F);
    }

    void Init() //just initialises positions and individual best positions
    {
        int dim = lowerBounds.rows;
        //Vector<double> ds = Vector<double>.Abs(upperBounds - lowerBounds); //absolute difference aka. distance
        Vector<double> ds = (upperBounds - lowerBounds)/(double)(rowNum-1);

        //uniform distribution along the Dim dimensional space
        for (int j = 0; j < points.Length; j++)
        {
            int remainder = j;
            for (int i = dim - 1; i >= 0; i--)
            {
                double mult = Math.Pow(rowNum, i);
                points[j][i] = lowerBounds[i] + Math.Floor(remainder / mult)*ds[i]; //the ith component of the jth point
                pMin[j][i] = points[j][i]; //best position is the current position
                remainder = remainder - (int)(Math.Floor(remainder / mult)*mult); //remainder after taking the rounded value
            }
        }
    }

    public Vector<double> BestPosition(Func<Vector<double>, double> F) //we assume it's minimised
    {
        double best = F(points[0]);
        int bestIndex = 0;
        for(int i=1; i<points.Length; i++)
        {
            double val = F(points[i]);
            if(val < best)
            {
                best = val;
                bestIndex = i;
            }
        }
        return points[bestIndex];
    }

    public void Step(Func<Vector<double>, double> F, int seed = 0) //update the positions based on this function
    {
        int dim = lowerBounds.rows;
        Random random = seed == 0? new Random() : new Random(seed); //use the seed to generate random number unless seed is 0

        for (int i=0; i<points.Length; i++)
        {
            Vector<double> rand = new Vector<double>(dim);
            for(int r=0; r<dim; r++) //just for creating random numbers
            {
                rand[r] = random.NextDouble(); //we create a random number between 0,1 for each element of rand
            }

            velocity[i] = w * velocity[i] + p * (Vector<double>.EWiseProduct(rand, pMin[i] - points[i]) + g * (Vector<double>.EWiseProduct(new Vector<double>(dim, true) - rand, gMin - points[i])));
            points[i] += velocity[i]; //move towards velocity
        }
        gMin = EvaluateParticles(F);
    }

    public Vector<double> EvaluateParticles(Func<Vector<double>, double> F)
    {
        double best = F(gMin);
        int bestIndex = -1;

        for (int i=0; i<points.Length; i++)
        {
            double val = F(points[i]); //current value

            if(val < F(pMin[i])) //is it better than the particle's previous every best value
            {
                pMin[i] = points[i];
            }

            if (val < best) //is it better than the current best value
            {
                best = val;
                bestIndex = i;
            }
        }
        if (bestIndex < 0) return gMin; //we couldn't find a better position yet
        return points[bestIndex];
    }
}
