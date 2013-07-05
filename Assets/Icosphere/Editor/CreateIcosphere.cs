using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// This class creates a polygonal sphere of varying smoothness from a
/// base polyhedran shape and a desired number of triangular subdivisions.
/// Created by Paul C. Isaac @ December 2012
/// </summary>
public class CreateIcosphere : ScriptableWizard
{
    public float Radius = 1.0f;
    public int Subdivisions = 0;
	
	public enum Shape
	{
		Tetrahedron,	// 4-sides
		Cuboid,			// 6-sides
		Octohedron,		// 8-sides
		Icosahedron,	// 20-sides
	};
	
	public Shape BaseShape = Shape.Icosahedron;
	
	class MeshData
	{
		public List<Vector3> VertexList = new List<Vector3>();
		public List<int> IndexList = new List<int>();
		
		public MeshData()
		{
			VertexList = new List<Vector3>();
			IndexList = new List<int>();
		}
		
		public void AddVertex(float a, float b, float c)
		{
			VertexList.Add( new Vector3(a,b,c) );
		}
		
		public void AddTriangle(int a, int b, int c)
		{
			IndexList.Add(a);
			IndexList.Add(b);
			IndexList.Add(c);
		}
	}
	
    [MenuItem ("GameObject/Create Other/N-gon Sphere")]
    static void CreateWizard()
    {
        ScriptableWizard.DisplayWizard("Polyhedron Sphere", typeof(CreateIcosphere));
    }
 
    void OnWizardCreate()
    {
		string name = BaseShape.ToString();
		if (Subdivisions > 0) name += string.Format("x{0}",Subdivisions);
        GameObject sphere = new GameObject( name );
		
		AddIcosphere(sphere);
		
        AddSphereCollider(sphere.transform, new Vector3(0, 0f, 0f), Quaternion.identity, Radius);
 
        Selection.activeObject = sphere;
    }
 
    void AddSphereCollider(Transform parent, Vector3 position, Quaternion rotation, float radius)
    {
        GameObject go = new GameObject("Sphere Collider");
        SphereCollider sphereCollider = go.AddComponent(typeof(SphereCollider)) as SphereCollider;
        sphereCollider.radius = radius;
        go.transform.position = position;
        go.transform.rotation = rotation;
        go.transform.parent = parent;
    }
	
	int GetMiddlePoint (List<Vector3> vertices, int p1, int p2, Dictionary<long,int> indexCache)
	{
        bool lessThan = p1 < p2;
        long upper = lessThan ? p1 : p2;
        long lower = lessThan ? p2 : p1;
        long key = (upper << 32) + lower;
 
        int result;
        if (indexCache.TryGetValue(key, out result))
        {
            return result;
        }
 
        // calculate new mid-point
        Vector3 middle = (vertices[p1] + vertices[p2]) / 2.0f;
 
		// normalize onto sphere
		middle = middle * (Radius / middle.magnitude);
		
        // add vertex to list
        int i = vertices.Count;
		vertices.Add(middle);
 
        // cache the index
        indexCache.Add(key, i);
        return i;
	}
	
	void Icosahedron(MeshData mesh)
	{
		float a = 1;
		float b = (1.0f + Mathf.Sqrt(5.0f)) / 2.0f;
		float c = 0;
		float scale = Radius/Mathf.Sqrt(a*a + b*b + c*c);
		a = a*scale;
		b = b*scale;
		c = c*scale;
		
		// 12 vertices of the icosahedron
        mesh.AddVertex( -a,  b,  c);
        mesh.AddVertex(  a,  b,  c);
        mesh.AddVertex( -a, -b,  c);
        mesh.AddVertex(  a, -b,  c);
        mesh.AddVertex(  c, -a,  b);
        mesh.AddVertex(  c,  a,  b);
        mesh.AddVertex(  c, -a, -b);
        mesh.AddVertex(  c,  a, -b);
        mesh.AddVertex(  b,  c, -a);
        mesh.AddVertex(  b,  c,  a);
        mesh.AddVertex( -b,  c, -a);
        mesh.AddVertex( -b,  c,  a);
		
        // 20 triangles of the icosahedron
		mesh.AddTriangle( 0, 11, 5);
		mesh.AddTriangle( 0, 5, 1);
		mesh.AddTriangle( 0, 1, 7);
		mesh.AddTriangle( 0, 7, 10);
		mesh.AddTriangle( 0, 10, 11);
        mesh.AddTriangle( 1, 5, 9);
        mesh.AddTriangle( 5, 11, 4);
        mesh.AddTriangle( 11, 10, 2);
        mesh.AddTriangle( 10, 7, 6);
        mesh.AddTriangle( 7, 1, 8);
        mesh.AddTriangle( 3, 9, 4);
        mesh.AddTriangle( 3, 4, 2);
        mesh.AddTriangle( 3, 2, 6);
        mesh.AddTriangle( 3, 6, 8);
        mesh.AddTriangle( 3, 8, 9);
        mesh.AddTriangle( 4, 9, 5);
        mesh.AddTriangle( 2, 4, 11);
        mesh.AddTriangle( 6, 2, 10);
        mesh.AddTriangle( 8, 6, 7);
        mesh.AddTriangle( 9, 8, 1);
	}

