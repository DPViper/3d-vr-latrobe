using UnityEngine;
using System.IO;
using System.Collections.Generic;

namespace LaTrobeVR
{
    public class SplatModelConverter : MonoBehaviour
    {
        [Header("Conversion Settings")]
        public string splatFilesPath = "Assets/Models/Splats/";
        public string outputPath = "Assets/Models/Converted/";
        public float pointSize = 0.1f;
        public int maxPoints = 100000; // Limit for performance
        
        [Header("Material Settings")]
        public Material splatMaterial;
        public bool useInstancing = true;
        
        [System.Serializable]
        public class SplatPoint
        {
            public Vector3 position;
            public Vector3 scale;
            public Quaternion rotation;
            public Color color;
        }
        
        [ContextMenu("Convert All Splat Files")]
        public void ConvertAllSplatFiles()
        {
            if (!Directory.Exists(splatFilesPath))
            {
                Debug.LogError($"Splat files path does not exist: {splatFilesPath}");
                return;
            }
            
            string[] splatFiles = Directory.GetFiles(splatFilesPath, "*.splat");
            
            foreach (string splatFile in splatFiles)
            {
                ConvertSplatFile(splatFile);
            }
        }
        
        public void ConvertSplatFile(string filePath)
        {
            Debug.Log($"Converting splat file: {filePath}");
            
            try
            {
                List<SplatPoint> points = ReadSplatFile(filePath);
                GameObject convertedModel = CreateUnityModel(points, Path.GetFileNameWithoutExtension(filePath));
                
                // Save as prefab
                string prefabPath = Path.Combine(outputPath, $"{Path.GetFileNameWithoutExtension(filePath)}.prefab");
                SaveAsPrefab(convertedModel, prefabPath);
                
                Debug.Log($"Successfully converted: {filePath} -> {prefabPath}");
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Error converting {filePath}: {e.Message}");
            }
        }
        
