using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using BackEnd;
using UnityEngine.UI;
using LitJson;
using System;

public class UserManager : MonoBehaviour
{
    [Header("Login & Register")]
    public Text LoginDescription;
    public InputField Email;
    public InputField Password;
    public GameObject LoginCanvas;

    [Header("Nickname")]
    public Text NickDescription;
    public InputField Nickname;
    public GameObject NickCanvas;

    [Header("Welcome")]
    public GameObject WelCanvas;
    public Text WelText;

    [Header("Menu")]
    public GameObject MenuCanvas;

    JsonData returnJson;
    string userInDate;
    float userCurrTime;

    [System.Obsolete]
    void Start()
    {
        Backend.Initialize(() =>
        {
            if (Backend.IsInitialized)
            {
                Debug.Log("뒤끝 초기화 완료");
            }
            else
            {
                Debug.Log("뒤끝 초기화 실패");
            }
        });

        BackendReturnObject BReturn = Backend.BMember.LoginWithTheBackendToken();
        if (BReturn.IsSuccess())
        {
            Debug.Log("자동 로그인 완료");
            LoginCanvas.SetActive(false);
        }
        else
            LoginCanvas.SetActive(true);

        NickCanvas.SetActive(false);
        WelCanvas.SetActive(false);
        MenuCanvas.SetActive(false);
    }

    public void Register()
    {
        BackendReturnObject BReturn = Backend.BMember.CustomSignUp(Email.text, Password.text);
        BackendReturnObject BReturn2 = Backend.BMember.UpdateCustomEmail(Email.text);

        if (BReturn.IsSuccess()) Debug.Log("동기 방식 회원가입 완료");
        else Error(BReturn.GetErrorCode(), "UserFunc");

        if (BReturn2.IsSuccess()) Debug.Log("동기 방식 이메일 등록 완료");

        Param param = new Param();
        param.Add("minTime", 0);

        BackendReturnObject BReturn3 = Backend.GameData.Insert("Time", param);

        if (BReturn3.IsSuccess()) Debug.Log("동기 방식 데이터 삽입 완료");

        Email.text = "";
        Password.text = "";

        LoginCanvas.SetActive(false);
        NickCanvas.SetActive(true);
    }

    public void Login()
    {
        BackendReturnObject BReturn = Backend.BMember.CustomLogin(Email.text, Password.text);
        
        if (BReturn.IsSuccess()) Debug.Log("동기 방식 로그인 완료");
        else Error(BReturn.GetErrorCode(), "UserFunc");

        GetUserInfo();
        LoginCanvas.SetActive(false);
        WelCanvas.SetActive(true);
        WelText.text = "어서 오세요, " + returnJson["nickname"] + "님!";
    }

    public void CreateUserNickname()
    {
        BackendReturnObject BReturn = Backend.BMember.CreateNickname(Nickname.text);

        if (BReturn.IsSuccess()) Debug.Log("동기 방식 닉네임 생성 완료");
        else Error(BReturn.GetErrorCode(), "UserNickname");

        GetUserInfo();
        WelCanvas.SetActive(true);
        NickCanvas.SetActive(false);
        WelText.text = "환영합니다, " + returnJson["nickname"] + "님!";
    }

    public void Logout()
    {
        Backend.BMember.Logout();
        MenuCanvas.SetActive(false);
        LoginCanvas.SetActive(true);
    }

    public void Signout()
    {
        Backend.BMember.SignOut();
        MenuCanvas.SetActive(false);
        LoginCanvas.SetActive(true);
    }

    public void GetUserInfo()
    {
        BackendReturnObject BReturn = Backend.BMember.GetUserInfo();

        returnJson = BReturn.GetReturnValuetoJSON()["row"];

        Debug.Log("Nickname: " + returnJson["nickname"] + ", InDate: " + returnJson["inDate"].ToString()
            + "\nSubScriptionType: " + returnJson["subscriptionType"].ToString() + ", EmailForFindPassword: " + returnJson["emailForFindPassword"]);
    }

    public void ReadData()
    {
        Where where = new Where();
        where.GreaterOrEqual("minTime", 0);

        BackendReturnObject BReturn = Backend.GameData.GetMyData("Time", where);

        if (BReturn.IsSuccess())
        {
            JsonData jsonData = BReturn.GetReturnValuetoJSON()["rows"][0];
            userCurrTime = float.Parse(jsonData["minTime"][0].ToString());
            userInDate = jsonData["inDate"][0].ToString();

            Debug.Log("Min Time: " + userCurrTime + ", InDate: " + userInDate);
            Debug.Log("동기 방식 정보 읽기 완료");
        }
        else Error(BReturn.GetErrorCode(), "ReadData");
    }

    public void UpdateData(float time)
    {
        ReadData();

        if (userCurrTime > time || userCurrTime == 0)
        {
            Where where = new Where();
            Param param = new Param();

            param.Add("minTime", (float)(Math.Truncate(time * 100) / 100));
            where.GreaterOrEqual("minTime", 0);

            BackendReturnObject BReturn = Backend.URank.User.UpdateUserScore("ed0afa10-a26c-11ec-9275-51c2c5ca16f7", "Time",
                userInDate, param);

            if (BReturn.IsSuccess()) Debug.Log("정보 수정 완료");
            else Debug.LogError(BReturn.GetErrorCode());
        }
    }

    void Error(string errorCode, string type)
    {
        if (errorCode == "DuplicatedParameterException")
        {
            if (type == "UserFunc") LoginDescription.text = ("중복된 사용자 아이디입니다.");
            else if (type == "UserNickname") NickDescription.text = ("중복된 닉네임입니다.");
        }
        else if (errorCode == "BadUnauthorizedException")
        {
            if (type == "UserFunc") LoginDescription.text = ("잘못된 사용자 아이디 혹은 비밀번호입니다.");
        }
        else if (errorCode == "UndefinedParameterException")
        {
            if (type == "UserNickname") NickDescription.text = ("닉네임을 다시 입력해주세요.");
        }
        else if (errorCode == "BadParamaterException")
        {
            if (type == "UserNickname") NickDescription.text = ("닉네임 앞/뒤 공백이 있거나 20자 이상입니다.");
        }
    }
}
