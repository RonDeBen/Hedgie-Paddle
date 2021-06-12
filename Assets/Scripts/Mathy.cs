using UnityEngine;

public static class Mathy{

    public static float NextGaussianFloat(){
        float u, v, S;

        do
        {
            u = 2.0f * Random.value - 1.0f;
            v = 2.0f * Random.value - 1.0f;
            S = u * u + v * v;
        }
        while (S >= 1.0f);

        float fac = Mathf.Sqrt(-2.0f * Mathf.Log(S) / S);
        return u * fac;
    }

    public static float Lerpy(float a, float b, float t){
        return (b - a)*t + a;
    }
}
