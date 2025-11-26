using System.Collections.Generic;
using UnityEngine;

public class HeavenRoomSocket : MonoBehaviour
{
    [Header("Socket Information")]
    public HeavenRoom parentRoom;
    public SocketType socketType = SocketType.Entrance;
    public SocketDirection socketDirection = SocketDirection.North;
    
    [Header("Connection Settings")]
    public bool isConnected = false;
    public bool isLocked = false;
    public bool requiresKey = false;
    
    [Header("Connected Objects")]
    public List<Transform> connectedTransforms = new List<Transform>();
    public List<HeavenRoomSocket> connectedSockets = new List<HeavenRoomSocket>();
    
    [Header("Heaven Theme")]
    public GameObject doorPrefab;
    public GameObject lockPrefab;
    public GameObject keyPrefab;
    
    [Header("Visual Effects")]
    public ParticleSystem entranceEffect;
    public Light socketLight;
    public Color lockedColor = Color.red;
    public Color unlockedColor = Color.yellow;
    
    private void Awake()
    {
        if (parentRoom == null)
        {
            parentRoom = GetComponent<HeavenRoom>();
        }
        
        InitializeSocket();
    }
    
    private void InitializeSocket()
    {
        // Set up socket based on type and direction
        SetupSocketPosition();
        SetupSocketVisuals();
        
        Debug.Log($"Initialized Heaven Room Socket: {socketType} facing {socketDirection}");
    }
    
    private void SetupSocketPosition()
    {
        // Position the socket based on direction
        Vector3 offset = Vector3.zero;
        float distance = 1f; // Distance from room center
        
        switch (socketDirection)
        {
            case SocketDirection.North:
                offset = Vector3.forward * distance;
                break;
            case SocketDirection.South:
                offset = Vector3.back * distance;
                break;
            case SocketDirection.East:
                offset = Vector3.right * distance;
                break;
            case SocketDirection.West:
                offset = Vector3.left * distance;
                break;
        }
        
        transform.localPosition = offset;
    }
    
    private void SetupSocketVisuals()
    {
        // Create visual representation of the socket
        if (doorPrefab != null)
        {
            GameObject door = Instantiate(doorPrefab, transform);
            door.transform.localPosition = Vector3.zero;
            door.name = "SocketDoor";
        }
        
        // Set up lighting
        if (socketLight == null)
        {
            GameObject lightObj = new GameObject("SocketLight");
            lightObj.transform.SetParent(transform);
            lightObj.transform.localPosition = Vector3.zero;
            socketLight = lightObj.AddComponent<Light>();
        }
        
        UpdateSocketVisuals();
    }
    
    public void ConnectToSocket(HeavenRoomSocket otherSocket)
    {
        if (CanConnectTo(otherSocket))
        {
            connectedSockets.Add(otherSocket);
            otherSocket.connectedSockets.Add(this);
            isConnected = true;
            otherSocket.isConnected = true;
            
            // Create hallway between sockets
            CreateHallwayToSocket(otherSocket);
            
            Debug.Log($"Connected socket {name} to {otherSocket.name}");
        }
    }
    
    public void DisconnectFromSocket(HeavenRoomSocket otherSocket)
    {
        if (connectedSockets.Contains(otherSocket))
        {
            connectedSockets.Remove(otherSocket);
            otherSocket.connectedSockets.Remove(this);
            
            // Remove hallway
            RemoveHallwayToSocket(otherSocket);
            
            Debug.Log($"Disconnected socket {name} from {otherSocket.name}");
        }
    }
    
    private bool CanConnectTo(HeavenRoomSocket otherSocket)
    {
        // Check if sockets can connect
        if (otherSocket == null || otherSocket == this)
            return false;
        
        if (isConnected && otherSocket.isConnected)
            return false;
        
        if (isLocked || otherSocket.isLocked)
            return false;
        
        // Check distance
        float distance = Vector3.Distance(transform.position, otherSocket.transform.position);
        if (distance > 5f) // Maximum connection distance
            return false;
        
        // Check if directions are compatible
        if (!AreDirectionsCompatible(socketDirection, otherSocket.socketDirection))
            return false;
        
        return true;
    }
    
    private bool AreDirectionsCompatible(SocketDirection dir1, SocketDirection dir2)
    {
        // Check if two socket directions are compatible for connection
        switch (dir1)
        {
            case SocketDirection.North:
                return dir2 == SocketDirection.South;
            case SocketDirection.South:
                return dir2 == SocketDirection.North;
            case SocketDirection.East:
                return dir2 == SocketDirection.West;
            case SocketDirection.West:
                return dir2 == SocketDirection.East;
            default:
                return false;
        }
    }
    
