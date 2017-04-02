using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IATimerManager : MonoBehaviour
{

    #region singleton
    private static IATimerManager m_instance;
    public static IATimerManager getInstance()
    {
        if (m_instance == null)
        {
            m_instance = new IATimerManager();
        }
        return m_instance;
    }

    public IATimerManager()
    {
        m_nodes = new List<NodeInfo>();
        m_sameSlot = new List<SameSlot>();
    }

    #endregion

    public int m_timeSlot = 1;

    public class NodeInfo
    {
        public int ID;
        public List<int> m_cars;
        public List<float> m_times;
    }

    public List<NodeInfo> m_nodes;

    private class SameSlot
    {
        public List<int> m_nodes;
    }

    private List<SameSlot> m_sameSlot;

    #region Build
    public void AddNode(int id)
    {
        NodeInfo ni = new NodeInfo();
        ni.ID = id;
        ni.m_cars = new List<int>();
        ni.m_times = new List<float>();
        m_nodes.Add(ni);
    }

    public void AddSlot(List<int> m_slot)
    {
        SameSlot s = new SameSlot();
        s.m_nodes = new List<int>();
        for (int i = 0; i < m_slot.Count; ++i)
        {
            s.m_nodes.Add(m_slot[i]);
        }
        m_sameSlot.Add(s);
    }
    #endregion

    #region GetTime

    public bool GetTimeForCar(int idCar, int idNode, float lastSlotTime, out float timeForSlot, out float timeForSlot2)
    {
        float desiredTime = lastSlotTime;// + m_timeSlot;
        //caso sin slots compartidos
        if (!isSameSlot(idNode))
        {
            desiredTime = GetTimeForNode(idNode, desiredTime, idCar);

            timeForSlot = desiredTime;
            timeForSlot2 = desiredTime + m_timeSlot;
            return false;
        }
        else
        {
            timeForSlot = desiredTime;
            timeForSlot2 = desiredTime + m_timeSlot;
            return true;
        }
    }

    public float AddTimeForCar(int idCar, float lastSlotTime, int idNode)
    {
        float newTime = lastSlotTime + m_timeSlot;

        // buscamos el tiempo menor
        NodeInfo ni = m_nodes[idNode];
        int index = 0;
        bool finded = false;
        while (!finded && index < ni.m_times.Count)
        {
            if (ni.m_times[index] < newTime)
            {
                ++index;
            }
            else
            {
                ni.m_times.Insert(index, newTime);
                ni.m_cars.Insert(index, idCar);
                finded = true;
            }
        }
        if (!finded)
        {
            ni.m_times.Add(newTime);
            ni.m_cars.Add(idCar);
        }
        return newTime;
    }

    private bool isSameSlot(int node)
    {
        for (int i = 0; i < m_sameSlot.Count; ++i)
        {
            List<int> nodes = m_sameSlot[i].m_nodes;
            for (int j = 0; j < nodes.Count; j++)
            {
                if (nodes[j] == node)
                {
                    return true;
                }
            }
        }

        return false;
    }

    private float GetTimeForNode(int node, float desiredTime, int car)
    {
        NodeInfo ni = m_nodes[node];
        int index = 0;
        bool finded = false;

        while (!finded && index < ni.m_times.Count)
        {
            if (ni.m_times[index] >= desiredTime)
            {
                finded = true;
            }
            else
            {
                ++index;
            }
        }

        //here we have 2 cases:
        // finded is false, because the time is bigger than all
        // finded is true, in index is the smal bigger time

        if (!finded)
        {
            if(ni.m_times.Count != 0)
            {
                desiredTime = Mathf.Max(desiredTime, ni.m_times[ni.m_times.Count - 1] + m_timeSlot);
            }
            ni.m_times.Add(desiredTime);
            ni.m_cars.Add(car);
            return desiredTime;
        }
        else
        {
            if (desiredTime + m_timeSlot <= ni.m_times[index])
            {
                if (index > 0)
                {
                    if (desiredTime - m_timeSlot >= ni.m_times[index - 1])
                    {
                        //add
                        ni.m_times.Insert(index, desiredTime);
                        ni.m_cars.Insert(index, car);
                        return desiredTime;
                    }
                }
                else
                {
                    ni.m_times.Insert(index, desiredTime);
                    ni.m_cars.Insert(index, car);
                    return desiredTime;
                }
            }
                //we dont fit where we belong

                finded = false;
                index = 1;
                float newTime = 0;
                while (!finded && index < ni.m_times.Count)
                {
                    if (newTime - m_timeSlot >= ni.m_times[index - 1] && newTime + m_timeSlot <= ni.m_times[index] && newTime >= desiredTime)
                    {
                        finded = true;
                        ni.m_times.Insert(index, newTime);
                        ni.m_cars.Insert(index, car);
                        return newTime;
                    }
                    else
                    {
                        newTime = ni.m_times[index] + m_timeSlot;
                        ++index;
                    }
                }

                if (!finded)
                {
                    ni.m_times.Add(ni.m_times[ni.m_times.Count - 1] + m_timeSlot);
                    ni.m_cars.Add(car);
                    return desiredTime;
                }
            
        }
        return desiredTime;
    }



    public void test()
    {
        AddNode(0);

        float a;
        float b;

        GetTimeForCar(0, 0, 4, out a, out b);
        GetTimeForCar(1, 0, 1, out a, out b);
        GetTimeForCar(2, 0, 2.5f, out a, out b);
        GetTimeForCar(3, 0, 6, out a, out b);
        GetTimeForCar(4, 0, 3, out a, out b);
        GetTimeForCar(5, 0, 5, out a, out b);
        GetTimeForCar(6, 0, 6.5f, out a, out b);

    }

    public void Clear()
    {
        m_nodes.Clear();
        m_sameSlot.Clear();
    }


    #endregion

}
