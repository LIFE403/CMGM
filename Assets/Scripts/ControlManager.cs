using System.Collections;
using UnityEngine;

public class ControlManager : MonoBehaviour
{
    [SerializeField] private Camera m_Camera;
    public float ClearTime;
    private CardModule Select { get; set; } = null;

    private void Awake()
    {
        StartCoroutine(UpdateControll());
    }

    private CardModule OnClickCard()
    {
        Ray ray = m_Camera.ScreenPointToRay(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction, Mathf.Infinity);

        if (hit.collider == null) return null;
        GameObject go = hit.collider.gameObject;

        return go.GetComponent<CardModule>();
    }

    private IEnumerator UpdateControll()
    {
        while (true)
        {
            yield return null;

            const float RotateSpeed = .2f;
            if (Input.GetMouseButtonDown(0))
            {
                var newCard = OnClickCard();
                if (newCard == null) continue;
                else if (Select != null && newCard.gameObject == Select.gameObject)
                {
                    Select.StartRotate(false, RotateSpeed);
                    Select = null;
                }
                else
                {
                    newCard.StartRotate(true, RotateSpeed);
                    yield return new WaitForSeconds(RotateSpeed);

                    if (Select == null) Select = newCard;
                    else if (newCard.CardColor == Select.CardColor)
                    {
                        Select.StartDisableCard(RotateSpeed);
                        newCard.StartDisableCard(RotateSpeed);
                        Select = null;

                        var gameManager = FindObjectOfType<GameManager>();
                        if (gameManager.CheckAllDisableCard())
                        {
                            var timer = FindObjectOfType<Timer>();
                            timer.StopTimer();

                            ClearTime = timer.clearTime;
                            Destroy(timer);

                            var user = FindObjectOfType<UserManager>();
                            user.UpdateData(ClearTime);

                            var rank = FindObjectOfType<RankManager>();
                            rank.GetRank();
                        }
                    }
                    else
                    {
                        Select.StartRotate(false, RotateSpeed);
                        newCard.StartRotate(false, RotateSpeed);
                        Select = null;
                    }
                }
            }

        }
    }

}
