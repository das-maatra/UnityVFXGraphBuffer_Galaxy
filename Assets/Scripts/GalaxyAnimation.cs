using UnityEngine;
using UnityEngine.VFX;
using System.Collections.Generic;

public class GalaxyAnimation : MonoBehaviour
{
    [SerializeField] 
    ComputeShader compute = null;

    [SerializeField]
    [Range(0.0f, 1.0f)]
    float speedMultiplier;

    GraphicsBuffer pointBuffer;
    GraphicsBuffer colorBuffer;
    GraphicsBuffer motionPropBuffer;
    List<Vector4> posList;
    List<Vector4> colorList;
    List<Vector4> motionPropList;

    void Start()
    {
        initializeGraphicsBuffers();
        initializeParticleProps();
    }

    void initializeGraphicsBuffers()
    {
        pointBuffer = new GraphicsBuffer
          (GraphicsBuffer.Target.Structured, 64 * 64, 4 * sizeof(float));
        GetComponent<VisualEffect>().SetGraphicsBuffer("PointBuffer", pointBuffer);

        colorBuffer = new GraphicsBuffer
          (GraphicsBuffer.Target.Structured, 64 * 64, 4 * sizeof(float));
        GetComponent<VisualEffect>().SetGraphicsBuffer("ColorBuffer", colorBuffer);

        motionPropBuffer = new GraphicsBuffer
          (GraphicsBuffer.Target.Structured, 64 * 64, 4 * sizeof(float));
        GetComponent<VisualEffect>().SetGraphicsBuffer("MotionPropBuffer", motionPropBuffer);

    }

    void initializeParticleProps()
    {
        speedMultiplier = 0.5f;

        initializeParticleMotion();
        initializeParticlePos();
        GenerateRandomColors();
    }

    void initializeParticlePos()
    {
        posList =  new List<Vector4>();

        for(int i = 0; i < (64*64); i++)
        {
            posList.Add(new Vector4(0f, Random.Range(-0.1f, 0.1f),0f,0f));
        }

        pointBuffer.SetData( posList );
    }

    void initializeParticleMotion()
    {
        motionPropList = new List<Vector4>();

        float angle;
        float radius;
        float radiusChange;
        float speed;

        for(int i = 0; i < (64*64); i++)
        {
            angle = Random.Range(0f, 360f) ;
            radius = 0.75f + Random.Range(0f, 0.25f);
            radiusChange =  Random.Range(0f, 1f) * 0.0005f;
            speed = Random.Range(0f, 0.0008f); 

            motionPropList.Add(new Vector4( angle, radius, radiusChange, speed ));
        }

        motionPropBuffer.SetData( motionPropList );
    }
    void GenerateRandomColors()
    {
        colorList = new List<Vector4>();

        for(int i = 0; i < (64*64); i++)
        {
            colorList.Add(new Vector4(1f,1f,1f,1f));
        }

        colorBuffer.SetData(colorList);
    }

    void OnDestroy()
    {
        pointBuffer?.Dispose();
        pointBuffer = null;

        colorBuffer?.Dispose();
        colorBuffer = null;

        motionPropBuffer?.Dispose();
        motionPropBuffer = null;
    }

    void Update()
    {
        compute.SetFloat("Time", Time.time);
        compute.SetFloat("SpeedMultiplier", speedMultiplier);
        compute.SetBuffer(0, "PointBuffer", pointBuffer);
        compute.SetBuffer(0, "PropBuffer", colorBuffer);
        compute.SetBuffer(0, "MotionPropBuffer", motionPropBuffer);
        compute.Dispatch(0, 8, 8, 1);
    }
}
