using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Configuration;
using UnityEngine;
using UnityEngine.Rendering;

namespace Students.Luca.Scripts
{
    public class BuoyantBody : MonoBehaviour
    {
        public bool doDebug = false;
        public bool generateDebugMesh = false;
        
        [Header("Components")]
        public MeshFilter meshFilter;
        public Rigidbody rb;
        public Transform centerOfMass;
        [Tooltip("(OPTIONAL) If set, up-facing triangles will only experience buoyant forces if the watersurface level is above this transforms position. (Useful for boats)")]
        public Transform waterEntryLevel = null; // Can be null; Otherwise, if its a boat for example, up-facing-triangles only will apply down forces when the objects waterEntryLevel is below the water surface. TODO HACKY; Not Optimal
        
        public Water currentWater;

        [Header("Settings")]
        public float dragCoefficient = .5f;
        [Tooltip("True: Only upward force (-Gravity) will be applied, False: Directional force will be applied depending on the Triangles normals.")]
        public bool useUpwardBuoyantForce = true;
        [Tooltip("True: For each tri the actual surface of the tri that is underwater is calculated; False: Tri is considered underwater if centroid is underwater.")]
        public bool calculatePreciseUnderwaterTris = false;

        [Tooltip("Limits how often the tris under water are being calculated per second. 0: Unlimited => Calculation every frame. (Higher values more performant, less inaccurate)")]
        public float uwAreaCalcInterval = 0;

        public AnimationCurve uwAreaCalcIntervalVelocityMultiplier;
        [Tooltip("If set to false, the uwAreaCalcInterval is dynamic depending on the objects current speed.")]
        public bool fixedUwAreaCalcInterval = false;
        
        private float nextUnderwaterCalculation = 0;

        public bool applyBuoyantDownForce = true;

        public bool hasAreasAboveWater = false;

        private UnderwaterAreaData[] underwaterFacesData;
        
        private Vector3[] vertices;
        private int[] triangles;
        
        private List<Triangle> underwaterTriangles;

        private void Awake()
        {
            underwaterTriangles = new List<Triangle>();
            
            
            meshFilter = (meshFilter == null?GetComponent<MeshFilter>():meshFilter);
            rb = (rb == null?GetComponent<Rigidbody>():rb);
            if (rb != null && centerOfMass != null)
            {
                rb.centerOfMass = centerOfMass.localPosition;
            }

            if (meshFilter != null)
            {
                underwaterFacesData = new UnderwaterAreaData[meshFilter.mesh.triangles.Length]; // meshFilter.mesh.vertexCount
                
                vertices = meshFilter.mesh.vertices;
                triangles = meshFilter.mesh.triangles;
            }
        }

        // Update is called once per frame
        void Update()
        {
            // TODO Just for testing purposes.
            if (centerOfMass != null && centerOfMass.localPosition != rb.centerOfMass)
            {
                rb.centerOfMass = centerOfMass.localPosition;
            }

            if (Mathf.Approximately(uwAreaCalcInterval, 0) || (uwAreaCalcInterval > 0 && nextUnderwaterCalculation <= 0))
            {
                if (calculatePreciseUnderwaterTris)
                {
                    CalculatePreciseUnderWaterAreas();
                
                    if (generateDebugMesh/* && underwaterTriangles?.Count > 0*/)
                    {
                        if (debugMeshObject == null)
                        {
                            CreateDebugMeshObject();
                        }
                        else
                        {
                            debugMeshObject.GetComponent<MeshFilter>().mesh = GenerateUnderwaterMesh();
                        }
                    }
                }
                else
                {
                    CalculateUnderWaterAreas();
                }

                if (uwAreaCalcInterval > 0)
                {
                    float multiplier = fixedUwAreaCalcInterval
                        ? 1
                        : uwAreaCalcIntervalVelocityMultiplier.Evaluate(rb.velocity.magnitude);
                    nextUnderwaterCalculation = uwAreaCalcInterval * multiplier;
                    
                }
                    
            }
                
            if (nextUnderwaterCalculation > 0)
                nextUnderwaterCalculation -= Time.deltaTime;

            
            
        }

        private void FixedUpdate()
        {
            if (calculatePreciseUnderwaterTris)
            {
                ApplyBuoyantForceNew();
            }
            else
            {
                ApplyBuoyantForce();
            }
            

            if (doDebug)
            {
                Debug.DrawRay(rb.transform.position, rb.velocity, Color.cyan);
                //Debug.DrawRay(rb.transform.position, rb.angularVelocity*10, Color.red);
            }
                
        }

