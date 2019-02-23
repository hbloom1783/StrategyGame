using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace MeshTest
{
    [RequireComponent(typeof(MeshFilter))]
    [RequireComponent(typeof(MeshRenderer))]
    [ExecuteInEditMode]
    public abstract class ProcMesh : MonoBehaviour
    {
        void Start()
        {
            RegenMesh();
        }

        public void RegenMesh()
        {
            GetComponent<MeshFilter>().sharedMesh = GetMesh();
            GetComponent<MeshFilter>().sharedMesh.name = "ProcMesh";
        }

        public abstract Mesh GetMesh();
    }
}