	void Tetrahedron(MeshData mesh)
	{
		float a = 1.0f;
		float b = 1.0f/Mathf.Sqrt(2);
		float c = Radius/Mathf.Sqrt(a*a + b*b);
		a = 0.67f*Radius*a/c;
		b = 0.67f*Radius*b/c;
		
		// 4 vertices of the tetra-hedron
        mesh.AddVertex( +a,0,-b);
        mesh.AddVertex( -a,0,-b);
        mesh.AddVertex( 0,+a, b);
        mesh.AddVertex( 0,-a, b);
		
        // 4 triangles of the tetra-ahedron
		mesh.AddTriangle( 0, 1, 2);
		mesh.AddTriangle( 1, 3, 2);
		mesh.AddTriangle( 0, 2, 3);
		mesh.AddTriangle( 0, 3, 1);
	}

	void Octahedron(MeshData mesh)
	{
		// 6 vertices of the octa-hedron
        mesh.AddVertex(0, -Radius, 0);
        mesh.AddVertex(-Radius, 0, 0);
        mesh.AddVertex(0, 0, -Radius);
        mesh.AddVertex(+Radius, 0, 0);
        mesh.AddVertex(0, 0, +Radius);
        mesh.AddVertex(0,  Radius, 0);
		
        // 8 triangles of the octa-ahedron
		mesh.AddTriangle( 0, 1, 2);
		mesh.AddTriangle( 0, 2, 3);
		mesh.AddTriangle( 0, 3, 4);
		mesh.AddTriangle( 0, 4, 1);
		mesh.AddTriangle( 5, 2, 1);
        mesh.AddTriangle( 5, 3, 2);
        mesh.AddTriangle( 5, 4, 3);
        mesh.AddTriangle( 5, 1, 4);
	}
	
	// Note: The cube faces are split into 4 nearly-equilateral triangles to make it compatible with subdivision.
	void Cuboid(MeshData mesh)
	{
		float c = Radius;
		float r = Radius/Mathf.Sqrt(3);
		
		// 6+8 vertices of the cuboid
        mesh.AddVertex( 0, c, 0);
        mesh.AddVertex( 0,-c, 0);
        mesh.AddVertex( c, 0, 0);
        mesh.AddVertex(-c, 0, 0);
        mesh.AddVertex( 0, 0, c);
        mesh.AddVertex( 0, 0,-c);
		
        mesh.AddVertex(-r, r, r);
        mesh.AddVertex(-r, r,-r);
        mesh.AddVertex( r, r,-r);
        mesh.AddVertex( r, r, r);
		
        mesh.AddVertex(-r,-r, r);
        mesh.AddVertex(-r,-r,-r);
        mesh.AddVertex( r,-r,-r);
        mesh.AddVertex( r,-r, r);

        // 6*4 triangles of the cuboid
		mesh.AddTriangle( 0, 7, 6);
		mesh.AddTriangle( 0, 8, 7);
		mesh.AddTriangle( 0, 9, 8);
		mesh.AddTriangle( 0, 6, 9);

		mesh.AddTriangle( 1,10,11);
        mesh.AddTriangle( 1,11,12);
        mesh.AddTriangle( 1,12,13);
        mesh.AddTriangle( 1,13,10);

		mesh.AddTriangle( 2, 8, 9);
		mesh.AddTriangle( 2, 9, 13);
		mesh.AddTriangle( 2, 13, 12);
		mesh.AddTriangle( 2, 12, 8);
		
		mesh.AddTriangle( 3, 6, 7);
		mesh.AddTriangle( 3, 7, 11);
		mesh.AddTriangle( 3, 11, 10);
		mesh.AddTriangle( 3, 10, 6);

		mesh.AddTriangle( 4, 9, 6);
		mesh.AddTriangle( 4,13, 9);
		mesh.AddTriangle( 4,10,13);
		mesh.AddTriangle( 4, 6,10);
		
		mesh.AddTriangle( 5, 7, 8);
		mesh.AddTriangle( 5, 8,12);
		mesh.AddTriangle( 5,12,11);
		mesh.AddTriangle( 5,11, 7);
	}
	
