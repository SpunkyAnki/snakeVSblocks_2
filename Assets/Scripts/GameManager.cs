using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static bool isGameStarted;
    public static bool levelCompleted;
    public static bool gameOver;

    public GameObject startpanel;
    public GameObject levelCompletedPanel;
    public GameObject gameOverPanel;

    public AudioSource newbodyPart;
    public AudioSource minusPlusSound;
    public AudioSource levelCompletedSound;
    public AudioSource gameOverSound;
    public AudioSource gameMusic;

    [HideInInspector] public bool MoveByTouch, StartTheGame;
    private Vector3 _mouseStartPos, _playerStartPos;
    [SerializeField] private float RoadSpeed, SwipeSpeed, Distance;
    [SerializeField] private GameObject Road;
    public static GameManager GameManagerInstance;
    private Camera _mainCam;
    public List<Transform> chains = new List<Transform>();
    public GameObject NewPart;

    private void Start()
    {
        GameManagerInstance = this;
        _mainCam = Camera.main;
        chains.Add(gameObject.transform);
        isGameStarted = levelCompleted = gameOver = false;
        startpanel.SetActive(true);
        gameMusic.Play();
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            StartTheGame = MoveByTouch = true;
            startpanel.SetActive(false);
        }

        if (Input.GetMouseButtonUp(0))
        {
            MoveByTouch = false;
        }

        if (MoveByTouch)
        {
            var plane = new Plane(Vector3.up, 0f);
            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (plane.Raycast(ray, out float distance))
            {
                Vector3 mousePos = ray.GetPoint(distance);
                Vector3 desiredPos = mousePos - _mouseStartPos;
                Vector3 move = _playerStartPos + desiredPos;

                move.x = Mathf.Clamp(move.x, -4f, 4f);
                move.z = -7f;

                var player = transform.position;

                player = new Vector3(Mathf.Lerp(player.x, move.x, Time.deltaTime * SwipeSpeed), player.y, player.z);
                transform.position = player;
            }
        }

        if (StartTheGame)
        {
            Road.transform.Translate(Vector3.forward * (-RoadSpeed * Time.deltaTime));
        }

        if(levelCompleted)
        {
            levelCompletedPanel.SetActive(true);
            levelCompletedSound.Play();

            if (Input.GetButtonDown("Fire1"))
            {
                SceneManager.LoadScene(0);
            }
        }

        if (chains.Count > 1)
        {
            for (int i = 1; i < chains.Count; i++)
            {
                var firstPart = chains.ElementAt(i - 1);
                var nextPart = chains.ElementAt(i);

                var DesiredDistance = Vector3.Distance(firstPart.position, nextPart.position);
            
                if (DesiredDistance <= Distance)
                {
                    nextPart.position = new Vector3(Mathf.Lerp(nextPart.position.x, firstPart.position.x, SwipeSpeed * Time.deltaTime), nextPart.position.y, Mathf.Lerp(nextPart.position.z, firstPart.position.z - 0.5f, SwipeSpeed * Time.deltaTime));
                }
            }
        }

    }

    private void LateUpdate()
    {
        if (StartTheGame)
        {
            _mainCam.transform.position = new Vector3(Mathf.Lerp(_mainCam.transform.position.x, transform.position.x, (SwipeSpeed - 5f) * Time.deltaTime), _mainCam.transform.position.y, _mainCam.transform.position.z);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("planet"))
        {
            other.transform.parent = null;
            other.gameObject.AddComponent<Rigidbody>().isKinematic = true;
            other.gameObject.AddComponent<StackManager>();
            other.gameObject.GetComponent<Collider>().isTrigger = true;
            other.tag = gameObject.tag;
            other.GetComponent<Renderer>().material = GetComponent<Renderer>().material;
            chains.Add(other.transform);
            newbodyPart.Play();
        }

        if (other.CompareTag("Plus"))
        {
            var NoAdd = Int16.Parse(other.transform.GetChild(0).name);

            for (int i = 0; i < NoAdd; i++)
            {
                GameObject Chain = Instantiate(NewPart, chains.ElementAt(chains.Count - 1).position + new Vector3(0f, 0f, 0.5f), Quaternion.identity);
                
                chains.Add(Chain.transform);
                minusPlusSound.Play();
            }
        }

        if (other.CompareTag("Minus") && chains.Count > 0)
        {
            var NoSub = Int16.Parse(other.transform.GetChild(0).name);

            if (GameManager.GameManagerInstance.chains.Count > NoSub)
            {
                for (int i = 0; i < NoSub; i++)
                {
                    GameManager.GameManagerInstance.chains.ElementAt(GameManager.GameManagerInstance.chains.Count - 1).gameObject.SetActive(false);
                    GameManager.GameManagerInstance.chains.RemoveAt(GameManager.GameManagerInstance.chains.Count - 1);
                    minusPlusSound.Play();
                }

            }
        }

        if (other.CompareTag("finish"))
        {
            levelCompleted = true;
            levelCompletedPanel.SetActive(true);
            levelCompletedSound.Play();
        }

        if (chains.Count == 0)
        {
            gameOverPanel.SetActive(true);
            gameOverSound.Play();
        }

    }

}
