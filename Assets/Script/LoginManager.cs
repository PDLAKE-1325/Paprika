using UnityEngine;
using UnityEngine.UI;

public class LoginManager : MonoBehaviour
{
    [SerializeField] NetWorkManager netWorkManager;

    [SerializeField] InputField login_email;
    [SerializeField] InputField login_password;
    [SerializeField] InputField register_email;
    [SerializeField] InputField register_username;
    [SerializeField] InputField register_password;

    [SerializeField] GameObject loginForm_obj;
    [SerializeField] GameObject RegisterForm_obj;

    bool login_form;

    public void Login()
    {
        netWorkManager.Login(login_email.text, login_password.text);
    }

    public void Register()
    {
        netWorkManager.Register(register_email.text, register_password.text, register_username.text);
    }

    public void ChangeForm(Text text)
    {
        login_form = !login_form;
        text.text = login_form ? "Register" : "Login";
        loginForm_obj.SetActive(login_form);
        RegisterForm_obj.SetActive(!login_form);
    }
}
