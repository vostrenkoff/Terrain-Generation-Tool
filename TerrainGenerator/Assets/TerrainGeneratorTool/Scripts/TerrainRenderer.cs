using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TerrainGen
{
    public class TerrainRenderer : MonoBehaviour
    {
        private MeshRenderer meshRenderer;
        private MeshFilter meshFilter;
        private Mesh mesh;

        public float _terrainHeight;
        public Texture2D _heightMap;
        public Shader shader;

        void Start()
        {
            if (_heightMap == null)
            {
                Debug.Log("Heightmap is not assigned!");
                return;
            }

            meshRenderer = gameObject.AddComponent<MeshRenderer>();
            meshRenderer.sharedMaterial = new Material(shader);
            meshFilter = gameObject.AddComponent<MeshFilter>();

            mesh = new Mesh();
            mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
            getMeshData(out Vector3[] vertices, out Color[] colors, out int[] triangles);
            mesh.SetVertices(vertices);
            mesh.SetTriangles(triangles, 0);
            mesh.RecalculateNormals();
            mesh.SetColors(colors);
            meshFilter.mesh = mesh;
            meshRenderer.sharedMaterial.SetFloat("vMultiplier", _terrainHeight);
        }

        void Update()
        {
            updateMeshData();
        }

        void updateMeshData()
        {
            meshRenderer.sharedMaterial.SetFloat("vMultiplier", _terrainHeight);
            meshFilter.mesh.RecalculateNormals();
        }

        void getMeshData(out Vector3[] vertices, out Color[] colors, out int[] triangles)
        {
            vertices = new Vector3[_heightMap.height * _heightMap.width];
            colors = new Color[_heightMap.height * _heightMap.width];
            List<int> trianglesList = new List<int>();

            for (int y = 0; y < _heightMap.height; y++)
            {
                for (int x = 0; x < _heightMap.width; x++)
                {
                    float vertexHeight = _heightMap.GetPixel(x, y).grayscale;
                    vertices[x + y * _heightMap.width] = new Vector3(x - _heightMap.width / 2, vertexHeight, y - _heightMap.height / 2);

                    colors[x + y * _heightMap.width] = new Color(Mathf.Clamp(2f * vertexHeight, 0f, 1f), Mathf.Clamp(2f * (1f - vertexHeight), 0f, 1f), Mathf.Pow(1f - vertexHeight, 3f));

                    if (x > 0 && y > 0)
                    {
                        trianglesList.Add(x + (y - 1) * _heightMap.width);
                        trianglesList.Add(x - 1 + (y - 1) * _heightMap.width);
                        trianglesList.Add(x - 1 + y * _heightMap.width);

                        trianglesList.Add(x - 1 + y * _heightMap.width);
                        trianglesList.Add(x + y * _heightMap.width);
                        trianglesList.Add(x + (y - 1) * _heightMap.width);
                    }
                }
            }

            triangles = trianglesList.ToArray();
        }
    }
}