using UnityEngine;

public class BitCrusher : MonoBehaviour
{
    public static bool IsEnabled = true;

    [Range(1, 16)]
    public int bitDepth = 10;
    [Range(1, 20)]
    public int sampleRateReduction = 2;

    private float holdSample = 0f;
    private int holdCounter = 0;

    void OnAudioFilterRead(float[] data, int channels)
    {
        if (!IsEnabled) return;

        float levels = Mathf.Pow(2, bitDepth);

        for (int i = 0; i < data.Length; i += channels)
        {
            holdCounter++;
            if (holdCounter >= sampleRateReduction)
            {
                holdCounter = 0;
                holdSample = Mathf.Round(data[i] * levels) / levels;
            }

            for (int c = 0; c < channels; c++)
                data[i + c] = holdSample;
        }
    }
}