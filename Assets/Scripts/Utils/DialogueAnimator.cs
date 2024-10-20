using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DialogueAnimator : MonoBehaviour
{

    private TextMeshProUGUI textMesh;
    Mesh mesh;
    Vector3[] vertices;
    Color[] colors;
    Color transparentColor;
    private Coroutine animCoroutine;
    [SerializeField] Color finalColor;
    [SerializeField] private float offsetDist = 5f;
    [SerializeField] private float charTime = 0.01f;
    [SerializeField] private float inProgressWaitTime = 0.05f;
    private bool inProgress;

    public bool InProgress
    {
        get { return inProgress; }
    }

    // Start is called before the first frame update
    void Awake()
    {
        textMesh = GetComponent<TextMeshProUGUI>();
        finalColor = textMesh.color;
    }

    private void Update()
    {
        if (inProgress && Input.GetMouseButtonDown(0))
        {
            StopCoroutine(animCoroutine);
            EndAnimation();
        }
    }

    public void PlayAnimation()
    {
        inProgress = true;
        animCoroutine = StartCoroutine(AnimateLine());
    }

    public IEnumerator AnimateLine()
    {
        textMesh.ForceMeshUpdate();
        // Set the full mesh as transparent
        transparentColor = finalColor;
        transparentColor.a = 0;
        textMesh.color = transparentColor;

        // This line seems necessary for reasons I don't fully understand
        yield return null;

        for (int i = 0; i < textMesh.textInfo.characterCount; i++)
        {
            yield return AnimateChar(i);
        }

        textMesh.color = finalColor;
        textMesh.ForceMeshUpdate();
        yield return null;
        inProgress = false;
    }

    private IEnumerator AnimateChar(int index)
    {
        mesh = textMesh.mesh;
        vertices = mesh.vertices;
        colors = mesh.colors;

        TMP_CharacterInfo c = textMesh.textInfo.characterInfo[index];

        if (c.character == ' ') yield break;

        // Debug.Log("Animating \'" + c.character + "\'");

        float timeElapsed = 0;
        Vector3 startPosition = vertices[c.vertexIndex];
        // Offsets from vertices[i] to itself, vertices[i + 1], vertices[i + 2], and vertices[i + 3] respectively that define dimensions of character quad
        Vector3[] vertexOffsets = new Vector3[4];
        for (int i = 0; i < 4; i++)
        {
            vertexOffsets[i] = vertices[c.vertexIndex + i] - vertices[c.vertexIndex];
        }

        // Now animate it back up while fading in
        while (timeElapsed < charTime)
        {
            Vector3 upperLeftPosition = Vector3.Lerp(startPosition + new Vector3(0, -offsetDist, 0), startPosition, timeElapsed / charTime);
            Color color = Color.Lerp(transparentColor, finalColor, timeElapsed / charTime);
            UpdateCharAppearance(c.vertexIndex, upperLeftPosition, vertexOffsets, color);
            timeElapsed += Time.deltaTime;
            yield return null;
        }

        // Make sure it finishes since we overshoot (timeElapsed / chartime) == 1
        UpdateCharAppearance(c.vertexIndex, startPosition, vertexOffsets, finalColor);

        yield break;
    }

    // Sets the position (based on first vertex) and color of every vertex corresponding to a given character and updates the mesh
    private void UpdateCharAppearance(int vertexIndex, Vector3 upperLeftPosition, Vector3[] vertexOffsets, Color color)
    {
        for (int i = 0; i < 4; i++)
        {
            vertices[vertexIndex + i] = upperLeftPosition + vertexOffsets[i];
            colors[vertexIndex + i] = color;
            mesh.vertices = vertices;
            mesh.colors = colors;
            textMesh.canvasRenderer.SetMesh(mesh);
        }
    }

    public void EndAnimation()
    {
        textMesh.color = finalColor;
        textMesh.ForceMeshUpdate();
        StartCoroutine(StopProgress());
    }

    // Puts a delay on changing of inProgress so that you can use it for other operations which happen in the same frame
    private IEnumerator StopProgress()
    {
        yield return new WaitForSeconds(inProgressWaitTime);
        inProgress = false;
    }

    // Debugging method to bring the textMesh.textInfo.characterInfo array to a readable string
    private string arrToString(TMP_CharacterInfo[] arr)
    {
        string toReturn = "[";
        for (int i = 0; i < arr.Length; i++)
        {
            toReturn += i.ToString() + ": \'" + arr[i].character + "\',\n";
        }
        toReturn += ']';
        return toReturn;
    }
}
