using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

[DefaultExecutionOrder(-1)]
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [SerializeField] private Blade blade;
    [SerializeField] private Spawner spawner;
    [SerializeField] private TMP_Text scoreText;
    [SerializeField] private Image fadeImage;
    [SerializeField] private TMP_Text gameOverText;
    [SerializeField] private GameObject pausePopup;
    private bool isPaused = false;

    public int score { get; private set; } = 0;

    private void Awake()
    {
        if (Instance != null) {
            DestroyImmediate(gameObject);
        } else {
            Instance = this;
        }
    }

    private void OnDestroy()
    {
        if (Instance == this) {
            Instance = null;
        }
    }

    private void Start()
    {
        NewGame();
    }

    private void NewGame()
    {
        Time.timeScale = 1f;

        ClearScene();

        blade.enabled = true;
        spawner.enabled = true;

        score = 0;
        scoreText.text = score.ToString();
    }

    private void ClearScene()
    {
        Fruit[] fruits = FindObjectsOfType<Fruit>();

        foreach (Fruit fruit in fruits) {
            Destroy(fruit.gameObject);
        }

        Bomb[] bombs = FindObjectsOfType<Bomb>();

        foreach (Bomb bomb in bombs) {
            Destroy(bomb.gameObject);
        }
    }

    public void IncreaseScore(int points)
    {
        score += points;
        scoreText.text = score.ToString();

        float hiscore = PlayerPrefs.GetFloat("hiscore", 0);

        if (score > hiscore)
        {
            hiscore = score;
            PlayerPrefs.SetFloat("hiscore", hiscore);
        }
    }

    public void Explode()
    {
        blade.enabled = false;
        spawner.enabled = false;

        StartCoroutine(ExplodeCoroutine());
    }

    private IEnumerator ExplodeCoroutine()
    {
        // Wait until GameOverText finishes
        yield return StartCoroutine(GameOverText());

        // Then run the explode/fade sequence
        yield return StartCoroutine(ExplodeSequence());
    }

    private IEnumerator GameOverText()
    {
        gameOverText.gameObject.SetActive(true);

        // Wait for the game over text animation to finish (supports Animator)
        Animator anim = gameOverText.gameObject.GetComponent<Animator>();

            // Ensure animator updates while timeScale == 0
            var previousUpdateMode = anim.updateMode;
            anim.updateMode = AnimatorUpdateMode.UnscaledTime;

            // Start the default state from the beginning
            anim.Play(0, -1, 0f);

            // Wait a frame for the animator to start
            yield return null;

            // Wait until the current state finishes (not looping). This will use unscaled time.
            while (anim.IsInTransition(0) || anim.GetCurrentAnimatorStateInfo(0).normalizedTime < 1f)
            {
                yield return null;
            }

            // Restore update mode
            anim.updateMode = previousUpdateMode;

        gameOverText.gameObject.SetActive(false);
    }

    private IEnumerator ExplodeSequence()
    {
        float elapsed = 0f;
        float duration = 0.5f;

        // Fade to white
        while (elapsed < duration)
        {
            float t = Mathf.Clamp01(elapsed / duration);
            fadeImage.color = Color.Lerp(Color.clear, Color.white, t);

            Time.timeScale = 1f - t;
            elapsed += Time.unscaledDeltaTime;

            yield return null;
        }

        yield return new WaitForSecondsRealtime(1f);

        NewGame();

        elapsed = 0f;

        // Fade back in
        while (elapsed < duration)
        {
            float t = Mathf.Clamp01(elapsed / duration);
            fadeImage.color = Color.Lerp(Color.white, Color.clear, t);

            elapsed += Time.unscaledDeltaTime;

            yield return null;
        }
    }

    //PauseBtn
    public void Pause()
    {
        if (isPaused) return;
        isPaused = true;

        Time.timeScale = 0f; //Stop game
        blade.enabled = false;
        spawner.enabled = false;

        if (pausePopup != null)
            pausePopup.SetActive(true);
    }

    // PlayBtn trong PausePopup
    public void Resume()
    {
        if (!isPaused) return;
        isPaused = false;

        Time.timeScale = 1f;
        blade.enabled = true;
        spawner.enabled = true;

        if (pausePopup != null)
            pausePopup.SetActive(false);
    }

    //MenuBtn trong PausePopup
    public void GoToMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("StartScene");
    }


}
