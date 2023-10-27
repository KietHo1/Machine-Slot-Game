using System.Collections;
using UnityEngine;
using UnitySocketIO.Events;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;

public class LoadingMN : MonoBehaviour
{
    [SerializeField] Slider progressBar;
    [SerializeField] float loadSpeed;
    private IEnumerator Start()
    {
        yield return LoadingBar(0.1f);

        yield return CheckConnect();

        yield return LoadingBar(0.7f);

        yield return LoadGameData();

        yield return LoadingBar(0.98f);

        GotoHome();
    }

    private IEnumerator CheckConnect()
    {
        while (!GameFunc.isConnect) yield return null;
    }

    IEnumerator LoadingBar(float value)
    {
        while (progressBar.value < value)
        {
            progressBar.value = Mathf.MoveTowards(progressBar.value, value, loadSpeed * Time.deltaTime);
            yield return null;
        }
    }




    void GotoHome()
    {
        SceneManager.LoadSceneAsync(1);
    }

    private IEnumerator LoadGameData()
    {
        if (GameFunc.isOnLine)
        {
            UserData.shop_id = JavaSControl.TryGetShopID();
            GameSetting.game_id = JavaSControl.TryGetGameID();

            yield return new WaitForSeconds(0.1f);

            GameFunc.LoadTerminalInfo();

            yield return new WaitForSeconds(0.1f);

            GameFunc.LoadGameInfo();

            yield return GameFunc.LoadingData();
        }
    }

}