        public Vector3 debugMeshScale = new Vector3(1.01f,1.01f,1.01f);
        public Color debugMeshColor = new Color(1, 0, 0, .5f);
        private GameObject debugMeshObject = null;

        private void CreateDebugMeshObject()
        {
            if(debugMeshObject != null)
                Destroy(debugMeshObject);
            
            debugMeshObject = new GameObject(gameObject.name + "_UnderwaterAreaDebugMesh");
            debugMeshObject.transform.SetParent(transform);
            debugMeshObject.transform.localPosition = Vector3.zero;
            debugMeshObject.transform.localRotation = Quaternion.identity;
            debugMeshObject.transform.localScale = debugMeshScale;

            MeshFilter mf = debugMeshObject.AddComponent<MeshFilter>();
            MeshRenderer mr = debugMeshObject.AddComponent<MeshRenderer>();

            Material material = new Material(Shader.Find("Specular"));
            //material.color = debugMeshColor;
            material.SetColor(Shader.PropertyToID("_Color"),debugMeshColor);
            mr.material = material;
            
            mf.mesh = GenerateUnderwaterMesh();
        }
        
        private void ApplyBuoyantForceNew()
        {
            if (!(underwaterTriangles?.Count > 0)) return;

            foreach (var triangle in underwaterTriangles)
            {
                if (triangle == null)
                    continue;
                
                float faceAngleToGravity = Vector3.Angle(triangle.faceNormal, Vector3.down);
                float angleToGravityMultiplier = applyBuoyantDownForce ? Mathf.Cos(faceAngleToGravity*Mathf.Deg2Rad) : Mathf.Clamp(Mathf.Cos(faceAngleToGravity*Mathf.Deg2Rad),0,1);
                
                // Buoyant Force
                Vector3 buoyantForce;

                // Buoyancy Formula: F = P * G * A * h | P = Density of Liquid; Gravity; A Surface Area of Attack; h Height / Distance below water
                if (useUpwardBuoyantForce)
                {
                    buoyantForce =  Vector3.up * (currentWater.density * triangle.surfaceArea * triangle.distUnderWater * angleToGravityMultiplier * Mathf.Abs(Physics.gravity.y));
                }
                else
                {
                    buoyantForce =  (currentWater.density * triangle.surfaceArea * triangle.distUnderWater * Mathf.Abs(angleToGravityMultiplier) * Physics.gravity.y)  * triangle.faceNormal.normalized;
                }

                // Drag Force
                Vector3 localVelocity = rb.GetPointVelocity(triangle.centroid);
                float angleOfAttack = Vector3.Angle(localVelocity, triangle.faceNormal);
                float dragForceMultiplier = Mathf.Cos(Mathf.Clamp(angleOfAttack,0,90)*Mathf.Deg2Rad);//Mathf.Clamp(Mathf.Cos(angleOfAttack*Mathf.Deg2Rad),0,1); // For simplicity: Angles more than 90° will result in zero-drag.
                //Vector3 dragForce = -.5f * currentWater.density * localVelocity.sqrMagnitude * triangle.surfaceArea * dragCoefficient * dragForceMultiplier * localVelocity.normalized;
                Vector3 dragForce = Vector3.zero;
                
                // Add Final Force
                Vector3 composedForce = buoyantForce + dragForce;
                composedForce.x = float.IsNaN(composedForce.x) ? 0 : Mathf.Clamp(composedForce.x, -10000, 10000);//(float.IsInfinity(composedForce.x)?0:composedForce.x);
                composedForce.y = float.IsNaN(composedForce.y) ? 0 : Mathf.Clamp(composedForce.y, -10000, 10000);//(float.IsInfinity(composedForce.y)?0:composedForce.y);
                composedForce.z = float.IsNaN(composedForce.z) ? 0 : Mathf.Clamp(composedForce.z, -10000, 10000);//(float.IsInfinity(composedForce.z)?0:composedForce.z);
                
                rb.AddForceAtPosition(composedForce, triangle.centroid);

                if (doDebug)
                {
                    Debug.DrawRay(triangle.centroid,buoyantForce,Color.green);
                    //Debug.DrawRay(triangle.centroid, dragForce, Color.magenta);
                    Debug.DrawRay(triangle.centroid, localVelocity, Color.cyan);
                    //Debug.DrawRay(triangle.centroid,triangle.faceNormal*10,Color.red);
                    //Debug.DrawRay(uad.centerPoint, composedForce, Color.yellow);
                }
            }
        }

