using UnityEngine;
using redd096;

[AddComponentMenu("redd096/Tasks FSM/Condition/Check Complete Action Task")]
public class CheckCompleteActionTask : ConditionTask
{
    [Header("Action to complete - default get component")]
    [SerializeField] ActionTask actionToComplete = default;

    bool isActionComplete;

    protected override void OnInitTask()
    {
        base.OnInitTask();

        //get references
        if (actionToComplete == null) actionToComplete = GetComponent<ActionTask>();

        //register to event
        if (actionToComplete)
            actionToComplete.onCompleteTask += OnCompleteTask;
    }

    void OnDestroy()
    {
        //unregister from event
        if (actionToComplete)
            actionToComplete.onCompleteTask -= OnCompleteTask;
    }

    public override bool OnCheckTask()
    {
        //return when action is complete
        return isActionComplete;
    }

    private void OnCompleteTask()
    {
        //set var
        isActionComplete = true;
    }

    public override void OnExitTask()
    {
        base.OnExitTask();

        //reset var
        isActionComplete = false;
    }
}
