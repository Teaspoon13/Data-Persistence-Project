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

    private string highScorePlayerName;
    private int highScore;
    
    private bool m_Started = false;
    private int m_Points;
    private int endPoints;
    
    private bool m_GameOver = false;

    
    // Start is called before the first frame update
    void Start()
    {
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

        HighScoreLoad();
        
        HighScoreText.text = "HIGH SCORE: " + highScorePlayerName + ": " + highScore;

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
        ScoreText.text = $"Score : {m_Points}";
    }

    public void GameOver()
    {
        m_GameOver = true;
        GameOverText.SetActive(true);

        endPoints = m_Points;

        if (endPoints > highScore)
        {
            HighScoreSave();
        }
    }

    //created a serializable data holder
    [System.Serializable]
    class SaveData
    {
        public string playerName;
        public int highScore;
    }

    public void HighScoreSave()
    {
        SaveData data = new SaveData();

        data.playerName = DataManager.Instance.playerName;
        data.highScore = endPoints;

        //turning data to json format
        string json = JsonUtility.ToJson(data);

        //turning json string to a json file
        File.WriteAllText(Application.persistentDataPath + "/highscore.json", json);
    }

    public void HighScoreLoad()
    {
        string path = Application.persistentDataPath + "/highscore.json";

        if (File.Exists(path))
        {
            //reads the recorded file
            string json = File.ReadAllText(path);

            //sets data to saved highScore values
            SaveData data = JsonUtility.FromJson<SaveData>(json);

            highScore = data.highScore;
            highScorePlayerName = data.playerName;

        }
    }

}
