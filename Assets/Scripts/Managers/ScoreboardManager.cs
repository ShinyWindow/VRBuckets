using UnityEngine;
using TMPro;
using Normal.Realtime;
using System.Collections;
using System.Collections.Generic;


// Displays player scores, combos, game mode, and countdown UI in real-time.
// Handles win logic and cleans up basketballs when the game ends.
public class ScoreboardManager : MonoBehaviour
{
    [Header("Player Labels")]
    public TextMeshProUGUI player1Label;
    public TextMeshProUGUI player2Label;

    [Header("Player Scores")]
    public TextMeshProUGUI player1ScoreText;
    public TextMeshProUGUI player2ScoreText;

    [Header("Player Combos")]
    public TextMeshProUGUI player1ComboText;
    public TextMeshProUGUI player2ComboText;

    [Header("Game State")]
    public TextMeshProUGUI gameModeText;
    public TextMeshProUGUI countdownText;
    public TextMeshProUGUI winnerText;

    [Header("Audio")]
    public AudioSource scoreSound;

    private RealtimeAvatarManager _avatarManager;
    private GameManager _gameManager;

    private bool _isCountingDown = false;
    private float _countdownDisplayUpdateTimer = 0f;
    private const float CountdownDisplayUpdateInterval = 0.25f;
    private const float TimeAttackDuration = 90f;

    private int lastScoreP1 = -1;
    private int lastScoreP2 = -1;

    void Start()
    {
        _avatarManager = FindObjectOfType<RealtimeAvatarManager>();
        _gameManager = GameManager.Instance;
    }

    void Update()
    {
        UpdatePlayerUI();
        HandleGameModeUI();
        UpdateCountdown();
        HandleGameOver();
    }

    private void UpdatePlayerUI()
    {
        var avatars = _avatarManager.avatars;

        player1Label.text = "Waiting...";
        player2Label.text = "";
        player1ScoreText.text = "";
        player2ScoreText.text = "";
        player1ComboText.text = "";
        player2ComboText.text = "";

        int index = 0;
        foreach (var kvp in avatars)
        {
            var avatar = kvp.Value;
            var playerScore = avatar.GetComponent<PlayerScore>();
            if (playerScore == null) continue;

            string name = $"Player {kvp.Key + 1}";
            int score = playerScore.GetScore();
            string scoreStr = score.ToString();
            string comboStr = $"x{playerScore.GetCombo() + 1}";

            if (index == 0)
            {
                player1Label.text = name;
                player1ScoreText.text = scoreStr;
                player1ComboText.text = comboStr;

                if (lastScoreP1 != -1 && score > lastScoreP1)
                    scoreSound?.Play();

                lastScoreP1 = score;
            }
            else if (index == 1)
            {
                player2Label.text = name;
                player2ScoreText.text = scoreStr;
                player2ComboText.text = comboStr;

                if (lastScoreP2 != -1 && score > lastScoreP2)
                    scoreSound?.Play();

                lastScoreP2 = score;
            }

            index++;
            if (index > 1) break;
        }
    }

    private void HandleGameModeUI()
    {
        if (_gameManager == null) return;

        var realtime = FindFirstObjectByType<Realtime>();

        if (_gameManager.CurrentGameMode == GameMode.TimeAttack && !_gameManager.IsGameOver)
        {
            if (realtime != null && realtime.clientID == 0 && _gameManager.TimeAttackStartRoomTime == 0)
                _gameManager.BeginTimeAttackIfNotStarted();

            if (_gameManager.TimeAttackStartRoomTime > 0 && !_isCountingDown)
                StartTimeAttackCountdown();

            gameModeText.text = "Time Attack!";
        }
        else
        {
            gameModeText.text = "Free Play!";
        }
    }

    private void UpdateCountdown()
    {
        if (!_isCountingDown)
        {
            countdownText.text = "";
            return;
        }

        var realtime = FindFirstObjectByType<Realtime>();
        double now = realtime != null ? realtime.room.time : 0;
        float remaining = (float)(TimeAttackDuration - (now - _gameManager.TimeAttackStartRoomTime));

        if (remaining <= 0f)
        {
            countdownText.text = "TIME'S UP!";
            _isCountingDown = false;
            _gameManager.SetGameOver(true);

            if (realtime != null && realtime.clientID == 0)
                CleanupBallsAndSetWinner();
        }
        else
        {
            _countdownDisplayUpdateTimer -= Time.deltaTime;
            if (_countdownDisplayUpdateTimer <= 0f)
            {
                countdownText.text = Mathf.CeilToInt(remaining).ToString();
                _countdownDisplayUpdateTimer = CountdownDisplayUpdateInterval;
            }
        }
    }

    private void HandleGameOver()
    {
        if (!_gameManager.IsGameOver)
        {
            winnerText.text = "";
            return;
        }

        if (FindObjectsOfType<Ball>().Length > 0)
            StartCoroutine(TakeOwnershipAndDestroyAllBasketballs());

        int topScore = int.MinValue;
        List<string> topScorers = new();
        int index = 0;

        foreach (var kvp in _avatarManager.avatars)
        {
            var avatar = kvp.Value;
            int score = avatar.GetComponent<PlayerScore>()?.GetScore() ?? 0;

            if (score > topScore)
            {
                topScore = score;
                topScorers.Clear();
                topScorers.Add($"Player {index + 1}");
            }
            else if (score == topScore)
            {
                topScorers.Add($"Player {index + 1}");
            }

            index++;
        }

        winnerText.text = topScorers.Count == 1
            ? $"{topScorers[0].ToUpper()} IS THE WINNER!!"
            : $"TIE: {string.Join(" & ", topScorers).ToUpper()}!!";
    }

    private void CleanupBallsAndSetWinner()
    {
        foreach (var ball in FindObjectsOfType<Ball>())
        {
            var rt = ball.GetComponent<RealtimeTransform>();
            if (rt != null && rt.isOwnedLocallySelf)
                Realtime.Destroy(ball.gameObject);
        }

        RealtimeAvatar topScorer = null;
        int highestScore = int.MinValue;

        foreach (var kvp in _avatarManager.avatars)
        {
            var avatar = kvp.Value;
            var score = avatar.GetComponent<PlayerScore>();
            if (score == null) continue;

            int s = score.GetScore();
            if (s > highestScore)
            {
                highestScore = s;
                topScorer = avatar;
            }
        }

        if (topScorer != null)
        {
            string winnerName = $"Player {topScorer.ownerID + 1}";
            /*_gameManager.SetWinner(winnerName);*/
        }
    }

    private void StartTimeAttackCountdown()
    {
        _isCountingDown = true;
        _countdownDisplayUpdateTimer = 0f;
        Debug.Log("[ScoreboardManager] Synced countdown started.");
    }

    public void CancelCountdown()
    {
        _isCountingDown = false;
        countdownText.text = "";
        Debug.Log("[ScoreboardManager] Countdown cancelled.");
    }

    public IEnumerator TakeOwnershipAndDestroyAllBasketballs()
    {
        yield return new WaitForSeconds(0.1f); // allow time for models to initialize

        foreach (var ball in FindObjectsOfType<Ball>())
        {
            var rt = ball.GetComponent<RealtimeTransform>();
            if (rt != null && rt.isOwnedLocallySelf)
                Realtime.Destroy(ball.gameObject);
        }
    }
}