        List<SplatPoint> ReadSplatFile(string filePath)
        {
            List<SplatPoint> points = new List<SplatPoint>();
            
            using (FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            using (BinaryReader reader = new BinaryReader(fs))
            {
                // Read splat file header (this is a simplified version)
                // Real splat files have more complex headers
                int pointCount = (int)(fs.Length / 32); // Assuming 32 bytes per point
                
                // Limit points for performance
                pointCount = Mathf.Min(pointCount, maxPoints);
                
                for (int i = 0; i < pointCount; i++)
                {
                    SplatPoint point = new SplatPoint();
                    
                    // Read position (3 floats)
                    point.position = new Vector3(
                        reader.ReadSingle(),
                        reader.ReadSingle(),
                        reader.ReadSingle()
                    );
                    
                    // Read scale (3 floats)
                    point.scale = new Vector3(
                        reader.ReadSingle(),
                        reader.ReadSingle(),
                        reader.ReadSingle()
                    );
                    
                    // Read rotation (4 floats as quaternion)
                    point.rotation = new Quaternion(
                        reader.ReadSingle(),
                        reader.ReadSingle(),
                        reader.ReadSingle(),
                        reader.ReadSingle()
                    );
                    
                    // Read color (4 floats as RGBA)
                    point.color = new Color(
                        reader.ReadSingle(),
                        reader.ReadSingle(),
                        reader.ReadSingle(),
                        reader.ReadSingle()
                    );
                    
                    // --- Robust Quaternion Validation ---
                     bool invalid =
                        float.IsNaN(point.rotation.x) || float.IsNaN(point.rotation.y) ||
                        float.IsNaN(point.rotation.z) || float.IsNaN(point.rotation.w) ||
                        float.IsInfinity(point.rotation.x) || float.IsInfinity(point.rotation.y) ||
                        float.IsInfinity(point.rotation.z) || float.IsInfinity(point.rotation.w) ||
                        IsQuaternionAllZero(point.rotation);

                    if (invalid)
                    {
                        Debug.LogWarning($"[SplatModelConverter] Skipped invalid quaternion at index {i}: {point.rotation}");
                        continue;
                    }
                    point.rotation = Quaternion.Normalize(point.rotation);
                    
                    // Only add valid points
                    points.Add(point);
                }
            }
            Debug.Log($"[SplatModelConverter] Loaded {points.Count} valid points.");
            return points;
        }
        
        GameObject CreateUnityModel(List<SplatPoint> points, string modelName)
        {
            GameObject modelRoot = new GameObject(modelName);
            
            if (useInstancing && points.Count > 1000)
            {
                CreateInstancedModel(points, modelRoot);
            }
            else
            {
                CreateIndividualObjects(points, modelRoot);
            }
            
            return modelRoot;
        }
        
        void CreateInstancedModel(List<SplatPoint> points, GameObject parent)
        {
            // Create a mesh for instancing
            Mesh splatMesh = CreateSplatMesh();
            
            // Create material instance
            Material instanceMaterial = new Material(splatMaterial);
            
            // Create matrices for instancing
            Matrix4x4[] matrices = new Matrix4x4[points.Count];
            Vector4[] colors = new Vector4[points.Count];
            
            int validCount = 0;
            for (int i = 0; i < points.Count; i++)
            {
                SplatPoint point = points[i];
                
                // FINAL quaternion check
                if (IsQuaternionAllZero(point.rotation) ||
                    float.IsNaN(point.rotation.x) || float.IsNaN(point.rotation.y) ||
                    float.IsNaN(point.rotation.z) || float.IsNaN(point.rotation.w) ||
                    float.IsInfinity(point.rotation.x) || float.IsInfinity(point.rotation.y) ||
                    float.IsInfinity(point.rotation.z) || float.IsInfinity(point.rotation.w))
                {
                    Debug.LogWarning($"[SplatModelConverter] Skipped invalid quaternion during instancing at index {i}: {point.rotation}");
                    continue;
                }
                
                // Create transformation matrix
                Matrix4x4 matrix = Matrix4x4.TRS(
                    point.position,
                    point.rotation,
                    point.scale * pointSize
                );
                
                matrices[validCount] = matrix;
                colors[validCount] = point.color;
                validCount++;
            }
            
            // Resize arrays to only include valid points
            System.Array.Resize(ref matrices, validCount);
            System.Array.Resize(ref colors, validCount);
            
            // Add component to handle instanced rendering
            SplatInstancedRenderer renderer = parent.AddComponent<SplatInstancedRenderer>();
            renderer.Initialize(splatMesh, instanceMaterial, matrices, colors);
        }
        
        void CreateIndividualObjects(List<SplatPoint> points, GameObject parent)
        {
            for (int i = 0; i < points.Count; i++)
            {
                SplatPoint point = points[i];
                
                // FINAL quaternion check
                if (IsQuaternionAllZero(point.rotation) ||
                    float.IsNaN(point.rotation.x) || float.IsNaN(point.rotation.y) ||
                    float.IsNaN(point.rotation.z) || float.IsNaN(point.rotation.w) ||
                    float.IsInfinity(point.rotation.x) || float.IsInfinity(point.rotation.y) ||
                    float.IsInfinity(point.rotation.z) || float.IsInfinity(point.rotation.w))
                {
                    Debug.LogWarning($"[SplatModelConverter] Skipped invalid quaternion during GameObject creation at index {i}: {point.rotation}");
                    continue;
                }
                
                Debug.Log($"Creating Splat_{i} at {point.position} with scale {point.scale * pointSize} and rotation {point.rotation}");
                
                GameObject splatObj = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                splatObj.name = $"Splat_{i}";
                splatObj.transform.SetParent(parent.transform);
                
                // Set position, rotation, and scale
                splatObj.transform.position = point.position;
                splatObj.transform.rotation = point.rotation;
                
                Vector3 safeScale = point.scale * pointSize;
                safeScale.x = Mathf.Clamp(safeScale.x, 0.01f, 10f);
                safeScale.y = Mathf.Clamp(safeScale.y, 0.01f, 10f);
                safeScale.z = Mathf.Clamp(safeScale.z, 0.01f, 10f);
                splatObj.transform.localScale = safeScale;
                
                // Set material and color
                Renderer renderer = splatObj.GetComponent<Renderer>();
                if (splatMaterial != null)
                {
                    Material instanceMaterial = new Material(splatMaterial);
                    instanceMaterial.color = point.color;
                    renderer.material = instanceMaterial;
                }
            }
        }
        
        Mesh CreateSplatMesh()
        {
            // Create a simple sphere mesh for splat points
            Mesh mesh = new Mesh();
            
            // Generate sphere vertices and triangles
            int resolution = 8; // Low resolution for performance
            List<Vector3> vertices = new List<Vector3>();
            List<int> triangles = new List<int>();
            
            // Generate sphere geometry
            for (int lat = 0; lat <= resolution; lat++)
            {
                float theta = lat * Mathf.PI / resolution;
                float sinTheta = Mathf.Sin(theta);
                float cosTheta = Mathf.Cos(theta);
                
                for (int lon = 0; lon <= resolution; lon++)
                {
                    float phi = lon * 2 * Mathf.PI / resolution;
                    float sinPhi = Mathf.Sin(phi);
                    float cosPhi = Mathf.Cos(phi);
                    
                    float x = cosPhi * sinTheta;
                    float y = cosTheta;
                    float z = sinPhi * sinTheta;
                    
                    vertices.Add(new Vector3(x, y, z));
                }
            }
            
            // Generate triangles
            for (int lat = 0; lat < resolution; lat++)
            {
                for (int lon = 0; lon < resolution; lon++)
                {
                    int current = lat * (resolution + 1) + lon;
                    int next = current + resolution + 1;
                    
                    triangles.Add(current);
                    triangles.Add(next);
                    triangles.Add(current + 1);
                    
                    triangles.Add(next);
                    triangles.Add(next + 1);
                    triangles.Add(current + 1);
                }
            }
            
            mesh.vertices = vertices.ToArray();
            mesh.triangles = triangles.ToArray();
            mesh.RecalculateNormals();
            
            return mesh;
        }
        
        void SaveAsPrefab(GameObject model, string prefabPath)
        {
            // Ensure output directory exists
            Directory.CreateDirectory(Path.GetDirectoryName(prefabPath));
            
            // Save as prefab (this would require Unity Editor API)
            #if UNITY_EDITOR
            UnityEditor.PrefabUtility.SaveAsPrefabAsset(model, prefabPath);
            #endif
        }

        private bool IsQuaternionAllZero(Quaternion q)
        {
            return Mathf.Abs(q.x) < 1e-6f &&
                   Mathf.Abs(q.y) < 1e-6f &&
                   Mathf.Abs(q.z) < 1e-6f &&
                   Mathf.Abs(q.w) < 1e-6f;
        }
    }
    
    // Component for handling instanced rendering
    public class SplatInstancedRenderer : MonoBehaviour
    {
        private Mesh mesh;
        private Material material;
        private Matrix4x4[] matrices;
        private Vector4[] colors;
        private MaterialPropertyBlock propertyBlock;
        
        public void Initialize(Mesh mesh, Material material, Matrix4x4[] matrices, Vector4[] colors)
        {
            this.mesh = mesh;
            this.material = material;
            this.matrices = matrices;
            this.colors = colors;
            this.propertyBlock = new MaterialPropertyBlock();
        }
        
        void Update()
        {
            if (mesh != null && material != null && matrices != null)
            {
                // Set color property
                propertyBlock.SetVectorArray("_Colors", colors);
                
                // Render instanced
                Graphics.DrawMeshInstanced(mesh, 0, material, matrices, matrices.Length, propertyBlock);
            }
        }
    }
} 