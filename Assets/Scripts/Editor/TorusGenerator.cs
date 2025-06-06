using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

public class TorusGenerator : EditorWindow
{
    float majorRadius = 1f;
    float minorRadius = 0.3f;
    int majorSegments = 24;
    int minorSegments = 12;
    Material material;
    string objectName = "Torus";

    [MenuItem("Tools/Torus Generator")]
    public static void ShowWindow()
    {
        GetWindow<TorusGenerator>("Torus Generator");
    }

    void OnGUI()
    {
        GUILayout.Label("Torus Settings", EditorStyles.boldLabel);
        majorRadius = EditorGUILayout.FloatField("Major Radius", majorRadius);
        minorRadius = EditorGUILayout.FloatField("Minor Radius", minorRadius);
        majorSegments = EditorGUILayout.IntSlider("Major Segments", majorSegments, 3, 128);
        minorSegments = EditorGUILayout.IntSlider("Minor Segments", minorSegments, 3, 64);
        material = (Material)EditorGUILayout.ObjectField("Material", material, typeof(Material), false);
        objectName = EditorGUILayout.TextField("Object Name", objectName);

        if (GUILayout.Button("Generate Torus"))
        {
            GenerateTorusInScene();
        }
    }

    void GenerateTorusInScene()
    {
        GameObject torus = new GameObject(objectName);
        MeshFilter mf = torus.AddComponent<MeshFilter>();
        MeshRenderer mr = torus.AddComponent<MeshRenderer>();
        mf.sharedMesh = GenerateTorusMesh();
        mr.sharedMaterial = material != null ? material : new Material(Shader.Find("Standard"));

        Undo.RegisterCreatedObjectUndo(torus, "Generate Torus");
        Selection.activeGameObject = torus;
    }

    Mesh GenerateTorusMesh()
    {
        Mesh mesh = new Mesh();
        List<Vector3> vertices = new List<Vector3>();
        List<Vector3> normals = new List<Vector3>();
        List<Vector2> uvs = new List<Vector2>();
        List<int> triangles = new List<int>();

        for (int i = 0; i <= majorSegments; i++)
        {
            float majorAngle = (float)i / majorSegments * Mathf.PI * 2f;
            Quaternion majorRot = Quaternion.AngleAxis(majorAngle * Mathf.Rad2Deg, Vector3.up);
            Vector3 center = majorRot * Vector3.right * majorRadius;

            for (int j = 0; j <= minorSegments; j++)
            {
                float minorAngle = (float)j / minorSegments * Mathf.PI * 2f;
                Vector3 normal = new Vector3(Mathf.Cos(minorAngle), Mathf.Sin(minorAngle), 0);
                Vector3 vertex = center + majorRot * (normal * minorRadius);

                vertices.Add(vertex);
                normals.Add(majorRot * normal);
                uvs.Add(new Vector2((float)i / majorSegments, (float)j / minorSegments));
            }
        }

        for (int i = 0; i < majorSegments; i++)
        {
            for (int j = 0; j < minorSegments; j++)
            {
                int a = i * (minorSegments + 1) + j;
                int b = a + minorSegments + 1;

                triangles.Add(a);
                triangles.Add(b);
                triangles.Add(a + 1);

                triangles.Add(a + 1);
                triangles.Add(b);
                triangles.Add(b + 1);
            }
        }

        mesh.SetVertices(vertices);
        mesh.SetNormals(normals);
        mesh.SetUVs(0, uvs);
        mesh.SetTriangles(triangles, 0);
        mesh.RecalculateBounds();

        return mesh;
    }
}
