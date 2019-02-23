using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEditor;
using System.Linq;

namespace MeshTest
{
    public class CellSideMesh : ProcMesh
    {
        public float outerRadius = 1.0f;
        public float blockHeight = 0.5f;
        public int blockCount = 1;

        // Start is called before the first frame update
        public override Mesh GetMesh()
        {
            Mesh mesh = new Mesh();
            mesh.Clear();

            List<Vector3> vertices = new List<Vector3>();
            List<Vector3> normals = new List<Vector3>();
            List<Vector2> uvs = new List<Vector2>();
            List<int> triangles = new List<int>();

            foreach (int block in Enumerable.Range(0, blockCount))
            {
                #region Vertices

                Vector3 p0 = Vector3.forward * outerRadius;
                Vector3 p1 = Quaternion.AngleAxis(60, Vector3.up) * p0;
                Vector3 p2 = p0 + (Vector3.down * blockHeight);
                Vector3 p3 = Quaternion.AngleAxis(60, Vector3.up) * p2;

                Vector3 blockSpaceOffset = blockHeight * block * Vector3.down;
                
                Vector3[] blockVertices =
                {
                    p0 + blockSpaceOffset,
                    p1 + blockSpaceOffset,
                    p2 + blockSpaceOffset,
                    p3 + blockSpaceOffset,
                };

                vertices.AddRange(blockVertices);

                #endregion

                #region Normals

                Vector3 n0 = Quaternion.AngleAxis(210, Vector3.up) * Vector3.forward;

                Vector3[] blockNormals =
                {
                    n0,
                    n0,
                    n0,
                    n0,
                };

                normals.AddRange(blockNormals);

                #endregion

                #region UVs

                Vector2[] blockUvs =
                {
                    new Vector2(1.00f, 1.00f),
                    new Vector2(0.00f, 1.00f),
                    new Vector2(1.00f, 0.00f),
                    new Vector2(0.00f, 0.00f),
                };

                uvs.AddRange(blockUvs);

                #endregion

                #region Triangles

                int blockCountOffset = block * 4;

                int[] blockTriangles = new int[]
                {
                    blockCountOffset + 0, blockCountOffset + 2, blockCountOffset + 1,
                    blockCountOffset + 1, blockCountOffset + 2, blockCountOffset + 3,
                };

                triangles.AddRange(blockTriangles);

                #endregion
            }

            mesh.vertices = vertices.ToArray();
            mesh.normals = normals.ToArray();
            mesh.uv = uvs.ToArray();
            mesh.triangles = triangles.ToArray();

            mesh.RecalculateBounds();

            return mesh;
        }
    }

    [CustomEditor(typeof(CellSideMesh))]
    [CanEditMultipleObjects]
    public class CellSideMeshEditor : Editor
    {
        SerializedProperty outerRadius;
        SerializedProperty blockHeight;
        SerializedProperty blockCount;

        void OnEnable()
        {
            outerRadius = serializedObject.FindProperty("outerRadius");
            blockHeight = serializedObject.FindProperty("blockHeight");
            blockCount = serializedObject.FindProperty("blockCount");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            EditorGUILayout.PropertyField(outerRadius);
            EditorGUILayout.PropertyField(blockHeight);
            EditorGUILayout.PropertyField(blockCount);
            serializedObject.ApplyModifiedProperties();

            if (GUI.changed) foreach (CellSideMesh t in targets) t.RegenMesh();
        }
    }
}