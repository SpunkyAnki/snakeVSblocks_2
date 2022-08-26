using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using UnityEngine.SceneManagement;

public class StackManager : MonoBehaviour
{
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
            GameManager.GameManagerInstance.chains.Add(other.transform);
        }

        if (other.CompareTag("Plus"))
        {
            var NoAdd = Int16.Parse(other.transform.GetChild(0).name);

            for (int i = 0; i < NoAdd; i++)
            {
                GameObject Chain = Instantiate(GameManager.GameManagerInstance.NewPart, GameManager.GameManagerInstance.chains.ElementAt(GameManager.GameManagerInstance.chains.Count - 1).position + new Vector3(0f, 0f, 0.5f), Quaternion.identity);

                GameManager.GameManagerInstance.chains.Add(Chain.transform);
            }

            other.GetComponent<Collider>().enabled = false;

        }

        if (other.CompareTag("Minus"))
        {
            var NoSub = Int16.Parse(other.transform.GetChild(0).name);

            if (GameManager.GameManagerInstance.chains.Count > NoSub)
            {
                for (int i = 0; i < NoSub; i++)
                {
                    GameManager.GameManagerInstance.chains.ElementAt(GameManager.GameManagerInstance.chains.Count - 1).gameObject.SetActive(false);
                    GameManager.GameManagerInstance.chains.RemoveAt(GameManager.GameManagerInstance.chains.Count - 1);
                }

            }

            other.GetComponent<Collider>().enabled = false;

        }

        if (other.CompareTag("finish"))
        {
            GameManager.GameManagerInstance.levelCompletedPanel.SetActive(true);
        }

        if (GameManager.GameManagerInstance.chains.Count == 0)
        {
            GameManager.GameManagerInstance.gameOverPanel.SetActive(true);
        }

    }
}

