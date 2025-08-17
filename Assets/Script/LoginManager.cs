using System;
using UnityEditor.VersionControl;
using UnityEngine;
using UnityEngine.UI;

public class LoginManager : Singleton<LoginManager>
{
    [SerializeField] NetWorkManager netWorkManager;

    [SerializeField] InputField login_email;
    [SerializeField] InputField login_password;
    [SerializeField] InputField register_email;
    [SerializeField] InputField register_username;
    [SerializeField] InputField register_password;

    [SerializeField] Text changeForm_text;

    [SerializeField] GameObject loginForm_obj;
    [SerializeField] GameObject RegisterForm_obj;

    [SerializeField] AudioClip dayBGM;
    [SerializeField] AudioClip nightBGM;

    public Text log_text;

    bool login_form;
    public float onLogTime;

    public void Login()
    {
        onLogTime += 1;
        log_text.text = "로그인 중...";
        netWorkManager.Login(login_email.text, login_password.text);
    }

    public void Register()
    {
        onLogTime += 1;
        log_text.text = "회원가입 중...";
        netWorkManager.Register(register_email.text, register_password.text, register_username.text);
    }

    public void ChangeForm()
    {
        login_form = !login_form;
        changeForm_text.text = login_form ? "Register" : "Login";
        loginForm_obj.SetActive(login_form);
        RegisterForm_obj.SetActive(!login_form);
    }

    void Update()
    {
        if (onLogTime > 1.2f) onLogTime = 1.2f;
        onLogTime = Mathf.Max(onLogTime - Time.deltaTime, 0);
        if (onLogTime <= 0) log_text.text = "";
        DateTime now = DateTime.Now;
        int hour = now.Hour;

        if (hour >= 7 && hour < 22)
        {
            SoundManager.Instance.PlayBgm(dayBGM);
        }
        else
        {
            SoundManager.Instance.PlayBgm(nightBGM);
        }
    }

}
