using UnityEngine;


// Determines which scoring zone a position falls into during gameplay.
// Returns zone 3, 2, or 1 based on collider areas set in the inspector.

public class ZoneManager : MonoBehaviour
{
    public static ZoneManager Instance;

    [Header("Zone Areas")]
    public Collider[] zone2Areas;  // Assign multiple in Inspector
    public Collider[] zone3Areas;  // Changed from single to array

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    public int GetZone(Vector3 position)
    {
        if (zone3Areas != null)
        {
            foreach (var zone in zone3Areas)
            {
                if (zone != null && zone.bounds.Contains(position))
                    return 3;
            }
        }

        if (zone2Areas != null)
        {
            foreach (var zone in zone2Areas)
            {
                if (zone != null && zone.bounds.Contains(position))
                    return 2;
            }
        }

        return 1; // Default fallback zone
    }
}
