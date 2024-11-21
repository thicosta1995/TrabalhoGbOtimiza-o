using UnityEngine;

public class PortalCullingVisualizer : MonoBehaviour
{
    public Camera referenceCamera; // Câmera de referência
    public Transform[] portals; // Lista de portais
    private LineRenderer[] lines; // Array para armazenar linhas

    void Start()
    {
        // Inicializa os LineRenderers, um para cada portal
        lines = new LineRenderer[portals.Length];
        for (int i = 0; i < portals.Length; i++)
        {
            GameObject lineObject = new GameObject("LineRenderer_" + i);
            lines[i] = lineObject.AddComponent<LineRenderer>();
            lines[i].material = new Material(Shader.Find("Sprites/Default"));
            lines[i].startColor = Color.cyan;
            lines[i].endColor = Color.cyan;
            lines[i].startWidth = 0.05f;
            lines[i].endWidth = 0.05f;
            lines[i].positionCount = 2;
            MeshRenderer portalMesh = portals[i].GetComponent<MeshRenderer>();
            if (portalMesh != null)
            {
                portalMesh.enabled = false;
            }
        }
       
    }

    void Update()
    {
        if (referenceCamera == null || portals == null) return;

        for (int i = 0; i < portals.Length; i++)
        {
            // Calcula o vetor entre a câmera e o portal
            Vector3 cameraPosition = referenceCamera.transform.position;
            Vector3 portalPosition = portals[i].position;

            // Define as posições da linha
            lines[i].SetPosition(0, cameraPosition); // Posição da câmera
            lines[i].SetPosition(1, portalPosition); // Posição do portal
        }
    }

void OnDrawGizmos()
    {
        if (referenceCamera == null)
            return;

        Gizmos.color = Color.red;

        // Obtenha os planos de corte da câmera
        float nearClip = referenceCamera.nearClipPlane;
        float farClip = referenceCamera.farClipPlane;
        float fov = referenceCamera.fieldOfView;
        float aspect = referenceCamera.aspect;

        // Calcule os cantos do frustum no plano próximo
        float nearHeight = 2.0f * Mathf.Tan(fov * 0.5f * Mathf.Deg2Rad) * nearClip;
        float nearWidth = nearHeight * aspect;

        // Calcule os cantos do frustum no plano distante
        float farHeight = 2.0f * Mathf.Tan(fov * 0.5f * Mathf.Deg2Rad) * farClip;
        float farWidth = farHeight * aspect;

        // Posição da câmera
        Vector3 camPos = referenceCamera.transform.position;
        Vector3 camForward = referenceCamera.transform.forward;
        Vector3 camRight = referenceCamera.transform.right;
        Vector3 camUp = referenceCamera.transform.up;

        // Cálculo dos cantos do frustum
        Vector3 nearCenter = camPos + camForward * nearClip;
        Vector3 farCenter = camPos + camForward * farClip;

        Vector3 nearTL = nearCenter + (camUp * nearHeight / 2) - (camRight * nearWidth / 2);
        Vector3 nearTR = nearCenter + (camUp * nearHeight / 2) + (camRight * nearWidth / 2);
        Vector3 nearBL = nearCenter - (camUp * nearHeight / 2) - (camRight * nearWidth / 2);
        Vector3 nearBR = nearCenter - (camUp * nearHeight / 2) + (camRight * nearWidth / 2);

        Vector3 farTL = farCenter + (camUp * farHeight / 2) - (camRight * farWidth / 2);
        Vector3 farTR = farCenter + (camUp * farHeight / 2) + (camRight * farWidth / 2);
        Vector3 farBL = farCenter - (camUp * farHeight / 2) - (camRight * farWidth / 2);
        Vector3 farBR = farCenter - (camUp * farHeight / 2) + (camRight * farWidth / 2);

        // Desenhar o frustum
        Gizmos.DrawLine(nearTL, nearTR);
        Gizmos.DrawLine(nearTR, nearBR);
        Gizmos.DrawLine(nearBR, nearBL);
        Gizmos.DrawLine(nearBL, nearTL);

        Gizmos.DrawLine(farTL, farTR);
        Gizmos.DrawLine(farTR, farBR);
        Gizmos.DrawLine(farBR, farBL);
        Gizmos.DrawLine(farBL, farTL);

        Gizmos.DrawLine(nearTL, farTL);
        Gizmos.DrawLine(nearTR, farTR);
        Gizmos.DrawLine(nearBL, farBL);
        Gizmos.DrawLine(nearBR, farBR);
    }

    Vector3[] GetFrustumCorners(Camera cam)
    {
        Vector3[] corners = new Vector3[4];
        cam.CalculateFrustumCorners(new Rect(0, 0, 1, 1), cam.farClipPlane, Camera.MonoOrStereoscopicEye.Mono, corners);
        for (int i = 0; i < 4; i++) corners[i] = cam.transform.TransformPoint(corners[i]);
        return corners;
    }
}
