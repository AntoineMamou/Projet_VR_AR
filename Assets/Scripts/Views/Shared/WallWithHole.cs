using UnityEngine;

/// <summary>
/// Génère un mur rectangulaire avec un trou au centre.
/// </summary>
[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class WallWithHole : MonoBehaviour
{
    [Header("Dimensions du mur")]
    public float width = 5f;
    public float height = 3f;

    [Header("Dimensions du trou")]
    public float holeWidth = 1f;
    public float holeHeight = 1.5f;

    void Start()
    {
        GenerateWall();
    }

    void GenerateWall()
    {
        // Sécurité : éviter un trou plus grand que le mur
        holeWidth = Mathf.Clamp(holeWidth, 0.1f, width - 0.1f);
        holeHeight = Mathf.Clamp(holeHeight, 0.1f, height - 0.1f);

        Mesh mesh = new Mesh();

        float w = width / 2f;
        float h = height / 2f;
        float hw = holeWidth / 2f;
        float hh = holeHeight / 2f;

        // 8 sommets (4 pour le haut, 4 pour le bas autour du trou)
        Vector3[] vertices = new Vector3[]
        {
            // Haut gauche
            new Vector3(-w,  h, 0),
            new Vector3( w,  h, 0),
            new Vector3( w,  hh, 0),
            new Vector3(-w,  hh, 0),

            // Bas gauche
            new Vector3(-w, -hh, 0),
            new Vector3( w, -hh, 0),
            new Vector3( w, -h, 0),
            new Vector3(-w, -h, 0),

            // Côtés du trou
            new Vector3(-hw,  hh, 0),
            new Vector3( hw,  hh, 0),
            new Vector3( hw, -hh, 0),
            new Vector3(-hw, -hh, 0)
        };

        // Triangles : on crée 4 rectangles autour du trou
        int[] triangles = new int[]
        {
            // Haut
            0, 1, 8,
            8, 1, 9,

            // Bas
            11, 10, 7,
            7, 10, 6,

            // Gauche
            0, 8, 3,
            3, 8, 11,
            11, 4, 3,
            4, 11, 7,

            // Droite
            9, 1, 2,
            2, 1, 6,
            6, 10, 2,
            2, 10, 5
        };

        // UV simples (pour texture)
        Vector2[] uv = new Vector2[vertices.Length];
        for (int i = 0; i < vertices.Length; i++)
        {
            uv[i] = new Vector2(vertices[i].x / width + 0.5f, vertices[i].y / height + 0.5f);
        }

        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.uv = uv;
        mesh.RecalculateNormals();

        GetComponent<MeshFilter>().mesh = mesh;
    }
}