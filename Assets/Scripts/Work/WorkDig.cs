using UnityEngine;

public class WorkDig : WorkBase
{
    private ResourceBase resource;

    public WorkDig(Vector2Int _targetPos, ResourceBase _resource
        ): base(_targetPos) {
        resource = _resource;
    }

    public ResourceSandStone GetSandStone()
    {
        return (ResourceSandStone)resource;
    }

    public override float Work(float _power)
    {
        if(resource.GetType() == typeof(ResourceSandStone))
            soundsManager.PlayAudio(2);
        else if(resource.GetType() == typeof(ResourceGold))
            soundsManager.PlayAudio(3);
        
        if (!resource)
        {
            progress = 0f;
            base.Work(_power);
            return 0f;
        }
        else
        {
            resource.Consume(_power * 100f);
            return progress;
        }
    }

}