        private void ApplyBuoyantForce()
        {
            if (underwaterFacesData.Length > 0)
            {
                for (int i = 0; i < underwaterFacesData.Length; i++)
                {
                    UnderwaterAreaData uad = underwaterFacesData[i];
                    
                    if (uad == null) // Break, iterated through all areas under water; (Rest of the array should be empty)
                        break;
                    
                    float faceAngleToGravity = Vector3.Angle(uad.areaNormal, Vector3.down);
                    float angleToGravityMultiplier = applyBuoyantDownForce ? Mathf.Cos(faceAngleToGravity*Mathf.Deg2Rad) : Mathf.Clamp(Mathf.Cos(faceAngleToGravity*Mathf.Deg2Rad),0,1);
                    
                    // Buoyant Force
                    Vector3 buoyantForce;

                    // Buoyancy Formula: F = P * G * A * h | P = Density of Liquid; Gravity; A Surface Area of Attack; h Height / Distance below water
                    if (useUpwardBuoyantForce)
                    {
                        buoyantForce =  Vector3.up * (currentWater.density * uad.surfaceArea * uad.distanceToSurface * angleToGravityMultiplier * Mathf.Abs(Physics.gravity.y));
                    }
                    else
                    {
                        buoyantForce =  (currentWater.density * uad.surfaceArea * uad.distanceToSurface * Mathf.Abs(angleToGravityMultiplier) * Physics.gravity.y)  * uad.areaNormal.normalized;
                    }
                    
                    /*
                     ///// OLD DRAG FORCE CALC
                    // Drag Force
                    float angleOfAttack = Vector3.Angle(rb.velocity, uad.areaNormal);
                    float dragForceMultiplier = Mathf.Clamp(Mathf.Cos(angleOfAttack*Mathf.Deg2Rad),0,1); // For simplicity: Angles more than 90° will result in zero-drag.
                    Vector3 localVelocity = dragForceMultiplier * Mathf.Cos(angleOfAttack*Mathf.Deg2Rad) * rb.velocity;
                    Vector3 dragForce = -.5f * currentWater.density * localVelocity.sqrMagnitude * uad.surfaceArea * dragCoefficient * localVelocity.normalized; // (0.5f) = Drag Coefficient?
                    
                    // Angular Drag Forcefloat angularAngleOfAttack = Vector3.Angle(rb.angularVelocity, uad.areaNormal);
                    float angularDragAnglForceMultiplier = 1;//Mathf.Cos(angularAngleOfAttack * Mathf.Deg2Rad);//Mathf.Clamp(Mathf.Cos(angularAngleOfAttack*Mathf.Deg2Rad),0,1); // For simplicity: Angles more than 90° will result in zero-drag.
                    float distanceToCenterOfMass = 1;//Vector3.Distance(rb.centerOfMass, uad.centerPoint);
                    Vector3 localAngularVelocity = angularDragAnglForceMultiplier * Mathf.Cos(angularAngleOfAttack*Mathf.Deg2Rad) * rb.angularVelocity * distanceToCenterOfMass;
                    Vector3 angularDragForce = -.5f * currentWater.density * localAngularVelocity.sqrMagnitude * uad.surfaceArea * (0.5f) * localAngularVelocity.normalized; // (0.5f) = Drag Coefficient?
                    */

                    // Drag Force
                    Vector3 localVelocity = rb.GetPointVelocity(uad.centerPoint);
                    float angleOfAttack = Vector3.Angle(localVelocity, uad.areaNormal);
                    float dragForceMultiplier = Mathf.Clamp(Mathf.Cos(angleOfAttack*Mathf.Deg2Rad),0,1); // For simplicity: Angles more than 90° will result in zero-drag.
                    Vector3 dragForce = -.5f * currentWater.density * localVelocity.sqrMagnitude * uad.surfaceArea * dragCoefficient * dragForceMultiplier * localVelocity.normalized;
                    //Vector3 dragForce = Vector3.zero;
                    
                    // Add Final Force
                    Vector3 composedForce = buoyantForce + dragForce;
                    composedForce.x = float.IsNaN(composedForce.x) ? 0 : Mathf.Clamp(composedForce.x, -10000, 10000);//(float.IsInfinity(composedForce.x)?0:composedForce.x);
                    composedForce.y = float.IsNaN(composedForce.y) ? 0 : Mathf.Clamp(composedForce.y, -10000, 10000);//(float.IsInfinity(composedForce.y)?0:composedForce.y);
                    composedForce.z = float.IsNaN(composedForce.z) ? 0 : Mathf.Clamp(composedForce.z, -10000, 10000);//(float.IsInfinity(composedForce.z)?0:composedForce.z);
                    
                    rb.AddForceAtPosition(composedForce, uad.centerPoint);

                    if (doDebug)
                    {
                        Debug.DrawRay(uad.centerPoint,buoyantForce,Color.green);
                        Debug.DrawRay(uad.centerPoint, dragForce, Color.magenta);
                        Debug.DrawRay(uad.centerPoint, localVelocity, Color.cyan);
                        //Debug.DrawRay(uad.centerPoint,uad.areaNormal*10,Color.red);
                        //Debug.DrawRay(uad.centerPoint, composedForce, Color.yellow);
                    }
                }
            }
        }

