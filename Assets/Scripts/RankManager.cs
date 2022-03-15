using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BackEnd;
using UnityEngine.UI;
using LitJson;
using UnityEngine.SceneManagement;

public class RankManager : MonoBehaviour
{
    public class UserInfo
    {
        public string nickname;
        public string minTime;
        public string rank;
    }

    public GameObject RankCanvas;

    [Header("Text")]
    public Text[] NameText = new Text[6];
    public Text[] TimeText = new Text[6];

    string rankUuid = "ed0afa10-a26c-11ec-9275-51c2c5ca16f7";

    private void Start()
    {
        RankCanvas.SetActive(false);
    }

    public void GetRank()
    {
        RankCanvas.SetActive(true);

        List<UserInfo> ranking = new List<UserInfo>();
        UserInfo myRank = new UserInfo();
        BackendReturnObject BReturn = Backend.URank.User.GetRankList(rankUuid, 5);
        BackendReturnObject BReturn2 = Backend.URank.User.GetMyRank(rankUuid);

        if (BReturn.IsSuccess())
        {
            JsonData rankList = BReturn.GetFlattenJSON();

            for (int i = 0; i < rankList["rows"].Count; i++)
            {
                UserInfo rankItem = new UserInfo();

                rankItem.nickname = rankList["rows"][i]["nickname"].ToString();
                rankItem.minTime = rankList["rows"][i]["score"].ToString();
                rankItem.rank = rankList["rows"][i]["rank"].ToString();

                ranking.Add(rankItem);
                Debug.Log("Name: " + rankItem.nickname + ", Min Time: " + rankItem.minTime + ", Rank: " + rankItem.rank);
            }
        }

        if (BReturn2.IsSuccess())
        {
            JsonData rankList = BReturn2.GetFlattenJSON();

            myRank.nickname = rankList["rows"][0]["nickname"].ToString();
            myRank.minTime = rankList["rows"][0]["score"].ToString();
            myRank.rank = rankList["rows"][0]["rank"].ToString();
        }

        for (int i = 0; i < ranking.Count; i++)
        {
            NameText[i].text = ranking[i].nickname;
            TimeText[i].text = ranking[i].minTime;
        }
        NameText[5].text = myRank.nickname;
        TimeText[5].text = myRank.minTime;
    }
}
