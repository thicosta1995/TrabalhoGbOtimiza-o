using UnityEngine;

public class PortalCulling : MonoBehaviour
{
    public Camera referenceCamera; // Câmera que realizará o portal culling
    public GameObject[] rooms;     // Lista de todas as salas
    public GameObject[] portals;   // Lista de portais entre as salas
    public LayerMask obstructionLayer; // Layer para objetos que podem obstruir a visão (ex.: paredes)

    private GameObject lastVisibleRoom;  // Última sala visível
    private GameObject lastVisiblePortal;  // Último portal visível

    void Start()
    {
        // Inicializa a sala atual com a sala inicial onde a câmera começa
        lastVisibleRoom = GetRoomContainingCamera();
        if (lastVisibleRoom != null)
        {
            SetRoomVisibility(lastVisibleRoom, true); // Torna a sala inicial visível
        }
        else
        {
            Debug.LogError("A câmera não está dentro de nenhuma sala no início.");
        }
    }

    void Update()
    {
        // Calcula os planos do frustum da câmera de referência
        Plane[] frustumPlanes = GeometryUtility.CalculateFrustumPlanes(referenceCamera);

        bool foundVisiblePortal = false;
        GameObject newVisiblePortal = null;
        GameObject newVisibleRoom = null;

        // Itera sobre todas as salas
        foreach (GameObject room in rooms)
        {
            bool isRoomVisible = false;

            // Verifica se algum portal conectado à sala está visível
            foreach (GameObject portal in portals)
            {
                Portal portalScript = portal.GetComponent<Portal>();

                // Testa se o portal está dentro do frustum
                if (GeometryUtility.TestPlanesAABB(frustumPlanes, portal.GetComponent<Collider>().bounds))
                {
                    // Verifica se há linha de visão (sem obstruções) para o portal
                    if (IsPortalVisible(portal))
                    {
                        // Verifica se o portal conecta com esta sala
                        if (portalScript.IsConnectedToRoom(room))
                        {
                            isRoomVisible = true;
                            newVisiblePortal = portal; // Salva o novo portal visível
                            newVisibleRoom = room; // Salva a nova sala visível
                            foundVisiblePortal = true; // Encontrou um portal visível
                            break; // Se encontrar um portal visível, não precisa verificar outros portais para esta sala
                        }
                    }
                }
            }

            // Se a sala atual for a última sala visível, mantenha ela visível
            if (lastVisibleRoom != null && !foundVisiblePortal)
            {
                // Mantenha a última sala visível
                if (room == lastVisibleRoom)
                {
                    isRoomVisible = true;
                }
            }

            // Ajusta a visibilidade da sala
            SetRoomVisibility(room, isRoomVisible);
        }

        // Se um novo portal foi encontrado, atualiza o último portal visível e a sala associada
        if (foundVisiblePortal && newVisiblePortal != lastVisiblePortal)
        {
            lastVisiblePortal = newVisiblePortal;
            lastVisibleRoom = newVisibleRoom;
        }
    }

    // Função para verificar se o portal está visível para a câmera
    bool IsPortalVisible(GameObject portal)
    {
        Vector3 cameraPosition = referenceCamera.transform.position;
        Vector3 portalPosition = portal.GetComponent<Collider>().bounds.center;

        // Realiza um Raycast para verificar obstruções
        if (Physics.Raycast(cameraPosition, portalPosition - cameraPosition, out RaycastHit hit, Vector3.Distance(cameraPosition, portalPosition), obstructionLayer))
        {
            // O portal está obstruído
            return false;
        }

        // O portal está visível
        return true;
    }

    // Função para ajustar a visibilidade da sala
    void SetRoomVisibility(GameObject room, bool isVisible)
    {
        if (room == null) return; // Verifica se a sala não é nula antes de tentar acessar seus componentes

        // Ajusta a visibilidade dos renderers da sala
        foreach (Renderer renderer in room.GetComponentsInChildren<Renderer>())
        {
            if (renderer != null) // Verifica se o renderer não é nulo
            {
                renderer.enabled = isVisible; // Torna o renderer visível ou invisível
            }
        }
    }

    // Função para obter a sala em que a câmera está (com base no collider da sala)
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
