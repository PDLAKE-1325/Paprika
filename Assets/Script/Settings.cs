using UnityEngine;
using UnityEngine.SceneManagement;

public class Settings : MonoBehaviour
{
    public void LogOut()
    {
        NetWorkManager.Instance.Logout();
        SceneManager.LoadScene("Login");
    }
}
