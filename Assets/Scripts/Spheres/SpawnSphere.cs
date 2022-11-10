using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;

public class SpawnSphere : MonoBehaviour
{
    [SerializeField] private GameObject _sphere;
    [SerializeField] private uint _rowSize, _colSize;
    [SerializeField] private Vector3 _startVector = Vector3.one;
    [SerializeField] private Vector2 offset = Vector3.one;
    [SerializeField] private SphereManager _sphereManager;

    private void Awake()
    {
        _sphereManager.Init(CreateSpheres());
    }


    private Transform[] CreateSpheres()
    {
        var spawnPos = _startVector;
        var countSpheres = _rowSize * _colSize;
        var spheres = new Transform[countSpheres];


        for (int y = 0, i = 0; y < _rowSize; y++)
        {
            for (int x = 0; x < _colSize; x++, i++)
            {
                spheres[i] = Instantiate(_sphere, spawnPos, new Quaternion(), this.transform).transform;
                spawnPos.x += offset.x;
            }
            spawnPos.z += offset.y;
            spawnPos.x = _startVector.x;
        }
        return spheres;
    }

    void Update()
    {

    }


}
