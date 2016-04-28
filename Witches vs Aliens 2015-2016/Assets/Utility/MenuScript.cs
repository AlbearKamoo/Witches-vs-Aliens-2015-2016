using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class MenuScript : MonoBehaviour
{

    public void PlayLocal()
    {
        SceneManager.LoadScene("PlayerRegistration");
    }

    public void PlayLAN()
    {
        SceneManager.LoadScene("GameSelect");
    }

    public void Quit()
    {
        Application.Quit();
    }
}
