using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Jobs;
using UnityEngine.Jobs;
using Unity.Mathematics;
using Unity.Burst;
using Random = Unity.Mathematics.Random;

public class SphereManager : MonoBehaviour
{
    [SerializeField] private float _sphereSpeed = 1;
    [SerializeField] private bool _disableMove = false;
    private TransformAccessArray transformAccessArray;
    

    void Start()
    {

    }

    void Update()
    {
        if (_disableMove == false)
        {
            uint randomSeed = (uint)UnityEngine.Random.Range(1, 1 << 8);
            var sphereJob = new SphereMoveJob
            {
                delta = Time.deltaTime,
                random = new Random(randomSeed),
                speed = _sphereSpeed,
            };
            print("Joba");
            JobHandle jobHandle = sphereJob.Schedule(transformAccessArray);

            jobHandle.Complete();
        }
    }

    struct SphereMoveJob : IJobParallelForTransform
    {
        public float delta;
        public Random random;
        public float speed;
        public void Execute(int index, TransformAccess transform)
        {
            var randomDir = random.NextFloat3Direction() * speed;
            var nextPos = new Vector3(randomDir.x, randomDir.y, randomDir.z);
            transform.position += nextPos * delta;
        }
    }

    public void Init(Transform[] spheres)
    {
        transformAccessArray = new(spheres);
    }

    private void OnDestroy()
    {
        transformAccessArray.Dispose();
    }
}
