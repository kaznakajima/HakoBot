using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;

public class Rod : MonoBehaviour
{
    public bool m_Activation
    {
        set
        {
            m_Activation = value;
            if (m_Activation)
            {

            }
        }
        get { return m_Activation; }
    }

    public void Destroy()
    {

    }
}
