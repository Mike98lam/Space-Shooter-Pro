using System.Collections;

using UnityEngine;
using TMPro;
using UnityEngine.UI;
public class UIManager : MonoBehaviour
{
    [SerializeField]
    private TMP_Text _scoreText;

    [SerializeField]
    private Sprite[] _liveSprites;

    [SerializeField]
    private Image _livesImg;

    [SerializeField]
    private TMP_Text _gameOverText;

    [SerializeField]
    private TMP_Text _restartText;

    [SerializeField]
    private TMP_Text _ammoText;

    [SerializeField]
    private TMP_Text _waveText;

    [SerializeField]
    private TMP_Text _enemyLeftText;

    private GameManager _gameManager;

    [SerializeField]
    private Slider _thrusterSlider;

    [SerializeField]
    private Image _thrusterColor;

    // Start is called before the first frame update
    void Start()
    {
       
        _scoreText.text = "Score: " + 0; //assign text component to the handle
        _ammoText.text = "Ammo Count: 15";
        _enemyLeftText.text = "Enemies Left: " + 0;
        _gameOverText.gameObject.SetActive(false);
        _gameManager = GameObject.Find("Game_Manager").GetComponent<GameManager>();
        if (_gameManager == null)
        {
            Debug.LogError("Couldn't find Game Manager");
        }
        _thrusterSlider = GetComponentInChildren<Slider>();
        _thrusterColor = _thrusterSlider.transform.Find("Fill Area").Find("Fill").GetComponent<Image>();
        _thrusterSlider.value = 100;
        if (_thrusterSlider == null)
        {
            Debug.LogError("Couldn't find the Slider");
        }

        if (_thrusterColor == null)
        {
            Debug.LogError("Couldn't find the Color of the thruster gauge");

        }
    }

    private void Update()
    {
        ThrusterColor();
    }

    public void EnemyRemaining(int enemyLeft)
    {
        _enemyLeftText.text = "Enemies Left: " + enemyLeft;
    }
    public void UpdateWave(int waveNumber)
    {
        if (waveNumber >= 0)
        {
            _waveText.text = "Wave: " + waveNumber;
            StartCoroutine(UpdateWave());
        }
        
    }
    IEnumerator UpdateWave()
    {
        _waveText.gameObject.SetActive(true);
        yield return new WaitForSeconds(3);
        _waveText.gameObject.SetActive(false);
    }
    public void BossWave()
    {
        _waveText.text = "Final Boss";
        StartCoroutine(UpdateWave());
    }
    public void UpdateScore(int playerScore)
    {
        _scoreText.text = "Score: " + playerScore;
    }

    public void UpdateAmmo(long playerAmmo)
    {
        _ammoText.text = "Ammo Count: " + playerAmmo;
    }

    public void AmmoEmpty()
    {
        _ammoText.text = "Ammo Count: EMPTY";
    }

    public void AmmoInfinite()
    {
        _ammoText.text = "Ammo Count: Infinite";
    }

    public void UpdateLives(int currentLives)
    {
        _livesImg.sprite = _liveSprites[currentLives];

        if (currentLives == 0)
        {
            GameOverSequence();
        }

    }
    void GameOverSequence()
    {
        _gameManager.GameOver();
        _gameOverText.gameObject.SetActive(true);
        _restartText.gameObject.SetActive(true);

        StartCoroutine(GameOverFlickerRoutine());

    }

    public void GameCompleteSequence()
    {
        _gameManager.GameOver();
        _gameOverText.gameObject.SetActive(true);
        _restartText.gameObject.SetActive(true);

        StartCoroutine(GameCompleteFlickerRoutine());
    }
    IEnumerator GameOverFlickerRoutine()
    {
        while (true)
        {
            _gameOverText.text = "GAME OVER";
            yield return new WaitForSeconds(.5f);
            _gameOverText.text = "";
            yield return new WaitForSeconds(.5f);
        }
    }
    IEnumerator GameCompleteFlickerRoutine()
    {
        while (true)
        {
            _gameOverText.text = "Congratulations!";
            yield return new WaitForSeconds(.5f);
            _gameOverText.text = "";
            yield return new WaitForSeconds(.5f);
        }
       
    }

    private void ThrusterColor()
    {
        if (_thrusterSlider.value <= 100 && _thrusterSlider.value >= 66)
        {
            _thrusterColor.color = Color.green;
        }

        if (_thrusterSlider.value <= 66 && _thrusterSlider.value >= 33)
        {
            _thrusterColor.color = Color.yellow;
        }

        if (_thrusterSlider.value <= 33 && _thrusterSlider.value >= 0)
        {
            _thrusterColor.color = Color.red;
        }

    }
}