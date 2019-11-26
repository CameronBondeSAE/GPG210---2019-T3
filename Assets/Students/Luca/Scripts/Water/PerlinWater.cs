using System.Collections;
using System.Collections.Generic;
using Students.Luca.Scripts;
using UnityEngine;

public class PerlinWater : Water
{
    public MeshFilter mf;

    public float amplitude = 1;
    public float speed = 1;
    public float smoothness = 1000;
    
    public float meshUpdateFrequency = 0;

    private float meshUpdateCooldown = 0;
    private Vector3 _worldPos;
    
    // Start is called before the first frame update
    void Start()
    {
        if (mf == null)
        {
            mf = GetComponent<MeshFilter>();
        }

        _worldPos = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (meshUpdateCooldown <= 0)
        {
            UpdateMesh();
            meshUpdateCooldown = meshUpdateFrequency;
        }
        else
        {
            meshUpdateCooldown -= Time.deltaTime;
        }
    }

    private void UpdateMesh() {
        var verts = mf.mesh.vertices;
        for (var i = 0; i < verts.Length; i++) { 
            var x = _worldPos.x + verts[i].x;
            var z = _worldPos.z + verts[i].z;
            verts[i].y = CalculatePerlinNoise(x, z);
        }
        
        mf.mesh.vertices = verts;
        mf.mesh.RecalculateBounds();
        mf.mesh.RecalculateNormals();
    }
    
    private float CalculatePerlinNoise(float x, float z)
    {
        var xC = x * Time.time / smoothness * speed;
        var yC = z * Time.time / smoothness * speed;
        
        return Mathf.PerlinNoise (xC, yC) * amplitude; 
    }

    public override float GetSurfaceLevelAtPos(float x, float z)
    {
        return _worldPos.y + CalculatePerlinNoise(x, z); 
    }
}
