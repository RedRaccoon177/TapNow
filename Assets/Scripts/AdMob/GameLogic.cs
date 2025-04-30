using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

/// <summary>
/// TapNow 게임의 핵심 로직을 담당하는 스크립트
/// - 타이머 감소 및 정지 제어
/// - TapNow 텍스트 주기적 등장
/// - 성공 횟수 계산 및 출력
/// - 광고 연동 (전면 / 보상형)
/// </summary>
public class GameLogic : MonoBehaviour
{
    // 게임 상단 타이머 UI (10.00초 → 0초까지 감소)
    public TextMeshProUGUI timerText;

    // TapNow 텍스트 UI (중간에 번쩍 등장함)
    public TextMeshProUGUI tapNowText;

    // 우측 상단 성공 횟수 텍스트 UI
    public TextMeshProUGUI blockCountText;

    // 게임오버 시 띄울 패널 (버튼 2개 포함)
    public GameObject gameOverPanel;

    // 설정할 기본 시작 타임 (초)
    public float startTime = 10f;

    // 내부 현재 시간 저장
    private float currentTime;

    // 현재 타이머가 줄어들고 있는지 여부
    private bool isCounting = false;

    // 성공한 TapNow 횟수 카운트
    private int blockCount = 0;

    // 광고 호출용 스크립트 참조
    public AdManager adManager;

    /// <summary>
    /// 게임 시작 시 초기 세팅
    /// </summary>
    void Start()
    {
        currentTime = startTime;
        UpdateTimerText();
        UpdateBlockCount();
        tapNowText.gameObject.SetActive(false);
        gameOverPanel.SetActive(false);
        StartCoroutine(TapNowCycle());
    }

    /// <summary>
    /// 매 프레임 타이머 감소 확인 및 클릭 처리
    /// </summary>
    void Update()
    {
        if (isCounting)
        {
            currentTime -= Time.deltaTime;
            if (currentTime <= 0f)
            {
                currentTime = 0f;
                GameOver();
            }
            UpdateTimerText();
        }

        // 플레이어가 터치 시 TapNow 상태면 타이머 멈춤
        if (!gameOverPanel.activeSelf && Input.GetMouseButtonDown(0) && tapNowText.gameObject.activeSelf)
        {
            StopTimer();
        }
    }

    /// <summary>
    /// 타이머 텍스트를 화면에 표시 (소수점 둘째자리)
    /// </summary>
    void UpdateTimerText()
    {
        timerText.text = $"{currentTime:F2}/S";
    }

    /// <summary>
    /// 성공 횟수 텍스트 업데이트
    /// </summary>
    void UpdateBlockCount()
    {
        blockCountText.text = $"Tap: {blockCount}";
    }

    /// <summary>
    /// TapNow 문구를 일정 주기로 보여주고, 타이머 시작
    /// </summary>
    IEnumerator TapNowCycle()
    {
        while (true)
        {
            float wait = Random.Range(1f, 4f); // 다음 TapNow까지 대기 시간
            yield return new WaitForSeconds(wait);

            tapNowText.gameObject.SetActive(true);
            isCounting = true;

            // 플레이어가 터치하면 tapNowText가 꺼지므로 그걸 기다림
            yield return new WaitUntil(() => !tapNowText.gameObject.activeSelf);

            // 타이머가 0이 되면 루프 종료
            if (currentTime <= 0f) yield break;
        }
    }

    /// <summary>
    /// 타이머를 멈추고 TapNow 숨김, 점수 증가
    /// </summary>
    void StopTimer()
    {
        if (gameOverPanel.activeSelf) return; // 게임 오버 상태면 무시

        isCounting = false;
        tapNowText.gameObject.SetActive(false);
        blockCount++;
        UpdateBlockCount();
    }


    /// <summary>
    /// 게임오버 처리 및 UI 표시
    /// </summary>
    void GameOver()
    {
        isCounting = false;
        gameOverPanel.SetActive(true);
    }

    /// <summary>
    /// 다시하기 버튼 클릭 시 호출됨
    /// 전면 광고 시청 후 메인 씬으로 전환
    /// </summary>
    public void OnRetryClicked()
    {
        adManager.ShowInterstitial(() => SceneManager.LoadScene("StartScene"));
    }

    /// <summary>
    /// 부활 버튼 클릭 시 호출됨
    /// 보상형 광고 시청 후 게임 재시작 (기록 유지)
    /// </summary>
    public void OnReviveClicked()
    {
        adManager.ShowRewarded(() =>
        {
            currentTime = startTime;
            UpdateTimerText();
            tapNowText.gameObject.SetActive(false);
            gameOverPanel.SetActive(false);
            StartCoroutine(TapNowCycle());
        });
    }
}
