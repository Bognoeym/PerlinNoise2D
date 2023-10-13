using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PerlinNoise : MonoBehaviour
{
    [SerializeField] private int octaves;
    [SerializeField] private float persistances = 1f;
    [SerializeField] GameObject linePrefab;

    private LineRenderer lineRenderer;
    private Vector3 linePosition;
    private GameObject goLine;

    void Start()
    {
        goLine = Instantiate(linePrefab, gameObject.transform.position, gameObject.transform.rotation);
        lineRenderer = goLine.GetComponent<LineRenderer>();
    }

    void Update()
    {
        float x = PerlinNoise_1(linePosition.x, persistances, octaves);  // 설정에 대한 최종 값 반환
        //print(x);

        lineRenderer.positionCount++;
        linePosition = new Vector3(linePosition.x += 0.003f, x, 0);
        lineRenderer.SetPosition(lineRenderer.positionCount - 1, linePosition);
        Camera.main.transform.position = new Vector3(linePosition.x, 0, -3);
    }

    public float Noise(int x)  // -1~1 사이 부동소수
    {
        x = (x << 13) ^ x;
        return 1f - ((x * (x * x * 15731 + 789221) + 1373312589) & 0x3f3f3f3f) / 333731800f;
        //return Mathf.PerlinNoise(0, x) * 2 - 1;
    }

    public float Sigmoid(float x)
    {
        return x * x * x * (x * (x * 6 - 15) + 10);  // 6x^5 - 15x^4 + 10x^3
    }

    public float CoherentNoise(int x)
    {
        int intX = (int)(Mathf.Floor(x));  // Mathf.Floor() -> 내림(소수점 버림)
        float n0 = Noise(intX);
        float n1 = Noise(intX + 1);
        float weight = x - Mathf.Floor(x); 
        float noise = Mathf.Lerp(n0, n1, Sigmoid(weight)); 
        return noise;
    }

    public float SmoothedNoise(int x)  // smooth 값 만듦
    {
        return CoherentNoise(x) / 2 + CoherentNoise(x - 1) / 4 + CoherentNoise(x + 1) / 4;
    }

    public float InterpolatedNoise(float x)
    {
        int integer_X = (int)x;
        float fractional_X = x - integer_X;  // x의 소수점

        float v1 = SmoothedNoise(integer_X);
        float v2 = SmoothedNoise(integer_X + 1);

        return Cosine_Interpolate(v1, v2, fractional_X);
    }

    // a와 b사이의 곡선에서 x만큼의 점
    public float Cosine_Interpolate(float a, float b, float x)  // 코사인 보간
    {
        float y = x * Mathf.PI;  // Mathf.PI는 원주율
        y = (1f - Mathf.Cos(y)) * 0.5f;
        return a * (1f - y) + b * y;  // a와 b 사이의 어떤 값
    }

    public float PerlinNoise_1(float x, float persistance, int octaves)
    {
        float total = 0;
        float p = persistance;
        int n = octaves;

        for (int i = 0; i < n; i++)
        {
            float frequency = (float)Mathf.Pow(2, i);  // 주기(2^i)
            float amplitude = Mathf.Pow(p, i);  // 진폭(p^i)

            total += InterpolatedNoise(x * frequency) * amplitude;
        }

        return total;
    }
}