        // calculatePreciseUnderwaterTris
        private void CalculatePreciseUnderWaterAreas()
        {
            hasAreasAboveWater = false;
            underwaterTriangles.Clear();
            if (meshFilter.mesh.triangles.Length <= 0 || currentWater == null)
            {
                return;
            }
            
            for (int i = 0; i < triangles.Length; i+=3)
            {
                List<VertData> triVertsData = new List<VertData>()
                {
                    new VertData(transform.TransformPoint(vertices[triangles[i]]), 0),
                    new VertData(transform.TransformPoint(vertices[triangles[i + 1]]), 1),
                    new VertData(transform.TransformPoint(vertices[triangles[i + 2]]), 2)
                };
                
                

                var currentWaterTransformY = currentWater.transform.position;

                var rayOriginHeight = currentWaterTransformY.y + currentWater.HeightIntensity * 5f+ 1f ;
                var ray = new Ray(new Vector3(triVertsData[0].pos.x,rayOriginHeight,triVertsData[0].pos.z), Vector3.down);
                // RayCast Vert1
                if (currentWater.meshCollider.Raycast(ray, out var hitV1, currentWater.HeightIntensity * 10f+ 2f))
                    triVertsData[0].WaterHeightAtPos = hitV1.point.y;
                
                // RayCast Vert2
                ray.origin = new Vector3(triVertsData[1].pos.x,rayOriginHeight,triVertsData[1].pos.z);
                if (currentWater.meshCollider.Raycast(ray, out var hitV2, currentWater.HeightIntensity * 10f+ 2f))
                    triVertsData[1].WaterHeightAtPos = hitV2.point.y;
                
                // RayCast Vert3
                ray.origin = new Vector3(triVertsData[2].pos.x,rayOriginHeight,triVertsData[2].pos.z);
                if (currentWater.meshCollider.Raycast(ray, out var hitV3, currentWater.HeightIntensity * 10f+ 2f))
                    triVertsData[2].WaterHeightAtPos = hitV3.point.y;

                // Check which Verts are underwater
                int trisUnderwater = triVertsData.Count(entry => entry.IsUnderwater());


                // Action dependent on how many tris underwater
                switch (trisUnderwater)
                {
                    case 1:
                    {
                        hasAreasAboveWater = true;
                        triVertsData.Sort((comp1, comp2) => comp1.distUnderWater.CompareTo(comp2.distUnderWater)); // Sorts DESC
                        triVertsData.Reverse();
                        // triVertsData: 0 -> under water | 1,2 -> above water (Actual Indexes)
                        
                        var v3 = triVertsData[0].pos; // Bottom (Under Water)

                        var v1OrigIndex = triVertsData[0].originalIndex + 1;
                        v1OrigIndex = (v1OrigIndex > 2) ? 0 : v1OrigIndex;

                        var v1ActualIndex = (v1OrigIndex == triVertsData[1].originalIndex) ? 1 : 2;
                        var v2ActualIndex = (v1ActualIndex == 1) ? 2 : 1;
                        Vector3 v1 = triVertsData[v1ActualIndex].pos; // Top Left (Above Water)
                        Vector3 v2 = triVertsData[v2ActualIndex].pos; // Right (Above Water)
                        
                        // New point @surface between Bottom Vert (V3) & Top Left Vert (V1)
                        var vectorV3V1 = v1 - v3;
                        var vectorMultiplierV3V1 = triVertsData[0].distUnderWater / (triVertsData[0].distUnderWater-triVertsData[v1ActualIndex].distUnderWater); // Approximation for simplicity
                        var vectorV1New = v3 + (vectorV3V1 * vectorMultiplierV3V1); // The new Vertex Position @v1/Top Left
                        
                        // New point @surface between Bottom Vert (V3) & Right Vert (V2)
                        var vectorV3V2 = v2 - v3;
                        var vectorMultiplierV3V2 = triVertsData[0].distUnderWater / (triVertsData[0].distUnderWater-triVertsData[v2ActualIndex].distUnderWater); // Approximation for simplicity
                        var vectorV2New = v3 + (vectorV3V2 * vectorMultiplierV3V2); // The new Vertex Position @v2/Right
                        
                        
                        Triangle tri = new Triangle(v3, vectorV1New, vectorV2New, currentWater);
                        if (waterEntryLevel == null || (waterEntryLevel.position.y < currentWater.transform.position.y || tri.faceNormal.y <= 0)) // TODO Hack: This ignores inside-faces of a "regular shaped" boat. Super Hacky.
                            underwaterTriangles.Add(tri);

                        break;
                    }
                    case 2:
                    {
                        hasAreasAboveWater = true;
                        triVertsData.Sort((comp1, comp2) => comp1.distUnderWater.CompareTo(comp2.distUnderWater)); // Sorts ASC
                        triVertsData.Reverse();
                        // triVertsData: 
                        
                        
                        var v1 = triVertsData[2].pos; // (Above Water)
                        
                        var v2OrigIndex = triVertsData[2].originalIndex - 1;
                        v2OrigIndex = (v2OrigIndex < 0) ? 2 : v2OrigIndex;
                        
                        var v2ActualIndex = (v2OrigIndex == triVertsData[0].originalIndex) ? 0 : 1;
                        var v3ActualIndex = (v2ActualIndex == 0) ? 1 : 0;
                        var v2 = triVertsData[v2ActualIndex].pos; // Left of V1 (Under Water)
                        var v3 = triVertsData[v3ActualIndex].pos; // Right of V1 (Under Water)
                        
                        //Debug.Log(v1.y +" - "+ v2.y +" - "+v3.y);
                        
                        //// Calculate 2 new points @surface Left & Right
                        // New point @surface between V2 (Below Water; Left of V1) & V1 (Above Water)
                        var vectorV2V1 = v1 - v2;
                        var vectorMultiplierV2V1 = triVertsData[v2ActualIndex].distUnderWater / (triVertsData[v2ActualIndex].distUnderWater-triVertsData[2].distUnderWater); // Approximation for simplicity
                        var vectorV1New = v2 + (vectorV2V1 * vectorMultiplierV2V1); // The new Vertex Position @v1/Top
                        
                        // New point @surface between V3 (Below Water; Right of V1) & V1 (Above Water)
                        var vectorV3V1 = v1 - v3;
                        var vectorMultiplierV3V1 = triVertsData[v3ActualIndex].distUnderWater / (triVertsData[v3ActualIndex].distUnderWater-triVertsData[2].distUnderWater); // Approximation for simplicity
                        var vectorV2New = v3 + (vectorV3V1 * vectorMultiplierV3V1); // The new Vertex Position @v2/Left
                        
                        /*Triangle tri1 = new Triangle(vectorV2New,vectorV1New , v2, currentWater); // Left new Tri
                        Triangle tri2 = new Triangle(v3, vectorV2New, v2, currentWater); // Right new Tri*/
                        Triangle tri1 = new Triangle(v2, vectorV1New, vectorV2New, currentWater); // Left new Tri
                        Triangle tri2 = new Triangle(v2, vectorV2New, v3, currentWater); // Right new Tri
                        if (waterEntryLevel == null || (waterEntryLevel.position.y < currentWater.transform.position.y || tri1.faceNormal.y <= 0)) // TODO Hack: This ignores inside-faces of a "regular shaped" boat. Super Hacky.
                            underwaterTriangles.Add(tri1);
                        if (waterEntryLevel == null || (waterEntryLevel.position.y < currentWater.transform.position.y || tri2.faceNormal.y <= 0)) // TODO Hack: This ignores inside-faces of a "regular shaped" boat. Super Hacky.
                            underwaterTriangles.Add(tri2);
                        
                        break;
                    }
                    case 3: // Whole triangle is under water
                    {
                        Triangle tri = new Triangle(triVertsData[0].pos, triVertsData[1].pos, triVertsData[2].pos, currentWater);
                        
                        if (waterEntryLevel == null || (waterEntryLevel.position.y < currentWater.transform.position.y || tri.faceNormal.y <= 0)) // TODO Hack: This ignores inside-faces of a "regular shaped" boat. Super Hacky.
                            underwaterTriangles.Add(tri);
                        
                        break;
                    }
                    case 0:
                    default:
                        hasAreasAboveWater = true;
                        continue;
                    break;
                }
            }
        }

