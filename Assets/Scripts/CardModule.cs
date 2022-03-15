using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardModule : MonoBehaviour
{
    [SerializeField] private SpriteRenderer m_FrontCardRenderer;
    [SerializeField] private SpriteRenderer m_BackCardRenderer;
    [SerializeField] private AudioSource m_AudioSource;

    public bool Active { get; set; } = true;

    public Color CardColor
    {
        get => m_BackCardRenderer.color;
        set
        {
            m_BackCardRenderer.color = value;
        }
    }

    /// <summary>
    /// 카드 회전 코루틴
    /// </summary>
    /// <param name="isUp">카드의 회전 방향</param>
    /// <param name="time">카드의 회전 시간</param>
    /// <returns></returns>
    private IEnumerator UpdateRotate(bool isUp, float time)
    {
        float currentTime = Time.time;
        
        Quaternion prev = isUp ? Quaternion.identity : Quaternion.Euler(0, 180, 0);
        Quaternion next = isUp ? Quaternion.Euler(0, 180, 0) : Quaternion.identity;

        while (Time.time - currentTime <= time)
        {
            transform.rotation = Quaternion.Lerp(
                prev,
                next,
                (Time.time - currentTime) / time);

            yield return null;
        }

        transform.rotation = next;
        yield break;
    }
    public void StartRotate(bool isUp, float time)
    {
        if (!Active) return;
        else if (!gameObject.activeSelf) return;

        StartCoroutine(UpdateRotate(isUp, time));
        m_AudioSource.Play();
    }

    private IEnumerator UpdateOpacity(float time)
    {
        Active = false;

        float currentTime = Time.time;

        while (Time.time - currentTime <= time)
        {
            CardColor = new Color(1, 1, 1,
                Mathf.Lerp(1, 0, (Time.time - currentTime) / time));

            yield return null;
        }

        CardColor = new Color(1, 1, 1, 0);
        gameObject.SetActive(false);

        yield break;
    }
    public void StartDisableCard(float time)
    {
        if (!Active) return;

        StartCoroutine(UpdateOpacity(time));
    }
}
