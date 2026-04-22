using UnityEngine;

public class MonsterVision : MonoBehaviour
{
    public Transform eyes;
    public Transform player;
    public float viewDistance = 15f;
    public float viewAngle = 120f;

    private PlayerHideController playerHideController;

    void Start()
    {
        if (player != null)
            playerHideController = player.GetComponent<PlayerHideController>();
    }

    public bool CanSeePlayer()
    {
        if (playerHideController != null && playerHideController.isHidden)
            return false;

        if (eyes == null || player == null)
            return false;

        Vector3 directionToPlayer = player.position - eyes.position;
        float distanceToPlayer = directionToPlayer.magnitude;

        if (distanceToPlayer > viewDistance)
            return false;

        float angle = Vector3.Angle(eyes.forward, directionToPlayer);

        if (angle > viewAngle * 0.5f)
            return false;

        RaycastHit hit;
        if (Physics.Raycast(eyes.position, directionToPlayer.normalized, out hit, distanceToPlayer))
        {
            if (hit.transform.root == player)
            {
                return true;
            }
        }

        return false;
    }
}