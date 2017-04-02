using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IASameSlot : MonoBehaviour {

    public Slot m_sameSlot;
    [System.Serializable]
    public class Slot
    {
        public int[] m_slot;
    }
}
