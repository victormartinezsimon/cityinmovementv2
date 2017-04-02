using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Builder : MonoBehaviour {

    private static Builder m_instance;

    void Awake()
    {
        if (m_instance != this && m_instance != null)
        {
            Destroy(this.gameObject);
        }
        m_instance = this;
        DontDestroyOnLoad(this.gameObject);
    }

    // Use this for initialization
    void Start () {
        //Generator.getInstance().GenerateCity();
    }
	
	// Update is called once per frame
	void Update () {
		if(Input.GetKeyDown(KeyCode.A))
        {
            IATimerManager.getInstance().Clear();
            IATimerManager.getInstance().test();
        }
	}
}
