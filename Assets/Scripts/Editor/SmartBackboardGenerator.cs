using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

public class SmartBackboardGenerator : EditorWindow
{
    float width = 1.8f;
    float height = 1.05f;
    int resolutionX = 20;
    int resolutionY = 12;
    Vector3 targetPoint = new Vector3(0, 1, 1);
    float focusStrength = 1f;
    Material material;
    string objectName = "SmartBackboard";

    [MenuItem("Tools/Smart Backboard Generator")]
    public static void ShowWindow()
    {
        GetWindow<SmartBackboardGenerator>("Smart Backboard");
    }

    void OnGUI()
    {
        GUILayout.Label("Target-Focused Backboard Settings", EditorStyles.boldLabel);
        width = EditorGUILayout.FloatField("Width", width);
        height = EditorGUILayout.FloatField("Height", height);
        resolutionX = EditorGUILayout.IntSlider("Resolution X", resolutionX, 2, 50);
        resolutionY = EditorGUILayout.IntSlider("Resolution Y", resolutionY, 2, 50);
        focusStrength = EditorGUILayout.Slider("Curve Strength", focusStrength, 0f, 2f);
        targetPoint = EditorGUILayout.Vector3Field("Focus Point (Local Space)", targetPoint);
        material = (Material)EditorGUILayout.ObjectField("Material", material, typeof(Material), false);
        objectName = EditorGUILayout.TextField("Object Name", objectName);

        if (GUILayout.Button("Generate Backboard"))
        {
            GenerateBackboard();
        }
    }

    void GenerateBackboard()
    {
        GameObject backboard = new GameObject(objectName);
        MeshFilter mf = backboard.AddComponent<MeshFilter>();
        MeshRenderer mr = backboard.AddComponent<MeshRenderer>();
        mf.sharedMesh = GenerateMesh();
        mr.sharedMaterial = material != null ? material : new Material(Shader.Find("Standard"));

        // Create the child TargetPoint
        GameObject child = new GameObject("TargetPoint");
        child.transform.parent = backboard.transform;
        child.transform.localPosition = targetPoint;

        Undo.RegisterCreatedObjectUndo(backboard, "Generate Smart Backboard");
        Selection.activeGameObject = backboard;
    }

    Mesh GenerateMesh()
    {
        Mesh mesh = new Mesh();

        List<Vector3> vertices = new List<Vector3>();
        List<Vector3> normals = new List<Vector3>();
        List<Vector2> uvs = new List<Vector2>();
        List<int> triangles = new List<int>();

        for (int y = 0; y <= resolutionY; y++)
        {
            float v = (float)y / resolutionY;
            float yPos = Mathf.Lerp(-height / 2f, height / 2f, v);

            for (int x = 0; x <= resolutionX; x++)
            {
                float u = (float)x / resolutionX;
                float xPos = Mathf.Lerp(-width / 2f, width / 2f, u);

                Vector3 flatPos = new Vector3(xPos, yPos, 0);
                float distanceFromCenter = flatPos.magnitude; // 0 at center, grows toward corners
                //float zOffset = -Mathf.Pow(distanceFromCenter / (Mathf.Max(width, height) / 2f), 2) * focusStrength;
                float zOffset = Mathf.Pow(distanceFromCenter, 2) * -focusStrength / Mathf.Pow(Mathf.Max(width, height) / 2f, 2);

                Vector3 curvedPos = new Vector3(xPos, yPos, zOffset);



                vertices.Add(curvedPos);
                normals.Add((new Vector3(xPos, yPos, zOffset)).normalized);

                //normals.Add((new Vector3(xPos, yPos, zOffset)).normalized);

                uvs.Add(new Vector2(u, v));
            }
        }

        for (int y = 0; y < resolutionY; y++)
        {
            for (int x = 0; x < resolutionX; x++)
            {
                int i = y * (resolutionX + 1) + x;

                triangles.Add(i);
                triangles.Add(i + resolutionX + 1);
                triangles.Add(i + 1);

                triangles.Add(i + 1);
                triangles.Add(i + resolutionX + 1);
                triangles.Add(i + resolutionX + 2);
            }
        }

        mesh.SetVertices(vertices);
        mesh.SetNormals(normals);
        mesh.SetUVs(0, uvs);
        // Reverse triangle winding to flip mesh
        /*for (int i = 0; i < triangles.Count; i += 3)
        {
            int temp = triangles[i];
            triangles[i] = triangles[i + 1];
            triangles[i + 1] = temp;
        }*/
        mesh.SetTriangles(triangles, 0);

        // Flip normals
        /*for (int i = 0; i < normals.Count; i++)
            normals[i] = -normals[i];*/
        mesh.SetNormals(normals);

        mesh.RecalculateBounds();

        return mesh;
    }
}