        private Mesh GenerateUnderwaterMesh()
        {
            if (underwaterTriangles == null || underwaterTriangles.Count <= 0)
                return null;
            
            var mesh = new Mesh();

            Vector3[] meshVerts = new Vector3[underwaterTriangles.Count * 3];
            int[] meshTris = new int[underwaterTriangles.Count * 3];

            for (int i = 0; i < underwaterTriangles.Count; i++)
            {
                var curMaxVertIndex = meshVerts.Length - 1;

                meshVerts[i * 3] = transform.InverseTransformPoint(underwaterTriangles[i].v1);
                meshTris[i * 3] = i * 3;
                
                meshVerts[i * 3 + 1] = transform.InverseTransformPoint(underwaterTriangles[i].v2);
                meshTris[i * 3 + 1] = i * 3 + 1;
                
                meshVerts[i * 3 + 2] = transform.InverseTransformPoint(underwaterTriangles[i].v3);
                meshTris[i * 3 + 2] = i * 3 + 2;
            }

            mesh.vertices = meshVerts;
            mesh.triangles = meshTris;
            mesh.RecalculateBounds();

            return mesh;
        }

        private class Triangle
        {
            public readonly Vector3 v1; // Top Left Vert
            public readonly Vector3 v2; // Right Vert
            public readonly Vector3 v3; // Bottom Vert

