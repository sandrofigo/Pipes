using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Steam : MonoBehaviour {

    [SerializeField]
    float blinkDelay = 1f;

    public bool blink = false;

    Image image;

    private void Start()
    {
        image = GetComponent<Image>();
    }

    float t;

    private void Update()
    {
        if (blink)
        {
            t += Time.deltaTime;

            if(t >= blinkDelay)
            {
                image.enabled = !image.enabled;
                t = 0;
            }
        }
    }

}
