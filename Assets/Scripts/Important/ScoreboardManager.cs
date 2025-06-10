using UnityEngine;
using TMPro;
using Normal.Realtime;
using System.Collections.Generic;
using System.Collections;
using System;

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
    private float _countdownRemaining = 0f;
    private const float TimeAttackDuration = 30f; // change as needed

    private float _countdownDisplayUpdateTimer = 0f;
    private const float CountdownDisplayUpdateInterval = 0.25f; // update 4x per second



    private int lastScoreP1 = -1;
    private int lastScoreP2 = -1;

    void Start()
    {
        _avatarManager = FindObjectOfType<RealtimeAvatarManager>();
        _gameManager = GameManager.Instance;

    }

    void Update()
    {
        var avatars = _avatarManager.avatars;

        // Reset defaults
        player1Label.text = "Waiting...";
        player2Label.text = "Waiting...";
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

            string playerName = $"Player {kvp.Key + 1}";
            int score = playerScore.GetScore();
            string scoreText = score.ToString();
            string streakText = $"x{playerScore.GetCombo() + 1}";

            if (index == 0)
            {
                player1Label.text = playerName;
                player1ScoreText.text = scoreText;
                player1ComboText.text = streakText;

                if (lastScoreP1 != -1 && score > lastScoreP1)
                    scoreSound?.Play();

                lastScoreP1 = score;
            }
            else if (index == 1)
            {
                player2Label.text = playerName;
                player2ScoreText.text = scoreText;
                player2ComboText.text = streakText;

                if (lastScoreP2 != -1 && score > lastScoreP2)
                    scoreSound?.Play();

                lastScoreP2 = score;
            }

            index++;
            if (index > 1) break;
        }

        // Display game mode
        if (_gameManager != null)
        {
            if (_gameManager.CurrentGameMode == GameMode.TimeAttack && !_gameManager.IsGameOver)
            {
                var realtime = FindFirstObjectByType<Realtime>();
                if (realtime != null && realtime.clientID == 0 && _gameManager.TimeAttackStartRoomTime == 0)
                {
                    _gameManager.BeginTimeAttackIfNotStarted(); // sets TimeAttackStartTime
                }

                if (_gameManager.TimeAttackStartRoomTime > 0 && !_isCountingDown)
                {
                    StartTimeAttackCountdown();
                }

                gameModeText.text = "Time Attack!";
            }
            else
            {
                gameModeText.text = "Free Play!";
            }


            if (_gameManager.CurrentGameMode == GameMode.TimeAttack && !_isCountingDown && !_gameManager.IsGameOver)
            {
                StartTimeAttackCountdown();
            }
        }

        // Update countdown timer if active
        if (_isCountingDown)
        {
            var realtime = FindFirstObjectByType<Realtime>();
            double now = realtime != null ? realtime.room.time : 0;
            double start = _gameManager.TimeAttackStartRoomTime;
            float remaining = (float)(TimeAttackDuration - (now - start));

            if (remaining <= 0f)
            {
                countdownText.text = "TIME'S UP!";
                _isCountingDown = false;
                _gameManager.SetGameOver(true);

                
                if (realtime != null && realtime.clientID == 0)
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
                        var playerScore = avatar.GetComponent<PlayerScore>();
                        if (playerScore == null) continue;

                        int score = playerScore.GetScore();
                        if (score > highestScore)
                        {
                            highestScore = score;
                            topScorer = avatar;
                        }
                    }

                    if (topScorer != null)
                    {
                        string winnerName = $"Player {topScorer.ownerID + 1}";
                        _gameManager.SetWinner(winnerName);
                    }
                }
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
        else
        {
            countdownText.text = "";
        }


        // Show winner if game is over
        // Show winner if game is over
        if (_gameManager.IsGameOver)
        {
            // Clean up remaining basketballs if any
            if (FindObjectsOfType<Ball>().Length > 0)
                StartCoroutine(TakeOwnershipAndDestroyAllBasketballs());

            int topScore = int.MinValue;
            List<string> topScorers = new();
            int avatarsIndex = 0;

            foreach (var kvp in _avatarManager.avatars)
            {
                var avatar = kvp.Value;
                int score = avatar.GetComponent<PlayerScore>()?.GetScore() ?? 0;

                if (score > topScore)
                {
                    topScore = score;
                    topScorers.Clear();
                    topScorers.Add($"Player {avatarsIndex + 1}");
                }
                else if (score == topScore)
                {
                    topScorers.Add($"Player {avatarsIndex + 1}");
                }

                avatarsIndex++;
            }

            if (topScorers.Count == 1)
                winnerText.text = $"{topScorers[0].ToUpper()} IS THE WINNER!!";
            else
                winnerText.text = $"TIE: {string.Join(" & ", topScorers).ToUpper()}!!";
        }
        else
        {
            winnerText.text = "";
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
        _countdownRemaining = 0f;
        countdownText.text = "";
        Debug.Log("[ScoreboardManager] Countdown cancelled.");
    }


    public IEnumerator TakeOwnershipAndDestroyAllBasketballs()
    {
        foreach (var ball in FindObjectsOfType<Ball>())
        {
            var rt = ball.GetComponent<RealtimeTransform>();
            if (rt != null && !rt.isOwnedLocallySelf)
                rt.RequestOwnership();
        }

        // Wait a frame to allow ownership to transfer
        yield return null;

        foreach (var ball in FindObjectsOfType<Ball>())
        {
            var rt = ball.GetComponent<RealtimeTransform>();
            if (rt != null && rt.isOwnedLocallySelf)
                Realtime.Destroy(ball.gameObject);
        }
    }


}
