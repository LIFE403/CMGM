using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private CardModule[] m_Cards = null;

    private void Awake()
    {
        m_Cards = transform.GetComponentsInChildren<CardModule>();

        OnGameStart();
    }

    public void OnGameStart()
    {
        List<Color> colors = new List<Color>();
        int length = m_Cards.Length / 2;

        SetRandomColor(colors, length);
        colors.AddRange(colors);

        Shuffle(colors, colors.Count);
        for (int i = 0; i < colors.Count; ++i)
        {
            m_Cards[i].CardColor = colors[i];
            m_Cards[i].gameObject.SetActive(true);
        }
    }

    private void SetRandomColor(List<Color> list, int count)
    {
        if (count == 0) return;
        Color color = new Color
        (
            Random.Range(0f, 1f),
            Random.Range(0f, 1f),
            Random.Range(0f, 1f),
            1
        );

        if (list.Exists(x => x.Equals(color))) SetRandomColor(list, count);
        else
        {
            list.Add(color);
            SetRandomColor(list, count - 1);
        }
    }

    private void Shuffle(List<Color> list, int count)
    {
        while (count-- > 1)
        {
            int i = Random.Range(0, count);

            Color temp = list[i];
            list[i] = list[count];
            list[count] = temp;
        }
    }

    public bool CheckAllDisableCard()
    {
        for (int i = 0; i < m_Cards.Length; ++i)
        {
            if (m_Cards[i].Active)
                return false;
        }

        return true;
    }
}