    private void CreateHallwayToSocket(HeavenRoomSocket otherSocket)
    {
        // Create hallway between this socket and the other socket
        Vector3 hallwayPosition = (transform.position + otherSocket.transform.position) / 2f;
        Vector3 direction = (otherSocket.transform.position - transform.position).normalized;
        Quaternion hallwayRotation = Quaternion.LookRotation(direction);
        
        // This would instantiate a hallway prefab
        // For now, just create a simple connection line
        GameObject hallway = new GameObject($"Hallway_{name}_{otherSocket.name}");
        hallway.transform.position = hallwayPosition;
        hallway.transform.rotation = hallwayRotation;
        hallway.transform.SetParent(transform.parent);
        
        // Add hallway component
        HeavenHallway hallwayComponent = hallway.AddComponent<HeavenHallway>();
        hallwayComponent.Initialize(this, otherSocket);
        
        connectedTransforms.Add(hallway.transform);
    }
    
    private void RemoveHallwayToSocket(HeavenRoomSocket otherSocket)
    {
        // Remove hallway between sockets
        for (int i = connectedTransforms.Count - 1; i >= 0; i--)
        {
            Transform hallway = connectedTransforms[i];
            if (hallway != null && hallway.name.Contains(otherSocket.name))
            {
                connectedTransforms.RemoveAt(i);
                DestroyImmediate(hallway.gameObject);
            }
        }
    }
    
    public void LockSocket(bool requiresKey = false)
    {
        isLocked = true;
        this.requiresKey = requiresKey;
        UpdateSocketVisuals();
        
        // Spawn lock visual if needed
        if (lockPrefab != null && requiresKey)
        {
            GameObject lockObj = Instantiate(lockPrefab, transform);
            lockObj.transform.localPosition = Vector3.zero;
            lockObj.name = "SocketLock";
        }
    }
    
    public void UnlockSocket()
    {
        isLocked = false;
        requiresKey = false;
        UpdateSocketVisuals();
        
        // Remove lock visual
        Transform lockTransform = transform.Find("SocketLock");
        if (lockTransform != null)
        {
            DestroyImmediate(lockTransform.gameObject);
        }
    }
    
    public void OpenSocket()
    {
        if (!isLocked)
        {
            // Open the socket (remove door, enable passage)
            Transform doorTransform = transform.Find("SocketDoor");
            if (doorTransform != null)
            {
                doorTransform.gameObject.SetActive(false);
            }
            
            // Enable effects
            if (entranceEffect != null)
            {
                entranceEffect.Play();
            }
        }
    }
    
    public void CloseSocket()
    {
        // Close the socket (restore door, disable passage)
        Transform doorTransform = transform.Find("SocketDoor");
        if (doorTransform != null)
        {
            doorTransform.gameObject.SetActive(true);
        }
        
        // Disable effects
        if (entranceEffect != null)
        {
            entranceEffect.Stop();
        }
    }
    
    private void UpdateSocketVisuals()
    {
        // Update visual appearance based on state
        if (socketLight != null)
        {
            if (isLocked)
            {
                socketLight.color = lockedColor;
                socketLight.intensity = 0.5f;
            }
            else
            {
                socketLight.color = unlockedColor;
                socketLight.intensity = 1f;
            }
        }
    }
    
    public bool CanPlayerPass()
    {
        return !isLocked || (isLocked && !requiresKey);
    }
    
    public void OnPlayerEnter()
    {
        // Handle player entering through this socket
        if (parentRoom != null)
        {
            parentRoom.VisitRoom();
        }
        
        // Trigger entrance effects
        if (entranceEffect != null)
        {
            entranceEffect.Play();
        }
    }
    
    public void OnPlayerExit()
    {
        // Handle player exiting through this socket
        // Stop entrance effects
        if (entranceEffect != null)
        {
            entranceEffect.Stop();
        }
    }
    
    // Getters
    public bool IsConnected => isConnected;
    public bool IsLocked => isLocked;
    public bool RequiresKey => requiresKey;
    public int ConnectionCount => connectedSockets.Count;
    
    // Debug visualization
    private void OnDrawGizmos()
    {
        // Draw socket position
        Gizmos.color = isLocked ? Color.red : Color.green;
        Gizmos.DrawWireSphere(transform.position, 0.5f);
        
        // Draw connections
        Gizmos.color = Color.yellow;
        foreach (HeavenRoomSocket connectedSocket in connectedSockets)
        {
            if (connectedSocket != null)
            {
                Gizmos.DrawLine(transform.position, connectedSocket.transform.position);
            }
        }
        
        // Draw direction arrow
        Gizmos.color = Color.blue;
        Vector3 direction = GetDirectionVector();
        Gizmos.DrawRay(transform.position, direction * 2f);
    }
    
    private Vector3 GetDirectionVector()
    {
        switch (socketDirection)
        {
            case SocketDirection.North: return Vector3.forward;
            case SocketDirection.South: return Vector3.back;
            case SocketDirection.East: return Vector3.right;
            case SocketDirection.West: return Vector3.left;
            default: return Vector3.forward;
        }
    }
}

public enum SocketType
{
    Entrance,
    Exit,
    Connection,
    Secret
}

public enum SocketDirection
{
    North,
    South,
    East,
    West
}
