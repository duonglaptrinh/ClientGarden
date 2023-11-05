using Game.Client.Extension;
using UnityEngine;

namespace TWT.Client
{
    public class LineObject : MonoBehaviour
    {
        private LineRenderer lineRenderer;

        public LineRenderer LineRenderer => lineRenderer ? lineRenderer : lineRenderer = GetComponent<LineRenderer>();

        private void Start()
        {
            FinishDrawLine();
        }

        public void FinishDrawLine()
        {
            var meshFilter = gameObject.GetOrAddComponent<MeshFilter>();
            var mesh = new Mesh();
            LineRenderer.BakeMesh(mesh);
            meshFilter.sharedMesh = mesh;
            
            var meshRenderer = gameObject.GetOrAddComponent<MeshRenderer>();
            meshRenderer.sharedMaterial = lineRenderer.material;
            
            Destroy(LineRenderer);
        }
    }
}