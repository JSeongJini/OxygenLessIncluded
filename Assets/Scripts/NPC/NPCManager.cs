using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCManager : MonoBehaviour
{
    private NPC[] npcList = null;

    public delegate void GameEnd(bool _isWin);
    public GameEnd onGameEnd = null;

    private void Awake()
    {
        npcList = GetComponentsInChildren<NPC>();
        StartCoroutine("CheckNPCAlive");
    }

    public void SleepAll()
    {
        foreach (NPC npc in npcList)
        {
           npc.SetSleepTrigger();
        }
    }

    public void ConnectWorkStream(List<WorkBase> _waitingStream, List<WorkBase> _doingList)
    {
        foreach(NPC npc in npcList)
        {
            npc.SetWorkStream(_waitingStream, _doingList);
        }
    }

    public NPC GetNPCByPos(Vector2Int _pos)
    {
        foreach (NPC npc in npcList)
        {
            if (npc.GetNPCPos() == _pos)
            {
                return npc;
            }
        }
        return null;
    }

    public void GatherNPCAll(Vector2Int _pos)
    {
        foreach (NPC npc in npcList)
        {
            npc.Gather(_pos);
        }
        StartCoroutine("GatherAllCoroutine");
    }

    private IEnumerator GatherAllCoroutine()
    {
        while (true)
        {
            int cnt = 0;
            foreach (NPC npc in npcList)
            {
                if (!npc.gameObject.activeSelf || npc.GetNPCState() == ENPCState.Die)
                    cnt++;
            }

            if (cnt == npcList.Length)
            {
                onGameEnd?.Invoke(true);
                break;
            }
                
            yield return null;
        }
    }

    private IEnumerator CheckNPCAlive()
    {
        while (true)
        {
            int dieCnt = 0;
            foreach (NPC npc in npcList)
            {
                ENPCState state = npc.GetNPCState();
                if (state == ENPCState.Die)
                    dieCnt++;
            }

            if (dieCnt == npcList.Length)
            {
                onGameEnd?.Invoke(false);
                break;
            }
            yield return new WaitForSeconds(0.2f);
        }
    }
    public void FindNewPathAll()
    {
        foreach (NPC npc in npcList)
            npc.FIndNewPath();
    }
}
