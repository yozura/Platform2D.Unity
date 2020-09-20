using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    // HP
    [Header("HP")]
    public int healthPoint;

    // Points
    [Header("Points")]
    public int totalPoint;
    public int stagePoint;

    // Stages
    [Header("Stages")]
    public int stageIndex;
    public GameObject[] Stages;

    // Player
    public PlayerMove player;

    // UI
    public Image[] hpImg;
    public Text pointTxt;
    public Text stageTxt;
    public GameObject retryBtn;

    private void Update()
    {
        ChangePoint();
    }

    public void ChangePoint()
    {
        pointTxt.text = (totalPoint + stagePoint).ToString();
    }

    public void NextStage()
    {
        // Change Stage
        if (stageIndex < Stages.Length - 1)
        {
            Stages[stageIndex].SetActive(false);
            stageIndex++;
            Stages[stageIndex].SetActive(true);
            PlayerReposition();

            stageTxt.text = "STAGE " + (stageIndex + 1);
        }
        else
        {
            // Player Control Lock
            Time.timeScale = 0f;

            // Result UI
            Debug.Log("게임끝!");

            // Restart Button UI
            Text btnText = retryBtn.GetComponentInChildren<Text>();
            btnText.text = "Clear!";
            retryBtn.SetActive(true);
        }
        
        // Calculate Point
        totalPoint += stagePoint;
        stagePoint = 0;
    }

    public void HealthDown()
    {
        if (healthPoint > 1)
        {
            healthPoint--;
            hpImg[healthPoint].color = new Color(1, 0, 0, 0.4f);
        }
        else
        {
            // All HP Image Off
            hpImg[0].color = new Color(1, 0, 0, 0.4f);
            // Player Die Effect
            player.OnDie();
            // Result UI
            Debug.Log("죽었땅");
            // Retry Button UI
            retryBtn.SetActive(true);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            // Player Reposition
            if(healthPoint > 1)
                PlayerReposition();
            
            // HP Down
            HealthDown();
        }
    }

    public void Restart()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(0);
    }

    private void PlayerReposition()
    {
        player.transform.position = new Vector3(0, 0, -1);
        player.VelocityZero();
    }
}
