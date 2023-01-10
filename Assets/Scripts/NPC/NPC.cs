using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class NPC : MonoBehaviour
{
    [SerializeField] private SoundsManager soundsManager= null;
    [SerializeField] private MapManager mapManager = null;
    [SerializeField] private CautionManager cautionManager = null;

    private Animator anim = null;

    private List<WorkBase> waitingStream = null;
    private List<WorkBase> doingList = null;
    private WorkBase curWork = null;
    private List<Node> path = null;
    private Vector2Int avoidPos;
    private ENPCState state = ENPCState.Idle;

    private bool sleepTrigger = false;
    private bool gatherTrigger = false;

    private float speed = 2f;
    private float digPower = 2f;
    private float oxygenRate = 100f;

    private void Awake()
    {
        anim = GetComponent<Animator>();
    }

    public void SetSleepTrigger()
    {
        sleepTrigger = true;
    }

    private void Start()
    {
        StartCoroutine("BreathOut");
        StartCoroutine("BreathIn");
    }
    private void Update()
    {
        if (state == ENPCState.Idle && !sleepTrigger)
        {
            avoidPos = mapManager.GetAvoidPos(GetNPCPos());
            if (avoidPos != -Vector2Int.one && avoidPos != GetNPCPos())
            {
                state = ENPCState.Avoiding;
                StartCoroutine("Avoiding", avoidPos);
            }
            else
            {
                if (waitingStream.Count == 0) return;

                curWork = waitingStream[0];
                waitingStream.RemoveAt(0);

                List<Vector2Int> workPosList = mapManager.GetWorkPos(GetNPCPos(), curWork.GetTargetPosition());
                if (workPosList.Count == 0)
                {
                    workPosList = null;
                    waitingStream.Add(curWork);
                    return;
                }

                for (int i = 0; i < workPosList.Count; i++)
                {
                    path = mapManager.PathFind(GetNPCPos(), workPosList[i]);
                    if (path.Count != 0) break;
                }
                if (path.Count == 0)
                {
                    path = null;
                    waitingStream.Add(curWork);
                    return;
                }
                doingList.Add(curWork);
                state = ENPCState.Moving;
                StartCoroutine("Moving");
            }
        }else if (sleepTrigger && !gatherTrigger && state != ENPCState.Die)
        {
            sleepTrigger = false;
            StopCoroutine("Moving");
            StopCoroutine("Working");
            StopCoroutine("Avoiding");
            StopCoroutine("Avoiding");
            StartCoroutine("Sleep");
        }
    }

    private IEnumerator Moving()
    {
        anim.SetFloat("speed", 1f);
        if (path == null)
        {
            state = ENPCState.Idle;
            anim.SetFloat("speed", 0f);
        }
        else{
            int distance = path.Count;
            for (int i = 1; i < distance; i++)
            {
                Vector3 startPos = transform.position;
                Vector3 targetPos = new Vector3(path[1].x, path[1].y, 0f);

                transform.localScale = (startPos.x < targetPos.x) ? new Vector3(-1f, 1f, 1f) : new Vector3(1f, 1f, 1f);
                anim.SetBool("isLadder", mapManager.IsLadder(GetNPCPos()) && mapManager.IsLadder(new Vector2Int((int)targetPos.x, (int)targetPos.y)));
                float elpased = 0f;
                while (elpased <= 1f)
                {
                    transform.position = Vector3.Lerp(startPos, targetPos, elpased);
                    elpased += Time.deltaTime * speed;
                    yield return null;
                }
                transform.position = targetPos;
                path.RemoveAt(1);
            }
            anim.SetFloat("speed", 0f);
            path = null;
            state = ENPCState.Working;
            StartCoroutine("Working");
        }
    }

    private IEnumerator Working()
    {
        if (curWork.GetType() == typeof(WorkBuild) &&
                mapManager.GetResourceByPos(curWork.GetTargetPosition()).GetType() != typeof(ResourceAir))
            waitingStream.Add(curWork);
        else
        {
            anim.SetBool("isWork", true);
            while (curWork.Work(digPower) > 0f)
            {
                yield return new WaitForSeconds(0.2f);
            }
        }

        anim.SetBool("isWork", false);
        doingList.Remove(curWork);
        state = ENPCState.Idle;
        yield return null;
    }


    private IEnumerator Sleep()
    {
        ENPCState tmp = state;
        state = ENPCState.Sleeping;
        anim.SetTrigger("sleep");
        yield return new WaitForSeconds(7.2f);
        anim.SetTrigger("wakeUp");
 

        state = tmp;
        if(state == ENPCState.Moving)
            StartCoroutine("Moving");
        else if (state == ENPCState.Working)
            StartCoroutine("Working");
        else if (state == ENPCState.Avoiding)
            StartCoroutine("Avoiding");
    }



    public Vector2Int GetNPCPos()
    {
        Vector2Int pos = new Vector2Int(
            (int)transform.position.x,
            (int)transform.position.y
            );
        return pos;
    }

    public void FIndNewPath()
    {
        if (state != ENPCState.Moving) return;
        StopCoroutine("Moving");
        Vector2Int targetPos = new Vector2Int(path[path.Count - 1].x, path[path.Count - 1].y);
        path = mapManager.PathFind(GetNPCPos(), targetPos);
        if(path.Count == 0)
        {
            doingList.Remove(curWork);
            waitingStream.Add(curWork);
            state = ENPCState.Idle;
            anim.SetFloat("speed", 0f);
            path = null;
        }
        else
        {
            StartCoroutine("Moving");
        }
    }


    private IEnumerator Avoiding()
    {
        List<Node> avoidPath = mapManager.PathFind(GetNPCPos(), avoidPos);
        

        for (int i = 1; i < avoidPath.Count; i++)
        {
            Vector3 startPos = transform.position;
            Vector3 targetPos = new Vector3(avoidPath[i].x, avoidPath[i].y, 0f);

            transform.localScale = (startPos.x < targetPos.x) ? new Vector3(-1f, 1f, 1f) : new Vector3(1f, 1f, 1f);
            anim.SetBool("isLadder", mapManager.IsLadder(GetNPCPos()) && mapManager.IsLadder(new Vector2Int((int)targetPos.x, (int)targetPos.y)));
            float elpased = 0f;
            while (elpased <= 1f)
            {
                transform.position = Vector3.Lerp(startPos, targetPos, elpased);
                elpased += Time.deltaTime * speed;
                yield return null;
            }
            transform.position = targetPos;
        }
        state = ENPCState.Idle;
    }

    public float GetOxygenRate()
    {
        return oxygenRate;
    }

    
    private IEnumerator BreathOut()
    {
        while(oxygenRate > 0f)
        {
            oxygenRate -= Time.deltaTime * 2f;
            yield return (0.5f);
        }
        if(oxygenRate <= 0f)
        {
            Die();
        }
        yield return null;
    }

    private IEnumerator BreathIn()
    {
        while (true)
        {
            yield return new WaitForSeconds(5f);
            ResourceBase air = mapManager.GetBreathAir(GetNPCPos());
            
            if (air != null && air.GetType() == typeof(ResourceAir))
            {
                float amount;
                if (oxygenRate < 88f)
                    amount = ((ResourceAir)air).Consume(12f);
                else
                    amount = ((ResourceAir)air).Consume(100f - oxygenRate);
                
                oxygenRate += amount;
                if (oxygenRate > 100f) oxygenRate = 100f;
            }
            if (oxygenRate < 20f)
            {
                cautionManager.ShowCaution(0);
            }
        }
    }

    private void Die()
    {
        StopAllCoroutines();
        state = ENPCState.Die;
        anim.SetTrigger("die");
        soundsManager.PlayAudio(6);
    }
    private void SetActiveFalse()
    {
        gameObject.SetActive(false);
    }


    public void Gather(Vector2Int _pos)
    {
        if (state == ENPCState.Die) return;
        StopCoroutine("Moving");
        StopCoroutine("Working");
        StopCoroutine("Avoiding");
        gatherTrigger = true;
        path = mapManager.PathFind(GetNPCPos(), _pos);
        if (path.Count != 0)
        {
            StartCoroutine("GatherCoroutine");
        }
        else
        {
            Die();
        }
    }

    private IEnumerator GatherCoroutine()
    {
        int distance = path.Count;
        anim.SetFloat("speed", 1f);
        for (int i = 1; i < distance; i++)
        {
            Vector3 startPos = transform.position;
            Vector3 targetPos = new Vector3(path[1].x, path[1].y, 0f);

            transform.localScale = (startPos.x < targetPos.x) ? new Vector3(-1f, 1f, 1f) : new Vector3(1f, 1f, 1f);
            anim.SetBool("isLadder", mapManager.IsLadder(GetNPCPos()) && mapManager.IsLadder(new Vector2Int((int)targetPos.x, (int)targetPos.y)));
            float elpased = 0f;
            while (elpased <= 1f)
            {
                transform.position = Vector3.Lerp(startPos, targetPos, elpased);
                elpased += Time.deltaTime * speed;
                yield return null;
            }
            transform.position = targetPos;
            path.RemoveAt(1);
        }
        anim.SetFloat("speed", 0f);
        path = null;
        yield return new WaitForSeconds(0.5f);
        gameObject.SetActive(false);
        
    }

    public void SetWorkStream(List<WorkBase> _waitingStream, List<WorkBase> _doingList)
    {
        waitingStream = _waitingStream;
        doingList = _doingList;
    }

    public ENPCState GetNPCState()
    {
        return state;
    }
}