            public readonly float distUnderWater = 0f;
            public readonly float surfaceArea = 0f;
            public readonly Vector3 centroid; // Center of Tri
            public readonly Vector3 faceNormal; // Directional Vector

            public Triangle(Vector3 pV1, Vector3 pV2, Vector3 pV3, Water currentWater)
            {
                v1 = pV1;
                v2 = pV2;
                v3 = pV3;

                centroid = CalculateTriangleCentroid(v1, v2, v3);
                
                // Get distance under water (From centroid)
                var rayOriginHeight = currentWater.transform.position.y + currentWater.HeightIntensity * 5f+ 1f;
                var ray = new Ray(new Vector3(centroid.x,rayOriginHeight,centroid.z), Vector3.down);
                // RayCast
                if (currentWater.meshCollider.Raycast(ray, out var hit, currentWater.HeightIntensity * 10f+ 2f))
                    distUnderWater = hit.point.y;
                
                faceNormal = Vector3.Cross (v2-v1, v3-v1);

                surfaceArea = faceNormal.magnitude / 2;
            }
        }
        
        private class VertData
        {
            public Vector3 pos;
            public int originalIndex = -1;
            
            private float waterHeightAtPos;

            public float WaterHeightAtPos
            {
                get => waterHeightAtPos;
                set
                {
                    waterHeightAtPos = value;
                    distUnderWater = value - pos.y;
                }
            }

            public float distUnderWater;

            public VertData(Vector3 pPos, int pOriginalIndex)
            {
                pos = pPos;
                originalIndex = pOriginalIndex;
            }

            public bool IsUnderwater()
            {
                return distUnderWater > 0;
            }
        }
        
