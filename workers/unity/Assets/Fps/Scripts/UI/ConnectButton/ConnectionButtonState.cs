using UnityEngine;

public class ConnectionButtonState : StateMachineBehaviour
{
    public ConnectionAnimController.ConnectionState State;
    private static Animator animatorCache;
    private static ConnectionAnimController animControllerCache;

    private ConnectionAnimController GetAnimController(Animator animator)
    {
        if (animatorCache == animator)
        {
            return animControllerCache;
        }

        Debug.Log("No animator cached. Caching now");
        animatorCache = animator;
        animControllerCache = animator.GetComponent<ConnectionAnimController>();
        return animControllerCache;
    }

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        GetAnimController(animator).SetState(State);
    }
}
