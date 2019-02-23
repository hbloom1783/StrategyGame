using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEditor;

namespace MeshTest
{
    public class CellTopMesh : ProcMesh
    {
        public float outerRadius = 1.0f;

        // Start is called before the first frame update
        public override Mesh GetMesh()
        {
            Mesh mesh = new Mesh();
            mesh.Clear();

            Vector3 p0 = Vector3.forward * outerRadius;
            Vector3 p1 = Quaternion.AngleAxis(60, Vector3.up) * p0;
            Vector3 p2 = Quaternion.AngleAxis(60, Vector3.up) * p1;
            Vector3 p3 = Quaternion.AngleAxis(60, Vector3.up) * p2;
            Vector3 p4 = Quaternion.AngleAxis(60, Vector3.up) * p3;
            Vector3 p5 = Quaternion.AngleAxis(60, Vector3.up) * p4;


            Vector3[] vertices = new Vector3[]
            {
                p0,
                p1,
                p2,
                p3,
                p4,
                p5,
            };

            Vector3[] normals = new Vector3[]
            {
                Vector3.up,
                Vector3.up,
                Vector3.up,
                Vector3.up,
                Vector3.up,
                Vector3.up,
            };

            Vector2[] uvs = new Vector2[]
            {
                new Vector2(0.50f, 1.00f),
                new Vector2(1.00f, 0.75f),
                new Vector2(1.00f, 0.25f),
                new Vector2(0.50f, 0.00f),
                new Vector2(0.00f, 0.25f),
                new Vector2(0.00f, 0.75f),
            };
            
            int[] triangles = new int[]
            {
		        0, 1, 3,
                1, 2, 3,
                0, 3, 4,
                0, 4, 5,
            };

            mesh.vertices = vertices;
            mesh.normals = normals;
            mesh.uv = uvs;
            mesh.triangles = triangles;

            mesh.RecalculateBounds();

            return mesh;
        }
    }

    [CustomEditor(typeof(CellTopMesh))]
    [CanEditMultipleObjects]
    public class CellTopMeshEditor : Editor
    {
        SerializedProperty outerRadius;

        void OnEnable()
        {
            outerRadius = serializedObject.FindProperty("outerRadius");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            EditorGUILayout.PropertyField(outerRadius);
            serializedObject.ApplyModifiedProperties();

            if (GUI.changed) foreach (CellTopMesh t in targets) t.RegenMesh();
        }
    }
}