using UnityEngine;

public class PortalCulling : MonoBehaviour
{
    public Camera referenceCamera; // C�mera que realizar� o portal culling
    public GameObject[] rooms;     // Lista de todas as salas
    public GameObject[] portals;   // Lista de portais entre as salas
    public LayerMask obstructionLayer; // Layer para objetos que podem obstruir a vis�o (ex.: paredes)

    private GameObject lastVisibleRoom;  // �ltima sala vis�vel
    private GameObject lastVisiblePortal;  // �ltimo portal vis�vel

    void Start()
    {
        // Inicializa a sala atual com a sala inicial onde a c�mera come�a
        lastVisibleRoom = GetRoomContainingCamera();
        if (lastVisibleRoom != null)
        {
            SetRoomVisibility(lastVisibleRoom, true); // Torna a sala inicial vis�vel
        }
        else
        {
            Debug.LogError("A c�mera n�o est� dentro de nenhuma sala no in�cio.");
        }
    }

    void Update()
    {
        // Calcula os planos do frustum da c�mera de refer�ncia
        Plane[] frustumPlanes = GeometryUtility.CalculateFrustumPlanes(referenceCamera);

        bool foundVisiblePortal = false;
        GameObject newVisiblePortal = null;
        GameObject newVisibleRoom = null;

        // Itera sobre todas as salas
        foreach (GameObject room in rooms)
        {
            bool isRoomVisible = false;

            // Verifica se algum portal conectado � sala est� vis�vel
            foreach (GameObject portal in portals)
            {
                Portal portalScript = portal.GetComponent<Portal>();

                // Testa se o portal est� dentro do frustum
                if (GeometryUtility.TestPlanesAABB(frustumPlanes, portal.GetComponent<Collider>().bounds))
                {
                    // Verifica se h� linha de vis�o (sem obstru��es) para o portal
                    if (IsPortalVisible(portal))
                    {
                        // Verifica se o portal conecta com esta sala
                        if (portalScript.IsConnectedToRoom(room))
                        {
                            isRoomVisible = true;
                            newVisiblePortal = portal; // Salva o novo portal vis�vel
                            newVisibleRoom = room; // Salva a nova sala vis�vel
                            foundVisiblePortal = true; // Encontrou um portal vis�vel
                            break; // Se encontrar um portal vis�vel, n�o precisa verificar outros portais para esta sala
                        }
                    }
                }
            }

            // Se a sala atual for a �ltima sala vis�vel, mantenha ela vis�vel
            if (lastVisibleRoom != null && !foundVisiblePortal)
            {
                // Mantenha a �ltima sala vis�vel
                if (room == lastVisibleRoom)
                {
                    isRoomVisible = true;
                }
            }

            // Ajusta a visibilidade da sala
            SetRoomVisibility(room, isRoomVisible);
        }

        // Se um novo portal foi encontrado, atualiza o �ltimo portal vis�vel e a sala associada
        if (foundVisiblePortal && newVisiblePortal != lastVisiblePortal)
        {
            lastVisiblePortal = newVisiblePortal;
            lastVisibleRoom = newVisibleRoom;
        }
    }

    // Fun��o para verificar se o portal est� vis�vel para a c�mera
    bool IsPortalVisible(GameObject portal)
    {
        Vector3 cameraPosition = referenceCamera.transform.position;
        Vector3 portalPosition = portal.GetComponent<Collider>().bounds.center;

        // Realiza um Raycast para verificar obstru��es
        if (Physics.Raycast(cameraPosition, portalPosition - cameraPosition, out RaycastHit hit, Vector3.Distance(cameraPosition, portalPosition), obstructionLayer))
        {
            // O portal est� obstru�do
            return false;
        }

        // O portal est� vis�vel
        return true;
    }

    // Fun��o para ajustar a visibilidade da sala
    void SetRoomVisibility(GameObject room, bool isVisible)
    {
        if (room == null) return; // Verifica se a sala n�o � nula antes de tentar acessar seus componentes

        // Ajusta a visibilidade dos renderers da sala
        foreach (Renderer renderer in room.GetComponentsInChildren<Renderer>())
        {
            if (renderer != null) // Verifica se o renderer n�o � nulo
            {
                renderer.enabled = isVisible; // Torna o renderer vis�vel ou invis�vel
            }
        }
    }

    // Fun��o para obter a sala em que a c�mera est� (com base no collider da sala)
    GameObject GetRoomContainingCamera()
    {
        foreach (GameObject room in rooms)
        {
            Collider roomCollider = room.GetComponent<Collider>();
            if (roomCollider != null && roomCollider.bounds.Contains(referenceCamera.transform.position))
            {
                return room;
            }
        }
        return null;
    }
}
