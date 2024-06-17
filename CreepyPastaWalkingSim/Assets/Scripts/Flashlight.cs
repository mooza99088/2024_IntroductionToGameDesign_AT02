using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flashlight : MonoBehaviour
{

    Light m_Light;
    public bool drainOverTime;
    public float maxBrightness;
    public float minBrightness;
    public float drainRate;



    // Start is called before the first frame update
    void Start()
    {
        m_Light = GetComponent<Light>();
        m_Light.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        m_Light.intensity = Mathf.Clamp(m_Light.intensity, minBrightness, maxBrightness);
        if (drainOverTime == true && m_Light.enabled == true)
        {
            if (m_Light.intensity > minBrightness)
            {
                m_Light.intensity -= Time.deltaTime *(drainRate/1000); 
            }
        }
        if (Input.GetKeyDown(KeyCode.F))
        {
            m_Light.enabled = !m_Light.enabled;
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            Replacebattery(.3f);
        }
          

        
    }

    public void Replacebattery(float amount)
    {
        m_Light.intensity += amount;
    }
}
