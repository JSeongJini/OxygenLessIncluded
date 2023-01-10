using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class NPC : MonoBehaviour
{
    [SerializeField] private SoundsManager soundsManager= null;
    [SerializeField] private MapManager mapManager = null;
    [SerializeField] private CautionManager cautionManager = null;

    #region Ä³½Ì
    private WaitForSeconds workDelay = new WaitForSeconds(0.2f);
    private WaitForSeconds breathDelay = new WaitForSeconds(0.5f);
    private int hashWork = Animator.StringToHash("isWork");
    private int hashSpeed = Animator.StringToHash("speed");
    private int hashIsLadder = Animator.StringToHash("isLadder");
    private int hashSleep = Animator.StringToHash("sleep");
    private int hashWakeUp = Animator.StringToHash("wakeUp");
    private List<Vector2Int> workPosList = null;
    private List<Node> path = new List<Node>();
    private List<Node> avoidPath = new List<Node>();
    private Vector3 leftScale = new Vector3(-1f, 1f, 1f);
    private Vector3 rightScale = new Vector3(1f, 1f, 1f);
    #endregion

    private Animator anim = null;

    [SerializeField]  private List<WorkBase> waitingStream = null;
    private List<WorkBase> doingList = null;
    private WorkBase curWork = null;
    private Vector2Int avoidPos;
    [SerializeField] private ENPCState state = ENPCState.Idle;

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

        hashWork = Animator.StringToHash("isWork");
        workPosList = new List<Vector2Int>();

    }
    private void Update()
    {
        if (state == ENPCState.Idle && !sleepTrigger)
        {
            if (!mapManager.CanStand(GetNPCPos()))
            {
                avoidPos = mapManager.GetAvoidPos(GetNPCPos());
                if (avoidPos != -Vector2Int.one && avoidPos != GetNPCPos())
                {
                    state = ENPCState.Avoiding;
                    StartCoroutine("Avoiding", avoidPos);
                }
            }
            else
            {  
                if (waitingStream.Count == 0)
                    return;

                curWork = waitingStream[0];
                waitingStream.RemoveAt(0);

                workPosList.Clear();
                mapManager.GetWorkPos(GetNPCPos(), curWork.GetTargetPosition(), workPosList);

                if (workPosList.Count == 0)
                {
                    waitingStream.Add(curWork);
                    return;
                }
                for (int i = 0; i < workPosList.Count; i++)
                {
                    path.Clear();
                    mapManager.PathFind(GetNPCPos(), workPosList[i], path);
                    if (path.Count != 0)
                    {
                        break;
                    }
                }
                if (path.Count == 0)
                {
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
        anim.SetFloat(hashSpeed, 1f);
        if (path.Count == 0)
        {
            state = ENPCState.Idle;
            anim.SetFloat(hashSpeed, 0f);
        }
        else{
            int distance = path.Count;
            for (int i = 1; i < distance; i++)
            {
                Vector3 startPos = transform.position;
                Vector3 targetPos = new Vector3(path[1].x, path[1].y, 0f);

                transform.localScale = (startPos.x < targetPos.x) ? leftScale : rightScale;
                anim.SetBool(hashIsLadder, mapManager.IsLadder(GetNPCPos()) && mapManager.IsLadder(new Vector2Int((int)targetPos.x, (int)targetPos.y)));
                float elpased = 0f;
                while (elpased <= 1f)
                {
                    transform.position = Vector3.Lerp(startPos, targetPos, elpased);
                    elpased += Time.deltaTime * speed;
                    yield return null;
                }
                transform.position = targetPos;
                if(path.Count > 1)
                    path.RemoveAt(1);
            }
            anim.SetFloat(hashSpeed, 0f);
            state = ENPCState.Working;
            StartCoroutine("Working");
        }
    }

    

    private IEnumerator Working()
    {
        if (curWork is WorkBuild &&
                !(mapManager.GetResourceByPos(curWork.GetTargetPosition()) is ResourceAir))
            waitingStream.Add(curWork);
        else
        {
            anim.SetBool(hashWork, true);
            while (curWork.Work(digPower) > 0f)
            {
                yield return workDelay;
            }
        }

        anim.SetBool(hashWork, false);
        doingList.Remove(curWork);
        state = ENPCState.Idle;
    }


    private IEnumerator Sleep()
    {
        ENPCState tmp = state;
        state = ENPCState.Sleeping;
        anim.SetTrigger(hashSleep);
        yield return new WaitForSeconds(7.2f);
        anim.SetTrigger(hashWakeUp);
 

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

    public void FindNewpath()
    {
        if (state != ENPCState.Moving) return;
        StopCoroutine("Moving");
        Vector2Int targetPos = new Vector2Int(path[path.Count - 1].x, path[path.Count - 1].y);
        path.Clear();
        mapManager.PathFind(GetNPCPos(), targetPos, path);
        if(path.Count == 0)
        {
            doingList.Remove(curWork);
            waitingStream.Add(curWork);
            state = ENPCState.Idle;
            anim.SetFloat(hashSpeed, 0f);
        }
        else
        {
            StartCoroutine("Moving");
        }
    }


    private IEnumerator Avoiding()
    {
        avoidPath.Clear();
  
        Vector3 startPos = transform.position;
        Vector3 targetPos = new Vector3(avoidPos.x, avoidPos.y, 0f);

        transform.localScale = (startPos.x < targetPos.x) ? leftScale : rightScale;
        anim.SetBool(hashIsLadder, mapManager.IsLadder(avoidPos));
        float elpased = 0f;
        while (elpased <= 1f)
        {
            transform.position = Vector3.Lerp(startPos, targetPos, elpased);
            elpased += Time.deltaTime * speed;
            yield return null;
        }
        transform.position = targetPos;
  
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
            yield return breathDelay;
        }
        if(oxygenRate <= 0f)
        {
            Die();
        }
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
        path.Clear();
        mapManager.PathFind(GetNPCPos(), _pos, path);
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
        anim.SetFloat(hashSpeed, 1f);
        for (int i = 1; i < distance; i++)
        {
            Vector3 startPos = transform.position;
            Vector3 targetPos = new Vector3(path[1].x, path[1].y, 0f);

            transform.localScale = (startPos.x < targetPos.x) ? new Vector3(-1f, 1f, 1f) : new Vector3(1f, 1f, 1f);
            anim.SetBool(hashIsLadder, mapManager.IsLadder(GetNPCPos()) && mapManager.IsLadder(new Vector2Int((int)targetPos.x, (int)targetPos.y)));
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
        anim.SetFloat(hashSpeed, 0f);
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
