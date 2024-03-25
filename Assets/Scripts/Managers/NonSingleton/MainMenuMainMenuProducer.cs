using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;


public class MainMenuProducer : MonoBehaviour
{
    public Button TestButton;

    // Start is called before the first frame update
    void Start()
    {
        TestButton.onClick.AddListener(()=>GameManager.Instance.LoadNextScene());
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
