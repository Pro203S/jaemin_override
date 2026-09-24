using System;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(AudioSource))]
[Serializable]
public sealed class MVClock : MonoBehaviour
{
    [SerializeField, Min(0f)] private double startDelay = 0.1d;
    [Tooltip("Seconds subtracted from song time. Positive values delay visual events.")]
    [SerializeField] private double audioOffset;

    private AudioSource audioSource;
    private double songStartDspTime;
    private double scheduleOffset;
    private bool isPlaying;
    private int playbackVersion;
    private readonly List<(double time, Action callback)> events = new();

    // Negative during the scheduled lead-in; zero when stopped.
    public double SongTime => isPlaying
        ? AudioSettings.dspTime - songStartDspTime
        : 0d;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.playOnAwake = false;
        audioSource.Stop();
    }

    // Register events before or after Play. Calling Play again starts a new run.
    public void Play()
    {
        if (!isActiveAndEnabled)
            return;

        if (isPlaying)
            Stop();

        if (audioSource.clip == null)
        {
            Debug.LogError("MVClock requires an AudioClip on its AudioSource.", this);
            return;
        }

        audioSource.Stop();
        audioSource.pitch = 1f;
        audioSource.dopplerLevel = 0f;
        audioSource.loop = false;
        double audioStartDspTime = AudioSettings.dspTime + Math.Max(0d, startDelay);
        songStartDspTime = audioStartDspTime + audioOffset;
        audioSource.PlayScheduled(audioStartDspTime);
        isPlaying = true;
        playbackVersion++;
    }

    // Stop cancels all pending events. Register them again for another run.
    public void Stop()
    {
        isPlaying = false;
        playbackVersion++;
        events.Clear();
        if (audioSource != null)
            audioSource.Stop();
    }

    // Applies to future Schedule calls only; replaces the previous offset.
    public void SetOffset(double offset)
    {
        if (double.IsNaN(offset) || double.IsInfinity(offset))
            throw new ArgumentOutOfRangeException(nameof(offset));

        scheduleOffset = offset;
    }

    public void Schedule(double targetTime, Action callback)
    {
        targetTime += scheduleOffset;
        if (double.IsNaN(targetTime) || double.IsInfinity(targetTime))
            throw new ArgumentOutOfRangeException(nameof(targetTime));
        if (callback == null)
            throw new ArgumentNullException(nameof(callback));

        int index = events.FindIndex(item => item.time > targetTime);
        events.Insert(index < 0 ? events.Count : index, (targetTime, callback));
    }

    private void Update()
    {
        if (!isPlaying)
            return;

        int version = playbackVersion;
        while (isPlaying && version == playbackVersion &&
               events.Count > 0 && SongTime >= events[0].time)
        {
            Action callback = events[0].callback;
            events.RemoveAt(0);
            try
            {
                callback();
            }
            catch (Exception exception)
            {
                Debug.LogException(exception, this);
            }
        }
    }

    private void OnDisable() => Stop();
}