	static void AddTriangle(List<int> indexList, int a, int b, int c)
	{
		indexList.Add(a);
		indexList.Add(b);
		indexList.Add(c);
	}
	
	public void AddIcosphere(GameObject parent)
	{
		MeshData data = new MeshData();

		switch (BaseShape)
		{
			default:
			case Shape.Tetrahedron:
				Tetrahedron(data);
				break;
			case Shape.Cuboid:
				Cuboid(data);
				break;
			case Shape.Icosahedron:
				Icosahedron(data);
				break;
			case Shape.Octohedron:
				Octahedron(data);
				break;
		}
		
		Dictionary<long,int> indexCache;
		indexCache = new Dictionary<long, int>();
		
		for (int i=0; i<Subdivisions; ++i)
		{
			List<int> ilist = new List<int>();
			for (int f=0; f<data.IndexList.Count; f+=3)
			{
				int v1 = data.IndexList[f+0];
				int v2 = data.IndexList[f+1];
				int v3 = data.IndexList[f+2];
				
				int va = GetMiddlePoint(data.VertexList, v1,v2, indexCache);
				int vb = GetMiddlePoint(data.VertexList, v2,v3, indexCache);
				int vc = GetMiddlePoint(data.VertexList, v3,v1, indexCache);
				
				AddTriangle(ilist, v1,va,vc);
				AddTriangle(ilist, v2,vb,va);
				AddTriangle(ilist, v3,vc,vb);
				AddTriangle(ilist, va,vb,vc);
				
				if (data.VertexList.Count > 64950)
				{
					UnityEngine.Debug.LogError("Subdivisions is too large => Mesh is limited to 65000 vertices!");
					break;
				}
			}
			data.IndexList = ilist;
		}
		
        Mesh mesh = new Mesh();

        mesh.vertices = data.VertexList.ToArray();
        mesh.triangles = data.IndexList.ToArray();
		
		// Unwrap coordinates onto a UV plane
		Vector2[] uvs = new Vector2[ mesh.vertices.Length ];
		for (int i=0; i<uvs.Length; ++i)
		{
			float x = 0.5f * (mesh.vertices[i].x + Radius) / Radius;
			float y = 0.5f * (mesh.vertices[i].y + Radius) / Radius;
			uvs[i] = new Vector2(x,y);
		}
		mesh.uv = uvs;
		
        mesh.RecalculateBounds();
        mesh.Optimize();
		mesh.RecalculateNormals();
		
        MeshFilter mFilter = parent.GetComponent(typeof(MeshFilter)) as MeshFilter;
		if (mFilter == null)
		{
			mFilter = parent.AddComponent( typeof(MeshFilter) ) as MeshFilter;
		}
        mFilter.mesh = mesh;
		parent.AddComponent(typeof(MeshRenderer));
		parent.renderer.sharedMaterial = new Material(Shader.Find("Diffuse"));
		
		UnityEngine.Debug.Log("Sphere generated from "+BaseShape.ToString());
		UnityEngine.Debug.Log("# vertices = "+mesh.vertexCount.ToString());
		UnityEngine.Debug.Log("# triangles = "+mesh.triangles.Length/3);
	}
}