using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.IO;

public class MainManager : MonoBehaviour
{
    public Brick BrickPrefab;
    public int LineCount = 6;
    public Rigidbody Ball;

    public Text ScoreText;
    public Text HighScoreText;
    public GameObject GameOverText;
    public static int highScore = 0;
    public string userName;
    public static string highScoreUser = "";
    public bool isNewHighScore = false;

    private bool m_Started = false;
    public int m_Points;
    
    private bool m_GameOver = false;


    // Start is called before the first frame update
    void Start()
    {
        GameOverText.SetActive(false);
        userName = StartMenuUIHandler.playerName;
        LoadUserAndHighScore();
        Debug.Log("Start of Scene HighScore User: " + highScoreUser);
        Debug.Log("User is: " + userName + " , High Score is: " + highScore);
        HighScoreText.text = "High Score : " + highScoreUser + " : " + highScore;
        const float step = 0.6f;
        int perLine = Mathf.FloorToInt(4.0f / step);
        
        int[] pointCountArray = new [] {1,1,2,2,5,5};
        for (int i = 0; i < LineCount; ++i)
        {
            for (int x = 0; x < perLine; ++x)
            {
                Vector3 position = new Vector3(-1.5f + step * x, 2.5f + i * 0.3f, 0);
                var brick = Instantiate(BrickPrefab, position, Quaternion.identity);
                brick.PointValue = pointCountArray[i];
                brick.onDestroyed.AddListener(AddPoint);
            }
        }
    }

    private void Update()
    {
        if (!m_Started)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                m_Started = true;
                float randomDirection = Random.Range(-1.0f, 1.0f);
                Vector3 forceDir = new Vector3(randomDirection, 1, 0);
                forceDir.Normalize();

                Ball.transform.SetParent(null);
                Ball.AddForce(forceDir * 2.0f, ForceMode.VelocityChange);
            }
        }
        else if (m_GameOver)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            }
        }
    }

    void AddPoint(int point)
    {
        m_Points += point;
        ScoreText.text = "Score : " + userName + " : " + m_Points;
    }

    public void GameOver()
    {
        CompareScoreToHighScore(m_Points);
        EstablishHighScoreAndUser();
        m_GameOver = true;
        GameOverText.SetActive(true);
        SaveUserAndHighScore();
    }

    public bool CompareScoreToHighScore(int points)
    {
        if (points > highScore)
        {
            isNewHighScore = true;
        }
        else
        {
            isNewHighScore = false;
        }
        return isNewHighScore;
    }

    public void EstablishHighScoreAndUser()
    {
        if (isNewHighScore == true)
        {
            highScore = m_Points;
            highScoreUser = userName;
            Debug.Log("Username = " + userName + ", highscore Username = " + highScoreUser);
        }
    }

    [System.Serializable]
    class SaveData
    {
        public int highScore;
        public string highScoreUser;
    }

    public void SaveUserAndHighScore()
    {
        SaveData data = new SaveData();
        data.highScore = highScore;
        data.highScoreUser = highScoreUser;

        string json = JsonUtility.ToJson(data);

        File.WriteAllText(Application.persistentDataPath + "/savefile.json", json);
    }

    public void LoadUserAndHighScore()
    {
        string path = Application.persistentDataPath + "/savefile.json";
        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            SaveData data = JsonUtility.FromJson<SaveData>(json);

            highScore = data.highScore;
            highScoreUser = data.highScoreUser;
        }
    }
    public void ResetHighScoreAndUserData()
    {
        SaveData data = new SaveData();
        data.highScore = 0;
        data.highScoreUser = "";

        string json = JsonUtility.ToJson(data);

        File.WriteAllText(Application.persistentDataPath + "/savefile.json", json);
    }
}
