using UnityEngine;
using UnityEngine.UI;

public class Timer : MonoBehaviour
{
    [SerializeField] private Text TimerText;
    private float totalTime = 0;
    public float clearTime = 0;

    private void Update()
    {
        totalTime += Time.deltaTime;

        TimerText.text = $"Time\n{totalTime:F2}";
    }

    public void StopTimer()
    {
        clearTime = totalTime;

        TimerText.text = $"Clear!\n{totalTime:F2}";
    }

}
