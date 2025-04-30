using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    public Text tapNowText;
    public Text resultText;

    private float tapNowTime;
    private bool canTap = false;

    void Start()
    {
        tapNowText.gameObject.SetActive(false);
        resultText.text = "";
        StartCoroutine(ShowTapNowAfterDelay());
    }

    IEnumerator ShowTapNowAfterDelay()
    {
        float waitTime = Random.Range(1f, 5f);
        yield return new WaitForSeconds(waitTime);

        tapNowTime = Time.time;
        tapNowText.gameObject.SetActive(true);
        canTap = true;
    }

    void Update()
    {
        if (canTap && Input.GetMouseButtonDown(0)) // 모바일 터치도 감지 가능
        {
            float reactionTime = Time.time - tapNowTime;
            resultText.text = $"반응 시간: {reactionTime:F5}초";
            canTap = false;
        }
    }
}