        private void CalculateUnderWaterAreas()
        {
            Array.Clear(underwaterFacesData, 0, underwaterFacesData.Length);
            if (meshFilter.mesh.triangles.Length <= 0 || currentWater == null)
            {
                return;
            }
            hasAreasAboveWater = false;

            int uwAreaCounter = 0;
            for (int i = 0; i < triangles.Length; i+=3)
            {
                Vector3 areaCenter = CalculateTriangleCentroid(vertices[triangles[i]],vertices[triangles[i+1]],vertices[triangles[i+2]]);
                Vector3 areaCenterWorldPos = transform.TransformPoint(areaCenter);

                float waterHeight = currentWater.transform.position.y;
                // TODO Inperformant?
                RaycastHit hit;
                Ray ray = new Ray(new Vector3(areaCenterWorldPos.x,currentWater.transform.position.y+currentWater.HeightIntensity*5f+ 1f,areaCenterWorldPos.z), Vector3.down);
                /*if(doDebug)
                    Debug.DrawRay(new Vector3(areaCenterWorldPos.x,currentWater.transform.position.y+currentWater.HeightIntensity*5f,areaCenterWorldPos.z), Vector3.down*currentWater.HeightIntensity*10,Color.red);
                */

                if (currentWater.meshCollider.Raycast(ray, out hit, currentWater.HeightIntensity * 10f+ 2f))
                {
                    waterHeight = hit.point.y;
                }
                
                float distanceUnderWater = waterHeight - areaCenterWorldPos.y;

                if (distanceUnderWater > 0)
                {
                    Vector3 areaNormal = Vector3.Cross (vertices[triangles[i+1]]-vertices[triangles[i]], vertices[triangles[i+2]]-vertices[triangles[i]]);
                    
                    Vector3 areaNormalWorld = transform.TransformDirection(areaNormal);

                    if (waterEntryLevel == null || (waterEntryLevel.position.y < currentWater.transform.position.y || areaNormalWorld.y <= 0)) // TODO Hack: This ignores inside-faces of a "regular shaped" boat. Super Hacky.
                    {
                        UnderwaterAreaData uad = new UnderwaterAreaData((areaNormal.magnitude/2),distanceUnderWater, areaNormalWorld, areaCenterWorldPos);
                        underwaterFacesData[uwAreaCounter] = uad;
                        uwAreaCounter++;
                    }
                    else
                    {
                        hasAreasAboveWater = true;
                    }
                }
                else
                {
                    hasAreasAboveWater = true;
                }
            }
        }

        private void OnDrawGizmos()
        {
            if (calculatePreciseUnderwaterTris && underwaterTriangles != null && underwaterTriangles.Count > 0)
            {
                foreach (var triangle in underwaterTriangles)
                {
                    Gizmos.color = Color.magenta;
                    Gizmos.DrawSphere(triangle.centroid, 0.01f);
                }
            }
            else if (triangles != null && triangles.Length > 0)
            {
                for (int i = 0; i < triangles.Length; i+=3)
                {
                    Gizmos.color = Color.magenta;
                    Vector3 center = transform.TransformPoint(CalculateTriangleCentroid(vertices[triangles[i]],vertices[triangles[i+1]],vertices[triangles[i+2]]));
                    Gizmos.DrawSphere(center, 0.005f);
                }
            }
            
        }

        private static Vector3 CalculateTriangleCentroid(Vector3 corner1, Vector3 corner2, Vector3 corner3)
        {
            Vector3 centroid = Vector3.zero;

            centroid.x = (corner1.x + corner2.x + corner3.x) / 3;
            centroid.y = (corner1.y + corner2.y + corner3.y) / 3;
            centroid.z = (corner1.z + corner2.z + corner3.z) / 3;

            return centroid;
        }
        
        private class UnderwaterAreaData
        {
            public float surfaceArea = 0;
            public float distanceToSurface = 0f;
            public Vector3 areaNormal = Vector3.zero;
            public Vector3 centerPoint = Vector3.zero;
            
            public UnderwaterAreaData(float pSurfaceArea, float pDistanceToSurface, Vector3 pAreaNormal, Vector3 pCenterPoint)
            {
                surfaceArea = pSurfaceArea;
                distanceToSurface = pDistanceToSurface;
                areaNormal = pAreaNormal;
                centerPoint = pCenterPoint;
            }
        }

        private void OnTriggerEnter(Collider col)
        {
            Water w = col.GetComponent<Water>();
            if (w != null)
            {
                currentWater = w;
            }
        }

        public bool IsInWater()
        {
            if (calculatePreciseUnderwaterTris)
                return underwaterTriangles?.Count > 0;
            else
                return underwaterFacesData?.Length > 0;
        }

        public bool IsUnderWater()
        { 
            return !hasAreasAboveWater;
        }
    }
}