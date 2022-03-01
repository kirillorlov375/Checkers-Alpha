using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class FPS : MonoBehaviour
{
    [SerializeField] TMP_Text fpsText;
    [Tooltip("In seconds")] [SerializeField] float fpsDisplayFrequency = 0.5f;

    #region Singleton
    public static FPS Instance { get; private set; }
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this);
        }
        else Destroy(gameObject);
    }
    #endregion

    void Start()
    {
        StartCoroutine(CalculateFPS());
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F11))
            fpsText.gameObject.SetActive(!fpsText.gameObject.activeInHierarchy);
    }

    IEnumerator CalculateFPS()
    {
        while (true)
        {
            var fps = Mathf.RoundToInt(1f / Time.smoothDeltaTime);
            fpsText.text = fps.ToString();
            yield return new WaitForSecondsRealtime(fpsDisplayFrequency);
        }
    }
}
