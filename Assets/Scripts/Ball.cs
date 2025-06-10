using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using Normal.Realtime;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

[RequireComponent(typeof(RealtimeView))]
public class Ball : MonoBehaviour
{
    public XRGrabInteractable grabInteractable;

    private Vector3 throwPosition;
    private int zoneThrownFrom = 1;
    private bool scored = false;
    private PlayerScore thrower;
    private bool hasBeenThrown = false;


    private void OnEnable()
    {
        grabInteractable.selectEntered.AddListener(OnGrabbed);
        grabInteractable.selectExited.AddListener(OnReleased);
    }

    private void OnDisable()
    {
        grabInteractable.selectEntered.RemoveListener(OnGrabbed);
        grabInteractable.selectExited.RemoveListener(OnReleased);
    }


    private void OnGrabbed(SelectEnterEventArgs args)
    {
        var view = GetComponent<RealtimeView>();

        if (hasBeenThrown && !scored)
        {
            var avatarManager = FindObjectOfType<RealtimeAvatarManager>();
            if (view != null && avatarManager.avatars.TryGetValue(view.ownerID, out var avatar))
            {
                var score = avatar.GetComponent<PlayerScore>();
                if (score != null)
                {
                    Debug.Log("[Ball] Missed last shot — resetting combo.");
                    score.ResetCombo();
                }
            }
        }

        hasBeenThrown = false;

        if (view != null && !view.isOwnedLocally)
        {
            Debug.Log("[Ball] Requesting ownership on grab...");
            view.RequestOwnership();
        }

        // Lock ownership so others can't take it while held
        if (view != null)
            view.preventOwnershipTakeover = true;

        Debug.Log("[Ball] OnGrabbed");
    }






    private void OnReleased(SelectExitEventArgs args)
    {
        Debug.Log("[Ball] OnReleased called.");

        throwPosition = transform.position;
        zoneThrownFrom = ZoneManager.Instance.GetZone(throwPosition);
        scored = false;
        hasBeenThrown = true;

        // Unlock ownership so someone else can grab it
        var view = GetComponent<RealtimeView>();
        if (view != null)
        {
            view.preventOwnershipTakeover = false;
        }

        Debug.Log($"[Ball] Throw position: {throwPosition}, Zone: {zoneThrownFrom}");
    }



    public void RegisterScore()
    {
        Debug.Log($"[Ball] Attempting to register score. Already scored: {scored}");

        if (scored)
        {
            Debug.Log("[Ball] Already scored — skipping.");
            return;
        }

        var view = GetComponent<RealtimeView>();
        if (view == null)
        {
            Debug.LogWarning("[Ball] Missing RealtimeView.");
            return;
        }

        var avatarManager = FindObjectOfType<RealtimeAvatarManager>();
        if (!avatarManager.avatars.TryGetValue(view.ownerID, out var avatar))
        {
            Debug.LogWarning($"[Ball] No avatar found for ownerID {view.ownerID}.");
            return;
        }

        var score = avatar.GetComponent<PlayerScore>();
        if (score == null)
        {
            Debug.LogWarning("[Ball] PlayerScore missing on avatar.");
            return;
        }

        // Score calculation with combo multiplier
        int comboMultiplier = Mathf.Clamp(score.GetCombo() + 1, 1, 3);
        int points = zoneThrownFrom * comboMultiplier;

        score.AddScore(points);
        score.IncrementCombo();

        Debug.Log($"[Ball] Scored {points} points from zone {zoneThrownFrom} (x{comboMultiplier}) for Player {view.ownerID}");

        scored = true;
        hasBeenThrown = false;

    }
}
