using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FactoryProgressManager : MonoBehaviour
{
    [Header("Progress Settings")]
    [SerializeField]
    private int targetBoxCount = 8;

    [SerializeField]
    private bool startTimerAutomatically = true;

    [Header("UI References")]
    [SerializeField]
    private TMP_Text progressText;

    [SerializeField]
    private TMP_Text timerText;

    [SerializeField]
    private TMP_Text statusText;

    [SerializeField]
    private Slider progressSlider;

    [Header("Optional Reset Effects")]
    [SerializeField]
    private ParticleSystem completionParticles;

    [SerializeField]
    private AudioSource completionAudio;

    private readonly HashSet<GameObject> countedBoxes =
        new HashSet<GameObject>();

    private float elapsedTime;
    private bool timerRunning;
    private bool sessionCompleted;

    public int SuccessfulBoxCount => countedBoxes.Count;

    private void Start()
    {
        ConfigureProgressSlider();
        ResetProgressTracking();

        if (!startTimerAutomatically)
        {
            timerRunning = false;

            if (statusText != null)
            {
                statusText.text = "Ready to begin";
            }
        }
    }

    private void Update()
    {
        if (!timerRunning || sessionCompleted)
        {
            return;
        }

        elapsedTime += Time.deltaTime;
        UpdateTimerUI();
    }

    public void BeginSession()
    {
        timerRunning = true;
        sessionCompleted = false;

        if (statusText != null)
        {
            statusText.text = "Training in progress";
        }
    }

    public void RegisterSuccessfulBox(GameObject box)
    {
        if (box == null || sessionCompleted)
        {
            return;
        }

        // Prevents one box from being counted more than once.
        if (!countedBoxes.Add(box))
        {
            return;
        }

        UpdateProgressUI();

        if (countedBoxes.Count >= targetBoxCount)
        {
            CompleteSession();
        }
        else if (statusText != null)
        {
            statusText.text = "Box successfully lifted";
        }
    }

    private void CompleteSession()
    {
        sessionCompleted = true;
        timerRunning = false;

        if (statusText != null)
        {
            statusText.text = "Training completed!";
        }

        if (completionParticles != null)
        {
            completionParticles.Play();
        }

        if (completionAudio != null)
        {
            completionAudio.Play();
        }
    }

    // Connect this method to the existing physical Reset Button.
    public void ResetProgressTracking()
    {
        countedBoxes.Clear();

        elapsedTime = 0f;
        sessionCompleted = false;
        timerRunning = true;

        if (completionParticles != null)
        {
            completionParticles.Stop(
                true,
                ParticleSystemStopBehavior.StopEmittingAndClear
            );
        }

        if (completionAudio != null)
        {
            completionAudio.Stop();
        }

        ConfigureProgressSlider();
        UpdateProgressUI();
        UpdateTimerUI();

        if (statusText != null)
        {
            statusText.text = "Training in progress";
        }

        Debug.Log("Progress tracking and timer have been reset.");
    }

    private void ConfigureProgressSlider()
    {
        if (progressSlider == null)
        {
            return;
        }

        progressSlider.minValue = 0;
        progressSlider.maxValue = targetBoxCount;
        progressSlider.wholeNumbers = true;
        progressSlider.interactable = false;
        progressSlider.value = countedBoxes.Count;
    }

    private void UpdateProgressUI()
    {
        if (progressText != null)
        {
            progressText.text =
                $"Boxes lifted: {countedBoxes.Count} / {targetBoxCount}";
        }

        if (progressSlider != null)
        {
            progressSlider.value = countedBoxes.Count;
        }
    }

    private void UpdateTimerUI()
    {
        if (timerText != null)
        {
            timerText.text = $"Time: {FormatTime(elapsedTime)}";
        }
    }

    private static string FormatTime(float seconds)
    {
        int totalSeconds = Mathf.FloorToInt(seconds);
        int minutes = totalSeconds / 60;
        int remainingSeconds = totalSeconds % 60;

        return $"{minutes:00}:{remainingSeconds:00}";
    }
}