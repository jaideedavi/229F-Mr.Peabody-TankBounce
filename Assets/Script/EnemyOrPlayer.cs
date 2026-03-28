using UnityEngine;

public class EnemyOrPlayer : MonoBehaviour
{
    private void OnDestroy()
    {
        ObjectDestroyChecker checker = FindObjectOfType<ObjectDestroyChecker>();
        if (checker != null)
        {
            checker.NotifyDestroyed(gameObject.name);
        }
    }
}

