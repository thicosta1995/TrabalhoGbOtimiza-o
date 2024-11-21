using UnityEngine;

public class Portal : MonoBehaviour
{
    public GameObject roomA; // Sala conectada pelo lado A do portal
    public GameObject roomB; // Sala conectada pelo lado B do portal

    // Verifica se este portal está conectado à sala especificada
    public bool IsConnectedToRoom(GameObject room)
    {
        return room == roomA || room == roomB;
    }
}
