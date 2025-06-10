using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using Normal.Realtime;

// Handles ball grab/release behavior, scoring logic, combo resets, and ownership syncing in multiplayer.
// Tracks which player last threw the ball and awards points based on throw zone and combo multiplier.
[RequireComponent(typeof(RealtimeView))]
public class Ball : MonoBehaviour
{
    public XRGrabInteractable grabInteractable;

    private Vector3 throwPosition;
    private int zoneThrownFrom = 1;
    private bool scored = false;
    private PlayerScore thrower;
    private PlayerScore lastThrower;
    private bool hasBeenThrown = false;

    private RealtimeView _view;
    private RealtimeAvatarManager _avatarManager;

    private void OnEnable()
    {
        grabInteractable.selectEntered.AddListener(OnGrabbed);
        grabInteractable.selectExited.AddListener(OnReleased);

        _view = GetComponent<RealtimeView>();
        _avatarManager = FindFirstObjectByType<RealtimeAvatarManager>();
    }

    private void OnDisable()
    {
        grabInteractable.selectEntered.RemoveListener(OnGrabbed);
        grabInteractable.selectExited.RemoveListener(OnReleased);
    }

    private void OnGrabbed(SelectEnterEventArgs args)
    {
        lastThrower = null;

        if (hasBeenThrown && !scored && _view != null &&
            _avatarManager.avatars.TryGetValue(_view.ownerID, out var avatar))
        {
            var score = avatar.GetComponent<PlayerScore>();
            score?.ResetCombo();
        }

        hasBeenThrown = false;

        if (_view != null && !_view.isOwnedLocally)
            _view.RequestOwnership();

        if (_view != null)
            _view.preventOwnershipTakeover = true;

    }

    private void OnReleased(SelectExitEventArgs args)
    {

        throwPosition = transform.position;
        zoneThrownFrom = ZoneManager.Instance.GetZone(throwPosition);
        scored = false;
        hasBeenThrown = true;

        if (_view != null)
            _view.preventOwnershipTakeover = false;


        if (_view != null && _avatarManager.avatars.TryGetValue(_view.ownerID, out var avatar))
            lastThrower = avatar.GetComponent<PlayerScore>();
    }

    public void RegisterScore()
    {
        if (scored)
        {
            //already scored, don't want to give double poitns
            return;
        }

        if (_view == null)
        {
            //No realtime view
            return;
        }

        if (!_avatarManager.avatars.TryGetValue(_view.ownerID, out var avatar))
        {
            //No avatar found on our client
            return;
        }

        var score = avatar.GetComponent<PlayerScore>();
        if (score == null)
        {
            //No playerscore on our avatar
            return;
        }

        int comboMultiplier = Mathf.Clamp(score.GetCombo() + 1, 1, 3);
        int points = zoneThrownFrom * comboMultiplier;

        score.AddScore(points);
        score.IncrementCombo();

        scored = true;
        hasBeenThrown = false;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!scored && hasBeenThrown && collision.gameObject.CompareTag("Ground"))
        {

            if (lastThrower != null)
            {
                //Ball was thrown and hit the ground without scoring
                lastThrower.ResetCombo();
            }

            hasBeenThrown = false;
        }
    }
}
